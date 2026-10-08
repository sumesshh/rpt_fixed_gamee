using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level_1_Manager : MonoBehaviour
{
    // Start is called before the first frame update
    public Canvas titleScreen;
    public Canvas[] games;
    public string currentGame = "Game_1";
    private int count = 0;
    public Scene[] scenes;

    private void Awake()
    {
        if (titleScreen != null) {
            titleScreen.gameObject.SetActive(true);
            StartCoroutine(Delay());
        }
        
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private IEnumerator Delay()
    {
        yield return new WaitForSeconds(2f);

        titleScreen.gameObject.SetActive(false);
        games[count].gameObject.SetActive(true);
    }

    private void UpdateElements()
    {
        if (count == games.Length) {
            SceneManager.LoadScene("Game_1");
            return;
        }
        for (int i = 0; i < games.Length; i++)
        {
            games[i].gameObject.SetActive(i == count);

        }
        Debug.Log(count);
    }

    public void next()
    {

        count++;
        UpdateElements();

    }

    public void PopperDelay()
    {
        StartCoroutine(Popper(5f));
    }

    private IEnumerator Popper(float delay)
    {
        yield return new WaitForSeconds(delay);
        next();
        Debug.Log("Enumerator worked");

    }

    

}
