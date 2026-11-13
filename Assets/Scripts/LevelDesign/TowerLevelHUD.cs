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
    private GUIStyle text;
    private GUIStyle title;
    private GUIStyle centered;
    private bool reloading;
    private void Update() { if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame) Restart(); }

    private void OnGUI()
    {
        if (playerHealth == null || objective == null) return;
        if (text == null)
        {
            text = new GUIStyle(GUI.skin.label) { fontSize = 16, wordWrap = true, normal = { textColor = Color.white } };
            title = new GUIStyle(text) { fontSize = 21, fontStyle = FontStyle.Bold };
            centered = new GUIStyle(text) { alignment = TextAnchor.MiddleCenter };
        }
        float scale = Mathf.Clamp(Screen.height / 900f, 0.75f, 1.5f);
        Matrix4x4 previous = GUI.matrix;
        GUI.matrix = Matrix4x4.Scale(new Vector3(scale, scale, 1f));
        float width = Screen.width / scale;
        float height = Screen.height / scale;
        Panel(new Rect(18, 18, 230, 105));
        GUI.Label(new Rect(32, 27, 210, 25), "EL MAGO", title);
        GUI.color = new Color(0.15f, 0.12f, 0.2f);
        GUI.DrawTexture(new Rect(33, 61, 194, 12), Texture2D.whiteTexture);
        GUI.color = new Color(0.68f, 0.22f, 0.3f);
        GUI.DrawTexture(new Rect(33, 61, 194f * playerHealth.CurrentHealth / playerHealth.MaxHealth, 12), Texture2D.whiteTexture);
        GUI.color = Color.white;
        GUI.Label(new Rect(32, 78, 210, 24), "Vida " + playerHealth.CurrentHealth + "/" + playerHealth.MaxHealth
            + "   ·   Pociones " + (inventory != null ? inventory.PotionCount.ToString() : "0"), text);
        float objectiveX = Mathf.Max(263f, width * 0.5f - 210f);
        Panel(new Rect(objectiveX, 18, 420, 76));
        GUI.Label(new Rect(objectiveX + 14, 24, 395, 27), level != null && level.IsOnTerrace ? "TERRAZA DEL ECLIPSE" : "TORRE DEL ECLIPSE", title);
        string goal = objective.Stage == EncounterStage.Fighting ? "Despejá la planta baja · Restantes: " + objective.RemainingEnemies
            : level != null && level.IsOnTerrace ? "Explorá la terraza" : "Ascendé por la escalera del santuario";
        GUI.Label(new Rect(objectiveX + 14, 55, 395, 28), goal, text);
        GUI.Label(new Rect(22, height - 59, 670, 46), "WASD: moverse   ·   Mouse: apuntar   ·   Clic izq./der.: magia/bastón\nE: interactuar   ·   Q: poción   ·   R: reiniciar", text);
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
            if (GUI.Button(new Rect(box.x + 65, box.y + 67, 230, 36), "Volver a intentarlo · R")) Restart();
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
        string scene = SceneManager.GetActiveScene().path;
        if (!Application.CanStreamedLevelBeLoaded(scene)) { Debug.LogError("El nivel debe estar en la lista de escenas.", this); return; }
        reloading = true;
        Time.timeScale = 1f;
        SceneManager.LoadSceneAsync(scene);
    }
}
