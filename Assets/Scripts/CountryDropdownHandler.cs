using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using UnityEngine.Networking;
using Newtonsoft.Json;
using UnityEditor;

public class CountryDropdownHandler : MonoBehaviour
{
    public TMP_Dropdown countryDropdown;
    public Image flagImage; // Assign in Inspector
    public string dial_code;

    private List<Country> countryList;
    private Dictionary<string, Sprite> flagSprites = new Dictionary<string, Sprite>();

    [System.Serializable]
    public class Country
    {
        public string name;
        public string code;
        public string dial_code;
    }

    IEnumerator Start()
    {
        yield return StartCoroutine(LoadCountryData());
        LoadFlagSprites();
        SetupDropdown();
        PopulateDropdown();
        countryDropdown.onValueChanged.AddListener(UpdateFlagImage);
        UpdateFlagImage(0);
    }

    IEnumerator LoadCountryData()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, "countries.json");

#if UNITY_ANDROID && !UNITY_EDITOR
        // Android specific handling
        UnityWebRequest request = UnityWebRequest.Get(filePath);
        yield return request.SendWebRequest();
        
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error loading JSON: " + request.error);
            yield break;
        }
        
        string jsonText = request.downloadHandler.text;
#else
        // Handle other platforms
        if (!File.Exists(filePath))
        {
            Debug.LogError("❌ countries.json not found at: " + filePath);
            yield break;
        }
        string jsonText = File.ReadAllText(filePath);
#endif

        countryList = JsonConvert.DeserializeObject<List<Country>>(jsonText);
        Debug.Log($"✅ Loaded {countryList?.Count} countries");
    }


    // Ensure dropdown has proper item template
    void SetupDropdown()
    {
        // Get the template object
        GameObject template = countryDropdown.template.gameObject;

        // Get the Item prefab from the template
        GameObject itemTemplate = null;
        Transform content = template.transform.Find("Viewport/Content");
        if (content != null && content.childCount > 0)
        {
            itemTemplate = content.GetChild(0).gameObject;
        }

        // If no item template exists, create one
        if (itemTemplate == null)
        {
            Debug.LogError("❌ No item template found in dropdown");
            return;
        }

        // Make sure the template has a properly configured image
        Image itemImage = itemTemplate.GetComponentInChildren<Image>();
        if (itemImage == null || itemImage.gameObject == itemTemplate)
        {
            // Create a separate image for the flag
            GameObject flagImageObj = new GameObject("Flag Image");
            flagImageObj.transform.SetParent(itemTemplate.transform, false);

            RectTransform rectTransform = flagImageObj.AddComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0, 0.5f);
            rectTransform.anchorMax = new Vector2(0, 0.5f);
            rectTransform.pivot = new Vector2(0, 0.5f);
            rectTransform.anchoredPosition = new Vector2(10, 0);
            rectTransform.sizeDelta = new Vector2(30, 20);

            itemImage = flagImageObj.AddComponent<Image>();

            // Position the text component 
            TMP_Text text = itemTemplate.GetComponentInChildren<TMP_Text>();
            if (text != null)
            {
                RectTransform textRect = text.GetComponent<RectTransform>();
                textRect.offsetMin = new Vector2(50, textRect.offsetMin.y);
            }
        }

        // This is crucial: we need to make sure the dropdown knows about this image component
        countryDropdown.itemImage = itemImage;

        Debug.Log("✅ Dropdown template setup complete");
    }

    // 📌 Load Country Data from JSON File
   

    // 📌 Load Flag Sprites from Resources Folder
    void LoadFlagSprites()
    {
        foreach (var country in countryList)
        {
            string path = $"Flags/{country.code}"; // Example: Resources/Flags/IN, Resources/Flags/US
            Sprite flag = Resources.Load<Sprite>(path);

            if (flag != null)
            {
                flagSprites[country.code] = flag;
                Debug.Log($"✅ Loaded flag for: {country.name} ({country.code})");
            }
            else
            {
                Debug.LogWarning($"⚠️ Flag not found for: {country.name} ({country.code}) at path {path}");
            }
        }
    }

    // 📌 Populate Dropdown with Countries
    void PopulateDropdown()
    {
        if (countryList == null || countryList.Count == 0)
        {
            Debug.LogError("Country list is empty!");
            return;
        }
        // Clear existing options
        countryDropdown.ClearOptions();

        // Create option data list
        List<TMP_Dropdown.OptionData> optionList = new List<TMP_Dropdown.OptionData>();

        // For each country, create an option with its name and flag
        foreach (var country in countryList)
        {
            string displayText = $"{country.dial_code} - {country.name}";
            Sprite flagSprite = null;

            if (flagSprites.ContainsKey(country.code))
            {
                flagSprite = flagSprites[country.code];
            }

            // Create a new option with both text and image
           // TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData(displayText, flagSprite);
            //optionList.Add(option);
        }

        // Add all options to the dropdown
        countryDropdown.AddOptions(optionList);

        // Force a refresh of the dropdown UI
        countryDropdown.RefreshShownValue();
    }

    // 📌 Manual handling for dropdown item visibility
    public void OnDropdownOpened()
    {
        // Custom event for when dropdown is clicked, if needed
        StartCoroutine(VerifyDropdownItems());
    }

    IEnumerator VerifyDropdownItems()
    {
        yield return new WaitForEndOfFrame();

        // If we need additional manual updates on open
        Transform content = countryDropdown.transform.Find("Template/Viewport/Content");
        if (content != null)
        {
            for (int i = 0; i < content.childCount && i < countryList.Count; i++)
            {
                Transform item = content.GetChild(i);
                Image img = item.GetComponentInChildren<Image>();
                if (img != null && img.gameObject != item.gameObject)
                {
                    // Re-assign the sprite directly in case it was overwritten
                    string countryCode = countryList[i].code;
                    if (flagSprites.ContainsKey(countryCode))
                    {
                        img.sprite = flagSprites[countryCode];
                    }
                }
            }
        }
    }

    // 📌 Update Selected Flag Image
    void UpdateFlagImage(int index)
    {
        if (index >= 0 && index < countryList.Count)
        {
            string countryCode = countryList[index].code;
            dial_code = countryList[index].dial_code;
            Debug.Log(dial_code);

            if (flagSprites.ContainsKey(countryCode))
            {
                flagImage.sprite = flagSprites[countryCode];
                Debug.Log($"✅ Updated main flag for {countryList[index].name}");
            }
            else
            {
                Debug.LogWarning($"⚠️ No flag found for {countryCode}");
            }

        }
    }
}