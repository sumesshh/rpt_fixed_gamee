using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LandScapeSelection : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject[] buttons;
    public Image[] levels;

    public Image[] signImages;
    public Image notCompletedImage;
    public Image completedImage;
    public Image oneStarImage;
    public Image twoStarImage;
    public Image threeStarImage;

    //public ScrollRect scrollRect;
    //public RectTransform content;
    //public RectTransform sample;

    public Color changeColor;

    public 

    void Start()
    {
        Debug.Log("Inside Landscapeselection");
        List<string> allSceneNames = new List<string>();
        foreach (var sceneList in LevelSelectManager.categorizedScenes.Values)
        {
            allSceneNames.AddRange(sceneList);
        }
        for (int i = 0;i< buttons.Length && i < allSceneNames.Count;i++) {

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
                if (signImages != null && changeColor != null)
                {
                    signImages[i].color = changeColor;

                }
            }
            else {
                //if (signImages != null && changeColor != null)
                //{
                //    signImages[i].color = changeColor;
                
                //}
                int stars = PlayerPrefs.GetInt(allSceneNames[i]);
                switch (stars) {

                    case 1:
                        buttonImage.sprite = oneStarImage.sprite;
                        break;
                    case 2:
                        buttonImage.sprite= twoStarImage.sprite;
                        break;
                    case 3:
                        buttonImage.sprite= threeStarImage.sprite;  
                        break;
                
                }
            
            }


        }
        //for (int i = 0; i < allSceneNames.Count; i++)
        //{
        //    if (!PlayerPrefs.HasKey(allSceneNames[i]))
        //    {
        //        if (scrollRect != null && content != null)
        //        {

        //            RectTransform target = levels[i].rectTransform;
        //            StartCoroutine(SetScrollPosition(target));
        //            break;
        //        }

        //    }

        //}

        //if (content != null && sample != null)
        //{
        //    StartCoroutine(SetScrollPosition());
        //}





    }
    //public void ScrollToElement(RectTransform target)
    //{
    //    Canvas.ForceUpdateCanvases(); // Ensure layout is ready

    //    RectTransform content = scrollRect.content;
    //    RectTransform viewport = scrollRect.viewport;

    //    // Get world position of target and viewport
    //    Vector3 worldPos = target.position;
    //    Vector3 localInContent = content.InverseTransformPoint(worldPos);
    //    Vector3 localInViewport = viewport.InverseTransformPoint(worldPos);

    //    // Calculate difference in local Y between the target and the viewport
    //    float diffY = localInContent.y - localInViewport.y;

    //    // Normalize it based on content height (must be positive and non-zero!)
    //    float contentHeight = content.rect.height;
    //    float viewportHeight = viewport.rect.height;

    //    if (contentHeight <= viewportHeight + 0.01f)
    //    {
    //        // Nothing to scroll — content fits within viewport
    //        return;
    //    }

    //    // Invert scroll direction because Unity ScrollRect vertical = 1 is top
    //    float normalizedDiff = diffY / (contentHeight - viewportHeight);
    //    scrollRect.verticalNormalizedPosition -= normalizedDiff;
    //    scrollRect.verticalNormalizedPosition = Mathf.Clamp01(scrollRect.verticalNormalizedPosition);
    //}

    // Update is called once per frame
    void Update()
    {
        
    }

    //IEnumerator SetScrollPosition() {

    //    yield return null; // Wait one frame for layout to complete
    //                       //scrollRect.verticalNormalizedPosition = 1f; // or 1f, depending on your layout

       
    //    float width = content.rect.width;
    //    float height = content.rect.height;
    //    Debug.Log("Height: " + height);
    //    Debug.Log("Width: " + width);

        
    //    Vector3[] parentCorners = new Vector3[4];
    //    Vector3[] childCorners = new Vector3[4];

    //    content.GetWorldCorners(parentCorners);
    //    sample.GetWorldCorners(childCorners);

    //    //Convert to local space
    //    //for (int i = 0; i < 4; i++)
    //    //{
    //    //    parentCorners[i] = content.InverseTransformPoint(parentCorners[i]);
    //    //    childCorners[i] = sample.InverseTransformPoint(childCorners[i]);
    //    //}

    //    float k = childCorners[0].y - parentCorners[0].y;
    //    float q = parentCorners[1].y - childCorners[1].y;

    //    float h = childCorners[0].x - parentCorners[0].x;
    //    float r = parentCorners[2].x - childCorners[2].x;

    //    float ogHeight = parentCorners[1].y - parentCorners[0].y;
    //    float ogWidth = parentCorners[2].x - parentCorners[1].x;

    //    Debug.Log("OG height: " + ogHeight);
    //    Debug.Log("OG width: " + ogWidth);

        

    //    Debug.Log("bottom: " + k);
    //    Debug.Log("top: " + q);
    //    Debug.Log("left: " + h);
    //    Debug.Log("right: " + r);


    //}
}
