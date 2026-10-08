using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameJersey : MonoBehaviour
{
    public RectTransform onePlayer;
    public RectTransform twoPlayer;
    public void appearNext()
    { 
        onePlayer.gameObject.SetActive(true);
        twoPlayer.gameObject.SetActive(true);
    }
}
