using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;

public class GameTreasure : MonoBehaviour
{
    public Button[] numberButtons;
    public Text[] buttonTexts;
    public Transform target;
    public Transform target2;
    public UnityEvent win;
    private int correct = 0;

    private List<int[]> numberSets = new List<int[]>()
    {
        new int[] { 20, 27, 22, 25 },
        new int[] { 32, 8, 55, 10 },
        new int[] { 44, 29, 36, 11 },
        new int[] { 20, 25, 18, 72 }
    };

    private int currentSetIndex = 0; // Tracks the current number set
    public float delayBetweenSets = 2f;

    void Start()
    {
        buttonTexts = new Text[numberButtons.Length];

        for (int i = 0; i < numberButtons.Length; i++)
        {
            buttonTexts[i] = numberButtons[i].GetComponentInChildren<Text>();
            int index = i; // Capture index for lambda expression
            numberButtons[i].onClick.AddListener(() => StartCoroutine(CheckAnswer(index)));
        }

        SetNumbers();
    }

    void SetNumbers()
    {
        if (currentSetIndex >= numberSets.Count)
        {
            currentSetIndex = 0; // Restart if all sets are used
        }

        int[] currentSet = numberSets[currentSetIndex];

        for (int i = 0; i < numberButtons.Length; i++)
        {
            buttonTexts[i].text = currentSet[i].ToString();
        }
    }

    IEnumerator CheckAnswer(int buttonIndex)
    {
        int[] currentSet = numberSets[currentSetIndex];
        int maxNumber = Mathf.Max(currentSet); // Find the highest number
        bool isCorrect = int.Parse(buttonTexts[buttonIndex].text) == maxNumber;

        if (isCorrect)
        {
            Debug.Log("Correct!");
            correct++;
            foreach (Button btn in numberButtons)
            {
                btn.interactable = false;
            }

            // Get the correct image from the clicked button's child
            Transform correctImageTransform = numberButtons[buttonIndex].transform.Find("treasure");

            if (correctImageTransform != null)
            {
                Image correctImage = correctImageTransform.GetComponent<Image>();
                correctImage.gameObject.SetActive(true);

                if (correct == 1)
                {
                    correctImage.transform.DOMove(target.position, 2f);
                }
                else if (correct == 2)
                {
                    correctImage.transform.DOMove(target2.position, 2f);
                    yield return new WaitForSeconds(2f);
                    EndGame();
                    yield break;
                }
            }
        }
        else
        {
            foreach (Button btn in numberButtons)
            {
                btn.interactable = false;
            }
            Transform wrongImageTransform = numberButtons[buttonIndex].transform.Find("crab");

            if (wrongImageTransform != null)
            {
                Debug.Log("Wrong! Try again.");
                Image wrongImage = wrongImageTransform.GetComponent<Image>();
                wrongImage.gameObject.SetActive(true);
                Vector3 initialPos = wrongImage.transform.position; // Store initial position

                wrongImage.transform.DOMoveY(initialPos.y + 1, 0.5f).OnComplete(() =>
                {
                    wrongImage.transform.DOMoveY(initialPos.y, 0.5f).OnComplete(() =>
                    {
                        wrongImage.gameObject.SetActive(false); // Hide it after returning
                    });
                });
            }
        }

        yield return new WaitForSeconds(delayBetweenSets);
        foreach (Button btn in numberButtons)
        {
            btn.interactable = true;
        }
        currentSetIndex++;
        SetNumbers();
    }

    public void EndGame()
    {
        Debug.Log("You Win!");
        /*foreach (Button btn in numberButtons)
        {
            btn.interactable = false;
        }*/
        win.Invoke();
    }
}
