using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;


public class Roller : MonoBehaviour
{

    public RectTransform target;  // Assign your UI image of the wheel

    public float moveDuration = 1f;   // Time taken for one side movement
    public float rotationAmount = 360f; // Rotation amount per move

    private Vector3 initialPosition;

    public bool rotate = true;

 

    Sequence moveSequence;

    private void Start()
    {
      
        //RotateWheel();
        initialPosition = transform.position;
        MoveAndRotateObject();
    }




    public void StopMovement()
    {
        if (moveSequence != null && moveSequence.IsPlaying()) // Check if active
        {
            moveSequence.Kill(); // Stop the movement
        }
    }

    private void MoveAndRotateObject()
    {
      
        moveSequence = DOTween.Sequence();

        // Move to target & rotate clockwise
        moveSequence.Append(transform.DOMoveX(target.position.x, moveDuration)
            .SetEase(Ease.InOutSine));

        if (rotate) {
            moveSequence.Join(transform.DORotate(new Vector3(0, 0, -rotationAmount), moveDuration, RotateMode.LocalAxisAdd)
                .SetEase(Ease.Linear));
        }
        

        // Move back to initial position & rotate counterclockwise
        

            moveSequence.Append(transform.DOMoveX(initialPosition.x, moveDuration)
                .SetEase(Ease.InOutSine));

        if (rotate) {
            moveSequence.Join(transform.DORotate(new Vector3(0, 0, rotationAmount), moveDuration, RotateMode.LocalAxisAdd)
                    .SetEase(Ease.Linear));

        }
            
        moveSequence.SetLoops(-1); // Loop infinitely
    }
}
