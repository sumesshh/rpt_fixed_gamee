using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using Unity.Properties;
using DG.Tweening;

[System.Serializable]
public class Button1
{
    public Button obj; // Assign UI Object
    public PropertyType property; // Assign Roll/Slide
}
 ;
public enum PropertyType
{
    Roll,
    Slide
}
public class Slide_roll : MonoBehaviour
{
    public RectTransform targetArea;
    private Button previous;
    private Dictionary<Button, Vector3> originalPositions = new Dictionary<Button, Vector3>();

    public List<Button1> objectlist = new List<Button1>(); // List to store buttons

    void Start()
    {
       
        // Assign click event to all buttons in the list
        foreach (Button1 btn in objectlist)
        {
            if (btn != null)
            {
                originalPositions[btn.obj] = btn.obj.transform.position;
                btn.obj.onClick.AddListener(() => OnButtonClick(btn));
            }
        }
    }


    void OnButtonClick(Button1 clickedButton)
    {
        Debug.Log("Button clicked: " + clickedButton.obj.name+ "Button property: " + clickedButton.property);
       
        selection(clickedButton);
    }
    void selection(Button1 clickedButton)

    {
        if (previous == null)
        {
            previous = clickedButton.obj;
            Vector3 targetAreaCenter = targetArea.rect.center;

            Vector3 targetCenterWorld = targetArea.TransformPoint(targetAreaCenter);
            Vector3 targetPosition = new Vector3(targetCenterWorld.x, clickedButton.obj.transform.position.y + 300f, clickedButton.obj.transform.position.z);
            clickedButton.obj.transform.position = targetPosition;
            object_action(clickedButton);
        }
        else if (clickedButton.obj != previous)
        {
            previous.transform.position = originalPositions[previous];
            previous = clickedButton.obj;
            Vector3 targetAreaCenter = targetArea.rect.center;

            Vector3 targetCenterWorld = targetArea.TransformPoint(targetAreaCenter);
            Vector3 targetPosition = new Vector3(targetCenterWorld.x, clickedButton.obj.transform.position.y + 300f, clickedButton.obj.transform.position.z);
            clickedButton.obj.transform.position = targetPosition;


             object_action(clickedButton);
        }
        else
        {

        }
    }
    private void object_action(Button1 clickedButton)
    {
        
        Vector3 targetAreaCenter = targetArea.rect.center;

        Vector3 targetCenterWorld = targetArea.TransformPoint(targetAreaCenter);
        Vector3 targetPosition = new Vector3(clickedButton.obj.transform.position.x, targetCenterWorld.y, clickedButton.obj.transform.position.z);
        
        if(clickedButton.property== PropertyType.Roll)
        {
            clickedButton.obj.transform.DOMove(targetPosition, 3f);

            clickedButton.obj.transform.DOLocalRotate(new Vector3(360, 0,0 ), 3f, RotateMode.FastBeyond360);

        }
        else
        {
            targetPosition = new Vector3(clickedButton.obj.transform.position.x, targetCenterWorld.y/2, clickedButton.obj.transform.position.z);
            clickedButton.obj.transform.DOMove(targetPosition, 3f);
        }

    }
}


