using UnityEngine;

// Optional helper class for star buttons
// Add this to each star button GameObject for easier visual management
public class StarButton : MonoBehaviour
{
    public GameObject filledStarImage;
    public GameObject emptyStarImage;

    public void SetFilled(bool filled)
    {
        if (filledStarImage != null)
            filledStarImage.SetActive(filled);

        if (emptyStarImage != null)
            emptyStarImage.SetActive(!filled);
    }
}