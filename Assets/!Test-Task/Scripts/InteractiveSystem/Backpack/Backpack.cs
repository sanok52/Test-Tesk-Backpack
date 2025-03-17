using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using DG.Tweening;

public class Backpack : InteractiveObject, IClickable, IOverable
{
    [Header("Backpack Settings")]
    [SerializeField] private Transform[] places; // ����� ��� ��������� � �������
    [SerializeField] private Button3D[] buttons; // ������ ��� ���������� ����������
    [SerializeField] private GameObject panelDrop; // ������ ��� ������ ��������

    [Header("Drop Settings")]
    [SerializeField] private Transform pointDrop; // ����� ��� ������� ���������
    [SerializeField] private float durationMove = 1f; // ������������ �������� �����������
    [SerializeField] private Ease easeMove = Ease.InExpo; // ��� �������� �����������

    [Header("Audio Settings")]
    [SerializeField] private AudioSource source; // �������� �����
    [SerializeField] private AudioClip clipPut; // ���� ��� ���������� ��������
    [SerializeField] private AudioClip clipDrop; // ���� ��� ������� ��������

    [Space]
    public UnityEvent<PickupObject> OnPut; // ������� ��� ���������� ��������
    public UnityEvent<PickupObject> OnDrop; // ������� ��� ������� ��������

    private PickupObject[] pickups; // ������ ��� �������� ��������� � �������

    private void Start()
    {
        // ������������� ������� ��� �������� ���������
        pickups = new PickupObject[places.Length];

        // ���������� ������������ ������� ��� ������
        for (int i = 0; i < buttons.Length; i++)
        {
            int n = i;
            buttons[i].OnClickUpEvent.AddListener(() => TryDropPickup(n));
        }

        // ���������� ��������� ������
        ButtonsUpdate();
    }

    private void Update()
    {
        // ���������� ������� � �������� ��������� � �������
        for (int i = 0; i < pickups.Length; i++)
        {
            if (pickups[i] != null && pickups[i].IsBackpack)
            {
                pickups[i].transform.localPosition = Vector3.zero;
                pickups[i].transform.localRotation = Quaternion.identity;
            }
        }
    }

    // ���������� �������� � ������
    public void AddPickup(PickupObject pickupObject)
    {
        if (pickupObject.ItemInfo.PlaceBackpack >= places.Length ||
            pickups[pickupObject.ItemInfo.PlaceBackpack] != null)
            return;

        // ���������� �������� � �������� ������� �� ������
        PutPickup(pickupObject);
        WebManager.Default.SendRequestPost(GetJSONEvent(new BackpackEvent(pickupObject.PickUpData.Id, "Put")), (request) =>
        {
            // ���� ������ ���������, ������� ������� �� �������
            if (request.result != UnityWebRequest.Result.Success)
                DropPickup(pickupObject.ItemInfo.PlaceBackpack);
        }); // ������ ������ ���, ����� �� ���� �������� (��� �������� ���������� �����), ���� ��� ����� �� �������. ����� ������� ���������� �������� ������ �� ����� �������, ���� ����������
    }

    // ��������� �������� � ������
    private void PutPickup(PickupObject pickupObject)
    {
        int n = pickupObject.ItemInfo.PlaceBackpack;

        // �������� ����������� �������� � ������
        pickupObject.transform.parent = places[n];
        pickupObject.transform.DOLocalRotate(Vector3.zero, durationMove).SetEase(easeMove).SetAutoKill(true);
        pickupObject.transform.DOLocalMove(Vector3.zero, durationMove).SetEase(easeMove).SetAutoKill(true)
            .OnComplete(() =>
            {
                pickups[n] = pickupObject; // ��������� �������� � ������
                ButtonsUpdate(); // ���������� ��������� ������
            });

        // ������� ��������, ��� �� � ������� � ��������������� ����
        pickupObject.PutInBackpack(this);
        SetButtonData(n, pickupObject.ItemInfo);
        source.PlayOneShot(clipPut);

        OnPut?.Invoke(pickupObject);
    }

    // ������� ��������� ������� �� �������
    public void TryDropPickup(int n)
    {
        // �������� ������� �� ������ ��� ������� ��������
        WebManager.Default.SendRequestPost(GetJSONEvent(new BackpackEvent(pickups[n].PickUpData.Id, "Drop")), (request) =>
        {
            // ���� ������ ��������, ����������� �������
            if (request.result == UnityWebRequest.Result.Success)
                DropPickup(n);
        });
    }

    // ������ �������� �� �������
    private void DropPickup(int n)
    {
        PickupObject pickupObject = pickups[n];

        // �������� ������� ��������
        pickupObject.transform.DOMove(pointDrop.position + (UnityEngine.Random.insideUnitSphere * 0.2f), durationMove).SetAutoKill(true).SetEase(easeMove)
            .OnComplete(() => pickupObject.DropOutBackpack()); //� ������ �������� ��������, ��� �� ��������
        pickupObject.transform.DORotateQuaternion(pointDrop.rotation, durationMove).SetEase(easeMove).SetAutoKill(true);
        pickupObject.transform.parent = null;

        // ������� ������ � �������� � ���������� ������
        pickups[n] = null;
        ButtonsUpdate();
        source.PlayOneShot(clipDrop);

        OnDrop?.Invoke(pickups[n]);
    }

    // ���������� ��������� ������
    private void ButtonsUpdate()
    {
        for (int i = 0; i < buttons.Length && i < places.Length; i++)
        {
            buttons[i].gameObject.SetActive(pickups[i] != null);
        }
    }

    // ��������� ������ ��� ������
    private void SetButtonData(int i, ItemInfo item)
    {
        buttons[i].SetSprite(item.Sprite);
        buttons[i].Text = $"������� {item.TypeTitle}";
    }

    // �������������� ������� � JSON
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
        return panelDrop.activeInHierarchy ? "" : "{���} ����� ������� ������";
    }

    public void OnOverEnter(InteractiveSystem interactiveSystem)
    { }
    public void OnOver(InteractiveSystem interactiveSystem)
    { }
    public void OnOverEnd(InteractiveSystem interactiveSystem)
    { }
}

// ��������� ��� ������� �������
[Serializable]
public struct BackpackEvent
{
    public int ItemId; // ID ��������
    public string TypeEvent; // ��� ������� (Put/Drop)

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