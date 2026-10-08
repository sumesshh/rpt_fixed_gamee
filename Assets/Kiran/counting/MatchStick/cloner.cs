using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(newDrag))]
public class CloneOnSuccessfulDrop : MonoBehaviour
{
    public GameObject counterObj;
    private Vector3 initialPosition;
    private Vector3 initialScale; // Store original scale
    private newDrag dragScript;

    private counter counterScript;

    private void Start()
    {
        counterScript = counterObj.GetComponent<counter>();
        dragScript = GetComponent<newDrag>();
        initialPosition = transform.position;
        initialScale = transform.localScale; // Capture original scale

        dragScript.onTargetZoneDrop.AddListener(HandleSuccessfulDrop);
    }

    private void OnDestroy()
    {
        if (dragScript != null)
        {
            dragScript.onTargetZoneDrop.RemoveListener(HandleSuccessfulDrop);
        }
    }

    private void HandleSuccessfulDrop()
    {
        // Create clone
        GameObject clone = Instantiate(
            gameObject,
            initialPosition,
            Quaternion.identity,
            transform.parent
        );

        // Reset clone's scale to original
        clone.transform.localScale = initialScale;

        // Configure clone's drag component
        newDrag cloneDrag = clone.GetComponent<newDrag>();
        if (counterScript.gameWon == false)
        {
            cloneDrag.allowMove = true;
        }
        else {
            cloneDrag.allowMove = false;
        }
        
        cloneDrag.changeInitialPos();

        // Add cloning capability to the new clone
        CloneOnSuccessfulDrop cloneScript = clone.GetComponent<CloneOnSuccessfulDrop>();
        if (cloneScript == null)
        {
            cloneScript = clone.AddComponent<CloneOnSuccessfulDrop>();
        }
    }
}