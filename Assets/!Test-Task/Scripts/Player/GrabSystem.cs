using System;
using UnityEngine;

[RequireComponent(typeof(InteractiveSystem))]
public class GrabSystem : MonoBehaviour
{
    [Header("Grab Settings")]
    [SerializeField] private InteractiveSystem interactiveSystem; // Система взаимодействия
    [SerializeField] private Transform pointGrab; // Точка, куда будет перемещаться объект при захвате

    [Header("Audio Settings")]
    [SerializeField] private AudioSource source; // Источник звука
    [SerializeField] private AudioClip clipGrab; // Звук при захвате объекта
    [SerializeField] private AudioClip clipDrop; // Звук при отпускании объекта

    private PickupObject currentPickup; // Текущий захваченный объект

    // Свойства для проверки состояния
    public bool CanGrab => currentPickup == null; // Можно ли захватить объект
    public bool CanDrop => currentPickup != null; // Можно ли отпустить объект
    public PickupObject CurrentPickup => currentPickup;

    private void Start()
    {
        interactiveSystem.OnClick += TryGrab; // При клике пытаемся захватить объект
        interactiveSystem.OnClickUp += TryDrop; // При отпускании кнопки пытаемся отпустить объект
    }

    private void Update()
    {
        // Если есть захваченный объект, обновляем его позицию и поворот
        if (currentPickup != null)
        {
            currentPickup.transform.position = pointGrab.position; 
            if (currentPickup.IsRotateDrag)
                currentPickup.transform.rotation = pointGrab.rotation;
        }
    }

    // Попытка захватить объект
    public void TryGrab(InteractiveObject interactive)
    {
        if (!CanGrab)
            return;

        // Пытаемся получить компонент PickupObject у объекта взаимодействия
        if (interactive.TryGetComponent(out PickupObject pickupObject) && pickupObject.CanDrag)
        {
            Grab(pickupObject); // Захватываем объект
        }
    }

    // Захват объекта
    private void Grab(PickupObject pickupObject)
    {
        currentPickup = pickupObject; // Запоминаем текущий захваченный объект
        pickupObject.Grab(this); // Вызываем метод Grab у объекта
        source.PlayOneShot(clipGrab); // Воспроизводим звук захвата
    }

    // Попытка отпустить объект
    public void TryDrop(InteractiveObject interactive)
    {
        if (!CanDrop)
            return;

        currentPickup.Drop(); // Вызываем метод Drop у объекта
        Drop(); // Отпускаем объект
        source.PlayOneShot(clipDrop); // Воспроизводим звук отпускания
    }

    // Отпускание объекта
    private void Drop()
    {
        currentPickup = null; // Сбрасываем текущий захваченный объект
    }
}