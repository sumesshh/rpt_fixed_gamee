using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SampleScript : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private GameObject AddPremium;   

    public void AddPremiumFeature()
    {
        AddPremium.GetComponent<Button>().interactable = false;
        AddPremium.GetComponentInChildren<TextMeshProUGUI>().text = "Active";
    }
}