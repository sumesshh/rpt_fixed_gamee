using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;
using Unity.VisualScripting;
using UnityEngine.Analytics;

public class UserRegistration : MonoBehaviour
{
    // Replace this with your actual Firebase token
    //BELOW CODE WORKS FOR GET
    //private string firebaseToken = "eyJhbGciOiJSUzI1NiIsImtpZCI6IjY3ZDhjZWU0ZTYwYmYwMzYxNmM1ODg4NTJiMjA5MTZkNjRjMzRmYmEiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL3NlY3VyZXRva2VuLmdvb2dsZS5jb20vcm9jay1wYXBlci10dWl0aW9uIiwiYXVkIjoicm9jay1wYXBlci10dWl0aW9uIiwiYXV0aF90aW1lIjoxNzQ3OTg4MDE4LCJ1c2VyX2lkIjoid0hjN3d6aTRrWVdzYnI3Q0wxTzVrMGNJMWt0MSIsInN1YiI6IndIYzd3emk0a1lXc2JyN0NMMU81azBjSTFrdDEiLCJpYXQiOjE3NDc5ODgwMjAsImV4cCI6MTc0Nzk5MTYyMCwicGhvbmVfbnVtYmVyIjoiKzkxOTUzOTUwNjUwMiIsImZpcmViYXNlIjp7ImlkZW50aXRpZXMiOnsicGhvbmUiOlsiKzkxOTUzOTUwNjUwMiJdfSwic2lnbl9pbl9wcm92aWRlciI6InBob25lIn19.jAmxjTTzuIBqyLWtWjDbs2dYU4-m5zw_k_ReG9hA2ICfmXIPP7z6dwJ6eDpnF7eTtNMkFMsDV3gF_KrZZJcbBOuOzynS8vXcNlUQO3buuNDNGpWsyD0I6gR8hoO18EgzdmqRc1s2UcdY8uvbRhNYoelGEDkes9ndSaZi5FN_eOxOnzn6p91FKcuTVPmouMnUr5xos0qEH4bDR0vt65vYjNalfa2VuVfUu9nrTmVGquUNf1-Ne5RbpcBRZ9ploHnBK22wuPzUUTz6aAzvkS8rvN9SRLUTHg56ZkM2Rw7DiWqFSTiYTczROMXKBA7ipAyK0LMZrKeyTjIswA1RQpk-Lg";
    private string apiUrl = "http://13.60.52.226/api/users/me/";

    private string purchaseUrl = "http://13.60.52.226/api/purchase/";



    // Replace this with your fresh Firebase ID token
    //private string firebaseToken = "eyJhbGciOiJSUzI1NiIsImtpZCI6IjY3ZDhjZWU0ZTYwYmYwMzYxNmM1ODg4NTJiMjA5MTZkNjRjMzRmYmEiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL3NlY3VyZXRva2VuLmdvb2dsZS5jb20vcm9jay1wYXBlci10dWl0aW9uIiwiYXVkIjoicm9jay1wYXBlci10dWl0aW9uIiwiYXV0aF90aW1lIjoxNzQ3OTk0OTg3LCJ1c2VyX2lkIjoid0hjN3d6aTRrWVdzYnI3Q0wxTzVrMGNJMWt0MSIsInN1YiI6IndIYzd3emk0a1lXc2JyN0NMMU81azBjSTFrdDEiLCJpYXQiOjE3NDc5OTQ5ODgsImV4cCI6MTc0Nzk5ODU4OCwicGhvbmVfbnVtYmVyIjoiKzkxOTUzOTUwNjUwMiIsImZpcmViYXNlIjp7ImlkZW50aXRpZXMiOnsicGhvbmUiOlsiKzkxOTUzOTUwNjUwMiJdfSwic2lnbl9pbl9wcm92aWRlciI6InBob25lIn19.ekM5wsULC08nEAmxtX0K2a3lwmocOxaXeQHHE-2gF9fAt-Itj8BGfKmZDZ_WuVttLWI4wb5s2TSSJQYfoYDdqJEw3GPFDbyMj5aNkUUglTJxjCtB-Tzig3j6BgdiIoQZvkw4eeMJkUAupsDH6h6hPN-6BNx0U55tCBfEnSpaF7KetOBUa37mDZunFWtGYZQ1j4Vkvr5j9q5e60taP4yn1Dg-kI9UFl240r9HVns3RgJYqpWyNE3TakhqXKANaZB39Ekg4522RClAEyD2xVu8hMb09OQMxcxTsZYVlrKM64pH5_qA1tblHEnMy9TbpM8ltettY9PdXD2sA-avfCdkxg";

    //private string firebaseToken = "eyJhbGciOiJSUzI1NiIsImtpZCI6IjZlODk1YzQ3YTA0YzVmNmRlMzExMmFmZjE2ODFhMzUwNzdkMWNjZDQiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL3NlY3VyZXRva2VuLmdvb2dsZS5jb20vcm9jay1wYXBlci10dWl0aW9uIiwiYXVkIjoicm9jay1wYXBlci10dWl0aW9uIiwiYXV0aF90aW1lIjoxNzQ5MTk0MTIyLCJ1c2VyX2lkIjoickVZZ1BYVEhzNVpQbXBoZDN4ZFZCVEZ2ZEJ6MiIsInN1YiI6InJFWWdQWFRIczVaUG1waGQzeGRWQlRGdmRCejIiLCJpYXQiOjE3NDkxOTQxMjMsImV4cCI6MTc0OTE5NzcyMywicGhvbmVfbnVtYmVyIjoiKzkxOTEzMzc3MTE1MyIsImZpcmViYXNlIjp7ImlkZW50aXRpZXMiOnsicGhvbmUiOlsiKzkxOTEzMzc3MTE1MyJdfSwic2lnbl9pbl9wcm92aWRlciI6InBob25lIn19.fPkjMLok_50VFtBn_BMXPEY6730o2kG_swzU-WwmqVuK75cF5b3HCBOp4aNjGpXGk_-41Bw_KWgOsqPVuSvUamMe1EiDVtZr_uIavDQnIh6IDKA8Gei43IW3JP-fgE8yRzdRTdNOgfJaAVVwWXI3UaXi0gx6cKReRaFp_P35LfjGzeHZYFyz1Q1-tUxHPegpn1kmhamXwuyJO1Mle1svg9abql-vnWqrjp6iBQhPY8IS5FYi1wfj1yauRH2qzhxpKCBwbtJWvG3Yl4RJjzTSTgJdjPVjDSu9og58TqqJrfTpRjCoR8pZwBe6izhZZPIrM8BB97x3p-ogX8e4QLreHA";

    public PanelShift panelScript;
    private void Awake()
    {
        print("ABCD");
    }


    //void Start_GetUserData()
    //{
    //    print("User registration called");
    //    print(firebaseToken);
    //    StartCoroutine(GetUserData(firebaseToken));
    //}

    void Start()
    {
        //StartCoroutine(UpdatePurchase(500,"Test"));
    }



    public IEnumerator GetUserData(string firebaseToken)
    {
        string json;
        print("Case1");
        UnityWebRequest request = UnityWebRequest.Get(apiUrl);

        print("Case2");

        // Add Firebase token to header
        request.SetRequestHeader("Authorization", "Firebase " + firebaseToken);
        //request.SetRequestHeader("Firebase", firebaseToken);
        print("Case3");

        // Send the request and wait for response
        yield return request.SendWebRequest();
        print("Case4");


        if (request.result == UnityWebRequest.Result.ConnectionError ||
            request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error: " + request.error);
            Debug.LogError("Response: " + request.downloadHandler.text);
            panelScript.fetched = true;
            yield break;
        }
        else
        {
            Debug.Log("User data received successfully:");
            Debug.Log(request.downloadHandler.text);
        }

        json = request.downloadHandler.text;

        var initial = JSON.Parse(json);
        var parsed = initial["data"];

        string displayName = parsed["display_name"];
        string email = parsed["email"];
        int age = parsed["age"].AsInt;
        string gender = parsed["gender"];
        bool purchased = parsed["is_subscribed"].AsBool;

        panelScript.SetUserData(displayName, email, age, gender, purchased);





    }

    public IEnumerator UpdateProfileWithForm(string name, string email, int age, string gender, string purchased)
    {
        // Prepare form data
        Debug.Log("Inside Update profile function");
        WWWForm form = new WWWForm();
        form.AddField("display_name", name);
        form.AddField("email", email);
        form.AddField("age", age);
        form.AddField("gender", gender);
        form.AddField("is_subscribed", purchased);
        //form.AddField("gender", 10);

        // Use UnityWebRequest PUT with form
        UnityWebRequest request = UnityWebRequest.Put(apiUrl, form.data);
        request.uploadHandler.contentType = "application/x-www-form-urlencoded"; // explicitly set content type
        request.downloadHandler = new DownloadHandlerBuffer();

        // Add headers

        string firebaseToken = PlayerPrefs.GetString("Firebase_Token", "");
        request.SetRequestHeader("Authorization", "Firebase " + firebaseToken);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError ||
            request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error: " + request.error);
            Debug.LogError("Response: " + request.downloadHandler.text);
        }
        else
        {
            Debug.Log("Profile updated successfully:");
            Debug.Log(request.downloadHandler.text);

            //Setting Player Prefs
            PlayerPrefs.SetString("display_name", name);
            PlayerPrefs.SetString("email", email);
            PlayerPrefs.SetString("gender", gender);
            PlayerPrefs.SetInt("age", age);
            PlayerPrefs.SetString("purchased", purchased);

            if (bool.Parse(purchased)) {
                PlayerPrefs.SetString("moveToGame", purchased);
                //panelScript.purchaseComplete = true;
            }
        }
    }


    public IEnumerator UpdatePurchase(int amount, string currencyCode) {

        Debug.Log("Inside Update Purchase function");

        WWWForm form = new WWWForm();
        form.AddField("amount", amount);
        form.AddField("currency_code", currencyCode);

        UnityWebRequest request = UnityWebRequest.Post(purchaseUrl, form);

        //request.uploadHandler.contentType = "application/x-www-form-urlencoded"; // explicitly set content type
        request.downloadHandler = new DownloadHandlerBuffer();

        // Add headers
        //Need to change
        string firebaseToken = PlayerPrefs.GetString("Firebase_Token", "");
        request.SetRequestHeader("Authorization", "Firebase " + firebaseToken);

        

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError ||
            request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error: " + request.error);
            Debug.LogError("Response: " + request.downloadHandler.text);
        }
        else
        {
            Debug.Log("Purchase updated successfully");
        }


    }
}

