using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NameAndMailDisplayer : MonoBehaviour
{
    // Start is called before the first frame update

    

    public TextMeshProUGUI personName;
    public TextMeshProUGUI mail;

    private string filePath;

    void Start()
    {
        //filePath = Path.Combine(Application.persistentDataPath, "UserData.json");

        //(string person, string email, string dob) = LoadUserData();

        string name = PlayerPrefs.GetString("display_name", "");
        string email = PlayerPrefs.GetString("email","");

        if (string.IsNullOrEmpty(name))
        {
            name = "User Name";
        }

        if (string.IsNullOrEmpty(email))
        {
            email = "User Mail";
        }
        personName.text = name;
        mail.text = email;

        
    }

    private void OnEnable()
    {

        string name = PlayerPrefs.GetString("display_name", "");
        string email = PlayerPrefs.GetString("email", "");

        if (string.IsNullOrEmpty(name))
        {
            name = "User Name";
        }

        if (string.IsNullOrEmpty(email))
        {
            email = "User Mail";
        }
        personName.text = name;
        mail.text = email;

    }

    // Update is called once per frame
    void Update()
    {
        
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

    public void CheckCredentials() {
        filePath = Path.Combine(Application.persistentDataPath, "UserData.json");

        (string person, string email, string dob) = LoadUserData();
        personName.text = person;
        mail.text = email;
    }
}
