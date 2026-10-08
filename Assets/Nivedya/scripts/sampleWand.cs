using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class sampleWand : MonoBehaviour
{
    public GameObject draggableObj;
    private newDrag dragScript;
    public GameObject ScratchMask;

    private bool eraserEnabled = false; // To avoid unnecessary updates

    void Start()
    {
        GameWand script = ScratchMask.GetComponent<GameWand>();
        Mask mask = ScratchMask.GetComponent<Mask>();
        script.enabled = true;
        mask.showMaskGraphic = true;

        if (draggableObj != null)
        {
            dragScript = draggableObj.GetComponent<newDrag>();
        }
        StartCoroutine(DisableEraserAfterDelay(script, mask, 0.1f));
    }

    IEnumerator DisableEraserAfterDelay(GameWand script, Mask mask, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (script != null)
            script.enabled = false;

        if (mask != null)
            mask.showMaskGraphic = false;
    }


    void Update()
    {
        if (dragScript != null && dragScript.isDragging && !eraserEnabled)
        {
            eraserEnableScript();
            eraserEnabled = true; // Prevent repeated calls
        }
        else if (dragScript != null && !dragScript.isDragging && eraserEnabled)
        {
            eraserDisableScript();
            eraserEnabled = false;
        }
    }

    void eraserEnableScript()
    {
        if (ScratchMask != null)
        {
            GameWand script = ScratchMask.GetComponent<GameWand>();
            if (script != null)
            {
                script.enabled = true;
            }

            Mask mask = ScratchMask.GetComponent<Mask>();
            if (mask != null)
            {
                mask.showMaskGraphic = true;
            }
        }
    }

    void eraserDisableScript()
    {
        if (ScratchMask != null)
        {
            GameWand script = ScratchMask.GetComponent<GameWand>();
            if (script != null)
            {
                script.enabled = false; // Disable when not dragging
            }

            Mask mask = ScratchMask.GetComponent<Mask>();
            if (mask != null)
            {
                mask.showMaskGraphic = false; // Optionally hide mask when not dragging
            }
        }
    }
}