using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

public class Lvl11_Game1 : MonoBehaviour
{
    public RectTransform target;
    public RectTransform bar;
    public RectTransform smallTarget;
    public RectTransform smallObj;

    public float rotation = 30f;

    public bool antiRotate = false;

    public UnityEvent OnWin;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MoveObj() {
        Sequence sequence = DOTween.Sequence();

        sequence.Append(transform.DOMove(target.position, 1f));
        if (smallObj != null) {
            sequence.Join(smallObj.DOMove(smallTarget.position, 1f));
        }
        sequence.Append(bar.DORotate(new Vector3(0f, 0f, rotation), 1f));
        if (antiRotate) {
            sequence.Join(transform.DORotate(new Vector3(0f, 0f, 0f), 1f));
        }
        //sequence.Join(transform.DORotate(new Vector3(0f,0f,0f), 1f));
        sequence.AppendCallback(() =>
        {
            OnWin?.Invoke();
        });
        sequence.Play();
    }

   
}
