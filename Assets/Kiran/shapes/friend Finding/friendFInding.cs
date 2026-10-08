using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class friendFInding : MonoBehaviour
{
    public Button square;
    public Button triangle;
    public Button circle;

    public bool sq = false;
    public bool tr = false;
    public bool ci = false;

    public Button squareButton;
    public Button triangleButton;
    public Button circleButton;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void squareclick()
    {
        squareButton.gameObject.SetActive(true);
        triangleButton.gameObject.SetActive(true);
        circleButton.gameObject.SetActive(true);

        sq = true;
        tr = false;
        ci = false;
    }
    public void triangleclick()
    {
        squareButton.gameObject.SetActive(true);
        triangleButton.gameObject.SetActive(true);
        circleButton.gameObject.SetActive(true);

        tr = true;
        sq = false;
        ci = false;
    }

    public void circleclick()
    {
        squareButton.gameObject.SetActive(true);
        triangleButton.gameObject.SetActive(true);
        circleButton.gameObject.SetActive(true);

        ci = true;
        sq = false;
        tr = false;
    }

    public void squarebut()
    {
        if (sq == true)
        {
            Debug.Log("Won");
        }
        else
        {
            Debug.Log("Lost");
        }
    }
    public void trianglebut()
    {
        if (tr == true)
        {
            Debug.Log("Won");
        }
        else
        {
            Debug.Log("Lost");
        }
    }

    public void circlebut()
    {
        if (ci == true)
        {
            Debug.Log("Won");
        }
        else
        {
            Debug.Log("Lost");
        }
    }
}