//attach the script to image of coin
//right click  on the coin image in the hierarchy and do : UI -> textmeshpro . name the textmeshpro component as 'coin_number'.
//drag and drop the 'coin_number' to 'Number text' in the inspector.
//change the tag of the coin in the inspector  to 'Coin'.
//in the inspector :add component -> rigidbody 2D -> b\change bodytype to 'kinematic'
//in the inspector :add component -> circlecollider 2D -> check the 'is Trigger' column ( intiallty the collider may be too small.
//so make it bigger enough to cover entire image)
//make the prefab of the coin
//initially uncheck the tick mark of coin  prefab in the inspector


using UnityEngine;
using TMPro;

public class Coin : MonoBehaviour
{
    public float fallSpeed = 200f;
    public TextMeshProUGUI numberText;
    private int number;
    private RectTransform rectTransform;
    private CoinSpawner spawner;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        spawner = FindObjectOfType<CoinSpawner>();

        // Ensure we have necessary components
        if (GetComponent<Collider2D>() == null)
        {
            Debug.LogError("Coin is missing a Collider2D component!");
        }

        if (GetComponent<Rigidbody2D>() == null)
        {
            Debug.LogError("Coin is missing a Rigidbody2D component!");
        }
    }

    void Update()
    {
        // Move the coin downward
        Vector2 position = rectTransform.anchoredPosition;
        position.y -= fallSpeed * Time.deltaTime;
        rectTransform.anchoredPosition = position;

        // Destroy if it falls too far
        if (position.y < -1000f)
        {
            spawner.RemoveCoinFromList(gameObject);
            Destroy(gameObject);
        }
    }

    public void SetNumber(int num)
    {
        number = num;
        numberText.text = num.ToString();
    }

    public int GetNumber()
    {
        return number;
    }
}