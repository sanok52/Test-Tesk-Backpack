using System.Collections;
using UnityEngine;

public class BackpackTrigger : MonoBehaviour
{
    [SerializeField] private Backpack backpack;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PickupObject pickup))
        {
            if (!pickup.IsBackpack && pickup.ItemInfo.CanBeInBackpack)
                backpack.AddPickup(pickup);
        }
    }
}