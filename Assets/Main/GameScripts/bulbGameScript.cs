using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class bulbGameScript : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject correctBulb;
    public GameObject wrongBulb;

    private Image correctAnswerImage;
    private Image wrongAnswerImage;

    public Color correctColor;
    public Color wrongColor;

    public UnityEvent onWin;

    private bool gameWon = false;

    private Color wrongDefaultColor;
    void Start()
    {
        correctAnswerImage = correctBulb.GetComponent<Image>();
        wrongAnswerImage = wrongBulb.GetComponent<Image>(); 

        wrongDefaultColor = wrongAnswerImage.color;


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void correctAnswerPressed() {
        if (gameWon) {
            return;
                
        }

        gameWon = true;
        correctAnswerImage.color = correctColor;
        onWin.Invoke();
        //wrongAnswerImage.color = wrongColor;


    
    }

    public void wrongAnswerPressed() {

        if (gameWon) {
            return;
        }

        StartCoroutine(ColorChange());
#if UNITY_ANDROID
        Handheld.Vibrate();
#elif UNITY_IOS
            //Taptic.Heavy();
#endif
    }

    IEnumerator ColorChange() {

        wrongAnswerImage.color = wrongColor;
        yield return new WaitForSeconds(0.5f);

        wrongAnswerImage.color = wrongDefaultColor;
    
    }
}
