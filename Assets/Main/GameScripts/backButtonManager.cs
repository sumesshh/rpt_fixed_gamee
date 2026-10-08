using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class backButtonManager : MonoBehaviour
{
    public GameObject numberInputpanel;
    public GameObject otpInputPanel;
    public GameObject credentialsPanel;
    public GameObject purchasePanel;

    public GameObject postReg;
    public GameObject RegCanvas;

   public GameObject successPanel;
    public GameObject failPanel;

    public GameObject confirmPanelInPostCanvas;

    public GameObject confirmPanelInPreCanvas;

    public GameObject levelSelectionPanel;

    public List<GameObject> panelStack = new List<GameObject>();
    private bool InsideGame;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {

            if (postReg.activeSelf)
            {

                Back();
            }

            else {


                if (purchasePanel.activeSelf)
                {
                    if (!successPanel.activeSelf && !failPanel.activeSelf) {
                        //credentialsPanel.SetActive(true);
                        //purchasePanel.SetActive(false);
                        purchasePanel.SetActive(false);
                        RegCanvas.SetActive(false);
                        postReg.SetActive(true);
                    }
                    

                }
                else if (credentialsPanel.activeSelf)
                {
                    numberInputpanel.SetActive(true);
                    credentialsPanel.SetActive(false);
                }
                else if (otpInputPanel.activeSelf)
                {
                    numberInputpanel.SetActive(true);
                    otpInputPanel.SetActive(false);

                }
                else if (numberInputpanel.activeSelf) {
                    //confirmPanelInPreCanvas.SetActive(true);
                    numberInputpanel.SetActive(false);
                    RegCanvas.SetActive(false);
                    postReg.SetActive(true);
                
                }

            }


        }
        
    }

    public void OpenPanel(GameObject panel)
    {
        //// Optional: deactivate current panel
        //if (panelStack.Count > 0)
        //{
        //    panelStack[panelStack.Count - 1].SetActive(false);
        //}

        panelStack.Add(panel);     // Push to stack
             // Show the new panel
    }

    public void Back() {

        if (panelStack.Count > 0)
        {

            panelStack[panelStack.Count - 1].SetActive(false);
            panelStack.RemoveAt(panelStack.Count - 1);

            if (panelStack.Count > 0)
            {
                panelStack[panelStack.Count - 1].SetActive(true); // Reactivate previous panel
            }
            else
            {
                levelSelectionPanel.SetActive(true);
            }


        }
        else {
            confirmPanelInPostCanvas.SetActive(true);
        
        }
    
    
    
    }

    public void ClearPanelStack(bool deactivatePanels = true)
    {
        if (deactivatePanels)
        {
            foreach (var panel in panelStack)
            {
                if (panel != null)
                    panel.SetActive(false);
            }
        }

        panelStack.Clear();
    }
}


