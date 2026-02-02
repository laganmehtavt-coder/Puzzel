using UnityEngine;

public class GameLogger : MonoBehaviour
{
    [Header("Logging Settings")]
    [Tooltip("Toggle this OFF to disable all Debug logs across the game.")]
    public bool enableLogs = true;

    private static GameLogger instance;

    void Awake()
    {
        // Ensure only one instance exists
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        ApplyLogSettings();
    }

    public static void ApplyLogSettings()
    {
        if (instance != null)
            Debug.unityLogger.logEnabled = instance.enableLogs;
    }

    public void SetLogging(bool value)
    {
        enableLogs = value;
        Debug.unityLogger.logEnabled = enableLogs;
    }
}