using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UISlider : MonoBehaviour
{

    public GameObject profilePanel;
    public RectTransform analysisPanel;

    void Awake()
    {

        // Force landscape
        Screen.orientation = ScreenOrientation.LandscapeLeft;

        // Get the device refresh rate (display frame rate)
        int deviceFrameRate = (int) Screen.currentResolution.refreshRateRatio.value;

        if (deviceFrameRate < 60)
            Application.targetFrameRate = 60;
        else
            Application.targetFrameRate = deviceFrameRate;
    }

    // Start is called before the first frame update
    void Start()
    {
        profilePanel.GetComponent<RectTransform>().DOAnchorPosX(-profilePanel.GetComponent<RectTransform>().rect.width, 0);
        analysisPanel.DOAnchorPosX(analysisPanel.rect.width, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ProfilePanelClicked() {
        profilePanel.GetComponent<RectTransform>().DOAnchorPosX(0, 0.25f);
    }

    public void GoBack() {
        profilePanel.GetComponent<RectTransform>().DOAnchorPosX(-profilePanel.GetComponent<RectTransform>().rect.width, 0.25f);

    }

    public void AnalysisPanelClicked() {
        analysisPanel.DOAnchorPosX(0, 0.25f);

    }

    public void GoBackFromAnalysis() {
        analysisPanel.DOAnchorPosX(analysisPanel.rect.width, 0.25f);
    }
}
