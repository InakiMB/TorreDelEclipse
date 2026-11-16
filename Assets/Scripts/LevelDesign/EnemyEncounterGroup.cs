using UnityEngine;

[DisallowMultipleComponent]
public sealed class EnemyEncounterGroup : MonoBehaviour
{
    [SerializeField] private Health playerHealth;
    [SerializeField] private BoxCollider activationArea;
    [SerializeField] private Health[] members = System.Array.Empty<Health>();
    public bool IsAlerted { get; private set; }
    private void OnEnable() { foreach (Health member in members) if (member != null) member.Damaged += OnMemberDamaged; }
    private void Update()
    {
        if (!IsAlerted && playerHealth != null && !playerHealth.IsDead && activationArea != null && activationArea.bounds.Contains(playerHealth.transform.position)) Alert();
    }
    private void OnMemberDamaged(int amount) => Alert();
    private void Alert()
    {
        if (IsAlerted || playerHealth == null || playerHealth.IsDead) return;
        IsAlerted = true;
        foreach (Health member in members)
        {
            if (member == null || member.IsDead) continue;
            if (member.TryGetComponent(out EnemyChaser warrior)) warrior.AlertToPlayer();
            else if (member.TryGetComponent(out EnemyMage mage)) mage.AlertToPlayer();
            else if (member.TryGetComponent(out EnemySupport support)) support.AlertToPlayer();
        }
    }
    private void OnDisable() { foreach (Health member in members) if (member != null) member.Damaged -= OnMemberDamaged; }
}
