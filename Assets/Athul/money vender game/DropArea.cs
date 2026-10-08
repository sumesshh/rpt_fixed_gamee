
//add this script to the box area where we want to drop the currency.
//add boxcollider2d to the droparea aaand adjust the size of the collider to cove the entire part of drop area.
//drag and drop  this script to 'onclick' event Of submit button.and select 'checktotal' method.
//drag and drop  this script to 'on targetzone drop' of every currency annd select 'clone currency' method.
//tag the droparea to droparea

using UnityEngine;
using UnityEngine.Events;

public class DropArea : MonoBehaviour
{
    //public AudioClip wrongAnswerSound;

    public UnityEvent win;  // Set this in the Inspector
    public UnityEvent loss; // Set this in the Inspector
    private int totalAmount = 0;

    // Store the last currency that entered the trigger
    private CurrencyValue lastCurrency;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Trigger Entered: " + other.name);
        CurrencyValue currency = other.GetComponent<CurrencyValue>();
        if (currency != null)
        {
            totalAmount += currency.value;
            Debug.Log("Added: " + currency.value + " | Total: " + totalAmount);

            // Store the currency for later use
            lastCurrency = currency;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        CurrencyValue currency = other.GetComponent<CurrencyValue>();
        if (currency != null)
        {
            totalAmount -= currency.value;
            Debug.Log("Removed: " + currency.value + " | Total: " + totalAmount);
        }
    }

    public void CheckTotal()
    {
        if (totalAmount == 70)
        {
            Debug.Log("You Win!");
            win.Invoke();
        }
        else
        {
            Debug.Log("Try Again!");
            loss.Invoke();
        }
    }

    // Modified CloneCurrency method to work without arguments
    public void CloneCurrency()
    {
        if (lastCurrency != null)
        {
            Debug.Log("Cloning Currency at: " + lastCurrency.OriginalPosition);

            // Instantiate the clone
            GameObject newCurrency = Instantiate(lastCurrency.gameObject, lastCurrency.OriginalPosition, Quaternion.identity);

            // Ensure the clone is parented to the canvas
            newCurrency.transform.SetParent(lastCurrency.transform.parent, false);

            // Set the position of the clone to the original position
            newCurrency.transform.localPosition = lastCurrency.OriginalPosition;

            // Reset the scale of the clone to match the original currency's scale
            newCurrency.transform.localScale = lastCurrency.OriginalScale;

            // Ensure the cloned object has the newDrag script
            newDrag dragScript = newCurrency.GetComponent<newDrag>();
            if (dragScript != null)
            {
                // Reinitialize the drag script if necessary
                dragScript.allowMove = true; // Ensure the cloned object is draggable
                dragScript.canvas = lastCurrency.GetComponent<newDrag>().canvas; // Reassign the canvas reference
                dragScript.initialPos = lastCurrency.OriginalPosition; // Set the initial position
            }
            else
            {
                Debug.LogWarning("newDrag script not found on the cloned object.");
            }

            Debug.Log("Clone Created: " + newCurrency.name);
            Debug.Log("Clone Position: " + newCurrency.transform.localPosition);
            Debug.Log("Clone Scale After Reset: " + newCurrency.transform.localScale);
        }
        else
        {
            Debug.LogWarning("No currency to clone.");
        }
    }
}