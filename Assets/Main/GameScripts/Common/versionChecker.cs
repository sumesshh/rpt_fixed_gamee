using System.Collections;
using System.Collections.Generic;
//using UnityEngine;
//using Unity.RemoteConfig;
//using Unity.Services.RemoteConfig;
using UnityEngine;
using Unity.Services.RemoteConfig;
using Unity.Services.Authentication;
using Unity.Services.Core;
using System.Threading.Tasks;
public class versionChecker : MonoBehaviour
{
    
    public GameObject ErrorPanel;
    struct userAttributes { }  // Required for Remote Config
    struct appAttributes { }

    private float minimumVersion;

    private string appLink = "https://play.google.com/store/apps/details?id=com.RextechStudios.RockPaperTuition";
    // Start is called before the first frame update
    void Start()
    {
        //ConfigManager.FetchCompleted += ApplyRemoteSettings;
        // ConfigManager.FetchConfigs(new userAttributes(), new appAttributes());

        // RuntimeConfig remoteConfig = ConfigManager.appConfig;
        // ConfigManager.FetchCompleted += ApplyRemoteSettings;
        // ConfigManager.FetchConfigs(new userAttributes(), new appAttributes());
        ErrorPanel.SetActive(false);
        RemoteConfigService.Instance.FetchCompleted += ApplyRemoteConfig;
        RemoteConfigService.Instance.FetchConfigsAsync(new userAttributes(), new appAttributes());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ApplyRemoteConfig(ConfigResponse configResponse) {

        minimumVersion = RemoteConfigService.Instance.appConfig.GetFloat("minimum_version", 1.0f);

        //string currentVersion = Application.version;

        float currentVersion = ParseVersionToFloat(Application.version);

        if (currentVersion < minimumVersion)
        {
            ShowUpdatePopup();
        }
        else {
            Debug.Log("App is up to date");
        }
    }

    bool IsOutdated(string userVersion, string latestVersion)
    {
        var userParts = userVersion.Split('.');
        var latestParts = latestVersion.Split('.');

        int length = Mathf.Max(userParts.Length, latestParts.Length);
        for (int i = 0; i < length; i++)
        {
            int userPart = i < userParts.Length ? int.Parse(userParts[i]) : 0;
            int latestPart = i < latestParts.Length ? int.Parse(latestParts[i]) : 0;

            if (userPart < latestPart) return true;
            if (userPart > latestPart) return false;
        }

        return false; // Equal versions
    }

    float ParseVersionToFloat(string version)
    {
        // Extracts the major.minor part and converts it to float
        // Example: "1.2.3" → 1.2
        string[] parts = version.Split('.');
        if (parts.Length >= 2)
        {
            string majorMinor = parts[0] + "." + parts[1];
            if (float.TryParse(majorMinor, out float result))
                return result;
        }

        // Fallback if version format is unexpected
        return 0f;
    }
    void ShowUpdatePopup()
    {
        Debug.Log("The app is not up to date");
        ErrorPanel.SetActive(true);

        //foreach(CanvasGroup page in pages) {
        //    if (page != null)
        //    {
        //        page.interactable = false;
        //        page.blocksRaycasts = false;
        //    }
        //}
    }

    public void GoToApp() { 
        Application.OpenURL(appLink);
    }
}
