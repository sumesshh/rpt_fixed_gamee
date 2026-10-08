using UnityEngine;
using UnityEngine.UI;
using System;

public class OtherRatingManager : MonoBehaviour
{
    [Header("Configuration")]
    public string developerEmail = "princethomas9539@gmail.com";
    public string emailSubject = "FEEDBACK/SUGGESTION";
    public string playStoreAppUrl = "https://play.google.com/store/apps/details?id=com.whatsapp";

    [Header("UI References")]
    public Image[] starImages = new Image[5]; // Assign your 5 star images in the inspector
    public Button submitButton; // Reference to the submit button
    public Sprite filledStarSprite; // Yellow star sprite
    public Sprite emptyStarSprite; // Gray star sprite

    private int currentRating = 0;

    void Start()
    {
        // Initialize all stars as unfilled
        ResetStars();

        // Add click listeners to each star
        for (int i = 0; i < starImages.Length; i++)
        {
            int starValue = i + 1; // Star positions are 1-5

            // Add a button component if needed
            Button starButton = starImages[i].GetComponent<Button>();
            if (starButton == null)
            {
                starButton = starImages[i].gameObject.AddComponent<Button>();
            }

            // Add click listener
            int index = i; // Capture the index for the lambda
            starButton.onClick.AddListener(() => OnStarClicked(index + 1));
        }

        // Add listener to submit button
        if (submitButton != null)
        {
            submitButton.onClick.AddListener(OnSubmitButtonClicked);
        }
    }

    private void ResetStars()
    {
        // Set all stars to empty (gray)
        for (int i = 0; i < starImages.Length; i++)
        {
            if (starImages[i] != null)
            {
                starImages[i].sprite = emptyStarSprite;
            }
        }
        currentRating = 0;
    }

    public void OnStarClicked(int starValue)
    {
        // Update current rating
        currentRating = starValue;

        // Fill this star and all stars to its left
        for (int i = 0; i < starImages.Length; i++)
        {
            if (starImages[i] != null)
            {
                // If this star's position is less than or equal to the clicked star
                // (remember arrays are 0-indexed but star values are 1-5)
                if (i < starValue)
                {
                    starImages[i].sprite = filledStarSprite; // Fill the star (yellow)
                }
                else
                {
                    starImages[i].sprite = emptyStarSprite; // Leave empty (gray)
                }
            }
        }
    }

    public void OnSubmitButtonClicked()
    {
        // Only process if user has selected a rating
        if (currentRating > 0)
        {
            if (currentRating == 5)
            {
                // For 5-star rating, redirect to Play Store
                RedirectToPlayStoreReview();
            }
            else
            {
                // For 1-4 star rating, open email feedback
                OpenEmailWithRatingAndDeviceInfo();
            }
        }
        else
        {
            // Optional: Show message asking user to select a rating
            Debug.Log("Please select a rating before submitting");
            // You could display a UI message here
        }
    }

    public void RedirectToPlayStoreReview()
    {
        // Open the Play Store URL
        Application.OpenURL(playStoreAppUrl);
    }

    public void OpenEmailWithRatingAndDeviceInfo()
    {
        // Get user's system ID (unique identifier)
        string userID = SystemInfo.deviceUniqueIdentifier;

        // Get device information
        string platform = Application.platform.ToString();
        string deviceModel = SystemInfo.deviceModel;
        string operatingSystem = SystemInfo.operatingSystem;

        // Create email body with rating and device information
        string emailBody = "Please Enter your message here\n\n" +
                          "******_______******\n\n" +
                          "Please Do Not Modify This\n\n" +
                          "User Rating: " + currentRating + " stars\n\n" +
                          "User_Name:" + userID + "\n\n" +
                          "Unique_Name:" + userID + "\n\n" +
                          "Platform:" + platform + "\n\n" +
                          "Model:" + deviceModel + "\n\n" +
                          "OS:" + operatingSystem;

        // Create the mailto URL
        string mailtoUrl = "mailto:" + developerEmail +
                          "?subject=" + WWW.EscapeURL(emailSubject) +
                          "&body=" + WWW.EscapeURL(emailBody);

        // Open the email client
        Application.OpenURL(mailtoUrl);
    }
}