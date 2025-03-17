using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using TMPro;
using System;

public class InteractiveSystem : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] private new Camera camera; // Камера, используемая для взаимодействия
    [SerializeField] private float overDist = 5f; // Максимальная дистанция взаимодействия

    [Header("Layer Masks")]
    [SerializeField] private LayerMask layerOver; // Слои для взаимодействия с объектами
    [SerializeField] private LayerMask layerOverUI; // Слои для взаимодействия с UI (У UI приоритет)

    [Header("UI Settings")]
    [SerializeField] private TextMeshProUGUI InfoOverTmp; // Текстовый элемент для отображения информации


    private HashSet<IClickable> clickables = new HashSet<IClickable>(); // Список объектов, поддерживающих клик
    private HashSet<IHoldable> holdables = new HashSet<IHoldable>(); // Список объектов, поддерживающих удержание
    private InteractiveObject currentOver; // Текущий объект, на который наведён курсор
    private IOverable currentOverable; // Текущий объект, поддерживающий наведение

    // События для взаимодействия с объектами
    public UnityAction<InteractiveObject> OnOverEnter; // Событие при начале наведения
    public UnityAction<InteractiveObject> OnOver; // Событие при наведении
    public UnityAction<InteractiveObject> OnOverExit; // Событие при окончании наведения
    public UnityAction<InteractiveObject> OnClick; // Событие при клике по объекту
    public UnityAction<InteractiveObject> OnOverEnterHold; // Событие при наведении с уже нажатой кнопкой
    public UnityAction<InteractiveObject> OnHold; // Событие при удержании
    public UnityAction<InteractiveObject> OnClickUp; // Событие при отпускании кнопки мыши

    private bool isClickDown;
    private bool isHold;
    private bool isClickUp;

    // Данные о нажатии кнопки
    internal void SetInputData(bool isClickDown, bool isHold, bool isClickUp)
    {
        this.isClickDown = isClickDown;
        this.isHold = isHold;
        this.isClickUp = isClickUp;
    }

    void Update()
    {
        // Попытка взаимодействия с объектами. Сначала на слоях UI, после для обычных объектов
        if (TryInteract(layerOverUI) || TryInteract(layerOver))
        {
        }
        else
        {
            // Если взаимодействия нет, сбрасываем наведение
            BreakOver();
        }

        if (isClickUp)
            ClickUpForAll(); // Говорим всем, что мышку отпустили
        else if (isHold)
            UpdateCurrentHold(); // Говорим всем, что мышка удерживается
    }

    // Попытка взаимодействия с объектами на определённом слое
    public bool TryInteract(LayerMask layer)
    {
        // Создаём луч от камеры
        if (Physics.Raycast(new Ray(camera.transform.position, camera.transform.forward), out RaycastHit hit, overDist, layer))
        {
            if (hit.transform.TryGetComponent(out InteractiveObject interactive))
            {
                // Если объект поддерживает наведение
                if (interactive.TryGetComponent(out IOverable overable))
                {
                    // Если наведение уже было на этом объекте, обновляем его
                    if (currentOverable == overable)
                        UpdateCurrentOver();
                    else
                        // Иначе начинаем новое наведение
                        SetCurrentOver(interactive, overable);
                }

                // Если нажата кнопка мыши
                if (isHold)
                {
                    // Если объект поддерживает клик и ещё не добавлен в список - добавляем
                    if (interactive.TryGetComponent(out IClickable clickable) && !clickables.Contains(clickable))
                    {
                        AddInCurrentClick(interactive, clickable, isClickDown);
                    }
                    // Если объект поддерживает удержание и ещё не добавлен в список - добавляем
                    else if (interactive.TryGetComponent(out IHoldable holdable) && !holdables.Contains(holdable))
                    {
                        AddInCurrentHold(interactive, holdable);
                    }
                }
                return true;
            }
        }
        return false; // Взаимодействие не было
    }

    // Обновление состояния удержания
    private void UpdateCurrentHold()
    {
        foreach (var holdable in holdables)
        {
            holdable.OnHold(this);
            OnHold?.Invoke(holdable as InteractiveObject);
        }
    }

    // Обработка отпускания кнопки мыши для всех объектов
    private void ClickUpForAll()
    {
        foreach (var clickable in clickables)
        {
            clickable.OnClickUp(this);
            OnClickUp?.Invoke(clickable as InteractiveObject);
        }

        clickables.Clear();
    }

    // Добавление объекта в список кликабельных
    private void AddInCurrentClick(InteractiveObject interactive, IClickable clickable, bool onClick = false)
    {
        clickables.Add(clickable);
        if (onClick) //Должен ли быть завиксирован клик по объекту? (Или мы только ждём отпускание?)
        {
            clickable.OnClick(this); // Вызов метода клика
            OnClick?.Invoke(interactive); // Вызов события клика
        }
    }

    // Добавление объекта в список удерживаемых
    private void AddInCurrentHold(InteractiveObject interactive, IHoldable holdable)
    {
        holdable = holdable != null ? holdable : interactive.GetComponent<IHoldable>();
        holdables.Add(holdable);
        holdable.OnOverEnterHold(this);
        OnOverEnterHold?.Invoke(interactive);
    }

    // Установка текущего объекта для наведения
    private void SetCurrentOver(InteractiveObject interactive, IOverable overable)
    {
        BreakOver(); // Сброс предыдущего наведения
        currentOver = interactive;
        currentOverable = overable != null ? overable : interactive.GetComponent<IOverable>();
        currentOverable.OnOverEnter(this);
        OnOverEnter?.Invoke(currentOver);
    }

    // Обновление текущего объекта для наведения
    private void UpdateCurrentOver()
    {
        if (currentOverable == null)
            return;

        currentOverable.OnOver(this); // Вызов метода наведения
        UpdateTextInfo(currentOverable.GetTextInfo()); // Обновление текстовой информации
        OnOver?.Invoke(currentOver);
    }

    // Сбоос наведения
    private void BreakOver()
    {
        if (currentOver != null)
        {
            currentOverable.OnOverEnd(this);
            OnOverExit?.Invoke(currentOver);
        }
        currentOver = null;
        currentOverable = null;
        UpdateTextInfo("");
    }

    // Обновление текстовой информации
    private void UpdateTextInfo(string text)
    {
        InfoOverTmp.text = text;
    }

    // Проверка, является ли объект тем, на который навели
    public bool IsCurrentOver(IOverable overable) => currentOverable == overable;

    // Проверка, содержится ли объект в списке удерживаемых
    public bool ContainsHold(IHoldable holdable) => holdables.Contains(holdable);
}