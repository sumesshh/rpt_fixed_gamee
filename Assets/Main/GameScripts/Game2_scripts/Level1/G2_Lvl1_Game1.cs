using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G2_Lvl1_Game1 : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject scoreObj;
    public newDrag[] donuts;
    public RectTransform[] targets;
    private int count = 1;
    private ScoreSaver scoreScript;
    void Start()
    {
        if (scoreObj != null)
        {
            scoreScript = scoreObj.GetComponent<ScoreSaver>();
            scoreScript.totalTime = 60f;
            scoreScript.elapsedTime = 60f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnDrop()
    {
        donuts[count].target = targets[count];
        donuts[count].changeTarget();
        donuts[count].allowMove = true;
        count++;

    }
}
