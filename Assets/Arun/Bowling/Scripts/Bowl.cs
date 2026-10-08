using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class Bowl : MonoBehaviour
{
    public Canvas canvas;
    private GraphicRaycaster raycaster;
    private EventSystem eventSystem;

    [System.Serializable]
    public class GameObjectProperty
    {
        public GameObject obj; // Assign UI Object
        public PropertyType property; // Assign Roll/Slide
    }

    public enum PropertyType
    {
        Roll,
        Slide
    }

    public List<GameObjectProperty> objectList = new List<GameObjectProperty>(); // List of UI objects

    void Start()
    {
        // Get GraphicRaycaster from Canvas
        if (canvas == null)
        {
            Debug.LogError("Canvas not assigned! Assign the Canvas in the Inspector.");
            return;
        }
        raycaster = canvas.GetComponent<GraphicRaycaster>();

        // Get EventSystem (must exist in the scene)
        eventSystem = EventSystem.current;
        if (eventSystem == null)
        {
            Debug.LogError("No EventSystem found in the scene! Add one by going to GameObject -> UI -> EventSystem.");
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Detect Left Click
        {
            DetectClickedUIObject();
        }
    }

    void DetectClickedUIObject()
    {
        PointerEventData pointerEventData = new PointerEventData(eventSystem)
        {
            position = Input.mousePosition // Get mouse position
        };

        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(pointerEventData, results);

        foreach (RaycastResult result in results)
        {
            foreach (GameObjectProperty objProp in objectList)
            {
                if (result.gameObject == objProp.obj)
                {
                    Debug.Log("Clicked on: " + objProp.obj.name + ", Property: " + objProp.property);
                    return; // Stop after first match
                }
            }
        }
    }
}
