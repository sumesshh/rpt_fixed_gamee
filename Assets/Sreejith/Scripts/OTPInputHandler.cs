using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class OTPInputHandler : MonoBehaviour
{

    public string otp;
    private TouchScreenKeyboard keyboard;
    private string userInput = "";
    private string previousText = "";
    private string currentText = "";

    public TextMeshProUGUI[] otpDisplay;

    private void Start()
    {

        // Open the keyboard in NumberPad mode (change to Default for text)
        keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.NumberPad);
        // Ensure the first input field is selected initially
        //if (otpInputFields.Length > 0)
        //{
        //    //otpInputFields[0].Select();
        //    //otpInputFields[0].ActivateInputField();
        //}

        // Add listeners to each input field
        //for (int i = 0; i < otpInputFields.Length; i++)
        //{
        //    int index = i;
        //    //otpInputFields[i].onValueChanged.AddListener((string value) => OnValueChanged(index, value));
        //    //otpInputFields[i].keyboardType = TouchScreenKeyboardType.NumberPad; // Only allow numbers
        //}
        string currentText = "";
    }

    private void Update()
    {
        // Handle input for the currently selected input field
        //HandleKeyboardInput();
        if (keyboard != null && keyboard.active)
        {
            // Get the latest entered text
            currentText = keyboard.text;

            for (int i = 0; i < 6; i++)
            {
                if (i < currentText.Length)
                {
                    otpDisplay[i].text = currentText[i].ToString();
                }
                else
                {
                    otpDisplay[i].text = "";
                }

            }
            // Check if a new digit has been entered
            //if (currentText.Length > previousText.Length)
            //{
            //    // Get the last entered digit
            //    string newDigit = currentText[currentText.Length - 1].ToString();

            //    // Display the new digit in the text box
            //    for (int i = 0; i < 6; i++)
            //    {
            //        if (i < currentText.Length)
            //        {
            //            otpDisplay[i].text = currentText[i].ToString();
            //        }
            //        else
            //        {
            //            otpDisplay[i].text = "";
            //        }

            //    }

            //    // Update previous text
            //    previousText = currentText;
            //}
        }
        if (Application.isEditor) // Only runs in the Unity Editor
        {
            
            if (Input.anyKeyDown)
            {
                string newWord = Input.inputString;
                currentText = currentText + newWord;
                Debug.Log("Text in editor: " + currentText);


                for (int i = 0; i < 6; i++)
                {
                    if (i < currentText.Length)
                    {
                        otpDisplay[i].text = currentText[i].ToString();
                        Debug.Log(otpDisplay[i].text);
                    }
                    else
                    {
                        otpDisplay[i].text = "";
                    }

                }
            }
                
        }
    }

    //private void HandleKeyboardInput()
    //{
    //    // Find the currently selected input field
    //    int currentIndex = GetCurrentSelectedIndex();

    //    if (currentIndex == -1) return;

    //    // Handle backspace logic
    //    if (Input.GetKeyDown(KeyCode.Backspace))
    //    {
    //        HandleBackspace(currentIndex);
    //    }
    //}

    //private void OnValueChanged(int index, string value)
    //{
    //    // Ensure only one character is entered
    //    if (value.Length > 1)
    //    {
    //        otpInputFields[index].text = value.Substring(0, 1);
    //    }

    //    // Auto-move to next input field if a digit is entered
    //    if (value.Length == 1 && index < otpInputFields.Length - 1)
    //    {
    //        otpInputFields[index + 1].Select();
    //        otpInputFields[index + 1].ActivateInputField();
    //    }
    //}

    //private void HandleBackspace(int currentIndex)
    //{
    //    // If current field is empty, move to previous field and clear it
    //    if (string.IsNullOrEmpty(otpInputFields[currentIndex].text))
    //    {
    //        if (currentIndex > 0)
    //        {
    //            // Clear previous field
    //            otpInputFields[currentIndex - 1].text = "";

    //            // Select and activate previous field
    //            otpInputFields[currentIndex - 1].Select();
    //            otpInputFields[currentIndex - 1].ActivateInputField();
    //        }
    //    }
    //    else
    //    {
    //        // Clear current field
    //        otpInputFields[currentIndex].text = "";
    //    }
    //}

    //private int GetCurrentSelectedIndex()
    //{
    //    for (int i = 0; i < otpInputFields.Length; i++)
    //    {
    //        if (EventSystem.current.currentSelectedGameObject == otpInputFields[i].gameObject)
    //        {
    //            return i;
    //        }
    //    }
    //    return -1;
    //}

    //public string GetFullOTP()
    //{
    //    // Concatenate all input fields to get the full OTP
    //    return string.Concat(System.Array.ConvertAll(otpInputFields, input => input.text));
   
    //}

    public string GetOTP()
    {
        otp = "";
        keyboard.active = false;
        foreach (var field in otpDisplay)
        {
            otp += field.text; // Concatenate OTP digits
        }
        Debug.Log("OTP: " + otp);
        return otp;
    }

    public void DisplayNumPad() {
        keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.NumberPad);
    }

}