using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;

public class S_Game_4_Manager : MonoBehaviour
{
    public TextMeshProUGUI firstQn;
    public TextMeshProUGUI secondQn;
    public TextMeshProUGUI thirdQn;

    public GameObject textAnimator1;
    public GameObject textAnimator2;
    public GameObject textAnimator3;

    public Button onButton;
    public Button underButton;

    public AudioSource shakeAudio;

    public GameObject popperManager;
    private PopperManagerNew popperManagerNew;

    int count;
    public GameObject ScoreObj;
    private ScoreSaver scoreScript;

    public UnityEvent GameWin;
    // Start is called before the first frame update
    void Start()
    {
        scoreScript = ScoreObj.GetComponent<ScoreSaver>();
        scoreScript.totalTime = 60;
        scoreScript.elapsedTime = 60;

        firstQn.gameObject.SetActive(true);
        secondQn.gameObject.SetActive(false);
        thirdQn.gameObject.SetActive(false);

        count = 0;
        if (popperManager != null) { 
            popperManagerNew = popperManager.GetComponent<PopperManagerNew>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void clickedUnderButton() {
        if (count == 0) {
            firstQn.gameObject.SetActive(false);
        

            secondQn.gameObject.SetActive(true);
            textAnimator2.SetActive(true);
            PlaySmallPopper();
            count++;
            CorrectChoice(underButton);


        }

        else if (count == 2){
            GameWin.Invoke();
            count++;
            CorrectChoice(underButton);
        }

        else {
            if (count == 3) {
                return;
            }
            count = 0;
            thirdQn.gameObject.SetActive(false);
            secondQn.gameObject.SetActive(false);
            firstQn.gameObject.SetActive(true);
            Shake(underButton);
            if (Application.isMobilePlatform)
            {
#if UNITY_ANDROID
                Handheld.Vibrate();
#elif UNITY_IOS
            //Taptic.Heavy();
#endif
            }
        }

    }

    public void clickedOnButton() {
        if (count == 1)
        {
            secondQn.gameObject.SetActive(false);
            

            thirdQn.gameObject.SetActive(true);
            textAnimator3.gameObject.SetActive(true);
            PlaySmallPopper();
            CorrectChoice(onButton);
            count++;
        }
        else {
            count = 0;
            thirdQn.gameObject.SetActive(false);
            secondQn.gameObject.SetActive(false);
            firstQn.gameObject.SetActive(true);
            Shake(onButton);
            if (Application.isMobilePlatform)
            {
#if UNITY_ANDROID
                Handheld.Vibrate();
#elif UNITY_IOS
            //Taptic.Heavy();
#endif
            }

        }
        
    }

    public void Shake(Button button)
    {

        button.interactable = false;
        PlayShakeAudio();
        Color initialColor;
        Transform btnTransform;
        Image buttonImage;
        btnTransform = button.GetComponent<Transform>();
        buttonImage = button.GetComponent<Image>();

        initialColor = buttonImage.color;

        Vector3 initialPosition = btnTransform.localPosition;

        Sequence ShakeTween = DOTween.Sequence()
        .Append(btnTransform.DOShakePosition(0.5f, new Vector3(50f, 0f, 0f), 15, 0, false, true))
        .Join(buttonImage.DOColor(Color.red, 0.3f))
        .OnComplete(() =>
        {
            btnTransform.localPosition = initialPosition;
            buttonImage.DOColor(initialColor, 0.3f).OnComplete(() => button.interactable = true);

        }
        )
        .Play();

    }

    public void CorrectChoice(Button button)
    {

        Color initialColor;
        Transform btnTransform;
        Image buttonImage;
        btnTransform = button.GetComponent<Transform>();
        buttonImage = button.GetComponent<Image>();

        initialColor = buttonImage.color;

        Vector3 initialPosition = btnTransform.localPosition;

        Sequence ShakeTween = DOTween.Sequence()
        .Join(buttonImage.DOColor(Color.green, 0.3f))
        .OnComplete(() =>
        {
            buttonImage.DOColor(initialColor, 0.3f);
            btnTransform.localPosition = initialPosition;
            
        }
        )
        .Play();
    }

    public void PlayShakeAudio()
    {
        shakeAudio.Play();
    }

    private IEnumerator Delay() {
        yield return new WaitForSeconds(2f);
    
    }

    private void PlaySmallPopper() {
        if (popperManagerNew != null) {
            popperManagerNew.PopperConfettisimple();
        }
    }
}
