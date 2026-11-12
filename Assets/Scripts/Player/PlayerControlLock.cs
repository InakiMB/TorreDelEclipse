using UnityEngine;

public static class PlayerControlLock
{
    public static void Freeze(GameObject player)
    {
        Disable<PlayerMovement>(player);
        Disable<PlayerAim>(player);
        Disable<PlayerSpellCaster>(player);
        Disable<PlayerStaffAttack>(player);
        Disable<PlayerInventory>(player);
        Disable<PlayerInteractor>(player);
        Disable<PlayerAnimation>(player);
        Animator animator = player.GetComponentInChildren<Animator>();
        if (animator != null && animator.runtimeAnimatorController != null) animator.SetFloat("Speed", 0f);
    }

    private static void Disable<T>(GameObject player) where T : Behaviour
    {
        T component = player.GetComponent<T>();
        if (component != null) component.enabled = false;
    }
}
