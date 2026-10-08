using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectionManager : MonoBehaviour
{
    // Start is called before the first frame update
    // void Start()
    // {
    //     Screen.orientation = ScreenOrientation.Portrait;
    // }
void Start()
{
    Screen.orientation = ScreenOrientation.LandscapeLeft;
}
    // Update is called once per frame
    void Update()
    {
        
    }

    public void PortraitButtonClicked() {

        StartCoroutine(SwitchToPortrait());
        //Screen.orientation = ScreenOrientation.Portrait;
        //SceneManager.LoadScene("Level1_Portrait");
    }

    public void LandScapeButtonClicked() {
        StartCoroutine(SwitchToLandscape());
        //Screen.orientation = ScreenOrientation.LandscapeLeft; // or LandscapeRight
        //SceneManager.LoadScene("Level1_LandScape");
    }

    private IEnumerator SwitchToPortrait()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        yield return new WaitForSeconds(0.5f); // Short delay to let Unity apply the orientation
        // Portrait mode is disabled for this build; stay in landscape.
    }

    private IEnumerator SwitchToLandscape()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        yield return new WaitForSeconds(0.5f); // Wait for orientation change
        SceneManager.LoadScene("Level1_LandScape");
    }
}
