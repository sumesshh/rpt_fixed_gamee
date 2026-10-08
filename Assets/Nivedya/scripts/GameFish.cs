using System.Collections;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameFish : MonoBehaviour
{
    public GameObject scoreObj;
    public RectTransform[] fishes; // UI images of fishes
    public Button[] numberButtons; // Buttons with numbers
    public Vector2[] startPositions; // Unique starting positions (off-screen)
    public Vector2[] scenePositions; // Unique scene positions (inside)

    private int correctFishCount; // Number of fishes in the scene
    private int round = 0; // Current round
    private int winCount = 0; // Tracks number of wins
    public UnityEvent win;
    private ScoreSaver scoreScript;

    void Start()
    {
        if (scoreObj != null)
        {
            scoreScript = scoreObj.GetComponent<ScoreSaver>();
            scoreScript.totalTime = 50f;
            scoreScript.elapsedTime = 50f;
        }
        foreach (var button in numberButtons)
        {
            button.onClick.AddListener(() => CheckAnswer(button));
        }

        StartNewRound();
    }

    void StartNewRound()
    {
        if (winCount >= 2)
        {
            Debug.Log("Game Over! You won 2 times.");
            win.Invoke();
            return;
        }

        round++;
        correctFishCount = Random.Range(1, 6);
        Debug.Log($"Round {round}: {correctFishCount} fishes");

        // Move all fishes to their unique off-screen positions and disable them
        for (int i = 0; i < fishes.Length; i++)
        {
            fishes[i].anchoredPosition = startPositions[i];
            fishes[i].gameObject.SetActive(false);
        }

        StartCoroutine(SpawnFishes());
    }

    IEnumerator SpawnFishes()
    {
        for (int i = 0; i < correctFishCount; i++)
        {
            fishes[i].gameObject.SetActive(true);
            fishes[i].DOAnchorPos(scenePositions[i], 2f).SetEase(Ease.OutBack);
            yield return new WaitForSeconds(0.5f);
        }
    }

    public void CheckAnswer(Button clickedButton)
    {
        int chosenNumber = int.Parse(clickedButton.GetComponentInChildren<Text>().text);

        if (chosenNumber == correctFishCount)
        {
            Debug.Log("Win!");
            CorrectChoice(clickedButton);
            winCount++;
        }
        else
        {
            Debug.Log("Lose!");
            Shake(clickedButton);
        }

        StartCoroutine(MoveFishesOut());
    }

    IEnumerator MoveFishesOut()
    {
        for (int i = 0; i < correctFishCount; i++)
        {
            fishes[i].DOAnchorPos(startPositions[i], 1f).SetEase(Ease.InBack);
            yield return new WaitForSeconds(0.3f);
        }

        yield return new WaitForSeconds(1f);
        StartNewRound();
    }

    public void CorrectChoice(Button button)
    {
        Image buttonImage = button.GetComponent<Image>();
        Color initialColor = buttonImage.color;
        button.interactable = false;

        Sequence blinkTween = DOTween.Sequence()
            .Append(buttonImage.DOColor(Color.green, 0.2f)) // Green color
            .Append(buttonImage.DOColor(initialColor, 0.2f)) // Back to normal
            .Append(buttonImage.DOColor(Color.green, 0.2f)) // Green again
            .Append(buttonImage.DOColor(initialColor, 0.2f)) // Back to normal
            .OnComplete(() => button.interactable = true) // Disable button after blinking
            .Play();
    }

    public void Shake(Button button)
    {
        Image buttonImage = button.GetComponent<Image>();
        Vector3 initialPosition = button.transform.localPosition;
        Color initialColor = buttonImage.color;

        Sequence shakeTween = DOTween.Sequence()
            .Append(button.transform.DOShakePosition(0.5f, new Vector3(50f, 0f, 0f), 15, 0, false, true))
            .Join(buttonImage.DOColor(Color.red, 0.3f))
            .OnComplete(() =>
            {
                button.transform.localPosition = initialPosition;
                buttonImage.DOColor(initialColor, 0.3f).OnComplete(() => button.interactable = true);
            })
            .Play();
    }
    public void reloadGame()
    {
        SceneManager.LoadScene("newGameNiv");
    }
}
