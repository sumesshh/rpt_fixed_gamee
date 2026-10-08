using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using static CircleDetector;
using static UnityEngine.GraphicsBuffer;

public class TreasureGame : MonoBehaviour
{
    public Button island1;
    public Button island2;
    public Button island3;
    public Image treasure;
    public Image target;
    public void DisableOtherButtons()
    { 
        island1.interactable = false;
        island2.interactable = false;
        island3.interactable = false;
        treasure.transform.DOMove(target.transform.position, 2f);

    }

}
