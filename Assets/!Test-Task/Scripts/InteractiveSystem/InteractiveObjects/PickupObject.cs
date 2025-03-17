using System;
using UnityEngine;

public class PickupObject : InteractiveObject, IClickable, IOverable
{
    [Header("Pickup Settings")]
    [SerializeField] private PickUpData pickUpData; // ������ �������, � id � ������� �� ���
    [SerializeField] private bool isRotateDrag = true; // ������ �� ������ �������������� ��� �����������

    private new Rigidbody rigidbody;
    private new Collider collider;
    private GrabSystem grabSystem; // ������� �������, ������� ��������� ���� ��������

    // �������� ��� ���������� ���������� �������
    public bool IsDrag { get; private set; } // ������������ �� ������ � ������ ������
    public bool IsBackpack { get; private set; } // ��������� �� ������ � �������
    public bool CanDrag => !IsBackpack && !IsDrag; // ����� �� ��������� ������

    public ItemInfo ItemInfo => pickUpData.itemInfo; // ���������� � ���� ��������
    public PickUpData PickUpData => pickUpData; // ������ �������
    public Rigidbody Rigidbody => rigidbody;
    public bool IsRotateDrag => isRotateDrag;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();
    }

    // ����� ��� ���������� �������
    public void Drop()
    {
        grabSystem = null;
        rigidbody.useGravity = true;
        collider.isTrigger = false;
        IsDrag = false;
    }

    // ����� ��� ������� �������
    public void Grab(GrabSystem grabSystem)
    {
        this.grabSystem = grabSystem; // ���������� ������� �������
        rigidbody.useGravity = false; // ��������� ����������
        collider.isTrigger = true; // �������� ���������� ����� ����������
        IsDrag = true;
    }

    // ����� ��� ��������� ������� � ������
    public void PutInBackpack(Backpack backpack)
    {
        // ���� ������ � ����� � ������, ������� ���
        if (IsDrag)
            grabSystem?.TryDrop(this);

        // ����������� ������ ��� ���������� � �������
        rigidbody.useGravity = false; // ��������� ����������
        collider.isTrigger = true; // �������� �������
        IsBackpack = true; // ������ ������ � �������
    }

    // ����� ��� ���������� ������� �� �������
    public void DropOutBackpack()
    {
        rigidbody.useGravity = true; // �������� ����������
        collider.isTrigger = false; // ��������� �������
        IsBackpack = false; // ������ ������ �� � �������
    }

    // ����� ��� ��������� ��������� ���������� �� ������� ��� ����������
    public string GetTextInfo()
    {
        if (!IsDrag && CanDrag) // ���� ������ �� � �����, �� ��� ����� �������, ���������� ���������
            return $"[���] ������� {PickUpData.itemInfo.TypeTitle}";
        else
            return ""; // ����� ���������� ������ ������
    }

    // ���������� ���������� IClickable
    public void OnClick(InteractiveSystem interactiveSystem) { }
    public void OnClickUp(InteractiveSystem interactiveSystem) { }

    // ���������� ���������� IOverable
    public void OnOverEnter(InteractiveSystem interactiveSystem) { }
    public void OnOver(InteractiveSystem interactiveSystem) { }
    public void OnOverEnd(InteractiveSystem interactiveSystem) { }
}

// ��������� ��� �������� ������ � ����������� �������
[Serializable]
public struct PickUpData
{
    public int Id; // ���������� ������������� �������
    public ItemInfo itemInfo; // ���������� � ��������
}