using FMODUnity;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(PlayerDodge), typeof(CharacterController))]
public sealed class PlayerDodgeAudio : MonoBehaviour
{
    [SerializeField] private EventReference rollEvent;
    [SerializeField] private EventReference landingEvent;
    [SerializeField, Range(0.5f, 2f)] private float rollPitch = 0.8f;
    private PlayerDodge dodge;
    private CharacterController controller;
    private void Awake() { dodge = GetComponent<PlayerDodge>(); controller = GetComponent<CharacterController>(); }
    private void OnEnable() { dodge.Started += OnRoll; dodge.MotionFinished += OnLanding; }
    private void OnDisable() { dodge.Started -= OnRoll; dodge.MotionFinished -= OnLanding; }
    private void OnRoll()
    {
        if (rollEvent.IsNull) return;
        var sound = RuntimeManager.CreateInstance(rollEvent);
        sound.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
        sound.setPitch(rollPitch);
        sound.setVolume(0.65f);
        sound.start(); sound.release();
    }
    private void OnLanding()
    {
        if (controller.enabled && controller.isGrounded) FmodOneShot.Play(landingEvent, transform.position);
    }
}
