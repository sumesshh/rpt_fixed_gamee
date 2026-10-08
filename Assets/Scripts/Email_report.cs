using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine.Networking;

public class EmailRedirect : MonoBehaviour
{
    [SerializeField] private TMP_InputField  subjectInputField;
    [SerializeField] private Button submitButton;
    [SerializeField] private string fixedEmailAddress = "your.email@example.com";
    [SerializeField] private string defaultSubject = "Message from Unity App";
    [SerializeField] private string emailBody = "Hello, I'm contacting you from your Unity application.";

    // WebGL method to open URL in browser
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void OpenNewTab(string url);
#endif

    private void Start()
    {
        // Initialize input field with default subject if needed
        if (subjectInputField != null && !string.IsNullOrEmpty(defaultSubject))
        {
            subjectInputField.text = defaultSubject;
        }

        // Add listener to button
        if (submitButton != null)
        {
            submitButton.onClick.AddListener(RedirectToEmail);
        }
    }

    public void RedirectToEmail()
    {

        // Get user's system ID (unique identifier)
        string userID = SystemInfo.deviceUniqueIdentifier;

        // Get device information
        string platform = Application.platform.ToString();
        string deviceModel = SystemInfo.deviceModel;
        string operatingSystem = SystemInfo.operatingSystem;


        // Get subject from input field or use default
        //string emailBody = subjectInputField != null && !string.IsNullOrEmpty(subjectInputField.text)
        //    ? subjectInputField.text
        //    : "";
        string emailBody = "";

        if (subjectInputField != null && !string.IsNullOrEmpty(subjectInputField.text)){ 
            emailBody = "******_______******\n\n" +
                          "Please Do Not Modify This\n\n" +
                          "User_Name:" + userID + "\n\n" +
                          "Unique_Name:" + userID + "\n\n" +
                          "Platform:" + platform + "\n\n" +
                          "Model:" + deviceModel + "\n\n" +
                          "OS:" + operatingSystem + "\n\n" +
                          subjectInputField.text;

        }

        string subject = System.Uri.EscapeDataString("Reporting game issue");
        emailBody = System.Uri.EscapeDataString(emailBody);

        // Create a mailto URL with the fixed email address and user-entered subject
        string mailtoUrl = $"mailto:{fixedEmailAddress}?subject={subject}&body={emailBody}";

        // Open the URL based on platform
#if UNITY_WEBGL && !UNITY_EDITOR
            OpenNewTab(mailtoUrl);
#else
        Application.OpenURL(mailtoUrl);
#endif

        Debug.Log($"Redirecting to email: {fixedEmailAddress} with subject: {subject}");
    }
}