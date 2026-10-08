using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Lvl10_Game6 : MonoBehaviour
{
    // Start is called before the first frame update
    public RectTransform target;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MoveTail()
    {
        transform.DOMove(target.position, 1.5f);
    }
}
