using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public sealed class CombatPrototypeHUD : MonoBehaviour
{
    [SerializeField] private Health playerHealth = null;
    [SerializeField] private Health[] enemyHealth = new Health[0];
    [SerializeField] private PlayerInventory playerInventory = null;
    [SerializeField] private PlayerInteractor playerInteractor = null;
    [SerializeField] private EncounterObjective objective = null;
    private GUIStyle labelStyle;
    private bool reloading;
    private Vector2 scroll;

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame) Restart();
    }

    private void OnGUI()
    {
        if (playerHealth == null) return;
        if (labelStyle == null) labelStyle = new GUIStyle(GUI.skin.label) { fontSize = 16, wordWrap = true };

        float width = Mathf.Min(370f, Screen.width - 24f);
        float height = Mathf.Min(440f, Screen.height - 24f);
        GUI.Box(new Rect(12f, 12f, width, height), GUIContent.none);
        GUILayout.BeginArea(new Rect(24f, 22f, width - 24f, height - 20f));
        scroll = GUILayout.BeginScrollView(scroll);
        GUILayout.Label("Vida: " + playerHealth.CurrentHealth + " / " + playerHealth.MaxHealth, labelStyle);
        if (playerInventory != null)
            GUILayout.Label("Pociones: " + playerInventory.PotionCount + " / " + playerInventory.MaxPotions, labelStyle);

        int alive = 0;
        for (int i = 0; i < enemyHealth.Length; i++)
        {
            Health enemy = enemyHealth[i];
            bool enemyAlive = enemy != null && !enemy.IsDead;
            if (enemyAlive) alive++;
            string status = enemyAlive ? enemy.CurrentHealth + " / " + enemy.MaxHealth : "eliminado";
            GUILayout.Label("Enemigo " + (i + 1) + ": " + status, labelStyle);
        }

        if (playerHealth.IsDead) GUILayout.Label("Derrota. Reiniciá para volver a intentarlo.", labelStyle);
        else if (objective != null)
        {
            switch (objective.Stage)
            {
                case EncounterStage.Fighting:
                    GUILayout.Label("Objetivo: despejar la sala. Restantes: " + objective.RemainingEnemies, labelStyle);
                    break;
                case EncounterStage.ExitUnlocked:
                    GUILayout.Label("Salida habilitada. Acercate al marcador y pulsá E.", labelStyle);
                    break;
                case EncounterStage.Completed:
                    GUILayout.Label("Victoria del prototipo. Recorrido completado.", labelStyle);
                    break;
            }
        }
        else if (enemyHealth.Length > 0 && alive == 0) GUILayout.Label("Zona despejada.", labelStyle);

        if (playerInteractor != null && playerInteractor.enabled && !string.IsNullOrEmpty(playerInteractor.CurrentPrompt))
            GUILayout.Label(playerInteractor.CurrentPrompt, labelStyle);
        if (playerInventory != null && Time.time < playerInventory.MessageUntil)
            GUILayout.Label(playerInventory.Message, labelStyle);

        GUILayout.Space(8f);
        GUILayout.Label("Clic izq.: hechizo. Clic der.: bastón.\nE: interactuar. Q: poción. R: reiniciar.", labelStyle);
        if (GUILayout.Button("Reiniciar", GUILayout.Height(28f))) Restart();
        GUILayout.EndScrollView();
        GUILayout.EndArea();
    }

    private void Restart()
    {
        if (reloading) return;
        string sceneName = SceneManager.GetActiveScene().name;
        if (!Application.CanStreamedLevelBeLoaded(sceneName))
        {
            Debug.LogError("Agrega la escena actual a la Scene List del Build Profile antes de reiniciar.", this);
            return;
        }
        reloading = true;
        Time.timeScale = 1f;
        SceneManager.LoadSceneAsync(sceneName);
    }
}
