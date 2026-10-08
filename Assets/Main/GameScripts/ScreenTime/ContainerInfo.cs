using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ContainerInfo : MonoBehaviour
{
    // Start is called before the first frame update
    public Button button;
    public TextMeshProUGUI date;
    public TextMeshProUGUI duration;
    public Image fillImage;
    public TextMeshProUGUI day;
    public GameObject tickImage;

    [HideInInspector]
    public float durationInSec;
    

}
