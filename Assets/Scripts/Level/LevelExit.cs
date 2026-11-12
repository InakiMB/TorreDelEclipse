using UnityEngine;

[DisallowMultipleComponent]
public sealed class LevelExit : MonoBehaviour, IInteractable
{
    [SerializeField] private EncounterObjective objective = null;
    public string Prompt => objective != null && objective.Stage == EncounterStage.ExitUnlocked
        ? "Completar recorrido de prueba" : "Salida bloqueada: eliminá a los enemigos";

    public bool CanInteract(GameObject actor)
    {
        Health health = actor.GetComponent<Health>();
        return objective != null && objective.isActiveAndEnabled && health != null && !health.IsDead
            && objective.Stage != EncounterStage.Completed && objective.Stage != EncounterStage.Defeated;
    }

    public void Interact(GameObject actor)
    {
        if (CanInteract(actor)) objective.TryComplete();
    }
}
