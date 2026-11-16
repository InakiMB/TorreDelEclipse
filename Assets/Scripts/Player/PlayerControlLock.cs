using UnityEngine;

public static class PlayerControlLock
{
    public static void Freeze(GameObject player)
    {
        Disable<PlayerDodge>(player);
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

    public static void Restore(GameObject player)
    {
        Animator animator = player.GetComponentInChildren<Animator>();
        if (animator != null) { animator.enabled = true; animator.Rebind(); animator.Update(0f); }
        Enable<PlayerMovement>(player); Enable<PlayerAim>(player); Enable<PlayerSpellCaster>(player);
        Enable<PlayerStaffAttack>(player); Enable<PlayerInventory>(player); Enable<PlayerInteractor>(player);
        Enable<PlayerAnimation>(player); Enable<PlayerDodge>(player);
    }
    private static void Enable<T>(GameObject player) where T : Behaviour
    {
        T component = player.GetComponent<T>();
        if (component != null) component.enabled = true;
    }
}
