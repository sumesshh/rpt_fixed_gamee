using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class M_Level_3_Manager : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject firstCanvas;
    public GameObject secondGame;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeCanvas() {
        StartCoroutine(Delay());
    }

    public void LoadNewScene() {
        StartCoroutine(OtherDelay());
    }

    private IEnumerator Delay() {
        yield return new WaitForSeconds(5f);
        secondGame.SetActive(true);
        firstCanvas.SetActive(false);
    }

    private IEnumerator OtherDelay()
    {
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene("M_Level_4");
    }
}

