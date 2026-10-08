using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class S_Game11_Manager : MonoBehaviour
{
    public Canvas canvas;
    public RectTransform Ball;
    public RectTransform targetArea;
    public Camera UICamera; // Assign your UI Camera
    public GameObject buttons;
    public GameObject firstQn;
    public GameObject secondQn;
    public GameObject textAnimation;
    public GameObject LevelManager;
    public Button inside;
    public Button outside;
    private bool isInside;
    public UnityEvent GameWin;
    private buttonShake shaker;

    private float duration = 1f;
    private bool canClick = true;
    private Vector3 originalScale;

    void Update()
    {
        //if (ball.transform.position != lastPos)
        //{
        //    lastPos = ball.transform.position;
        //    ActivateSecond();
        //}

        //Checker();

        if (canClick && Input.GetMouseButtonDown(0))
        {
            MoveBall();
            ActivateSecond();
            canClick = false;
        }

    }

    private void Start()
    {
        isInside = false;
        firstQn.SetActive(true);
        buttons.SetActive(false);
        secondQn.SetActive(false);
        
        
        shaker = LevelManager.GetComponent<buttonShake>();

        originalScale = Ball.localScale;

        // Ensure we have a reference to the camera
        if (UICamera == null && canvas.renderMode == RenderMode.ScreenSpaceCamera)
        {
            UICamera = canvas.worldCamera;
        }
    }

    //void IsBallInsideTarget()
    //{
    //    // Get the screen-space Rect of the target
    //    Rect targetRect = RectTransformToScreenSpace(target);

    //    // Get the ball's screen position
    //    Vector2 ballScreenPos = RectTransformUtility.WorldToScreenPoint(uiCamera, ball.position);

    //    // Check if the ball's position is inside the target's rect
    //    if (targetRect.Contains(ballScreenPos))
    //    {

    //        isInside = true;
    //        Debug.Log("Inside other");

    //    }
        
    //}

    //Rect RectTransformToScreenSpace(RectTransform rt)
    //{
    //    Vector3[] corners = new Vector3[4];
    //    rt.GetWorldCorners(corners); // Get world corners of RectTransform

    //    Vector2 min = RectTransformUtility.WorldToScreenPoint(uiCamera, corners[0]); // Bottom-left
    //    Vector2 max = RectTransformUtility.WorldToScreenPoint(uiCamera, corners[2]); // Top-right

    //    return new Rect(min, max - min);
    //}

    void ActivateSecond() {
        
        firstQn.SetActive(false);
        buttons.SetActive(true);
        secondQn.SetActive(true);
        textAnimation.SetActive(true);
    
    }

    public void InsideCheck() {
        if (isInside)
        {
            outside.interactable = false;
            inside.interactable = false;
            if (GameWin != null)
            {
                GameWin.Invoke();
            }

        }
        else {
            Shake(inside);
        
        }
    
    }

    public void OutsideCheck() {
        if (!isInside)
        {
            inside.interactable = false;
            outside.interactable = false;
            if (GameWin != null)
            {
                GameWin.Invoke();
               
            }
        }
        else {
            Shake(outside);
        
        }
    
    }

    public void Shake(Button button)
    {
        shaker.PlayShakeAudio();
        button.interactable = false;
        Color initialColor;
        Transform btnTransform;
        Image buttonImage;
        btnTransform = button.GetComponent<Transform>();
        buttonImage = button.GetComponent<Image>();

        initialColor = buttonImage.color;

        Vector3 initialPosition = btnTransform.localPosition;

        Sequence ShakeTween = DOTween.Sequence()
        .Append(btnTransform.DOShakePosition(0.5f, new Vector3(50f, 0f, 0f), 15, 0, false, true))
        .Join(buttonImage.DOColor(Color.red, 0.3f))
        .OnComplete(() =>
        {
            btnTransform.localPosition = initialPosition;
            buttonImage.DOColor(initialColor, 0.3f).OnComplete(() => button.interactable = true);

        }
        )
        .Play();

    }

    void MoveBall()
    {
        // Convert mouse position to world space
        Vector2 mousePos = Input.mousePosition;
        Vector3 worldPoint;

        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(
            canvas.GetComponent<RectTransform>(),
            mousePos,
            UICamera,
            out worldPoint))
        {
            // Calculate move direction in world space
            Vector3 moveDirection = (worldPoint - Ball.position).normalized;

            // Check if click is within target area
            if (RectTransformUtility.RectangleContainsScreenPoint(
                targetArea,
                mousePos,
                UICamera))
            {
                // Move to exact click position if inside target area
                Ball.DOMove(worldPoint, duration).SetEase(Ease.OutSine)
                    .OnComplete(() => canClick = false);
                Debug.Log("Inside");
                isInside = true;
            }
            else
            {
                // Move in direction of click if outside target area
                //Vector3 outsidePosition = Ball.position + (moveDirection);
                //Ball.DOMove(outsidePosition, duration).SetEase(Ease.OutSine)
                Ball.DOMove(worldPoint, duration).SetEase(Ease.OutSine)
                    .OnComplete(() => canClick = false);
                Debug.Log("Outside");
                isInside = false;
            }

            // Apply rotation and scale animations
            Ball.DOLocalRotate(new Vector3(0, 0, 360), duration, RotateMode.FastBeyond360);
            Ball.DOScale(originalScale * 0.9f, duration)
                .OnComplete(() => Ball.DOScale(originalScale, duration * 0.5f));
        }
    }

    //public void Checker() {
    //    Vector2 mousePos = Input.mousePosition;
       

    //    //if (RectTransformUtility.ScreenPointToWorldPointInRectangle(
    //    //    canvas.GetComponent<RectTransform>(),
    //    //    mousePos,
    //    //    uiCamera,
    //    //    out worldPoint))
    //    //{
         
           

    //        // Check if click is within target area
    //        if (RectTransformUtility.RectangleContainsScreenPoint(
    //            target,
    //            mousePos,
    //            uiCamera))
    //        {
    //            // Move to exact click position if inside target area
    //            isInside = true;
    //            Debug.Log("Inside Manager");
    //        }
    //        else
    //        {
    //            isInside = false;
    //            Debug.Log("Outside Manager");
    //        }

           
    //    //}
    //}


}
