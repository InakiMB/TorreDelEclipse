using UnityEngine;

[DisallowMultipleComponent]
public sealed class BossPlayerProtection : MonoBehaviour, IDamageGuard
{
    private BossEncounter encounter;
    public bool PreventsDamage => encounter != null && encounter.PreventsDamage;
    public void Bind(BossEncounter source) => encounter = source;
}
