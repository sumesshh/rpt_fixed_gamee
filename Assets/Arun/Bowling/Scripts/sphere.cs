using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;


public class sphere : MonoBehaviour
{
    public Transform targetPosition;
    public float speed = 2f;
    public float rollSpeed = 200f;
    public float stopDistance = 0.5f;
    bool roll=false;

    void OnButtonClick()
    {

        if (targetPosition != null)

        {
            float distance = Vector3.Distance(transform.position, targetPosition.position);


            if (distance <= stopDistance)
            {
                return;
            }
            Vector3 direction = (targetPosition.position - transform.position).normalized;
            transform.position = Vector3.Lerp(transform.position, targetPosition.position, speed * Time.deltaTime);
            Vector3 rotationAxis = new Vector3(targetPosition.position.y, -targetPosition.position.x, 0);
            float rotationAngle = rollSpeed * Time.deltaTime;
            transform.Rotate(rotationAxis, rotationAngle, Space.World);
        }
    }
    void Update()
    {

        if (Input.GetMouseButtonDown(0)) // Detect Left Click
        {
            roll=true;
           
        }
        if (roll)
        {

            OnButtonClick();
        }

    }
}

