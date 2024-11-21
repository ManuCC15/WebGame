using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public Text resourceUITextPrefab;
    public Transform resourceUIParent;

    private Dictionary<string, Text> resourceUIElements = new Dictionary<string, Text>();

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

    void Start()
    {
        InventoryManager.Instance.ResourceUpdated += UpdateResourceUI;

        foreach (var resource in InventoryManager.Instance.resources)
        {
            CreateResourceUI(resource.name, resource.quantity);
        }
    }

    void CreateResourceUI(string resourceName, int initialQuantity)
    {
        Text resourceText = Instantiate(resourceUITextPrefab, resourceUIParent);
        resourceText.text = $"{resourceName}: {initialQuantity}";
        resourceUIElements.Add(resourceName, resourceText);
    }

    void UpdateResourceUI(string resourceName, int newQuantity)
    {
        if (resourceUIElements.TryGetValue(resourceName, out Text resourceText))
        {
            resourceText.text = $"{resourceName}: {newQuantity}";
        }
    }
}

