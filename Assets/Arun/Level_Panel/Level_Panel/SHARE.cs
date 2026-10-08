using UnityEngine;

public class ShareButton : MonoBehaviour
{
    // ===== SHARE DATA =====
    private string subject = "SHARE";
    private string body = "https://play.google.com/store/apps/details?id=com.DefaultCompany.RPT";

    // ===== SHARE BUTTON =====
    public void OnShareButtonClick()
    {
#if UNITY_ANDROID
        ShareOnAndroid();
#elif UNITY_IOS
        Debug.Log("iOS Share disabled to avoid build error");
#endif
    }

    // ===== ANDROID SHARE =====
    private void ShareOnAndroid()
    {
        using (AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent"))
        using (AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent"))
        {
            intentObject.Call<AndroidJavaObject>("setAction",
                intentClass.GetStatic<string>("ACTION_SEND"));
            intentObject.Call<AndroidJavaObject>("setType", "text/plain");
            intentObject.Call<AndroidJavaObject>("putExtra",
                intentClass.GetStatic<string>("EXTRA_SUBJECT"), subject);
            intentObject.Call<AndroidJavaObject>("putExtra",
                intentClass.GetStatic<string>("EXTRA_TEXT"), body);

            using (AndroidJavaClass unityPlayer =
                   new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            using (AndroidJavaObject currentActivity =
                   unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            using (AndroidJavaObject chooser =
                   intentClass.CallStatic<AndroidJavaObject>(
                       "createChooser", intentObject, "Share Via"))
            {
                currentActivity.Call("startActivity", chooser);
            }
        }
    }

    // ===== RATE US BUTTON =====
    public void OnRateUsClick()
    {
#if UNITY_ANDROID
        Application.OpenURL("market://details?id=com.DefaultCompany.RPT");
#elif UNITY_IOS
        // Replace YOUR_APP_ID after App Store creates it
        Application.OpenURL("https://apps.apple.com/app/idYOUR_APP_ID");
#endif
    }
}
