using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Numerics;
using System.Text.RegularExpressions;
using Firebase.Auth;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


[System.Serializable]
public class UserData
{
    public string fullName;
    public string email;
    public string dob;
}
public class PanelShift : MonoBehaviour
{

    // Start is called before the first frame update
    [Header("First Screen")]
    public GameObject canvasOne;
    public GameObject audioManager;
    public GameObject demoLevelSelect;



    [Header("OTP authentication")]
    public GameObject canvasTwo;
    public GameObject numberInputPanel;
    public GameObject otpVerficationPanel;
    public Toggle agreeToggle;
    public TextMeshProUGUI tickCheckBoxMessage;
    public TextMeshProUGUI phoneNumberDisplay;
    public GameObject paymentPanel;
    public GameObject noInternetPanel;

    [Header("Data Input")]
    public GameObject boyPanel;
    public GameObject boyImage;
    public GameObject girlImage;
    public GameObject changeToGirlButton;
    public GameObject changeToBoyButton;
    public TMP_InputField fullNameInputField;
    public TMP_InputField emailInputField;
    public TMP_InputField dobInputField;
    public TextMeshProUGUI errorDisplay;

    [Header("Timer")]
    public TextMeshProUGUI TimerText;
    public float timerRemaining;
    public Button resend;
    private OTPInputHandler otpScript;

    [Header("PostLogin")]
    public GameObject RegCanvas;
    public GameObject postLoginCanvas;
    public GameObject Page6;
    public GameObject Page7;
    public GameObject Page7_termscondn;
    public CanvasGroup Page_7;
    public GameObject Page7_deleteAcc;
    public GameObject Page7_report_issue;






    [Header("Authentication")]
    [SerializeField] TMP_InputField phoneNumber;

    public TextMeshProUGUI otpErrorMessage;
    public GameObject dropDown;

    private CountryDropdownHandler countryDropdownHandler;
    private FirebaseAuth firebaseAuth;
    private PhoneAuthProvider provider;
    private string verificationId;
    private uint phoneAuthTimeoutMs = 60 * 1000;

    private string filePath;
    private bool pressed = false;

    
    string displayName;
    string email;
    int age;
    bool male;
    bool purchased;

    public UserRegistration backendScript;

    public bool fetched;

    public bool purchaseComplete;

    public backButtonManager backButtonScript;

    [Header("Prompt panels")]
    public GameObject loginPrompt;
    public GameObject paymentPrompt;


    void Awake()
    {

        SwitchToLandscape();

        // Get the device refresh rate (display frame rate)
        int deviceFrameRate = (int)Screen.currentResolution.refreshRateRatio.value;

        if (deviceFrameRate < 60)
            Application.targetFrameRate = 60;
        else
            Application.targetFrameRate = deviceFrameRate;
    }


    void Start()
    {


        otpScript = otpVerficationPanel.GetComponent<OTPInputHandler>();

        filePath = Path.Combine(Application.persistentDataPath, "UserData.json");




        //if (File.Exists(filePath))
        //{
        //    string jsonContent = File.ReadAllText(filePath);
        //    UserData userData = JsonUtility.FromJson<UserData>(jsonContent);

        //    if (!string.IsNullOrEmpty(userData.fullName) &&
        //        !string.IsNullOrEmpty(userData.email) &&
        //        !string.IsNullOrEmpty(userData.dob))
        //    {
        //        Debug.Log("All details are present. Redirecting...");
        //        postLoginCanvas.SetActive(true);
        //        audioManager.SetActive(true);
        //        canvasOne.SetActive(false);
        //        RegCanvas.SetActive(false);
        //    }
        //    else
        //    { audioManager.SetActive(false);
        //        postLoginCanvas.SetActive(false);
        //        canvasOne.SetActive(true);
        //        RegCanvas.SetActive(false);
        //        Debug.Log("Some details are missing!");
        //    }
        //}
        //else
        //{
        //    audioManager.SetActive(false);
        //    postLoginCanvas.SetActive(false);
        //    canvasOne.SetActive(true);
        //    RegCanvas.SetActive(false);
        //    Debug.Log("JSON file not found!");
        //}

        string moveCheck = PlayerPrefs.GetString("moveToGame", false.ToString());

        int alreadyLoggedIn = PlayerPrefs.GetInt("dataEntered", 0);

        int demoActive = PlayerPrefs.GetInt("DemoActive", 0);

        Debug.Log("MOve check " + moveCheck);
        Debug.Log("already logged in: " + alreadyLoggedIn);
        Debug.Log("Demoactive: " + demoActive);

        if (bool.Parse(moveCheck))
        {
            // ✅ Perform your operations here
            Debug.Log("Moving to game...");
            postLoginCanvas.SetActive(true);
            audioManager.SetActive(true);
            canvasOne.SetActive(false);
            RegCanvas.SetActive(false);
            SwitchToLandscape();
        }
        //else if (alreadyLoggedIn == 1)
        //{

        //    audioManager.SetActive(false);
        //    postLoginCanvas.SetActive(false);
        //    canvasOne.SetActive(false);
        //    RegCanvas.SetActive(true);
        //    paymentPanel.SetActive(true);
        //    SetDataFields();
        //}
        else if (demoActive == 1)
        {
            //audioManager.SetActive(false);
            //postLoginCanvas.SetActive(false);
            //canvasOne.SetActive(false);
            //RegCanvas.SetActive(true);


            if (LevelSelectManager.demoCount > 2)
            {
                //RegCanvas.SetActive(true);
                //paymentPanel.SetActive(true);
                //postLoginCanvas.SetActive(false);

                postLoginCanvas.SetActive(true);
                audioManager.SetActive(true);
                canvasOne.SetActive(false);
                RegCanvas.SetActive(false);

                paymentPrompt.SetActive(true);
                backButtonScript.OpenPanel(paymentPrompt);
                SwitchToLandscape();
            }
            else if (LevelSelectManager.demoCount == 2 && alreadyLoggedIn != 1)
            {
                //RegCanvas.SetActive(true);
                //numberInputPanel.SetActive(true);
                //postLoginCanvas.SetActive(false);


                postLoginCanvas.SetActive(true);
                audioManager.SetActive(true);
                canvasOne.SetActive(false);
                RegCanvas.SetActive(false);
                SwitchToLandscape();
                loginPrompt.SetActive(true);
                backButtonScript.OpenPanel(loginPrompt);
                

            }
            else
            {
                postLoginCanvas.SetActive(true);
                audioManager.SetActive(true);
                canvasOne.SetActive(false);
                RegCanvas.SetActive(false);
                SwitchToLandscape();
            }
            
        }
        else
        {
            audioManager.SetActive(false);
            postLoginCanvas.SetActive(false);
            canvasOne.SetActive(true);
            RegCanvas.SetActive(false);
        }



        firebaseAuth = FirebaseAuth.DefaultInstance;

        // Request RECEIVE_SMS permission manually
        //#if UNITY_ANDROID
        //        if (!UnityEngine.Android.Permission.HasUserAuthorizedPermission("android.permission.RECEIVE_SMS"))
        //        {
        //            UnityEngine.Android.Permission.RequestUserPermission("android.permission.RECEIVE_SMS");
        //        }

        //#endif

        //        if (dropDown != null)
        //        {
        //            countryDropdownHandler = dropDown.GetComponent<CountryDropdownHandler>();
        //            if (countryDropdownHandler != null)
        //            {
        //                Debug.Log("Dial code imported: " + countryDropdownHandler.dial_code);
        //            }
        //            else
        //            {
        //                Debug.LogError("CountryDropdownHandler script is missing on dropDown object.");
        //            }
        //        }
        //        else
        //        {
        //            Debug.LogError("Dropdown object is not assigned in the inspector.");
        //        }

    }

    // Update is called once per frame
    void Update()
    {
        if (otpVerficationPanel.activeSelf) {
            if (timerRemaining > 1)
            {
                timerRemaining = timerRemaining - Time.deltaTime;
                UpdateTimerUI(timerRemaining);
            }
            else
            {
                resend.interactable = true;
            }
        }


        
    }

    public void NextButtonPressed() {



        //canvasTwo.SetActive(true);
        canvasOne.SetActive(false);
        postLoginCanvas.SetActive(true);
        Page6.SetActive(true);
        audioManager.SetActive(true);
        //numberInputPanel.SetActive(true);
        SwitchToLandscape();
        
        PlayerPrefs.SetInt("DemoActive", 1);
    }

    //private void SendOtpPressed() {
    //    if (agreeToggle.isOn) {
    //        authenticationScript.LoginFunction();
    //    }
    //}

    //make verify_otp funciton return a boolean if all process happen smoothly
    private void verifyButtonPressed() {
        //bool success;
        //authenticationScript.Verify_Otp();
        //if (success) { 


        //}

    }

    //Attach the below function to the send OTP button
    public void LoginFunction()
    {

        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            noInternetPanel.SetActive(true);
            Debug.Log("No internet Panel set active");
            return;
        }

        timerRemaining = 60;
        resend.interactable = false;
        if (string.IsNullOrEmpty(phoneNumber.text))
        {
            //debug.text = "Enter a valid phone number!";
            Debug.LogError("Phone number is empty.");
            tickCheckBoxMessage.text = "Enter a valid phone number! ";
            StartCoroutine(ShowAndHide(tickCheckBoxMessage));

            return;
        }
        else if (!agreeToggle.isOn)
        {
            Debug.Log("Check Agreement");
            tickCheckBoxMessage.text = "Please agree to the terms and conditions before proceeding.";
            StartCoroutine(ShowAndHide(tickCheckBoxMessage));
            return;
        }
        else {
            otpVerficationPanel.SetActive(true);
            numberInputPanel.SetActive(false);

        }


        tickCheckBoxMessage.gameObject.SetActive(false);

        //string countryCode = countryDropdownHandler.dial_code;
        string fullPhoneNumber = "+91" + phoneNumber.text;
        string LongPhoneNumber = "+91-" + phoneNumber.text;
        phoneNumberDisplay.text = $"Please enter the 6-digit code sent to your email <color=#37B33C>{LongPhoneNumber}</color> for verification";


        Debug.Log("Country Code: " + "+91");
        Debug.Log("Full Phone Number: " + fullPhoneNumber);
        pressed = false;

        //loginPanel.SetActive(false);

        provider = PhoneAuthProvider.GetInstance(firebaseAuth);
        provider.VerifyPhoneNumber(
            new Firebase.Auth.PhoneAuthOptions
            {
                PhoneNumber = fullPhoneNumber,
                TimeoutInMilliseconds = phoneAuthTimeoutMs,

            },
            verificationCompleted: (credential) =>
            {
                //Debug.Log("Auto Verification Completed!");

                //firebaseAuth.SignInWithCredentialAsync(credential).ContinueWith(task =>
                //{
                //    if (task.IsCompleted && !task.IsFaulted)
                //    {
                //        FirebaseUser newUser = task.Result;
                //        Debug.Log("Auto Sign-in Success: " + newUser.PhoneNumber);
                //        boyPanel.SetActive(true);
                //        numberInputPanel.SetActive(false);
                //        //SceneManager.LoadScene("sample");
                //    }
                //    else
                //    {
                //        Debug.LogError("Auto Sign-in Failed: " + task.Exception?.Message);
                //        debug.text = "Auto verification failed. Enter OTP manually.";
                //        otpVerficationPanel.SetActive(true);
                //        numberInputPanel.SetActive(false);

                //    }
                //});
            },
            verificationFailed: (error) =>
            {
                tickCheckBoxMessage.text = "Some Error occured. Please try again";
                StartCoroutine(ShowAndHide(tickCheckBoxMessage));
                //otpVerficationPanel.SetActive(false);
                //numberInputPanel.SetActive(true);
                Debug.LogError("Verification Failed: " + error);
                numberInputPanel.SetActive(true);
                otpVerficationPanel.SetActive(false);
                //debug.text = "Error: " + error;
            },
            codeSent: (id, token) =>
            {
                verificationId = id;
                //otpVerficationPanel.SetActive(true);
                //numberInputPanel.SetActive(false);
                //otpPanel.SetActive(true);
                Debug.Log("Token: " + token);
                //debug.text = "Code sent!";
                Debug.Log("OTP Sent Successfully.");
            },
            codeAutoRetrievalTimeOut: (id) =>
            {
                //tickCheckBoxMessage.text = "Timed Out. Please try again";
                //StartCoroutine(ShowAndHide(tickCheckBoxMessage));
                //otpVerficationPanel.SetActive(false);
                //numberInputPanel.SetActive(true);
                Debug.Log("Code Retrieval Timeout.");
            }
        );
    }

    //Attach the below function to the verify OTP button
    public void Verify_Otp()
    {
        if (pressed) {
            return;
        }
        pressed = true;
        string otp;
        otp = otpScript.GetOTP();
        if (string.IsNullOrEmpty(verificationId))
        {
            //debug.text = "Error: verificationId is empty!";
            
            Debug.LogError("Verification ID is missing.");
            pressed = false;
            return;
        }

        if (string.IsNullOrEmpty(otp))
        {
            otpErrorMessage.text = "Please Enter the OTP";
            StartCoroutine (ShowAndHide(otpErrorMessage));
            //debug.text = "Error: OTP is empty!";
            Debug.LogError("OTP is missing.");
            pressed= false;
            return;
        }

        PhoneAuthCredential credential = provider.GetCredential(verificationId, otp);

        Debug.Log("OTP Verification Started..." + otp);
        Debug.Log("Verification ID: " + verificationId);

        firebaseAuth.SignInAndRetrieveDataWithCredentialAsync(credential).ContinueWith(task =>
        {
            Debug.Log("Started sign-in function");
            if (task.IsFaulted || task.IsCanceled)
            {
                // ✅ Show user-facing error here
                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    otpErrorMessage.text = "Entered OTP is not correct. Retry.";
                    StartCoroutine(ShowAndHide(otpErrorMessage));
                    //debug.text = "Verification failed. Please check the OTP and try again.";
                    // Optional: Show a popup panel
                    // errorPopupPanel.SetActive(true);
                });
                //debug.text = "Verification failed: " + task.Exception?.Message;
                Debug.LogError("Sign-in error: " + task.Exception);
                pressed = false;
                return;
            }

            FirebaseUser newUser = task.Result.User;
            Debug.Log("User signed in successfully");
            Debug.Log("Phone Number: " + newUser.PhoneNumber);
            //otpVerficationPanel.SetActive(false);
            //boyPanel.SetActive(true);

            //UnityEngine.WSA.Application.InvokeOnAppThread(() =>
            //{
            //    otpVerficationPanel.SetActive(false);
            //    boyPanel.SetActive(true);
            //}, false);


            // ✅ GET FIREBASE ID TOKEN HERE
            newUser.TokenAsync(true).ContinueWith(tokenTask =>
            {
                if (tokenTask.IsCanceled || tokenTask.IsFaulted)
                {
                    Debug.LogError("Token retrieval failed: " + tokenTask.Exception);
                    return;
                }

                string idToken = tokenTask.Result;
                PlayerPrefs.SetString("Firebase_Token", idToken);
                Debug.Log("Firebase ID Token: " + idToken);
                StartCoroutine(backendScript.GetUserData(idToken));
                StartCoroutine(WaitForData());

                // ✅ You can now use this ID token for backend authentication.
                // For example, pass it to a function that sends the token to your API:
                //StartCoroutine(SendTokenToBackend(idToken));
            });

            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                //otpVerficationPanel.SetActive(false);
                //boyPanel.SetActive(true);
               
            });
        });
    }


    public void SetUserData(string name, string mail, int userAge, string userGender, bool userPurchased) {
        displayName = name;
        email = mail;
        age = userAge;
        if (userGender == "boy")
        {
            male = true;
        }
        else {
            male = false;
        }

 

        if (userPurchased)
        {
            purchased = true;
        }
        else {
            purchased = false;
        }

        PlayerPrefs.SetString("display_name", name);
        PlayerPrefs.SetString("email", mail);
        PlayerPrefs.SetString("gender", userGender);
        PlayerPrefs.SetInt("age", userAge);
        PlayerPrefs.SetString("purchased", userPurchased.ToString());

        if (userPurchased)
        {
            PlayerPrefs.SetString("moveToGame", true.ToString());
            
        }
        

        fetched = true;

    }

    private void SetDataFields() {

        string name = PlayerPrefs.GetString("display_name", "");
        string mail = PlayerPrefs.GetString("email", "");
        int age = PlayerPrefs.GetInt("age", 0);
        string gender = PlayerPrefs.GetString("gender", "");

        fullNameInputField.text = name;
        emailInputField.text = mail;
        dobInputField.text = age.ToString();
        if (gender == "boy")
        {
            ChangeToBoy();

        }
        else {
            ChangeToGirl();
        }


    }

    IEnumerator WaitForData() {
        
        while (!fetched) {
            yield return null;

        }

        //First time user
        if (string.IsNullOrEmpty(displayName) || string.IsNullOrEmpty(email) || age == 0)
        {
            otpVerficationPanel.SetActive(false);
            boyPanel.SetActive(true);
            Debug.Log("New USer");
        }
        //Data exists but not purchased
        else if (!purchased)
        {
            otpVerficationPanel.SetActive(false);
            boyPanel.SetActive(true);
            fullNameInputField.text = displayName;
            emailInputField.text = email;
            dobInputField.text = age.ToString();

            if (male)
            {
                ChangeToBoy();
            }
            else {
                ChangeToGirl();
            }

            Debug.Log("Old user, no purchase");
        }
        else {
            SwitchToLandscape();
            otpVerficationPanel.SetActive(false);
            postLoginCanvas.SetActive(true);
            audioManager.SetActive(true);
            canvasOne.SetActive(false);
            RegCanvas.SetActive(false);
            Debug.Log("Old user with purchase");
            
           
        
        }
    }


    
    public void ChangeToGirl() { 
        changeToBoyButton.SetActive(true);
        girlImage.SetActive(true);
        changeToGirlButton.SetActive(false);
        boyImage.SetActive(false);
    }

    public void ChangeToBoy() {
        changeToGirlButton.SetActive(true);
        boyImage.SetActive(true);
        changeToBoyButton.SetActive(false);
        girlImage.SetActive(false);
    }

    public void OnSubmit()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            noInternetPanel.SetActive(true);
            Debug.Log("No internet Panel set active");
            return;
        }
        string fullName = fullNameInputField.text.Trim();
        string email = emailInputField.text.Trim();
        string dob = dobInputField.text.Trim();
        int age;
        string gender;
        

        if (ValidateInputs(fullName, email, dob))
        {

            Debug.Log("Registration Successful");
            boyPanel.SetActive(false);
            //Changing for new flow
            //paymentPanel.SetActive(true);

            //demoLevelSelect.SetActive(true);
            RegCanvas.SetActive(false);
            postLoginCanvas.SetActive(true);
            Page6.SetActive(true);
            SwitchToLandscape();


            SaveUserData(fullName, email, dob);

            //displayName = fullName;
            //this.email = email;
            //this.age = int.Parse(dob);
            // Proceed with storing data or sending to a server
            if (boyImage.activeSelf)
            {
                gender = "boy";
            }
            else {
                gender = "girl";
            }

            age = int.Parse(dob);




            //Setting Global variables
            displayName = fullName;
            this.email = email;
            this.age = age;
            this.male = boyImage.activeSelf;

            //Getting purchased player pref
            string purchased = PlayerPrefs.GetString("purchased", false.ToString());

            PlayerPrefs.SetInt("dataEntered", 1);

            //Updating backend
            StartCoroutine(backendScript.UpdateProfileWithForm(fullName, email, age, gender, purchased ));


        }
    }

    public void OnInAppPurchaseSuccess() {

        //Getting Player Prefs
        string name = PlayerPrefs.GetString("display_name","");
        string mail = PlayerPrefs.GetString("email", "");
        int age = PlayerPrefs.GetInt("age", 0);
        string gender = PlayerPrefs.GetString("gender", "");

        string purchased = true.ToString();


        
        //Updating backend
        StartCoroutine(backendScript.UpdateProfileWithForm(name, mail, age, gender, purchased));
        //StartCoroutine(OnPurchaseComplete());


    
    }

    IEnumerator OnPurchaseComplete() {

        while (!purchaseComplete) {
            yield return null;
        }

        postLoginCanvas.SetActive(true);
        audioManager.SetActive(true);
        canvasOne.SetActive(false);
        RegCanvas.SetActive(false);

    }

    private bool ValidateInputs(string fullName, string email, string dob)
    {
        if (string.IsNullOrEmpty(fullName) || fullName.Length < 3)
        {
            errorDisplay.text = "Full name must be at least 3 characters.";
            Debug.Log("Full Name must be at least 3 characters.");
            StartCoroutine(ShowAndHide(errorDisplay));
            return false;
        }

        if (!IsValidEmail(email))
        {
            errorDisplay.text = "Invalid Email Format.";
            Debug.Log("Invalid Email Format.");
            StartCoroutine(ShowAndHide(errorDisplay));
            return false;
        }

        if (!IsValidDOB(dob))
        {
            //Debug.Log("Invalid Date of Birth (Use YYYY-MM-DD).");
            errorDisplay.text = "Enter a valid age";
            StartCoroutine(ShowAndHide(errorDisplay));
            return false;
        }

     
        return true;
    }

    private bool IsValidEmail(string email)
    {
        string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
        return Regex.IsMatch(email, emailPattern);
    }

    private bool IsValidDOB(string dob)
    {
        //DateTime parsedDate;
        
        //if (DateTime.TryParse(dob, out parsedDate))
        //{
        //    Debug.Log("You entered: " + parsedDate.ToString("dd MMMM yyyy"));
        //    int age = DateTime.Now.Year - parsedDate.Year;
        //    if (age < 5) // Example: Minimum age restriction
        //    {
        //        return false;
        //    }
        //    return true;
        //}

        if (string.IsNullOrEmpty(dob))
        {
            
            return false;
        }

        if (int.TryParse(dob, out int age))
        {
            if (age < 0 )
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        else
        {
            return false;
        }
        
    }

    private void SaveUserData(string fullName, string email, string dob)
    {
        Debug.Log("Inside  Save User data function");
        UserData userData = new UserData { fullName = fullName, email = email, dob = dob };
        string json = JsonUtility.ToJson(userData, true);
        File.WriteAllText(filePath, json);
        Debug.Log("User data saved to: " + filePath);
    }

    public (string fullName, string email, string dob) LoadUserData()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            UserData userData = JsonUtility.FromJson<UserData>(json);

            //fullNameInputField.text = userData.fullName;
            //emailInputField.text = userData.email;
            //dobInputField.text = userData.dob;


            Debug.Log("User data loaded successfully.");
            return (userData.fullName,userData.email, userData.dob);
        }
        else
        {
            return ("", "", "");
        }
    }
    void UpdateTimerUI(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        TimerText.text = "Request new code in "+ string.Format("{0:00}:{1:00}s", minutes, seconds);
    }
    public void PostLogin()
    {   audioManager.SetActive(true);
        RegCanvas.SetActive(false);
        postLoginCanvas.SetActive(true);
        Page6.SetActive(true);
        SwitchToLandscape();
    }
    public void profile()
    {
        Page6.SetActive(false);
        Page7.SetActive(true);
    }
    public void backProfile()
    {
        Page7.SetActive(false);
        Page6.SetActive(true);
    }
    public void TermsAndconditions()
    {
        Page7.SetActive(false);
        Page7_termscondn.SetActive(true );
    }
    public void back_terms()
    {
        Page7.SetActive(true );
        Page7_termscondn.SetActive(false );
    }
    public void deleteAccount()
    {
        Page_7.alpha = Mathf.Clamp01(0.5f);
        Page_7.interactable = false;
        Page7_deleteAcc.SetActive(true);
    }
    public void cancel_delete_Acc()
    {
        Page_7.alpha = Mathf.Clamp01(1f);
        Page_7.interactable = true;
        Page7_deleteAcc.SetActive(false);
    }

    public void DisablePanel() {
        Page_7.alpha = Mathf.Clamp01(0.5f);
        Page_7.interactable = false;
    }

    public void EnablePanel() {
        Page_7.alpha = Mathf.Clamp01(1f);
        Page_7.interactable = true;
    }

    public void GoToTermsAndConditions() {
        Debug.Log("Site opened");
        Application.OpenURL("https://sites.google.com/view/rockpapertuition/home");
    }

    public void GoToPrivacyPolicy() {
        Debug.Log("Privacy policy site opened");
        Application.OpenURL("https://sites.google.com/view/rock-paper-tuition/home");
    
    }

    public void report_issuePage()
    {
        DisablePanel();
        Page7_report_issue.SetActive(true );
    }
    public void report_issuerev()
    {
        Page7_report_issue.SetActive(false);
        EnablePanel();
    }
    public IEnumerator ShowAndHide(TextMeshProUGUI textDisplay)
    {
        textDisplay.gameObject.SetActive(true);  // Show the GameObject
        yield return new WaitForSeconds(2f);  // Wait for 2 seconds
        textDisplay.gameObject.SetActive(false);  // Hide the GameObject
        textDisplay.text = "";
    }

    public static IEnumerator ShortDisplay(TextMeshProUGUI textDisplay) {
        textDisplay.text = "Go Premium to unlock all levels";
        textDisplay.gameObject.SetActive(true);  // Show the GameObject
        yield return new WaitForSeconds(2f);  // Wait for 2 seconds
        textDisplay.gameObject.SetActive(false);  // Hide the GameObject
        textDisplay.text = "";
    }

    public void ClearJsonData()
    {
        //if (File.Exists(filePath))
        //{
        //    File.WriteAllText(filePath, "{}"); // Clears file with empty JSON object
        //    Debug.Log("JSON file cleared successfully.");
        //}
        //else
        //{
        //    Debug.LogError("JSON file not found!");
        //}

        PlayerPrefs.SetString("moveToGame", false.ToString());
    }

    public void QuitApp()
    {
        Application.Quit();
        Debug.Log("Application.Quit() called"); // Will not show when built on Android
    }

    public void SkipButtonPressed() {
        PlayerPrefs.SetString("moveToGame", true.ToString());
    }

    public void OnLoginPromptPressed() {
        backButtonScript.Back();
        RegCanvas.SetActive(true);
        numberInputPanel.SetActive(true);
        postLoginCanvas.SetActive(false);

        SwitchToPortrait();
        //postLoginCanvas.SetActive(true);
        //audioManager.SetActive(true);
        //canvasOne.SetActive(false);
        //RegCanvas.SetActive(false);

        loginPrompt.SetActive(false);
    }

    public void OnPaymentPromptPressed() {
        backButtonScript.Back();
        RegCanvas.SetActive(true);
        paymentPanel.SetActive(true);
        postLoginCanvas.SetActive(false);
        SwitchToPortrait();
        //postLoginCanvas.SetActive(true);
        //audioManager.SetActive(true);
        //canvasOne.SetActive(false);
        //RegCanvas.SetActive(false);

        paymentPrompt.SetActive(false);
    }


    public void CustomLogin() {
        RegCanvas.SetActive(true);
        Page6.SetActive(true);
        numberInputPanel.SetActive(true);
        postLoginCanvas.SetActive(false);


        //postLoginCanvas.SetActive(true);
        //audioManager.SetActive(true);
        //canvasOne.SetActive(false);
        //RegCanvas.SetActive(false);

        loginPrompt.SetActive(false);

        backButtonScript.ClearPanelStack();
    }

    public void SwitchToLandscape() {
        Screen.autorotateToPortrait = false;
        Screen.autorotateToPortraitUpsideDown = false;
        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = true;
        Screen.orientation = ScreenOrientation.LandscapeLeft;
    }

    public void SwitchToPortrait() {
        // Portrait is disabled for this app; TestSceneFull stays landscape.
        SwitchToLandscape();
    }
}
