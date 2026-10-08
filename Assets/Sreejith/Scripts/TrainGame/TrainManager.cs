using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.UIElements;
using UnityEditor;
using TMPro;
using System.Runtime.CompilerServices;

public class TrainManager : MonoBehaviour
{
    public RectTransform targetOne;
    public RectTransform targetTwo;
    public RectTransform obj;
    public RectTransform objTwo;
    public Camera uiCamera;
    private bool isAnyInsideOne;
    public RectTransform carriage;
    public RectTransform slotTwo;
    public RectTransform slotOne;
    private Dictionary<string,bool> slotManager = new Dictionary<string,bool>();
    private List<RectTransform> draggableObjects;
    private Vector3 objOneInitialPos;
    private Vector3 carriageInitialPos;
    // Start is called before the first frame update
    void Start()
    {
        draggableObjects = new List<RectTransform>();
        draggableObjects.Add(obj);
        draggableObjects.Add(objTwo);
        isAnyInsideOne = false;
        objOneInitialPos = obj.position;
        carriageInitialPos = carriage.position;
        slotManager["slotOne"] = true;
        slotManager["slotTwo"] = false;
        slotManager["slotThree"] = false;
    }

    // Update is called once per frame
    void Update()
    {
        CheckTargetOne();
    }


    void CheckTargetOne() {
        // Get the screen-space Rect of the target
        Rect targetRect = RectTransformToScreenSpace(targetOne);

        isAnyInsideOne = false;

        foreach (var obj in draggableObjects) {

            // Get the object's screen position
            Vector2 objScreenPos = RectTransformUtility.WorldToScreenPoint(uiCamera, obj.position);

            // Check if the object is inside the target
            if (targetRect.Contains(objScreenPos))
            {
                isAnyInsideOne = true;
                Debug.Log(obj.name + " is inside the target zone!");
                carriage.DOMove(slotTwo.position, 1f);
                break; // Exit loop since at least one object is inside
            }

        }

        if (!isAnyInsideOne && carriage.position != slotOne.position)
        {
            carriage.DOMove(carriageInitialPos, 1f);
            slotManager["slotTwo"] = false;
        }


    }
    
    //void IsBallInsideTargetOne()
    //{
        

    //    // Get the ball's screen position
    //    Vector2 ballScreenPos = RectTransformUtility.WorldToScreenPoint(uiCamera, obj);

    //    // Check if the ball's position is inside the target's rect
    //    if (targetRect.Contains(ballScreenPos))
    //    {

    //        isInsideTargetOne = true;
    //        Debug.Log("Inside");
    //        carriage.DOMove(slotTwo.position, 1f);
            
    //    }
    //    else {
    //        isInsideTargetOne = false;
    //        if (carriage.position != slotOne.position) { 
    //            carriage.DOMove(carriageInitialPos, 1f);
    //            slotManager["slotTwo"] = false;
    //        }
    //    }
    //}

    Rect RectTransformToScreenSpace(RectTransform rt)
    {
        Vector3[] corners = new Vector3[4];
        rt.GetWorldCorners(corners); // Get world corners of RectTransform

        Vector2 min = RectTransformUtility.WorldToScreenPoint(uiCamera, corners[0]); // Bottom-left
        Vector2 max = RectTransformUtility.WorldToScreenPoint(uiCamera, corners[2]); // Top-right

        return new Rect(min, max - min);
    }

    private int GetNumber(RectTransform drag)
    {
        TextMeshProUGUI tmp = drag.transform.Find("Number").GetComponent<TextMeshProUGUI>();
        return int.Parse(tmp.text);
    }

    private bool CompareNums(RectTransform obj1, RectTransform obj2) { 
        int obj1Num = GetNumber(obj1);
        int obj2Num = GetNumber(obj2);
        if (obj1Num > obj2Num)
        {
            return true;
        }
        else {
            return false;
        }
    
    }


    public void SlotOneOnDrop() {
        bool isCorrect = CompareNums(carriage, obj);
        Debug.Log(isCorrect);
        if (isCorrect && isAnyInsideOne)
        {
            ObjectOneMoveToSlotOne();
        }
        else {
            ObjectOneToInitial();
        }
    
    }
    public void ObjectOneMoveToSlotOne() {
        carriageInitialPos = carriage.position;
        obj.DOMove(slotOne.position, 0.5f);
        Debug.Log("Moving to slot ");
        slotManager["slotTwo"] = true;
    }

    public void ObjectOneToInitial() {
        obj.DOMove(objOneInitialPos, 0.5f);
        Debug.Log("Moving to initial");
    }
}
