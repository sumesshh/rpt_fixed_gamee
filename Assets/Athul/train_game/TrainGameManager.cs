using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

public class TrainGameManager : MonoBehaviour
{
    [System.Serializable]
    public class TrainCoach
    {
        public GameObject coachObject;
        public int number;
    }

    [Header("Game Setup")]
    public List<TrainCoach> coaches = new List<TrainCoach>();

    [Header("Events")]
    public UnityEvent onCorrectPlacement;
    public UnityEvent onIncorrectPlacement;
    public UnityEvent onGameComplete;

    private int currentCoachIndex = 0;
    private List<int> correctOrder;

    void Start()
    {
        // Hide all coaches initially
        foreach (var coach in coaches)
        {
            coach.coachObject.SetActive(false);

            // Subscribe to the coach's drop events
            newDrag dragScript = coach.coachObject.GetComponent<newDrag>();
            if (dragScript != null)
            {
                // Important: Using lambda with captured variable can cause issues
                // Store the coach in a local variable
                var currentCoach = coach;
                dragScript.onTargetZoneDrop.RemoveAllListeners(); // Clear existing listeners
                dragScript.onTargetZoneDrop.AddListener(() => OnCoachPlaced(currentCoach));
                dragScript.NotOnTargetZoneDrop.RemoveAllListeners(); // Clear existing listeners
                dragScript.NotOnTargetZoneDrop.AddListener(OnIncorrectPlacement);
            }
        }

        Debug.Log("Initializing game with coaches count: " + coaches.Count);

        // Set up the correct order (50, 70, 30)
        correctOrder = new List<int> { 50, 70, 30 };

        // Spawn the first coach (50)
        SpawnNextCoach();
    }

    void SpawnNextCoach()
    {
        Debug.Log("Attempting to spawn next coach. Current index: " + currentCoachIndex);

        if (currentCoachIndex >= correctOrder.Count)
        {
            Debug.Log("Game Complete!");
            onGameComplete?.Invoke();
            return;
        }

        // Find the coach with the number matching our correct order
        int targetNumber = correctOrder[currentCoachIndex];
        TrainCoach coachToSpawn = coaches.Find(c => c.number == targetNumber);

        Debug.Log($"Looking for coach with number {targetNumber}");

        if (coachToSpawn != null)
        {
            Debug.Log($"Spawning coach with number {coachToSpawn.number}");
            coachToSpawn.coachObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning($"Could not find coach with number {targetNumber}");
        }
    }

    void OnCoachPlaced(TrainCoach placedCoach)
    {
        Debug.Log($"Coach placed: {placedCoach.number}, Current expected: {correctOrder[currentCoachIndex]}");

        // Check if this coach was placed in the correct order
        if (placedCoach.number == correctOrder[currentCoachIndex])
        {
            Debug.Log("Correct placement! Moving to next coach");
            onCorrectPlacement?.Invoke();
            currentCoachIndex++;

            // Add a small delay before spawning the next coach
            Invoke("SpawnNextCoach", 0.5f);
        }
        else
        {
            Debug.Log("Incorrect placement!");
            onIncorrectPlacement?.Invoke();
        }
    }

    void OnIncorrectPlacement()
    {
        Debug.Log("Coach placed incorrectly");
        onIncorrectPlacement?.Invoke();
    }

    // Public method to check current game state
    public void DebugGameState()
    {
        Debug.Log($"Current coach index: {currentCoachIndex}");
        Debug.Log($"Next expected coach number: {(currentCoachIndex < correctOrder.Count ? correctOrder[currentCoachIndex] : "None")}");
        Debug.Log("Active coaches:");
        foreach (var coach in coaches)
        {
            Debug.Log($"Coach {coach.number}: {coach.coachObject.activeSelf}");
        }
    }
}