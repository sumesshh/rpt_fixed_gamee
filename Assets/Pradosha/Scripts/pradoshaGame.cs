using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pradoshaGame : MonoBehaviour
{
    public GameObject[] Games;
    int count = 0;
    public void next()
    {
        count++;
        for(int i=0;i<Games.Length;i++)
        {
            if(i==count)
            {
                Games[i].SetActive(true);
            }
            else
            {
                Games[i].SetActive(false);
            }
        }
    }
    public void prev()
    {
        count--;
        for (int i = 0; i < Games.Length; i++)
        {
            if (i == count)
            {
                Games[i].SetActive(true);
            }
            else
            {
                Games[i].SetActive(false);
            }
        }
    }

}
