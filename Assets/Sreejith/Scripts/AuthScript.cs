using UnityEngine;
using Firebase.Auth;
using TMPro;
using UnityEngine.SceneManagement;

public class AuthScript : MonoBehaviour
{
    [Header("UI References")]
    public TMP_InputField phoneNumber;
    public TMP_InputField otpInput;
    public TMP_Text debugText;
    public GameObject otpPanel;
    public GameObject loginPanel;

    private FirebaseAuth auth;
    private PhoneAuthProvider provider;
    private string verificationId;
    private uint timeoutMs = 60000;

    void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
        provider = PhoneAuthProvider.GetInstance(auth);
    }
public void LoginFunction()
{
    if (string.IsNullOrEmpty(phoneNumber.text))
    {
        debugText.text = "Enter phone number";
        return;
    }

    var phoneOptions = new PhoneAuthOptions
    {
        PhoneNumber = phoneNumber.text,
        TimeoutInMilliseconds = 60000,
        ForceResendingToken = null
    };

    provider.VerifyPhoneNumber(
        phoneOptions,
        credential =>
        {
            auth.SignInWithCredentialAsync(credential);
        },
        error =>
        {
            debugText.text = "Verification failed";
            Debug.LogError(error);
        },
        (id, token) =>
        {
            verificationId = id;
            otpPanel.SetActive(true);
            loginPanel.SetActive(false);
            debugText.text = "OTP Sent";
        },
        id =>
        {
            verificationId = id;
        }
    );
}

    public void VerifyOTP()
    {
        if (string.IsNullOrEmpty(verificationId))
        {
            debugText.text = "No verification ID";
            return;
        }

        if (string.IsNullOrEmpty(otpInput.text))
        {
            debugText.text = "Enter OTP";
            return;
        }

        PhoneAuthCredential credential =
            provider.GetCredential(verificationId, otpInput.text);

        auth.SignInWithCredentialAsync(credential).ContinueWith(task =>
        {
            if (task.IsCompleted && !task.IsFaulted)
            {
                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    SceneManager.LoadScene("sample");
                });
            }
            else
            {
                debugText.text = "OTP Failed";
                Debug.LogError(task.Exception);
            }
        });
    }
}