using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public sealed class TowerLevelHUD : MonoBehaviour
{
    [SerializeField] private Health playerHealth = null;
    [SerializeField] private PlayerInventory inventory = null;
    [SerializeField] private PlayerInteractor interactor = null;
    [SerializeField] private EncounterObjective objective = null;
    [SerializeField] private TowerLevelFlow level = null;
    [SerializeField] private BossEncounter bossEncounter;
    private GUIStyle text;
    private GUIStyle title;
    private GUIStyle centered;
    private GUIStyle small;
    private PlayerDodge dodge;
    private bool reloading;
    private void Start() { if (playerHealth != null) dodge = playerHealth.GetComponent<PlayerDodge>(); }
    private void Update() { if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame) Restart(); }

    private void OnGUI()
    {
        if (playerHealth == null || objective == null) return;
        if (text == null)
        {
            text = new GUIStyle(GUI.skin.label) { fontSize = 16, wordWrap = true, normal = { textColor = Color.white } };
            title = new GUIStyle(text) { fontSize = 21, fontStyle = FontStyle.Bold };
            centered = new GUIStyle(text) { alignment = TextAnchor.MiddleCenter };
            small = new GUIStyle(text) { fontSize = 13 };
        }
        float scale = Mathf.Clamp(Screen.height / 900f, 0.75f, 1.5f);
        Matrix4x4 previous = GUI.matrix;
        GUI.matrix = Matrix4x4.Scale(new Vector3(scale, scale, 1f));
        float width = Screen.width / scale;
        float height = Screen.height / scale;
        Panel(new Rect(18, 18, 230, dodge != null ? 126 : 105));
        GUI.Label(new Rect(32, 27, 210, 25), "EL MAGO", title);
        GUI.color = new Color(0.15f, 0.12f, 0.2f);
        GUI.DrawTexture(new Rect(33, 61, 194, 12), Texture2D.whiteTexture);
        GUI.color = new Color(0.68f, 0.22f, 0.3f);
        GUI.DrawTexture(new Rect(33, 61, 194f * playerHealth.CurrentHealth / playerHealth.MaxHealth, 12), Texture2D.whiteTexture);
        GUI.color = Color.white;
        GUI.Label(new Rect(32, 78, 210, 24), "Vida " + playerHealth.CurrentHealth + "/" + playerHealth.MaxHealth
            + "   ·   Pociones " + (inventory != null ? inventory.PotionCount.ToString() : "0"), text);
        if (dodge != null)
        {
            string status = playerHealth.IsDead ? "Esquive no disponible" : dodge.IsBusy ? "Rodando..."
                : dodge.CooldownRemaining > 0f ? "Recuperando esquive..." : "Esquive listo · Espacio";
            GUI.Label(new Rect(32, 104, 210, 21), status, small);
            GUI.color = new Color(0.15f, 0.17f, 0.22f);
            GUI.DrawTexture(new Rect(33, 131, 194, 3), Texture2D.whiteTexture);
            GUI.color = new Color(0.55f, 0.75f, 0.85f);
            GUI.DrawTexture(new Rect(33, 131, 194 * dodge.CooldownProgress, 3), Texture2D.whiteTexture);
            GUI.color = Color.white;
        }
        float objectiveX = Mathf.Max(263f, width * 0.5f - 210f);
        Panel(new Rect(objectiveX, 18, 420, 76));
        GUI.Label(new Rect(objectiveX + 14, 24, 395, 27), level != null && level.IsOnTerrace ? "TERRAZA DEL ECLIPSE" : "TORRE DEL ECLIPSE", title);
        string goal = objective.Stage == EncounterStage.Fighting ? "Despejá la planta baja · Restantes: " + objective.RemainingEnemies
            : level != null && level.IsOnTerrace ? "Explorá la terraza" : "Ascendé por la escalera del santuario";
        if (bossEncounter != null && level != null && level.IsOnTerrace)
            goal = bossEncounter.Completed ? "El eclipse ha terminado" : bossEncounter.ArenaSealed ? "Derrotá al Custodio del Eclipse" : "Entrá en el círculo para invocar al Custodio";
        GUI.Label(new Rect(objectiveX + 14, 55, 395, 28), goal, text);
        if (bossEncounter != null && bossEncounter.Boss != null && !bossEncounter.Completed && !playerHealth.IsDead)
        {
            BossController boss = bossEncounter.Boss;
            float barWidth = Mathf.Min(550, width - 70); float barX = (width - barWidth) * 0.5f;
            Panel(new Rect(barX - 12, height - 177, barWidth + 24, 54));
            GUI.Label(new Rect(barX, height - 173, barWidth, 25), "CUSTODIO DEL ECLIPSE · FASE " + boss.Phase, centered);
            GUI.color = new Color(0.12f, 0.08f, 0.13f); GUI.DrawTexture(new Rect(barX, height - 141, barWidth, 8), Texture2D.whiteTexture);
            GUI.color = boss.Phase == 2 ? new Color(0.65f, 0.24f, 0.78f) : new Color(0.7f, 0.21f, 0.24f);
            GUI.DrawTexture(new Rect(barX, height - 141, barWidth * boss.Health.CurrentHealth / boss.Health.MaxHealth, 8), Texture2D.whiteTexture); GUI.color = Color.white;
        }
        GUI.Label(new Rect(22, height - 59, 750, 46), "WASD: moverse   ·   Mouse: apuntar   ·   Clic izq./der.: magia/bastón\nEspacio: esquivar   ·   E: interactuar   ·   Q: poción   ·   R: reiniciar", text);
        string prompt = interactor != null && interactor.enabled ? interactor.CurrentPrompt : "";
        if (inventory != null && Time.time < inventory.MessageUntil) prompt = inventory.Message;
        if (!string.IsNullOrEmpty(prompt) && !playerHealth.IsDead)
        {
            Panel(new Rect(width * 0.5f - 225, height - 112, 450, 43));
            GUI.Label(new Rect(width * 0.5f - 214, height - 108, 428, 35), prompt, centered);
        }
        if (playerHealth.IsDead)
        {
            Rect box = new Rect(width * 0.5f - 180, height * 0.5f - 65, 360, 130);
            Panel(box);
            GUI.Label(new Rect(box.x + 25, box.y + 16, 310, 38), "DERROTA", centered);
            if (GUI.Button(new Rect(box.x + 65, box.y + 67, 230, 36), bossEncounter != null && bossEncounter.CanRetry ? "Reintentar al jefe · R" : "Volver a intentarlo · R")) Restart();
        }
        else if (objective.Stage == EncounterStage.Completed)
        {
            Rect box = new Rect(width * 0.5f - 220, height * 0.5f - 80, 440, 160); Panel(box);
            GUI.Label(new Rect(box.x + 20, box.y + 16, 400, 36), "VICTORIA · EL ECLIPSE HA TERMINADO", centered);
            GUI.Label(new Rect(box.x + 20, box.y + 55, 400, 34), "La torre vuelve a quedar en silencio.", centered);
            if (GUI.Button(new Rect(box.x + 105, box.y + 104, 230, 36), "Volver a jugar · R")) Restart();
        }
        GUI.matrix = previous;
    }
    private static void Panel(Rect rect)
    {
        GUI.color = new Color(0.035f, 0.045f, 0.07f, 0.88f);
        GUI.DrawTexture(rect, Texture2D.whiteTexture);
        GUI.color = Color.white;
    }
    private void Restart()
    {
        if (reloading) return;
        if (bossEncounter != null && bossEncounter.Retry()) return;
        string scene = SceneManager.GetActiveScene().path;
        if (!Application.CanStreamedLevelBeLoaded(scene)) { Debug.LogError("El nivel debe estar en la lista de escenas.", this); return; }
        reloading = true;
        Time.timeScale = 1f;
        SceneManager.LoadSceneAsync(scene);
    }
}
