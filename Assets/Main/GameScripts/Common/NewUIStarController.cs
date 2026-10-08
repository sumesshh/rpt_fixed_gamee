using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewUIStarController : MonoBehaviour
{

    // Start is called before the first frame update
   
    public Image[] levels;

    //public Image[] signImages;
    public Image notCompletedImage;
    public Image completedImage;
    public Image oneStarImage;
    public Image twoStarImage;
    public Image threeStarImage;

    //public ScrollRect scrollRect;
    //public RectTransform content;
    //public RectTransform sample;

    public Color changeColor;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Inside Landscapeselection");
        List<string> allSceneNames = new List<string>();
        foreach (var sceneList in LevelSelectManager.categorizedScenes.Values)
        {
            allSceneNames.AddRange(sceneList);
        }
        for (int i = 0; i < levels.Length && i < allSceneNames.Count; i++)
        {

            //Image buttonImage = notCompletedImage;
            Image buttonImage = levels[i];
            //if (buttonImage != null)
            //{
            //    Debug.Log("buttonImage found");
            //    if (LevelSelectManager.gameCompleted[i])
            //    {
            //        Debug.Log("changing image true");
            //        buttonImage.sprite = completedImage.sprite;  // ✅ Assign the sprite
            //    }
            //    else
            //    {
            //        Debug.Log("Changing Image false");
            //        buttonImage.sprite = notCompletedImage.sprite;  // ✅ Assign the sprite
            //    }
            //}
            //else
            //{
            //    Debug.Log("Not Image found");
            //}

            if (!PlayerPrefs.HasKey(allSceneNames[i]))
            {
                buttonImage.sprite = notCompletedImage.sprite;
               
            }
            else
            {
                //if (signImages != null && changeColor != null)
                //{
                //    signImages[i].color = changeColor;

                //}
                int stars = PlayerPrefs.GetInt(allSceneNames[i]);
                switch (stars)
                {

                    case 1:
                        buttonImage.sprite = oneStarImage.sprite;
                        break;
                    case 2:
                        buttonImage.sprite = twoStarImage.sprite;
                        break;
                    case 3:
                        buttonImage.sprite = threeStarImage.sprite;
                        break;

                }

            }


        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
