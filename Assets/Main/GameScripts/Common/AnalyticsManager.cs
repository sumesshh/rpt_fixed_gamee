using UnityEngine;
using Firebase;
using Firebase.Analytics;
using Firebase.Extensions;
using UnityEngine.SceneManagement;

public class AnalyticsManager : MonoBehaviour
{
    public static AnalyticsManager Instance { get; private set; }

    public string currentLevelName = "None";
    private bool firebaseReady = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeFirebase();
    }

    private void InitializeFirebase()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
                firebaseReady = true;

                Debug.Log("Firebase Ready");
            }
            else
            {
                Debug.LogError("Firebase dependency error: " + task.Result);
            }
        });
    }

    void OnApplicationQuit()
    {
        LogGameExit();
    }

    void OnApplicationPause(bool pause)
    {
        if (pause)
            LogGameExit();
    }

    public void LogGameExit()
    {
        if (!firebaseReady) return;

        string scene = SceneManager.GetActiveScene().name;

        FirebaseAnalytics.LogEvent("game_exit",
            new Parameter("scene_name", scene),
            new Parameter("level_name", currentLevelName)
        );
    }

    public void HomeButtonPressed()
    {
        if (!firebaseReady) return;

        FirebaseAnalytics.LogEvent("home_pressed");
    }

    public void ReplayGamePressed()
    {
        if (!firebaseReady) return;

        FirebaseAnalytics.LogEvent("replay_game_pressed");
    }

    public void ReplayLevel()
    {
        if (!firebaseReady) return;

        FirebaseAnalytics.LogEvent("replay_level_pressed");
    }

    public void LoginDenied()
    {
        if (!firebaseReady) return;

        FirebaseAnalytics.LogEvent("login_denied");
    }

    public void PaymentDenied()
    {
        if (!firebaseReady) return;

        FirebaseAnalytics.LogEvent("payment_denied");
    }

    public void SetLevel(string levelName)
    {
        currentLevelName = levelName;
    }
}