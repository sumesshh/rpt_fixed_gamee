using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScreenTimeDisplayer : MonoBehaviour
{
    public List<ContainerInfo> containerInfos = new List<ContainerInfo>();
   
    public float fullAmount = 3600f;

    public TextMeshProUGUI screenTimeDisplay;

    public Image progressBar;

    private Dictionary<String, String> compareDict = new Dictionary<string, string>(){

        { "Sunday", "Sun" },
    { "Monday", "Mon" },
    { "Tuesday", "Tue" },
    { "Wednesday", "Wed" },
    { "Thursday", "Thu" },
    { "Friday", "Fri" },
    { "Saturday", "Sat" }


    };
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 7; i++)
        {
            DateTime day = DateTime.Now.Date.AddDays(-i);
            string key = day.ToString("dd-MM-yyyy");
            float duration = PlayerPrefs.GetFloat(key, 0f);
            string displayText = GetOrdinalDate(day.Day);

           
            

            if (containerInfos != null) {
                containerInfos[i].fillImage.fillAmount = duration / fullAmount;

                containerInfos[i].duration.text = DisplayInHourMinutes(duration);
                containerInfos[i].durationInSec = duration;


                containerInfos[i].date.text = displayText;
                containerInfos[i].day.text = compareDict[day.DayOfWeek.ToString()];

                if (duration / fullAmount >= 1)
                {
                    containerInfos[i].tickImage.SetActive(true);
                }
            }

            if (i == 0) {
                String display = FormatChange(duration);
                screenTimeDisplay.text = display;
                progressBar.fillAmount = duration / fullAmount;
            }

            var currentContainer = containerInfos[i];
            currentContainer.button.onClick.AddListener(() => DisplayText(currentContainer));
            //containerInfos[i].button.onClick.AddListener(() => DisplayText(containerInfos[i]));
            


            Debug.Log($"{key}: {duration} seconds");
        }

    }

    String FormatChange(float seconds) { 
        TimeSpan time = TimeSpan.FromSeconds(seconds);

        int hours = time.Hours;
        int minutes = time.Minutes;

        string display = hours + " hr " + minutes + " minutes";
        return display;
    
    }

    String DisplayInHourMinutes(float seconds) {
        TimeSpan time = TimeSpan.FromSeconds(seconds);
        return time.ToString(@"hh\:mm");
    }

    void DisplayText(ContainerInfo thisContainerInfo) {
        float locDuration = thisContainerInfo.durationInSec;
        screenTimeDisplay.text = FormatChange(locDuration);
        progressBar.fillAmount = locDuration / fullAmount;
    }

    string GetOrdinalDate(int day)
    {
        if (day >= 11 && day <= 13)
            return day + "th";

        switch (day % 10)
        {
            case 1: return day + "st";
            case 2: return day + "nd";
            case 3: return day + "rd";
            default: return day + "th";
        }
    }




    // Update is called once per frame
    void Update()
    {
        
    }
}
