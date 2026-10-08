//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.SceneManagement;
//using System.Linq;
//using JetBrains.Annotations;

//public class levelChanger: MonoBehaviour
//{
//    // Start is called before the first frame update
//    public Canvas titleScreen;
//    public Canvas[] games;
//    public GameObject popperManager;
//    public Canvas CommonCanvas;
//    public GameObject scoreObj;

//    public GameObject ReplayManagerObj;
//    public ReplayManager replayScript;


//    public class ParticleAudioPair
//    {
//        public ParticleSystem particlesType;      // Name of the particle system (child of the parent)
//        public AudioClip audioClip;      // Audio clip directly assigned in the Inspector
//    }


//    private int count = 0;
//    private PopperManagerNew popperScript;
//    private ScoreSaver scoreScript;


//    private void Awake()
//    {


//        if (titleScreen != null)
//        {
//            titleScreen.gameObject.SetActive(true);
//            StartCoroutine(Delay());
//        }
//        else
//        {
//            //games[count].gameObject.SetActive(true);
//            CommonCanvas.gameObject.SetActive(true);
//            if (AudioCommon.Instance != null)
//            {
//                AudioCommon.Instance.Searcher();
//            }


//        }

//        if (ReplayManagerObj != null)
//        {
//            replayScript = ReplayManagerObj.GetComponent<ReplayManager>();
//        }
//    }
//    void Start()
//    {
//        popperScript = popperManager.GetComponent<PopperManagerNew>();
//        if (scoreObj != null)
//        {
//            scoreScript = scoreObj.GetComponent<ScoreSaver>();
//        }


//    }

//    // Update is called once per frame
//    void Update()
//    {

//    }

//    private IEnumerator Delay()
//    {
//        yield return new WaitForSeconds(2f);

//        titleScreen.gameObject.SetActive(false);
//        games[count].gameObject.SetActive(true);
//        CommonCanvas.gameObject.SetActive(true);
//        if (AudioCommon.Instance != null)
//        {
//            AudioCommon.Instance.Searcher();
//        }

//    }

//    private void UpdateElements()
//    {
//        if (count == games.Length)
//        {
//            if (AudioCommon.Instance != null)
//            {
//                AudioCommon.Instance.Forget();
//            }
//            SceneManager.LoadScene("S_Game_1");
//            return;
//        }
//        for (int i = 0; i < games.Length; i++)
//        {
//            games[i].gameObject.SetActive(i == count);

//        }
//        if (replayScript != null)
//        {
//            replayScript.onWin();
//            replayScript.canvasTemplate = games[count].gameObject;
//            replayScript.InstantiateCanvas();
//            Debug.Log("replayScriptcalled");

//        }

//        if (scoreScript != null)
//        {
//            scoreScript.resetTimer();
//            scoreScript.startTimer();


//        }
//        Debug.Log(count);
//    }

//    public void next()
//    {
      
//        count++;
        
//        UpdateElements();

//    }

//    public void PopperDelay()
//    {
//        StartCoroutine(Popper(3f));
//    }

//    private IEnumerator Popper(float delay)
//    {
//        yield return new WaitForSeconds(delay);
//        float startVolume = popperScript.audioPopper.volume;

//        while (popperScript.audioPopper.volume > 0)
//        {
//            popperScript.audioPopper.volume -= startVolume * Time.deltaTime / 1f;
//            yield return null;
//        }

//        popperScript.audioPopper.Stop(); // Stop the audio after fading out
//        popperScript.audioPopper.volume = startVolume; // Reset volume for future use
//        StopAll();
//        //yield return new WaitUntil(() => !(popperScript.particleAudioPairs.Any(p => p.particlesType.isPlaying)));
//        next();
//        Debug.Log("Enumerator worked");

//    }

//    public void StopAll()
//    {
//        foreach (var pt in popperScript.particleAudioPairs)
//        {
//            if (pt != null)
//            {
//                pt.particlesType.Stop();
//                pt.particlesType.Clear();

//            }
//        }
//    }

//    private IEnumerator AudioFade()
//    {
//        float startVolume = popperScript.audioPopper.volume;

//        while (popperScript.audioPopper.volume > 0)
//        {
//            popperScript.audioPopper.volume -= startVolume * Time.deltaTime / 1f;
//            yield return null;
//        }

//        popperScript.audioPopper.Stop(); // Stop the audio after fading out
//        popperScript.audioPopper.volume = startVolume; // Reset volume for future use
//    }



//}
