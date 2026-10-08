using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

public class Lvl10_Game2 : MonoBehaviour
{
    // Start is called before the first frame update

    public RectTransform car;
    public RectTransform rope;
    public RectTransform parent;

    public RectTransform target;

    public UnityEvent onWin;



    private Sequence sequence;

    public float rotate = -20;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MoveCar() {
        sequence = DOTween.Sequence();

        sequence.Append(rope.DOMove(target.position, 1f));
        sequence.Join(rope.DORotate(new Vector3(0f, 0f, rotate), 1f));
        sequence.Join(rope.DOScaleX(2.5f, 1f));

        sequence.AppendCallback(GameWon);
        sequence.Append(parent.DOMoveX(-10, 2f));
    
    }

    private void GameWon() { 
        if(onWin != null) onWin.Invoke();   
    }
}
