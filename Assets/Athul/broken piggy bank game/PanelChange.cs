using UnityEngine;
using UnityEngine.UI;

public class PanelChange : MonoBehaviour
{
    public GameObject normalPiggyBankPanel;
    public GameObject brokenPiggyBankPanel;
    public AudioClip breakSound;
    public Button piggyBankButton;

    private AudioSource audioSource;

    private void Start()
    {
        // Ensure the broken panel is disabled at the start
        brokenPiggyBankPanel.SetActive(false);
        normalPiggyBankPanel.SetActive(true);

        audioSource =gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.clip = breakSound;


        if (piggyBankButton != null)
        {
            piggyBankButton.onClick.AddListener(BreakPiggyBank);
        }
        else
        {
            Debug.LogError("Piggy bank button is not assigned in the inspector");
        }
    }

    public void BreakPiggyBank()
    {
        // Break the piggy bank
        normalPiggyBankPanel.SetActive(false);
        brokenPiggyBankPanel.SetActive(true);

        // Play the breaking sound
        if (breakSound != null)
        {
            audioSource.Play();
        }
    }
}