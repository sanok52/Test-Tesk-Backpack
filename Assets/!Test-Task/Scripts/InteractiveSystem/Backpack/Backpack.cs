using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using DG.Tweening;

public class Backpack : InteractiveObject, IClickable, IOverable
{
    [Header("Backpack Settings")]
    [SerializeField] private Transform[] places; // Места для предметов в рюкзаке
    [SerializeField] private Button3D[] buttons; // Кнопки для управления предметами
    [SerializeField] private GameObject panelDrop; // Панель для выбора действия

    [Header("Drop Settings")]
    [SerializeField] private Transform pointDrop; // Точка для выброса предметов
    [SerializeField] private float durationMove = 1f; // Длительность анимации перемещения
    [SerializeField] private Ease easeMove = Ease.InExpo; // Тип анимации перемещения

    [Header("Audio Settings")]
    [SerializeField] private AudioSource source; // Источник звука
    [SerializeField] private AudioClip clipPut; // Звук при добавлении предмета
    [SerializeField] private AudioClip clipDrop; // Звук при выбросе предмета

    [Space]
    public UnityEvent<PickupObject> OnPut; // Событие при добавлении предмета
    public UnityEvent<PickupObject> OnDrop; // Событие при выбросе предмета

    private PickupObject[] pickups; // Массив для хранения предметов в рюкзаке

    private void Start()
    {
        // Инициализация массива для хранения предметов
        pickups = new PickupObject[places.Length];

        // Назначение обработчиков событий для кнопок
        for (int i = 0; i < buttons.Length; i++)
        {
            int n = i;
            buttons[i].OnClickUpEvent.AddListener(() => TryDropPickup(n));
        }

        // Обновление состояния кнопок
        ButtonsUpdate();
    }

    private void Update()
    {
        // Обновление позиции и поворота предметов в рюкзаке
        for (int i = 0; i < pickups.Length; i++)
        {
            if (pickups[i] != null && pickups[i].IsBackpack)
            {
                pickups[i].transform.localPosition = Vector3.zero;
                pickups[i].transform.localRotation = Quaternion.identity;
            }
        }
    }

    // Добавление предмета в рюкзак
    public void AddPickup(PickupObject pickupObject)
    {
        if (pickupObject.ItemInfo.PlaceBackpack >= places.Length ||
            pickups[pickupObject.ItemInfo.PlaceBackpack] != null)
            return;

        // Добавление предмета и отправка запроса на сервер
        PutPickup(pickupObject);
        WebManager.Default.SendRequestPost(GetJSONEvent(new BackpackEvent(pickupObject.PickUpData.Id, "Put")), (request) =>
        {
            // Если запрос неудачный, удаляем предмет из рюкзака
            if (request.result != UnityWebRequest.Result.Success)
                DropPickup(pickupObject.ItemInfo.PlaceBackpack);
        }); // Сделал именно так, чтобы не было ожидания (Это вызывает неприятную паузу), пока идёт ответ от сервера. Можно сделать добавление предмета только по итогу запроса, если необходимо
    }

    // Помещение предмета в рюкзак
    private void PutPickup(PickupObject pickupObject)
    {
        int n = pickupObject.ItemInfo.PlaceBackpack;

        // Анимация перемещения предмета в рюкзак
        pickupObject.transform.parent = places[n];
        pickupObject.transform.DOLocalRotate(Vector3.zero, durationMove).SetEase(easeMove).SetAutoKill(true);
        pickupObject.transform.DOLocalMove(Vector3.zero, durationMove).SetEase(easeMove).SetAutoKill(true)
            .OnComplete(() =>
            {
                pickups[n] = pickupObject; // Помещение предмета в список
                ButtonsUpdate(); // Обновление состояния кнопок
            });

        // Говорим предмету, что он в рюкзаке и воспроизведеним звук
        pickupObject.PutInBackpack(this);
        SetButtonData(n, pickupObject.ItemInfo);
        source.PlayOneShot(clipPut);

        OnPut?.Invoke(pickupObject);
    }

    // Попытка выбросить предмет из рюкзака
    public void TryDropPickup(int n)
    {
        // Отправка запроса на сервер для выброса предмета
        WebManager.Default.SendRequestPost(GetJSONEvent(new BackpackEvent(pickups[n].PickUpData.Id, "Drop")), (request) =>
        {
            // Если запрос успешный, выбрасываем предмет
            if (request.result == UnityWebRequest.Result.Success)
                DropPickup(n);
        });
    }

    // Выброс предмета из рюкзака
    private void DropPickup(int n)
    {
        PickupObject pickupObject = pickups[n];

        // Анимация выброса предмета
        pickupObject.transform.DOMove(pointDrop.position + (UnityEngine.Random.insideUnitSphere * 0.2f), durationMove).SetAutoKill(true).SetEase(easeMove)
            .OnComplete(() => pickupObject.DropOutBackpack()); //В концее сообщаем предмету, что он выброшен
        pickupObject.transform.DORotateQuaternion(pointDrop.rotation, durationMove).SetEase(easeMove).SetAutoKill(true);
        pickupObject.transform.parent = null;

        // Очистка данных о предмете и обновление кнопок
        pickups[n] = null;
        ButtonsUpdate();
        source.PlayOneShot(clipDrop);

        OnDrop?.Invoke(pickups[n]);
    }

    // Обновление состояния кнопок
    private void ButtonsUpdate()
    {
        for (int i = 0; i < buttons.Length && i < places.Length; i++)
        {
            buttons[i].gameObject.SetActive(pickups[i] != null);
        }
    }

    // Установка данных для кнопки
    private void SetButtonData(int i, ItemInfo item)
    {
        buttons[i].SetSprite(item.Sprite);
        buttons[i].Text = $"Достать {item.TypeTitle}";
    }

    // Преобразование события в JSON
    public static string GetJSONEvent(BackpackEvent backpackEvent)
    {
        return JsonUtility.ToJson(backpackEvent);
    }

    public void OnClick(InteractiveSystem interactiveSystem)
    {
        panelDrop.SetActive(true);
    }

    public void OnClickUp(InteractiveSystem interactiveSystem)
    {
        panelDrop.SetActive(false);
    }

    public string GetTextInfo()
    {
        return panelDrop.activeInHierarchy ? "" : "{ЛКМ} чтобы открыть рюкзак";
    }

    public void OnOverEnter(InteractiveSystem interactiveSystem)
    { }
    public void OnOver(InteractiveSystem interactiveSystem)
    { }
    public void OnOverEnd(InteractiveSystem interactiveSystem)
    { }
}

// Структура для события рюкзака
[Serializable]
public struct BackpackEvent
{
    public int ItemId; // ID предмета
    public string TypeEvent; // Тип события (Put/Drop)

    public BackpackEvent(int itemId, string typeEvent)
    {
        ItemId = itemId;
        TypeEvent = typeEvent;
    }

    public string GetText()
    {
        return $"ItemId: {ItemId}, TypeEvent: {TypeEvent}";
    }
}