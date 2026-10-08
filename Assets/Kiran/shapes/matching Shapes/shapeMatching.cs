using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class shapeMatching : MonoBehaviour
{
    public Button circle1;
    public Button circle2;

    public Button triangle1;
    public Button triangle2;

    public Button square1;
    public Button square2;

    public Button rect1;
    public Button rect2;
    
    public bool circle = true;
    public bool triangle = true;
    public bool square = true;
    public bool rect = true;

    public int circleCount = 0;
    public int triangleCount = 0;
    public int squareCount = 0;
    public int rectCount = 0;

    public GameObject poppperManager;
    private PopperManagerNew popperManagerNew;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (circleCount==2 && rectCount ==2&& triangleCount == 2 &&squareCount==2)
        {
            Debug.Log("won");
        }


    }
    public void isCircle()
    {
        if (circle == true)
        {
            Debug.Log(circleCount);
            triangle = false;
            square = false;
            rect = false;

            circleCount++;
            if (circleCount == 2)
            {
                triangle = true;
                square = true;
                rect = true;
            
                circle1.gameObject.SetActive(false);
                circle2.gameObject.SetActive(false);
            }
        }
        else 
        {
            Debug.Log("wrongChoice");
        }

    }
    public void isTriangle()
    {
        if (triangle == true)
        {
            Debug.Log(triangleCount);
            circle = false;
            square = false;
            rect = false;

            triangleCount++;
            if (triangleCount == 2)
            {
                circle = true;
                square = true;
                rect = true;
            
                triangle1.gameObject.SetActive(false);
                triangle2.gameObject.SetActive(false);
            }
        }
        else
        {
            Debug.Log("wrongChoice");
        }
    }
    public void isSquare()
    {
        if (square == true)
        {
            Debug.Log(squareCount);
            circle = false;
            triangle = false;
            rect = false;

            squareCount++;
            if (squareCount == 2)
            {
                circle = true;
                triangle = true;
                rect = true;
            
                square1.gameObject.SetActive(false);
                square2.gameObject.SetActive(false);
            }
        }
        else
        {
            Debug.Log("wrongChoice");
        }
    }
    public void isRect()
    {
        if (rect == true)
        {
            Debug.Log(rectCount);
            circle = false;
            square = false;
            triangle = false;

            rectCount++;
            if (rectCount == 2)
            {
                circle = true;
                square = true;
                triangle = true;
           
                rect1.gameObject.SetActive(false);
               rect2.gameObject.SetActive(false);
            }
        }
        else
        {
            Debug.Log("wrongChoice");
        }
    }
    
}