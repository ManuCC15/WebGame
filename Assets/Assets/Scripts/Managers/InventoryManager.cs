using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [System.Serializable]
    public class Resource
    {
        public string name;
        public int quantity;
        public int maxQuantity;
    }

    public List<Resource> resources = new List<Resource>();

    public delegate void OnResourceUpdated(string resourceName, int newQuantity);
    public event OnResourceUpdated ResourceUpdated;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddResource(string resourceName, int amount)
    {
        Resource resource = resources.Find(r => r.name == resourceName);

        if (resource != null)
        {
            int newQuantity = Mathf.Clamp(resource.quantity + amount, 0, resource.maxQuantity);
            int addedAmount = newQuantity - resource.quantity;
            resource.quantity = newQuantity;

            if (addedAmount > 0 && ResourceUpdated != null)
            {
                ResourceUpdated.Invoke(resourceName, resource.quantity);
            }
        }
    }

    public void ConsumeResource(string resourceName, int amount)
    {
        Resource resource = resources.Find(r => r.name == resourceName);

        if (resource != null && resource.quantity >= amount)
        {
            resource.quantity -= amount;
            ResourceUpdated?.Invoke(resourceName, resource.quantity);
        }
    }

    public bool HasEnoughResource(string resourceName, int requiredAmount)
    {
        Resource resource = resources.Find(r => r.name == resourceName);
        return resource != null && resource.quantity >= requiredAmount;
    }

    public void SellResource(string resourceName, int amount, int goldValuePerUnit)
    {
        if (HasEnoughResource(resourceName, amount))
        {
            ConsumeResource(resourceName, amount);
            AddResource("Oro", amount * goldValuePerUnit);
        }
    }

    public int GetResourceQuantity(string resourceName)
    {
        Resource resource = resources.Find(r => r.name == resourceName);
        return resource != null ? resource.quantity : 0;
    }
}


