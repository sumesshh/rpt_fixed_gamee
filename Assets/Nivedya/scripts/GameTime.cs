using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class GameTime : MonoBehaviour
{
    public RectTransform sun;
    public RectTransform target1;
    public RectTransform target2;
    public RectTransform target3;
    public RectTransform target4;
    public GameObject morning;
    public GameObject noon;
    public GameObject night;
    public Button One;
    public Button Two;
    public Button Three;
    public Sprite[] suns;
    public Image bg; // Background Image to change color

    private Image sunImage;
    private float transitionTime = 1f;

    private Color morningColor = new Color(1f, 0.9f, 0.5f);
    private Color noonColor = new Color(0.6f, 0.8f, 1f);      // Example: Bright Blue
    private Color nightColor = new Color(0.1f, 0.1f, 0.3f);    // Example: Dark Blue

    private void Start()
    {
        sunImage = sun.GetComponent<Image>();
        SetOneAppear();
    }

    public void SetOneAppear()
    {
        MoveSun(target1.position, 2f, () => ShowPanel(morning), -1, morningColor);
        One.onClick.AddListener(SetTwoAppear);
    }

    public void SetTwoAppear()
    {
        HidePanel(morning);
        MoveSun(target2.position, 2f, () => ShowPanel(noon), 0, noonColor);
        Two.onClick.AddListener(SetThreeAppear);
    }

    public void SetThreeAppear()
    {
        HidePanel(noon);
        MoveSun(target3.position, 2f, () => ShowPanel(night), 1, nightColor);
        Three.onClick.AddListener(EndGame);
    }

    public void EndGame()
    {
        HidePanel(night);
        MoveSun(target4.position, 2f, null, -1, Color.black); // Final transition (optional)
    }

    void MoveSun(Vector3 targetPosition, float duration, TweenCallback onCompleteAction, int spriteIndex, Color targetColor)
    {
        float transitionTriggerTime = duration - transitionTime;
        Tween moveTween = null;

        moveTween = sun.DOMove(targetPosition, duration).SetEase(Ease.InOutQuad)
            .OnUpdate(() =>
            {
                if (spriteIndex >= 0 && moveTween.Elapsed() >= transitionTriggerTime)
                {
                    ChangeImageSmoothly(spriteIndex);
                    spriteIndex = -1;
                }
            })
            .OnComplete(onCompleteAction);

        // Change the background color smoothly
        bg.DOColor(targetColor, duration).SetEase(Ease.InOutQuad);
    }

    void ChangeImageSmoothly(int index)
    {
        if (suns.Length == 0 || index >= suns.Length) return;

        sunImage.DOFade(0, 0.5f).OnComplete(() =>
        {
            sunImage.sprite = suns[index];
            sunImage.DOFade(1, 0.5f);
        });
    }

    void ShowPanel(GameObject panel)
    {
        panel.SetActive(true);
        CanvasGroup canvasGroup = panel.GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            canvasGroup = panel.AddComponent<CanvasGroup>();
        }

        canvasGroup.alpha = 0;
        canvasGroup.DOFade(1, 0.5f);

        panel.transform.localScale = Vector3.zero;
        panel.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
    }

    void HidePanel(GameObject panel)
    {
        CanvasGroup canvasGroup = panel.GetComponent<CanvasGroup>();

        if (canvasGroup != null)
        {
            canvasGroup.DOFade(0, 0.5f).OnComplete(() => panel.SetActive(false));
        }

        panel.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack);
    }
}
