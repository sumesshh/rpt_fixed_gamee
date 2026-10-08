using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;

public class WormManager : MonoBehaviour
{
    [System.Serializable]
    public class WormData
    {
        public GameObject wormPrefab;
        public int correctAnswerIndex;
    }

    public WormData[] wormData;
    public Transform[] answerButtons;
    public Transform canvasCenter;
    public Transform leftExit;
    public Transform rightEntry;
    public AudioClip winSound;
    public AudioClip loseSound;
    public UnityEvent winEvent;

    public float moveDuration = 4f;
    public float stepDuration = 0.25f;
    public float stepHeight = 0.02f;
    public float shakeStrength = 10f;
    public int shakeVibrato = 10;

    private GameObject currentWorm;
    private int currentLevel = 0;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>(); // Add AudioSource component
        StartLevel();
    }

    void StartLevel()
    {
        if (currentLevel >= wormData.Length)
        {
            Debug.Log("All levels completed!");
            winEvent.Invoke();
            return;
        }

        // Enable all answer buttons
        SetButtonsInteractable(true);

        currentWorm = Instantiate(wormData[currentLevel].wormPrefab, rightEntry.position, Quaternion.identity, canvasCenter);
        MoveWormToCenter(currentWorm.transform);

        for (int i = 0; i < answerButtons.Length; i++)
        {
            int index = i;
            answerButtons[i].GetComponent<Button>().onClick.RemoveAllListeners();
            answerButtons[i].GetComponent<Button>().onClick.AddListener(() => OnAnswerSelected(index));
        }
    }

    void MoveWormToCenter(Transform worm)
    {
        Sequence wormSequence = DOTween.Sequence();
        int stepCount = Mathf.FloorToInt(moveDuration / stepDuration);

        for (int i = 0; i < stepCount; i++)
        {
            float stepX = Mathf.Lerp(worm.position.x, canvasCenter.position.x, (float)(i + 1) / stepCount);
            float stepY = (i % 2 == 0) ? worm.position.y + stepHeight : worm.position.y;

            wormSequence.Append(worm.DOMove(new Vector3(stepX, stepY, 0), stepDuration).SetEase(Ease.InOutSine));
            wormSequence.Join(worm.DORotate(new Vector3(0, 0, (i % 2 == 0) ? 5f : -5f), stepDuration / 2).SetLoops(2, LoopType.Yoyo));
        }

        wormSequence.Append(worm.DOMove(canvasCenter.position, stepDuration).SetEase(Ease.OutQuad));
        wormSequence.Join(worm.DORotate(Vector3.zero, stepDuration));
    }

    Sequence MoveWormToExit(Transform worm)
    {
        Sequence wormSequence = DOTween.Sequence();
        int stepCount = Mathf.FloorToInt(moveDuration / stepDuration);

        for (int i = 0; i < stepCount; i++)
        {
            float stepX = Mathf.Lerp(worm.position.x, leftExit.position.x, (float)(i + 1) / stepCount);
            float stepY = (i % 2 == 0) ? worm.position.y + stepHeight : worm.position.y;

            wormSequence.Append(worm.DOMove(new Vector3(stepX, stepY, 0), stepDuration).SetEase(Ease.InOutSine));
            wormSequence.Join(worm.DORotate(new Vector3(0, 0, (i % 2 == 0) ? 5f : -5f), stepDuration / 2).SetLoops(2, LoopType.Yoyo));
        }

        wormSequence.Append(worm.DOMove(leftExit.position, stepDuration).SetEase(Ease.OutQuad));
        wormSequence.Join(worm.DORotate(Vector3.zero, stepDuration));
        return wormSequence;
    }

    void OnAnswerSelected(int selectedIndex)
    {
        if (selectedIndex == wormData[currentLevel].correctAnswerIndex)
        {
            Debug.Log("Correct Answer!");
            audioSource.PlayOneShot(winSound);

            // Disable all buttons
            SetButtonsInteractable(false);

            MoveWormToExit(currentWorm.transform).OnComplete(() =>
            {
                Destroy(currentWorm);
                currentLevel++;
                StartLevel(); // Start the next level (buttons will be re-enabled here)
            });
        }
        else
        {
            Debug.Log("Wrong Answer!");
            audioSource.PlayOneShot(loseSound);
            ShakeWorm();
        }
    }

    void ShakeWorm()
    {
        currentWorm.transform.DOShakePosition(0.5f, new Vector3(shakeStrength, 0, 0), shakeVibrato, 90, false, true);
    }

    void SetButtonsInteractable(bool interactable)
    {
        foreach (Transform buttonTransform in answerButtons)
        {
            Button button = buttonTransform.GetComponent<Button>();
            if (button != null)
            {
                button.interactable = interactable;
            }
        }
    }
}