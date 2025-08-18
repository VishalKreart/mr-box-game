using UnityEngine;
using UnityEngine.Purchasing;

[CreateAssetMenu(fileName = "IAPProductCatalog", menuName = "IAP/Product Catalog")]
public class IAPProductCatalog : ScriptableObject
{
    [System.Serializable]
    public class Product
    {
        public string productId;
        public UnityEngine.Purchasing.ProductType productType;
        public string displayName;
        public string description;
        public decimal price;
        public string iconPath;
    }
    
    [Header("IAP Products")]
    public Product[] products = new Product[]
    {
        new Product
        {
            productId = "remove_ads",
            productType = UnityEngine.Purchasing.ProductType.NonConsumable,
            displayName = "Remove Ads",
            description = "No more interstitial ads during gameplay",
            price = 2.99m,
            iconPath = "Icons/remove_ads_icon"
        },
        new Product
        {
            productId = "continue_pack",
            productType = UnityEngine.Purchasing.ProductType.Consumable,
            displayName = "Continue Pack",
            description = "20 continues without watching ads",
            price = 1.99m,
            iconPath = "Icons/continue_pack_icon"
        },
        new Product
        {
            productId = "vip_mode",
            productType = UnityEngine.Purchasing.ProductType.Subscription,
            displayName = "VIP Mode",
            description = "Monthly subscription - removes ads + 2 free continues per day",
            price = 4.99m,
            iconPath = "Icons/vip_mode_icon"
        }
    };
    
    [Header("Store Settings")]
    public string androidStoreName = "Google Play";
    public string iosStoreName = "App Store";
    
    //[Header("Product IDs by Platform")]
    [System.Serializable]
    public class PlatformProductIds
    {
        public string androidProductId;
        public string iosProductId;
        public string productId; // Default/fallback
    }
    
    public PlatformProductIds[] platformProductIds = new PlatformProductIds[]
    {
        new PlatformProductIds
        {
            androidProductId = "com.yourcompany.mrbox.remove_ads",
            iosProductId = "com.yourcompany.mrbox.remove_ads",
            productId = "remove_ads"
        },
        new PlatformProductIds
        {
            androidProductId = "com.yourcompany.mrbox.continue_pack",
            iosProductId = "com.yourcompany.mrbox.continue_pack",
            productId = "continue_pack"
        },
        new PlatformProductIds
        {
            androidProductId = "com.yourcompany.mrbox.vip_mode",
            iosProductId = "com.yourcompany.mrbox.vip_mode",
            productId = "vip_mode"
        }
    };
    
    public string GetProductIdForPlatform(string baseProductId)
    {
        foreach (var platformProduct in platformProductIds)
        {
            if (platformProduct.productId == baseProductId)
            {
                #if UNITY_ANDROID
                    return platformProduct.androidProductId;
                #elif UNITY_IOS
                    return platformProduct.iosProductId;
                #else
                    return platformProduct.productId;
                #endif
            }
        }
        return baseProductId;
    }
    
    public Product GetProductById(string productId)
    {
        foreach (var product in products)
        {
            if (product.productId == productId)
                return product;
        }
        return null;
    }
    
    public Product GetProductByDisplayName(string displayName)
    {
        foreach (var product in products)
        {
            if (product.displayName == displayName)
                return product;
        }
        return null;
    }
}
