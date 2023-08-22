using System;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using UnityEngine.UI;
using TMPro;

namespace Samples.Purchasing.Core.BuyingConsumables
{
    public class BuyingConsumables : MonoBehaviour, IDetailedStoreListener
    {
        IStoreController m_StoreController; // The Unity Purchasing system.
        [SerializeField] IAPsTracker iaptracker;

        //Your products IDs. They should match the ids of your products in your store.
        public string unlockGame = "unlock_game";
        public string powerup1 = "powerup";
        public string powerup2 = "powerup2";


     //   [SerializeField] GameObject unlockGameButton;
        [SerializeField] TMP_Text powerup1amount;
        [SerializeField] TMP_Text powerup2amount;

        void Awake()
        {
            InitializePurchasing();
        }

        void Start()
        {
            iaptracker = IAPsTracker._instance;
            CloudSave.Instance?.GetIAPs(this);
        }

        void InitializePurchasing()
        {
            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
            builder.AddProduct(unlockGame, ProductType.Consumable);
            builder.AddProduct(powerup1, ProductType.Consumable);
            builder.AddProduct(powerup2, ProductType.Consumable);

            UnityPurchasing.Initialize(this, builder);
        }

        public void BuyUnlockGame()
        {
            m_StoreController.InitiatePurchase(unlockGame);
        }

        public void BuyPowerUp1()
        {
            m_StoreController.InitiatePurchase(powerup1);
        }

        public void BuyPowerUp2()
        {
            m_StoreController.InitiatePurchase(powerup2);
        }

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            Debug.Log("In-App Purchasing successfully initialized");
            m_StoreController = controller;
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            OnInitializeFailed(error, null);
        }

        public void OnInitializeFailed(InitializationFailureReason error, string message)
        {
            var errorMessage = $"Purchasing failed to initialize. Reason: {error}.";

            if (message != null)
            {
                errorMessage += $" More details: {message}";
            }

            Debug.Log(errorMessage);
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
        {
            //Retrieve the purchased product
            var product = args.purchasedProduct;

            //Add the purchased product to the players inventory
            if (product.definition.id == unlockGame)
            {
                UnlockGame();
            }
            else if (product.definition.id == powerup1)
            {
                UnlockPowerup1();
            }
            else if (product.definition.id == powerup2)
            {
                UnlockPowerup2();
            }

            Debug.Log($"Purchase Complete - Product: {product.definition.id}");

            //We return Complete, informing IAP that the processing on our side is done and the transaction can be closed.
            return PurchaseProcessingResult.Complete;
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            Debug.Log($"Purchase failed - Product: '{product.definition.id}', PurchaseFailureReason: {failureReason}");
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
        {
            Debug.Log($"Purchase failed - Product: '{product.definition.id}'," +
                $" Purchase failure reason: {failureDescription.reason}," +
                $" Purchase failure details: {failureDescription.message}");
        }

        void UnlockGame()
        {
            iaptracker.unlockGame = true;
            CloudSave.Instance.SaveEntry("iap_unlockedGame", iaptracker.unlockGame.ToString());
            UpdateUI();
        }

        void UnlockPowerup1()
        {
            iaptracker.boost1amount += 5;
            CloudSave.Instance.SaveEntry("iap_boost1", iaptracker.boost1amount.ToString());
            UpdateUI();
        }

        void UnlockPowerup2()
        {
            iaptracker.boost2amount += 5;
            CloudSave.Instance.SaveEntry("iap_boost2", iaptracker.boost2amount.ToString());
            UpdateUI();
        }

        public void UpdateUI()
        {
            if(iaptracker.unlockGame)
            {
          //      unlockGameButton.SetActive(false);
            }
            powerup1amount.text = "You have: " + iaptracker.boost1amount.ToString();
            powerup2amount.text = "You have: " + iaptracker.boost2amount.ToString();

        }
    }
}
