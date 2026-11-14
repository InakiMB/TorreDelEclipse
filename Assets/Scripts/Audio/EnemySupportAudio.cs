using FMODUnity;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(EnemySupport))]
public sealed class EnemySupportAudio : MonoBehaviour
{
    [SerializeField] private EventReference chargeEvent;
    [SerializeField] private EventReference healEvent;
    [SerializeField] private EventReference hasteEvent;
    [SerializeField, Range(0.5f, 2f)] private float healingPitch = 0.9f;
    [SerializeField, Range(0.5f, 2f)] private float hastePitch = 1.25f;
    private EnemySupport support;
    private void Awake() => support = GetComponent<EnemySupport>();
    private void OnEnable()
    {
        support.CastStarted += OnCast;
        support.AllyHealed += OnHealed;
        support.HasteActivated += OnHaste;
    }
    private void OnDisable()
    {
        support.CastStarted -= OnCast;
        support.AllyHealed -= OnHealed;
        support.HasteActivated -= OnHaste;
    }
    private void OnCast(SupportSpell spell) => Play(chargeEvent, transform.position, spell == SupportSpell.Heal ? 1.1f : 0.85f, 0.55f);
    private void OnHealed(Health ally, int amount) => Play(healEvent, ally.transform.position, healingPitch, 0.8f);
    private void OnHaste() => Play(hasteEvent, transform.position, hastePitch, 0.7f);
    private static void Play(EventReference sound, Vector3 position, float pitch, float volume)
    {
        if (sound.IsNull) return;
        var instance = RuntimeManager.CreateInstance(sound);
        instance.set3DAttributes(RuntimeUtils.To3DAttributes(position));
        instance.setPitch(pitch);
        instance.setVolume(volume);
        instance.start();
        instance.release();
    }
}
