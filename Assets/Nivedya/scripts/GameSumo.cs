using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class GameSumo : MonoBehaviour
{
    public RectTransform fighter;
    public RectTransform Player;
    public RectTransform FighterPlayer;
    public RectTransform PlayerPlayer;
    public GameObject l1;
    public GameObject l2;
    public UnityEvent win;
    //public Image imageToPopUp;  // The Image component you want to animate
    public RectTransform emptyObjectPosition;  // The position of your empty object
    // Start is called before the first frame update
    void Start()
    {

    }
    public void fightPlayWin()
    {
        Sequence sequence = DOTween.Sequence();

        sequence.AppendInterval(1f) // Wait for 1 second
                .Append(fighter.DOMove(PlayerPlayer.position, 2f)) // Move to target
                .Join(fighter.DORotate(new Vector3(0, 0, 360), 2f, RotateMode.FastBeyond360)) // Rotate while moving
                .AppendCallback(() => PopImageAtPosition()); // After animation, call PopImageAtPosition
    }

    public void PopImageAtPosition()
    {
        // Move the fighter to the empty object's UI position
        fighter.anchoredPosition = emptyObjectPosition.anchoredPosition;

        // Set initial scale to 0
        fighter.localScale = Vector3.zero;

        // Animate the scale up to make it appear with a "pop" effect
        fighter.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
    }


    public void fightPlayLose()
    {
        Sequence newSequence = DOTween.Sequence();
        newSequence.AppendInterval(0.5f)
        .Append(Player.DOMove(FighterPlayer.transform.position, 2f))
        .Join(Player.DORotate(new Vector3(0, 0, 360), 2f, RotateMode.FastBeyond360))
        .AppendCallback(() => win.Invoke());
    }

    public void StartPanelChange()
    {
        StartCoroutine(PanelChangeCoroutine());
    }

    private IEnumerator PanelChangeCoroutine()
    {
        yield return new WaitForSeconds(2f);
        l1.SetActive(false);
        l2.SetActive(true);
    }

    public void reloadGame()
    {
        SceneManager.LoadScene("newGameNiv");
    }
}
