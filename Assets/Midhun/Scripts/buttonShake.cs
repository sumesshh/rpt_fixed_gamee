using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class buttonShake : MonoBehaviour
{
    // Start is called before the first frame update
    public Button[] wrongbuttons;
    public Button[] correctbuttons;
    private Vector3 initialPosition;
    public AudioSource failAudio;
    public AudioSource shakeAudio;

    void Start()
    {
        foreach (Button btn in wrongbuttons) {
            btn.onClick.AddListener(() => Shake(btn));
        }

        foreach (Button btn in correctbuttons) { 
            btn.onClick.AddListener(() => CorrectChoice(btn));
        }

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Shake(Button button) {

        button.interactable = false;
        PlayShakeAudio();
        Color initialColor;
        Transform btnTransform;
        Image buttonImage;
        btnTransform = button.GetComponent<Transform>();
        buttonImage = button.GetComponent<Image>();

        initialColor = buttonImage.color;

        initialPosition = btnTransform.localPosition;

        Sequence ShakeTween = DOTween.Sequence()
        .Append(btnTransform.DOShakePosition(0.5f, new Vector3(50f, 0f, 0f), 15, 0, false, true))
        .Join(buttonImage.DOColor(Color.red,0.3f))
        .OnComplete(() =>
        {
            btnTransform.localPosition = initialPosition;
            buttonImage.DOColor(initialColor,0.3f).OnComplete(() => button.interactable = true);
            
        }
        )
        .Play();
        
    }

    public void PlayFailAudio() { 
        failAudio.Play();
    }

    public void PlayShakeAudio() {
        shakeAudio.Play();
    }

    public void CorrectChoice(Button button)
    {

        Color initialColor;
        Transform btnTransform;
        Image buttonImage;
        btnTransform = button.GetComponent<Transform>();
        buttonImage = button.GetComponent<Image>();

        initialColor = buttonImage.color;

        initialPosition = btnTransform.localPosition;

        Sequence ShakeTween = DOTween.Sequence()
        .Join(buttonImage.DOColor(Color.green, 0.4f))
        .OnComplete(() =>
        {
            btnTransform.localPosition = initialPosition;
            button.interactable = false;
        }
        )
        .Play();
    }
}
