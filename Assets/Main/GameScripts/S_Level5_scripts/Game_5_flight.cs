using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Game_5_flight : MonoBehaviour
{
    public RectTransform finalTarget;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnSuccessfulDrop() {
        transform.DOMove(finalTarget.position, 1.5f);
    
    }
}
