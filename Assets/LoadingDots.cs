using UnityEngine;
using TMPro;
using System.Collections;

public class LoadingDots : MonoBehaviour
{
    public TextMeshProUGUI loadingText;
    private string baseText = "Loading";

    void Start()
    {
        StartCoroutine(AnimateDots());
    }

    IEnumerator AnimateDots()
    {
        while (true)
        {
            for (int i = 0; i <= 3; i++)
            {
                loadingText.text = baseText + new string('.', i);
                yield return new WaitForSeconds(0.125f);
            }
        }
    }
}
