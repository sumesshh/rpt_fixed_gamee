using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;
using UnityEngine.Events;

public class coconutTree : MonoBehaviour
{
    public RectTransform treeTransform; // UI Image (Tree)
    public RectTransform coconutTransform;
    public RectTransform coconut2Transform;
    public RectTransform coconut3Transform;
    public Button treeButton;
    public float shakeDuration = 1f; // Increase this to slow the shake duration
    public float shakeMagnitude = 10f; // Adjust to control side-to-side shake range
    public float shakeSpeed = 0.05f; // Delay between each shake (slows down the movement)
    public UnityEvent win;
    private Vector3 originalPosition;

    void Start()
    {
        if (treeTransform == null)
            treeTransform = GetComponent<RectTransform>(); // Get UI transform if not set

        if (treeButton != null)
            treeButton.onClick.AddListener(StartShaking);

        originalPosition = treeTransform.anchoredPosition; // UI uses anchoredPosition
    }

    public void StartShaking()
    {
        Debug.Log("Tree clicked! Shaking started...");
        StartCoroutine(ShakeTree());
    }

    IEnumerator ShakeTree()
    {
        float elapsedTime = 0f;
        while (elapsedTime < shakeDuration)
        {
            float offsetX = Random.Range(-shakeMagnitude, shakeMagnitude); // Only shaking on X-axis

            // Set the Y-position fixed, only change X-position for side-to-side shake
            treeTransform.anchoredPosition = new Vector3(originalPosition.x + offsetX, originalPosition.y, originalPosition.z);

            elapsedTime += Time.deltaTime;

            // Delay between each shake move to slow it down
            yield return new WaitForSeconds(shakeSpeed);
        }

        treeTransform.anchoredPosition = originalPosition; // Reset position
        Debug.Log("Shaking Ended!");
        Sequence coconutSequence = DOTween.Sequence();
        if (coconutTransform != null) { coconutSequence.Append(coconutTransform.DOMoveY(-1.3f, 1f)); }
        DOVirtual.DelayedCall(0.2f, () =>
        {
        if(coconut2Transform!=null){ coconutSequence.Append(coconut2Transform.DOMoveY(-1.3f, 1f)); }
        });
        DOVirtual.DelayedCall(0.5f, () =>
        {
        if(coconut3Transform!=null){ coconutSequence.Append(coconut3Transform.DOMoveY(-1.3f, 1f));
            }
        });
        coconutSequence.OnComplete(() => win.Invoke());
    }
}
