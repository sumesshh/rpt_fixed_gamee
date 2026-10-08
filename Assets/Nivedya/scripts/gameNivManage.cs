using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameNivManage : MonoBehaviour
{
    public GameObject popper;
    public GameObject newGame;
    public GameObject oldGame;
    public AudioSource yay;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void playWin()
    {
        StartCoroutine(SetActiveTrueAfterDelay());
    }

    private IEnumerator SetActiveTrueAfterDelay()
    {
        popper.SetActive(true);
        yay.Play();
        yield return new WaitForSeconds(2.5f);
        newGame.SetActive(true);
        oldGame.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
