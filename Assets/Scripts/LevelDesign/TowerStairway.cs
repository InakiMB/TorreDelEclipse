using UnityEngine;

[DisallowMultipleComponent]
public sealed class TowerStairway : MonoBehaviour, IInteractable
{
    [SerializeField] private EncounterObjective objective = null;
    [SerializeField] private Transform destination = null;
    [SerializeField] private TopDownCamera view = null;
    [SerializeField] private bool ascending = true;
    public string Prompt => objective != null && objective.Stage == EncounterStage.Fighting
        ? "Ascenso sellado: despejá la planta baja"
        : ascending ? "Subir a la terraza" : "Bajar a la planta baja";

    public bool CanInteract(GameObject actor)
    {
        Health health = actor.GetComponent<Health>();
        return destination != null && objective != null && objective.isActiveAndEnabled
            && health != null && !health.IsDead && actor.GetComponent<CharacterController>() != null
            && objective.Stage != EncounterStage.Completed && objective.Stage != EncounterStage.Defeated;
    }
    public void Interact(GameObject actor)
    {
        if (!CanInteract(actor) || objective.Stage != EncounterStage.ExitUnlocked) return;
        CharacterController controller = actor.GetComponent<CharacterController>();
        PlayerMovement movement = actor.GetComponent<PlayerMovement>();
        bool wasMoving = movement != null && movement.enabled;
        if (wasMoving) movement.enabled = false;
        controller.enabled = false;
        actor.transform.SetPositionAndRotation(destination.position, destination.rotation);
        controller.enabled = true;
        if (wasMoving) movement.enabled = true;
        Physics.SyncTransforms();
        if (view != null) view.SnapToTarget();
    }
}
