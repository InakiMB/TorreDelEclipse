using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(BossController))]
public sealed class BossPresentation : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Transform wings;
    [SerializeField] private Transform torso;
    [SerializeField] private Transform leftWing;
    [SerializeField] private Transform rightWing;
    [SerializeField] private LineRenderer warning;
    [SerializeField] private ParticleSystem burst;
    [SerializeField] private Light runeLight;
    [SerializeField] private AnimationClip[] clips;
    private BossController boss;
    private BossAction previous;
    private Quaternion leftRest, rightRest;
    private float wingAmount;
    private void Awake()
    {
        boss = GetComponent<BossController>();
        if (wings != null && torso != null) wings.SetParent(torso, true);
        if (leftWing != null) leftRest = leftWing.localRotation;
        if (rightWing != null) rightRest = rightWing.localRotation;
        if (wings != null) wings.gameObject.SetActive(false);
        if (warning != null) warning.enabled = false;
    }
    private void OnEnable() { boss.ActionChanged += OnAction; boss.Cue += OnCue; }
    private void OnDisable() { boss.ActionChanged -= OnAction; boss.Cue -= OnCue; if (warning != null) warning.enabled = false; }
    private void OnAction(BossAction action)
    {
        previous = action;
        if (animator == null) return;
        string clip = action == BossAction.Hunt ? "BossIdle" : action == BossAction.Dying ? "BossDeath"
            : action == BossAction.Summon ? "BossSummon" : action == BossAction.Transform ? "BossTransform"
            : action == BossAction.Combo ? "BossCombo" : action == BossAction.Slam ? "BossSlam"
            : action == BossAction.Charge ? "BossIdle" : action == BossAction.Leap ? "BossLeap"
            : action == BossAction.Fan || action == BossAction.Rings || action == BossAction.Spiral ? "BossCast" : "BossIdle";
        Play(clip, boss.ActionDuration);
        if (action == BossAction.Transform && wings != null) wings.gameObject.SetActive(true);
    }
    private void Play(string name, float duration)
    {
        animator.applyRootMotion = false; animator.speed = 1;
        if (duration > 0.01f)
            foreach (AnimationClip clip in clips) if (clip != null && clip.name.EndsWith(name)) { animator.speed = clip.length / duration; break; }
        animator.CrossFadeInFixedTime(name, 0.08f, 0, 0);
    }
    private void LateUpdate()
    {
        if (boss.Action == BossAction.Hunt && animator != null)
        {
            string state = boss.MoveSpeed > 0.2f ? "BossWalk" : "BossIdle";
            if (!animator.GetCurrentAnimatorStateInfo(0).IsName(state) && !animator.IsInTransition(0)) Play(state, 0);
            animator.speed = state == "BossWalk" ? Mathf.Clamp(boss.MoveSpeed / 2.8f, 0.5f, 1.6f) : 1;
        }
        float desired = boss.Phase == 2 && boss.Action != BossAction.Dead && boss.Action != BossAction.Defeated ? 1f : 0f;
        wingAmount = Mathf.MoveTowards(wingAmount, desired, Time.deltaTime * 0.7f);
        if (wings != null)
        {
            if (desired > 0) wings.gameObject.SetActive(true);
            float flap = Mathf.Sin(Time.time * 2.5f) * 4f;
            if (leftWing != null) leftWing.localRotation = leftRest * Quaternion.Euler(0, (1 - wingAmount) * 65f + flap, (1 - wingAmount) * -20f);
            if (rightWing != null) rightWing.localRotation = rightRest * Quaternion.Euler(0, -(1 - wingAmount) * 65f - flap, (1 - wingAmount) * 20f);
            if (boss.Action == BossAction.Dying) wings.localScale = Vector3.one * Mathf.Lerp(1f, 0.05f, boss.ActionProgress);
        }
        if (runeLight != null) { runeLight.intensity = boss.Phase == 2 ? 2f : 0.7f; runeLight.color = boss.Phase == 2 ? new Color(0.65f, 0.16f, 1f) : new Color(1f, 0.55f, 0.17f); }
        DrawWarning();
    }
    private void DrawWarning()
    {
        if (warning == null) return;
        bool charge = boss.Action == BossAction.Charge && boss.ActionElapsed < 0.85f;
        bool circle = boss.Action == BossAction.Slam && boss.ActionElapsed < 1.4f || boss.Action == BossAction.Leap && boss.ActionElapsed < 1.6f;
        warning.enabled = charge || circle;
        if (!warning.enabled) return;
        float floor = transform.position.y - GetComponent<UnityEngine.AI.NavMeshAgent>().baseOffset + 0.08f;
        if (circle && UnityEngine.AI.NavMesh.SamplePosition(boss.AttackPoint, out UnityEngine.AI.NavMeshHit ground, 4f, UnityEngine.AI.NavMesh.AllAreas)) floor = ground.position.y + .08f;
        warning.startColor = warning.endColor = boss.Phase == 2 ? new Color(0.9f, 0.3f, 1f) : new Color(1f, 0.65f, 0.16f);
        warning.positionCount = charge ? 2 : 48; warning.loop = !charge;
        if (charge)
        {
            Vector3 start = transform.position; start.y = floor;
            warning.SetPosition(0, start); warning.SetPosition(1, start + boss.AttackDirection * 8f);
        }
        else for (int i = 0; i < 48; i++)
        {
            float angle = i * Mathf.PI * 2 / 48; Vector3 point = boss.AttackPoint;
            point.y = floor; warning.SetPosition(i, point + new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle)) * boss.SlamRadius);
        }
    }
    private void OnCue(BossCue cue, Vector3 point)
    {
        if (cue == BossCue.Charge && animator != null) Play("BossCharge", .55f);
        if (burst != null && (cue == BossCue.Spawn || cue == BossCue.Transform || cue == BossCue.Slam || cue == BossCue.Death))
        { burst.transform.position = point; burst.Play(); }
    }
}
