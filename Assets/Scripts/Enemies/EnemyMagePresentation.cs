using UnityEngine;
using UnityEngine.AI;

[DisallowMultipleComponent]
[RequireComponent(typeof(EnemyMage), typeof(Health), typeof(NavMeshAgent))]
public sealed class EnemyMagePresentation : MonoBehaviour
{
    [SerializeField] private Animator visualAnimator;
    [SerializeField] private Transform chargeOrb;
    [SerializeField] private Light chargeLight;
    private EnemyMage mage;
    private Health health;
    private NavMeshAgent agent;
    private static readonly int Speed = Animator.StringToHash("Speed");
    private static readonly int Cast = Animator.StringToHash("Cast");
    private static readonly int CastSpeed = Animator.StringToHash("CastSpeed");
    private static readonly int RunSpeed = Animator.StringToHash("RunSpeed");
    private static readonly int Dead = Animator.StringToHash("Dead");
    private void Awake()
    {
        mage = GetComponent<EnemyMage>(); health = GetComponent<Health>(); agent = GetComponent<NavMeshAgent>();
        if (visualAnimator == null) visualAnimator = GetComponentInChildren<Animator>();
        if (visualAnimator != null) visualAnimator.applyRootMotion = false;
        if (chargeOrb != null) chargeOrb.gameObject.SetActive(false);
    }
    private void OnEnable() { mage.CastStarted += OnCast; health.Died += OnDeath; }
    private void OnDisable() { mage.CastStarted -= OnCast; health.Died -= OnDeath; }
    private void LateUpdate()
    {
        if (visualAnimator != null)
        {
            visualAnimator.SetFloat(Speed, !health.IsDead && !mage.IsCasting && agent.enabled && agent.isOnNavMesh && !agent.isStopped
                ? agent.velocity.magnitude : 0f);
            visualAnimator.SetFloat(CastSpeed, mage.CastPlaybackSpeed);
            visualAnimator.SetFloat(RunSpeed, Mathf.Clamp(agent.velocity.magnitude / 2.5f, 0.6f, 1.5f));
        }
        float charge = mage.ChargeProgress;
        if (chargeOrb != null)
        {
            chargeOrb.gameObject.SetActive(!health.IsDead && mage.IsCasting && charge > 0f);
            chargeOrb.localScale = Vector3.one * Mathf.Lerp(0.1f, 0.45f, charge);
        }
        if (chargeLight != null) chargeLight.intensity = !health.IsDead ? charge * 2.5f : 0f;
    }
    private void OnCast()
    {
        if (visualAnimator == null) return;
        visualAnimator.SetFloat(CastSpeed, mage.CastPlaybackSpeed);
        visualAnimator.SetTrigger(Cast);
    }
    private void OnDeath()
    {
        if (visualAnimator != null) { visualAnimator.ResetTrigger(Cast); visualAnimator.SetBool(Dead, true); }
        if (chargeOrb != null) chargeOrb.gameObject.SetActive(false);
        if (chargeLight != null) chargeLight.intensity = 0f;
    }
}
