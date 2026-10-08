using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectClickHandler : MonoBehaviour
{
    // Assign in the Inspector: Target positions for object movement
    public Transform obj1; // First position
    public Transform obj2; // Second position

    // Delay before objects vanish or reset
    public float delayTime = 1.0f;

    // Click tracking variables
    private bool isFirst = true;
    private string firstSelectedType;
    private GameObject firstSelectedObject;

    // Store original positions
    private Dictionary<GameObject, Vector3> originalPositions = new Dictionary<GameObject, Vector3>();

    void Start()
    {
        // Store the original positions of objects
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Moveable"))
        {
            originalPositions[obj] = obj.transform.position;
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Detects left mouse click
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                GameObject clickedObject = hit.collider.gameObject;
                HandleClick(clickedObject);
            }
        }
    }

    void HandleClick(GameObject clickedObject)
    {
        string type = clickedObject.name.Split('_')[0]; // Extracts type from name (e.g., "Ball_1")

        if (isFirst)
        {
            // First object clicked: Move to obj1
            clickedObject.transform.position = obj1.position;

            // Store selection
            firstSelectedType = type;
            firstSelectedObject = clickedObject;
            isFirst = false;

            Debug.Log("First " + type + " moved to position 1.");
        }
        else
        {
            if (clickedObject == firstSelectedObject)
            {
                Debug.Log("You clicked the same object again. Choose its pair.");
                return;
            }

            if (type == firstSelectedType) // Correct pair
            {
                clickedObject.transform.position = obj2.position;
                Debug.Log("Correct pair! " + type + " moved to position 2. Will vanish shortly.");
                StartCoroutine(CorrectPairSequence(clickedObject));
            }
            else // Wrong pair
            {
                Debug.Log("Wrong choice! Expected " + firstSelectedType + ", but got " + type + ".");
                StartCoroutine(WrongPairSequence());
            }
        }
    }

    IEnumerator CorrectPairSequence(GameObject secondObject)
    {
        yield return new WaitForSeconds(delayTime);
        firstSelectedObject.SetActive(false);
        secondObject.SetActive(false);
        isFirst = true;
    }

    IEnumerator WrongPairSequence()
    {
        yield return new WaitForSeconds(delayTime);
        if (originalPositions.ContainsKey(firstSelectedObject))
        {
            firstSelectedObject.transform.position = originalPositions[firstSelectedObject];
        }
        isFirst = true;
    }
}
