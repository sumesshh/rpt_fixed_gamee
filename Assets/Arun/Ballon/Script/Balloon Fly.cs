using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BalloonFly : MonoBehaviour
{
    [Header("References")]
    public Button Small_log;
    public Button Big_Log;
    public RectTransform balloon;
    public Camera UICamera;  // Add a reference to the UI Camera

    [Header("Wind Effect Settings")]
    public float maxMovement = 50f;
    public float moveDuration = 2f;
    public float movementInterval = 1f;
    public float rotationAmount = 5f;

    public UnityEvent Win;
    public UnityEvent Lose;

    private Vector2 original_Balloon, origin_Small_log, origin_Big_Log;
    private Vector2 anchorPoint;

    private void Start()
    {
        StoreOriginalPositions();
        SetupButtonListeners();
    }

    private void StoreOriginalPositions()
    {
        original_Balloon = balloon.anchoredPosition;
        origin_Small_log = Small_log.GetComponent<RectTransform>().anchoredPosition;
        origin_Big_Log = Big_Log.GetComponent<RectTransform>().anchoredPosition;
    }

    private void SetupButtonListeners()
    {
        Small_log.onClick.AddListener(small_logMOVE);
        Big_Log.onClick.AddListener(big_logMOVE);
    }

    public void small_logMOVE()
    {
        Big_Log.interactable = false;
        balloon_move_small(() => Fly_Up());
    }

    public void big_logMOVE()
    {
        Small_log.interactable = false;
        balloon_move_big();
    }


    public void balloon_move_small(TweenCallback onComplete)
    {
        RectTransform rectTransform = Small_log.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
          
            balloon.DOAnchorPos(
                new Vector2(rectTransform.anchoredPosition.x, rectTransform.anchoredPosition.y + balloon.rect.height / 3),
                1f 
            ).SetEase(Ease.InOutSine).OnComplete(onComplete);  
        }
    }


    //private void balloon_move_big()
    //{
    //    RectTransform rectTransform = Big_Log.GetComponent<RectTransform>();
    //    if (rectTransform != null)
    //    {
    //        balloon.DOAnchorPos(
    //            new Vector2(rectTransform.anchoredPosition.x, rectTransform.anchoredPosition.y + (balloon.rect.height / 2) - 100),
    //            1f
    //        )
    //        .SetEase(Ease.InOutSine) // Smooth movement
    //        .OnComplete(() =>
    //        {
    //            anchorPoint = balloon.anchoredPosition;
    //            StartWindEffect(); // Start wind effect after movement finishes
    //        });
    //    }


    //Win?.Invoke();
    //}
    private void balloon_move_big()
    {
        RectTransform rectTransform = Big_Log.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            balloon.DOAnchorPos(
                new Vector2(rectTransform.anchoredPosition.x, rectTransform.anchoredPosition.y + (balloon.rect.height / 2) - 100),
                1f
            )
            .SetEase(Ease.InOutSine) // Smooth movement
            .OnComplete(() =>
            {
                anchorPoint = balloon.anchoredPosition;
                StartWindEffect(); // Start wind effect after movement finishes

                // Delay Win event by 3 seconds
                DOVirtual.DelayedCall(1f, () => Win?.Invoke());
            });
        }
    }


    private void Fly_Up()
    {
        Vector2 targetPosition = new Vector2(balloon.anchoredPosition.x, balloon.anchoredPosition.y + 700f);
        balloon.DOAnchorPos(targetPosition, 4f);

        Vector2 logTarget = new Vector2(Small_log.GetComponent<RectTransform>().anchoredPosition.x, Small_log.GetComponent<RectTransform>().anchoredPosition.y + 700f);
        Small_log.GetComponent<RectTransform>().DOAnchorPos(logTarget, 4f).OnComplete(() => StartCoroutine(delay(1f)));
    }

    private IEnumerator delay(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        ResetUIElements();
    }

    public void ResetUIElements()
    {
        Small_log.interactable = true;
        Big_Log.interactable = true;
        balloon.anchoredPosition = original_Balloon;
        Small_log.GetComponent<RectTransform>().anchoredPosition = origin_Small_log;
        Big_Log.GetComponent<RectTransform>().anchoredPosition = origin_Big_Log;
    }

    private void StartWindEffect()
    {
        InvokeRepeating(nameof(SwayBalloon), 0f, movementInterval);
    }

    private void SwayBalloon()
    {
        float randomX = Random.Range(-maxMovement, maxMovement);
        float rotation = -(randomX / maxMovement) * rotationAmount;

        Sequence swaySequence = DOTween.Sequence();
        swaySequence.Join(balloon.DOLocalRotate(new Vector3(0, 0, rotation), moveDuration).SetEase(Ease.InOutSine));
        swaySequence.Join(balloon.DOAnchorPos(new Vector2(anchorPoint.x + randomX, anchorPoint.y), moveDuration).SetEase(Ease.InOutSine));
    }

    private void OnDestroy()
    {
        balloon.DOKill();
    }
}
