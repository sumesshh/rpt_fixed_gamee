using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class Lvl11_Game5 : MonoBehaviour
{
    public RectTransform dragTransform;
    public RectTransform smallLogRect;
    public RectTransform parentRect;
    public RectTransform ballonTarget;
    public RectTransform commonTarget;
    public RectTransform balloonLockPositionBig;
    public RectTransform balloonLockPositionSmall;

    public GameObject retry;
    public GameObject congratulaions;
    private newDrag[] dragScripts;
    public UnityEvent OnWin;

    [Header("Wind Effect Settings")]
    public float maxMovement = 50f;
    public float moveDuration = 2f;
    public float movementInterval = 1f;
    public float rotationAmount = 5f;

    private Vector3 balloonInitalPos;
    private Vector3 smallLogInitial;
    private Vector3 commonPos;
    private Vector2 anchorPoint;


    // Start is called before the first frame update
    void Start()
    {   
        
        dragScripts = dragTransform.gameObject.GetComponents<newDrag>();

        balloonInitalPos = dragTransform.position;
        smallLogInitial = smallLogRect.position;
        commonPos = parentRect.position;


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BigLogDrop() {
        foreach (var dragScript in dragScripts) {
            dragScript.allowMove = false;
        }

        Sequence sequence = DOTween.Sequence();
        sequence.AppendCallback(() =>
        {


            Vector2 size = dragTransform.rect.size;
            Vector2 pivotChange = new Vector2(0, 0.5f); // Difference between (0.5, 0.5) and (0.5, 0)

            // Adjust anchored position to counteract the movement
     

            // Change the pivot
            dragTransform.pivot = new Vector2(0.5f, 0);
            dragTransform.anchoredPosition -= new Vector2(0, pivotChange.y * size.y);
        });
        sequence.Append(dragTransform.DOMove(balloonLockPositionBig.position, 1f));
        sequence.AppendCallback(() =>
        {

            //// Convert local position to anchored position
            //Vector2 offset = dragTransform.pivot - new Vector2(0.5f, 0.5f);
            //Vector2 size = dragTransform.rect.size;
            //Vector2 newAnchoredPosition = dragTransform.anchoredPosition + new Vector2(offset.x * size.x, offset.y * size.y);

            //// Change pivot
            //dragTransform.pivot = new Vector2(0.5f, 0f);

            //// Restore the corrected anchored position
            //dragTransform.anchoredPosition = newAnchoredPosition;
            //// Store the current world position
            //Vector3 worldPosition = dragTransform.position;

            //// Change the pivot
            //dragTransform.pivot = new Vector2(0.5f, 0f);

            //// Restore the world position
            //dragTransform.position = worldPosition;

            //StartWindEffect();

            
            Sequence windSequence = DOTween.Sequence();

            windSequence.Append(
                dragTransform.DORotate(new Vector3(0, 0, 10f), 0.5f)
                .SetEase(Ease.InOutSine))
                .SetRelative(true);

            windSequence.Append(
                 dragTransform.DORotate(new Vector3(0, 0, -10f), 0.5f)
                .SetEase(Ease.InOutSine));

            windSequence.SetLoops(-1, LoopType.Yoyo);
            //dragTransform.DORotate(new Vector3(0, 0, 10f), 2f)
            //.SetLoops(-1, LoopType.Yoyo)  // Infinite Yoyo loop
            //.SetEase(Ease.InOutSine)
            //.SetRelative(true);     // Smooth easing
            retry.SetActive(false);
            congratulaions.SetActive(true);
            OnWin?.Invoke();

        });


    }

    public void SmallLogDrop() {
        foreach (var dragScript in dragScripts) { 
            dragScript.allowMove = false;
        }

        Sequence sequence = DOTween.Sequence();
        sequence.Append(dragTransform.DOMove(balloonLockPositionSmall.position, 0.5f));
        sequence.Append(parentRect.DOMove(commonTarget.position, 3f));
        sequence.AppendCallback(() =>
        {
           
            //smallLogRect.position = smallLogInitial;
            parentRect.position = commonPos;
            dragTransform.position = balloonInitalPos;
            StartCoroutine(RetryDelay());

            foreach (var dragScript in dragScripts)
            {
                dragScript.allowMove = true;
            }

        });

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
        swaySequence.Join(dragTransform.DOLocalRotate(new Vector3(0, 0, rotation), moveDuration).SetEase(Ease.InOutSine));
        swaySequence.Join(dragTransform.DOAnchorPos(new Vector2(anchorPoint.x + randomX, anchorPoint.y), moveDuration).SetEase(Ease.InOutSine));
    }

    private IEnumerator RetryDelay() {
        retry.SetActive(true);
        yield return new WaitForSeconds(2f);
        retry.SetActive(false);
    }
}
