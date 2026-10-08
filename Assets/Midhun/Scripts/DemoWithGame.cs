using System.Collections;
using System.Collections.Generic;
//using System.Diagnostics;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class DemoWithGame : MonoBehaviour
{

    public RectTransform[] targetTransform;
    public GameObject child;
    public GameObject arrow;
    public GameObject hand;
    public float duration = 5f;
    public int loopCount = 2;
    private Vector3 initialPos;
    private newDrag childScript;
    public GameObject Score;

    private Tween movementTween;
    private ScoreSaver scoreScript;
    // Start is called before the first frame update
    void Start()
    {
        initialPos = transform.position;
        //childScript = child.GetComponent<newDrag>();
        childScript.allowMove = false;
        Debug.Log("AllowMove changed in start" + childScript.allowMove);
       
       

    }

    private void Awake()
    {
        scoreScript = Score.GetComponent<ScoreSaver>();
        childScript = child.GetComponent<newDrag>();
        childScript.allowMove = false;
        
      
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        
        if (scoreScript != null) {
            scoreScript.pauseTimer();
        }
        if (childScript != null) { 
              childScript.allowMove = false;
           
        }
        MoveAlongPath();
    }

    void MoveAlongPath()
    {

        if (childScript != null)
        {
        
            childScript.allowMove = false; // Ensuring it stays false
        }
        Vector3[] path = new Vector3[targetTransform.Length];
        


        Debug.Log(path);
      
        for (int i = 0; i < targetTransform.Length; i++)
        {
            path[i] = targetTransform[i].position;
        }

        


        movementTween = DOTween.Sequence()
        .Append(transform.DOPath(path, duration, PathType.CatmullRom)
                 .SetEase(Ease.Linear))
        .AppendCallback(() => { transform.position = initialPos; })

        //.Append(transform.DOPath(revPath, duration, PathType.CatmullRom)
        // .SetEase(Ease.Linear))
        .SetLoops(loopCount)
        .OnComplete(changeAllow)
        .Play();

        

        
    }

    void changeAllow() { 

        ScoreSaver scoreScript = Score.GetComponent<ScoreSaver>();
        if (scoreScript != null) {
            scoreScript.start = true;
        }
        childScript.allowMove = true;
        arrow.SetActive(false);
        hand.SetActive(false);
        if (scoreScript != null) {
            scoreScript.continueTimer();

        }
        

    }
}
