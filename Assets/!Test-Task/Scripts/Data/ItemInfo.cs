using System.Collections;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Item", menuName = "Inventory/ItemInfo"), Serializable]
public class ItemInfo : ScriptableObject
{
    [field: SerializeField] public string TypeTitle { get; private set; }
    [field: SerializeField] public Sprite Sprite { get; private set; }
    [field: SerializeField, Min(0)] public float Weight { get; private set; }
    [field: SerializeField, Min(0)] public int PlaceBackpack { get; private set; }
    [field: SerializeField] public bool CanBeInBackpack { get; private set; }
}