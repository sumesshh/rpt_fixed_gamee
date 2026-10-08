using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class UserProfileUpdate : MonoBehaviour
{
    
    public TMP_InputField fullNameInputField;
    public TMP_InputField emailInputField;
    public TMP_InputField dobInputField;


    public TextMeshProUGUI nameDisplay;
    public TextMeshProUGUI mailDisplay;

    public TextMeshProUGUI messageText; // Assign in Inspector
    public TextMeshProUGUI errorDisplay;

    public UserRegistration backendScript;
    public PanelShift panelScript;


    private string filePath;
    // Start is called before the first frame update
    public void Start()
    {
        messageText.gameObject.SetActive(false);
        filePath = Path.Combine(Application.persistentDataPath, "UserData.json");
        //(string fullName, string email, string dob) = LoadUserData();

        string name = PlayerPrefs.GetString("display_name", "");
        string mail = PlayerPrefs.GetString("email", "");
        int age = PlayerPrefs.GetInt("age", 0);


        fullNameInputField.text = name;
        emailInputField.text = mail;
        dobInputField.text = age.ToString();
       

        //string fullName = fullNameInputField.text.Trim();
        //string email = emailInputField.text.Trim();
        //string dob = dobInputField.text.Trim();
        
    }

    private void OnEnable()
    {
        messageText.gameObject.SetActive(false);
        filePath = Path.Combine(Application.persistentDataPath, "UserData.json");
        //(string fullName, string email, string dob) = LoadUserData();

        string name = PlayerPrefs.GetString("display_name", "");
        string mail = PlayerPrefs.GetString("email", "");
        int age = PlayerPrefs.GetInt("age", 0);

        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(mail)) {
            panelScript.CustomLogin();
            this.gameObject.SetActive(false);
            return;
        
        }


        fullNameInputField.text = name;
        emailInputField.text = mail;
        dobInputField.text = age.ToString();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnSubmit()
    {
        string fullName = fullNameInputField.text.Trim();
        string email = emailInputField.text.Trim();
        string dob = dobInputField.text.Trim();
        int age;
        string gender;

        if (ValidateInputs(fullName, email, dob))
        {
            Debug.Log("Registration Successful");

            age = int.Parse(dob);

            gender = PlayerPrefs.GetString("gender", "");
            string purchased = PlayerPrefs.GetString("purchased", false.ToString());

            //Updating backend
            StartCoroutine(backendScript.UpdateProfileWithForm(fullName, email, age, gender, purchased));


            ShowMessage();
            SaveUserData(fullName, email, dob);
            nameDisplay.text = fullName;
            mailDisplay.text = email;
            // Proceed with storing data or sending to a server
        }
    }

    private bool ValidateInputs(string fullName, string email, string dob)
    {
        //if (string.IsNullOrEmpty(fullName) || fullName.Length < 3)
        //{
        //    Debug.Log("Full Name must be at least 3 characters.");
        //    return false;
        //}

        //if (!IsValidEmail(email))
        //{
        //    Debug.Log("Invalid Email Format.");
        //    return false;
        //}

        //if (!IsValidDOB(dob))
        //{
        //    Debug.Log("Invalid Date of Birth (Use YYYY-MM-DD).");
        //    return false;
        //}


        //return true;

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
        //return false;

        if (string.IsNullOrEmpty(dob))
        {

            return false;
        }

        if (int.TryParse(dob, out int age))
        {
            if (age < 1 || age > 120)
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
            return (userData.fullName, userData.email, userData.dob);
        }
        else
        {
            return ("", "", "");
        }
    }
    public void ShowMessage()
    {
        
        messageText.gameObject.SetActive(true);
        StartCoroutine(HideTextAfterDelay(2f)); // Hide after 2 seconds
    }

    private IEnumerator HideTextAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        messageText.gameObject.SetActive(false);
    }
    public IEnumerator ShowAndHide(TextMeshProUGUI textDisplay)
    {
        textDisplay.gameObject.SetActive(true);  // Show the GameObject
        yield return new WaitForSeconds(2f);  // Wait for 2 seconds
        textDisplay.gameObject.SetActive(false);  // Hide the GameObject
        textDisplay.text = "";
    }
}
