using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public sealed class CombatPrototypeHUD : MonoBehaviour
{
    [SerializeField] private Health playerHealth = null;
    [SerializeField] private Health[] enemyHealth = new Health[0];

    private GUIStyle labelStyle;
    private bool reloading;

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
        {
            Restart();
        }
    }

    private void OnGUI()
    {
        if (playerHealth == null) return;
        if (labelStyle == null)
        {
            labelStyle = new GUIStyle(GUI.skin.label) { fontSize = 18, wordWrap = true };
        }

        float panelHeight = 155f + enemyHealth.Length * 25f;
        GUI.Box(new Rect(12f, 12f, 340f, panelHeight), GUIContent.none);
        GUILayout.BeginArea(new Rect(24f, 22f, 315f, panelHeight - 20f));
        GUILayout.Label("Vida: " + playerHealth.CurrentHealth + " / " + playerHealth.MaxHealth, labelStyle);
        int alive = 0;
        for (int i = 0; i < enemyHealth.Length; i++)
        {
            Health enemy = enemyHealth[i];
            bool enemyAlive = enemy != null && !enemy.IsDead;
            if (enemyAlive) alive++;
            string status = enemyAlive ? enemy.CurrentHealth + " / " + enemy.MaxHealth : "eliminado";
            GUILayout.Label("Enemigo " + (i + 1) + ": " + status, labelStyle);
        }

        if (playerHealth.IsDead)
        {
            GUILayout.Label("Has caido.", labelStyle);
        }
        else if (enemyHealth.Length > 0 && alive == 0)
        {
            GUILayout.Label("Zona despejada.", labelStyle);
        }

        GUILayout.Label("Clic izquierdo: hechizo. R: reiniciar.", labelStyle);
        if (GUILayout.Button("Reiniciar", GUILayout.Height(28f))) Restart();
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
