using System;
using UnityEngine;

public class PickupObject : InteractiveObject, IClickable, IOverable
{
    [Header("Pickup Settings")]
    [SerializeField] private PickUpData pickUpData; // Данные объекта, с id и ссылкой на тип
    [SerializeField] private bool isRotateDrag = true; // Должен ли объект поворачиваться при перемещении

    private new Rigidbody rigidbody;
    private new Collider collider;
    private GrabSystem grabSystem; // Система захвата, которая управляет этим объектом

    // Свойства для управления состоянием объекта
    public bool IsDrag { get; private set; } // Перемещается ли объект в данный момент
    public bool IsBackpack { get; private set; } // Находится ли объект в рюкзаке
    public bool CanDrag => !IsBackpack && !IsDrag; // Можно ли захватить объект

    public ItemInfo ItemInfo => pickUpData.itemInfo; // Информация о типе предмета
    public PickUpData PickUpData => pickUpData; // Данные объекта
    public Rigidbody Rigidbody => rigidbody;
    public bool IsRotateDrag => isRotateDrag;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();
    }

    // Метод для отпускания объекта
    public void Drop()
    {
        grabSystem = null;
        rigidbody.useGravity = true;
        collider.isTrigger = false;
        IsDrag = false;
    }

    // Метод для захвата объекта
    public void Grab(GrabSystem grabSystem)
    {
        this.grabSystem = grabSystem; // Запоминаем систему захвата
        rigidbody.useGravity = false; // Отключаем гравитацию
        collider.isTrigger = true; // Включаем триггерный режим коллайдера
        IsDrag = true;
    }

    // Метод для помещения объекта в рюкзак
    public void PutInBackpack(Backpack backpack)
    {
        // Если объект в руках у игрока, бросаем его
        if (IsDrag)
            grabSystem?.TryDrop(this);

        // Настраиваем объект для нахождения в рюкзаке
        rigidbody.useGravity = false; // Отключаем гравитацию
        collider.isTrigger = true; // Включаем триггер
        IsBackpack = true; // Объект теперь в рюкзаке
    }

    // Метод для извлечения объекта из рюкзака
    public void DropOutBackpack()
    {
        rigidbody.useGravity = true; // Включаем гравитацию
        collider.isTrigger = false; // Выключаем триггер
        IsBackpack = false; // Объект больше не в рюкзаке
    }

    // Метод для получения текстовой информации об объекте для интерфейса
    public string GetTextInfo()
    {
        if (!IsDrag && CanDrag) // Если объект не в руках, но его можно поднять, возвращаем подсказку
            return $"[ЛКМ] Поднять {PickUpData.itemInfo.TypeTitle}";
        else
            return ""; // Иначе возвращаем пустую строку
    }

    // Реализация интерфейса IClickable
    public void OnClick(InteractiveSystem interactiveSystem) { }
    public void OnClickUp(InteractiveSystem interactiveSystem) { }

    // Реализация интерфейса IOverable
    public void OnOverEnter(InteractiveSystem interactiveSystem) { }
    public void OnOver(InteractiveSystem interactiveSystem) { }
    public void OnOverEnd(InteractiveSystem interactiveSystem) { }
}

// Структура для хранения данных о подбираемом объекте
[Serializable]
public struct PickUpData
{
    public int Id; // Уникальный идентификатор объекта
    public ItemInfo itemInfo; // Информация о предмете
}