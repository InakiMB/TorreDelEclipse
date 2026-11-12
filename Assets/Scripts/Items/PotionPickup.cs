using UnityEngine;

[DisallowMultipleComponent]
public sealed class PotionPickup : MonoBehaviour, IInteractable
{
    private bool collected;
    public string Prompt => "Recoger poción";

    public bool CanInteract(GameObject actor)
    {
        return !collected && actor.GetComponent<PlayerInventory>() != null;
    }

    public void Interact(GameObject actor)
    {
        if (!CanInteract(actor)) return;
        PlayerInventory inventory = actor.GetComponent<PlayerInventory>();
        if (!inventory.TryAddPotion(1)) return;
        collected = true;
        Destroy(gameObject);
    }
}
