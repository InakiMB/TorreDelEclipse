using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(PlayerDodge))]
public sealed class PlayerDodgePresentation : MonoBehaviour
{
    [SerializeField] private Animator visualAnimator;
    [SerializeField] private AnimationClip rollClip;
    [SerializeField] private Transform rigidHead;
    [SerializeField] private Transform headBone;
    private PlayerDodge dodge;
    private static readonly int Roll = Animator.StringToHash("Dodge");
    private static readonly int Rolling = Animator.StringToHash("Dodging");
    private static readonly int RollSpeed = Animator.StringToHash("RollSpeed");
    private static readonly int StaffAttack = Animator.StringToHash("StaffAttack");
    private void Awake()
    {
        dodge = GetComponent<PlayerDodge>();
        if (visualAnimator == null) visualAnimator = GetComponentInChildren<Animator>();
        // El FBX separa la cara rigida del cuerpo animado: debe seguir al hueso Head.
        if (rigidHead != null && headBone != null) rigidHead.SetParent(headBone, true);
    }
    private void OnEnable() { dodge.Started += OnStarted; dodge.Finished += OnFinished; }
    private void OnDisable()
    {
        dodge.Started -= OnStarted;
        dodge.Finished -= OnFinished;
        OnFinished();
    }
    private void LateUpdate()
    {
        if (visualAnimator != null) visualAnimator.SetBool(Rolling, dodge.IsRolling);
    }
    private void OnStarted()
    {
        if (visualAnimator == null) return;
        visualAnimator.applyRootMotion = false;
        visualAnimator.ResetTrigger(StaffAttack);
        visualAnimator.SetBool(Rolling, true);
        visualAnimator.SetFloat(RollSpeed, rollClip != null ? rollClip.length / dodge.ActiveDuration : 1f);
        visualAnimator.SetTrigger(Roll);
    }
    private void OnFinished()
    {
        if (visualAnimator == null) return;
        visualAnimator.ResetTrigger(Roll);
        visualAnimator.SetBool(Rolling, false);
    }
}
