using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using TMPro.Examples;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using UnityEngine.UI;
public class IAPManager : MonoBehaviour
{
    // Start is called before the first frame update
    private string premium = "ncert_1_499";
    public TextMeshProUGUI priceDisplay;
    //private Button premiumButton;

    public GameObject WinPopupPanel;
    public GameObject FailPopupPanel;
    public CanvasGroup mainPanel;
    public PanelShift panelShiftScript;
    public UserRegistration backendScript;

    //public GameObject postLoginCanvas;
    //public GameObject loginCanvas;
    //public TextMeshProUGUI errorDisplay;

    //public GameObject panelShiftObj;

    

    //private PanelShift panelShiftScript;

    [HideInInspector]
    public bool purchased = false;

    private void Start()
    {
        //panelShiftScript = panelShiftObj.GetComponent<PanelShift>();
        StartCoroutine(CheckInitialization());
       
    }
    IEnumerator CheckInitialization()
    { 
        bool isInitialized = false;
        while (!isInitialized)
        {
            if (CodelessIAPStoreListener.Instance.HasProductInCatalog(premium))
            { 
                isInitialized = true;
            }
            yield return new WaitForSeconds(0.5f);
        }
        //if(CodelessIAPStoreListener.Instance.GetProduct(premium).hasReceipt)
        //{
        //    sampleObj.AddPremiumFeature();
        //}
        Product product = CodelessIAPStoreListener.Instance.GetProduct(premium);
        if (product != null && product.hasReceipt)
        {
            purchased = true;
            Debug.Log("Restoring previous purchase...");
            // Ensure premium access is given
            //Add logic for premium subscription bought
        }
        else { 
            //Add logic for premium subscription not bought
            //postLoginCanvas.SetActive(false);
            //loginCanvas.SetActive(true);
        
        }
    }
    public void OnPurchaseComplete(Product product)
    {
        if (product.definition.id == premium)
        {

            Debug.Log("Payment Successful");
            mainPanel.alpha = Mathf.Clamp01(0.5f);
            mainPanel.interactable = false;
            WinPopupPanel.SetActive(true);
            purchased = true;
            panelShiftScript.OnInAppPurchaseSuccess();


            decimal price = product.metadata.localizedPrice;
            int amount = Mathf.RoundToInt((float)price);    
            string currencyCode = product.metadata.localizedPrice + " " + product.metadata.isoCurrencyCode;
            StartCoroutine(backendScript.UpdatePurchase(amount, currencyCode));
        }
        

        

    }


    public void OnCustomPurchaseComplete(Product product) {
        if (product.definition.id == premium)
        {

            Debug.Log("Payment Successful");
            //mainPanel.alpha = Mathf.Clamp01(0.5f);
            //mainPanel.interactable = false;
            //WinPopupPanel.SetActive(true);
            //purchased = true;
            panelShiftScript.OnInAppPurchaseSuccess();


            decimal price = product.metadata.localizedPrice;
            int amount = Mathf.RoundToInt((float)price);
            string currencyCode = product.metadata.localizedPrice + " " + product.metadata.isoCurrencyCode;
            StartCoroutine(backendScript.UpdatePurchase(amount, currencyCode));
        }

    }

    
    public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
    {
        Debug.Log(product.definition.id + "purchase failure reason" + failureDescription);
        if (failureDescription.reason == PurchaseFailureReason.DuplicateTransaction)
        {
            Debug.Log("Product already owned. Restoring purchase...");
            //sampleObj.AddPremiumFeature(); // Unlock premium features again
            mainPanel.alpha = Mathf.Clamp01(0.5f);
            mainPanel.interactable = false;
            WinPopupPanel.SetActive(true);
            panelShiftScript.OnInAppPurchaseSuccess();
            return;
        }
        mainPanel.alpha = Mathf.Clamp01(0.5f);
        mainPanel.interactable = false;
        FailPopupPanel.SetActive(true);
    }

    public void OnCustomPurchaseFailed(Product product, PurchaseFailureDescription failureDescription) {

        Debug.Log(product.definition.id + "purchase failure reason" + failureDescription);
        if (failureDescription.reason == PurchaseFailureReason.DuplicateTransaction)
        {
            Debug.Log("Product already owned. Restoring purchase...");
            //sampleObj.AddPremiumFeature(); // Unlock premium features again
            panelShiftScript.OnInAppPurchaseSuccess();
            return;
        }
        
    }
    public void OnProductFetched(Product product)
    {
        if (product.definition.id == premium) 
        {
            UpdateButtonPrice(priceDisplay, product);
        }
    }

    private void UpdateButtonPrice(TextMeshProUGUI textField, Product product)
    {
       
        if (textField != null)
        {
            textField.text = product.metadata.localizedPrice + " " + product.metadata.isoCurrencyCode;
        }
        
    }
    public void tryAgain()
    {
        mainPanel.alpha = Mathf.Clamp01(1f);
        mainPanel.interactable = true;
        FailPopupPanel.SetActive(false);
    }

    public void RedeemButtonPressed() {

        //if (purchased)
        //{
        //    postLoginCanvas.SetActive(true);
        //    loginCanvas.SetActive(false);
        //}
        //else {
        //    errorDisplay.text = "Upgrade to premium to continue";
        //    StartCoroutine(panelShiftScript.ShowAndHide(errorDisplay));
        //}

        //Debug.Log("Payment Successful");
        //mainPanel.alpha = Mathf.Clamp01(0.5f);
        //mainPanel.interactable = false;
        //WinPopupPanel.SetActive(true);
        //purchased = true;

        Product product = CodelessIAPStoreListener.Instance.GetProduct(premium);

        if (product != null && product.hasReceipt)
        {
            Debug.Log("Previous purchase found. Redeeming...");

            // Same as OnPurchaseComplete logic
            mainPanel.alpha = Mathf.Clamp01(0.5f);
            mainPanel.interactable = false;
            WinPopupPanel.SetActive(true);
            purchased = true;
            panelShiftScript.OnInAppPurchaseSuccess();

            // Send backend update if needed
            //decimal price = product.metadata.localizedPrice;
            //int amount = Mathf.RoundToInt((float)price);
            //string currencyCode = product.metadata.localizedPrice + " " + product.metadata.isoCurrencyCode;
            //StartCoroutine(backendScript.UpdatePurchase(amount, currencyCode));
        }
        else
        {
            Debug.Log("No previous purchase found to redeem.");
            // Optional: show popup or message to user
            FailPopupPanel.SetActive(true);
        }
    }
}
[Serializable]
public class ReceiptData
{
    public string Payload;
    public string Store;
    public string TransactionID;
}
[Serializable]
public class Payload
{
    public string json;
    public string signature;
    public PaymentData paymentData;
}
[Serializable]
public class PaymentData
{
    public string orderId;
    public string productId;
    public string packageName;
    public string purchaseToken;
    public int purchaseState;
    public long purchaseTime;
    public bool acknowledged;
}