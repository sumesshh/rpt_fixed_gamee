using UnityEngine;
using TMPro;

public class TimerManager : MonoBehaviour
{
    // UI References
    [SerializeField] private TextMeshProUGUI timerText; // Display timer
    [SerializeField] private TextMeshProUGUI allTimesText; // To display previous times


    // Timer variables
    private bool isTimerRunning = false;
    private float timer = 0f;

    // Start/Stop buttons
    [SerializeField] private GameObject startButton;
    [SerializeField] private GameObject stopButton;

    // Track previous times
    private string previousTimes = "";

    void Start()
    {
        // Initially disable the Stop button and enable the Start button
        stopButton.SetActive(false);
    }

    void Update()
    {
        // Start the timer if it's running
        if (isTimerRunning)
        {
            timer += Time.deltaTime;
            timerText.text = $"Time: {FormatTime(timer)}"; // Display the timer
        }
    }

    // Start button functionality
    public void OnStartButtonPressed()
    {
        isTimerRunning = true; // Start the timer
        startButton.SetActive(false); // Disable Start button
        stopButton.SetActive(true); // Enable Stop button
    }

    // Stop button functionality
    public void OnStopButtonPressed()
    {
        isTimerRunning = false; // Stop the timer
        AddTimeToHistory(); // Add the current time to previous times
        startButton.SetActive(true); // Enable Start button
        stopButton.SetActive(false); // Disable Stop button
    }

    // Add the current time to the history
    private void AddTimeToHistory()
    {
        string formattedTime = FormatTime(timer); // Format the current time
        previousTimes += $"{formattedTime}\n"; // Add it to the list of previous times
        allTimesText.text = previousTimes; // Update the text to show all previous times
        timer = 0f; // Reset the timer for the next cycle
    }

    // Format the timer value into a readable string (HH:MM:SS)
    private string FormatTime(float timeInSeconds)
    {
        int minutes = (int)(timeInSeconds / 60);
        int seconds = (int)(timeInSeconds % 60);
        return $"{minutes:00}:{seconds:00}";
    }
}
