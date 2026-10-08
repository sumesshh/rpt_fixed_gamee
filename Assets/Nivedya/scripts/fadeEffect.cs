using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class fadeEffect : MonoBehaviour
{
    public CanvasGroup fadeCanvasGroup;
    public float fadeDuration = 0.5f;
    public Button bigWin;

    private void Start()
    {
        bigWin.onClick.AddListener(StartFade);
    }

    public void StartFade() {
        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        // Fade in (from black to clear)
        float time = 0;
        while (time < fadeDuration)
        {
            time += Time.deltaTime*3;
            fadeCanvasGroup.alpha = 1 - (time / fadeDuration);
            yield return null;
        }

        fadeCanvasGroup.alpha = 0;
    }
}
