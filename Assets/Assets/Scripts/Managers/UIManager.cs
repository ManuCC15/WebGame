using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public Transform resourceUIParent;

    public TextMeshProUGUI resourceUITextPrefab;
    private Dictionary<string, TextMeshProUGUI> resourceUIElements = new Dictionary<string, TextMeshProUGUI>();

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
        // Suscribir al evento de actualización de recursos
        InventoryManager.Instance.ResourceUpdated += UpdateResourceUI;

        // Inicializar la UI
        ClearResourceUI();

        foreach (var resource in InventoryManager.Instance.resources)
        {
            CreateResourceUI(resource.name, resource.quantity);
        }
    }

    void CreateResourceUI(string resourceName, int initialQuantity)
    {
        if (!resourceUIElements.ContainsKey(resourceName)) // Evitar duplicados
        {
            TextMeshProUGUI resourceText = Instantiate(resourceUITextPrefab);
            resourceText.transform.SetParent(resourceUIParent, false);

            // Asignar el texto inicial
            resourceText.text = $"{resourceName}: {initialQuantity}";

            // Guardar el UI del recurso
            resourceUIElements.Add(resourceName, resourceText);
        }
    }

    // Actualizar la UI cuando cambian los recursos
    void UpdateResourceUI(string resourceName, int newQuantity)
    {
        if (resourceUIElements.TryGetValue(resourceName, out TextMeshProUGUI resourceText))
        {
            resourceText.text = $"{resourceName}: {newQuantity}";
        }
        else
        {
            Debug.LogWarning($"Intento de actualizar un recurso inexistente: {resourceName}");
        }
    }

    void ClearResourceUI()
    {
        foreach (var uiElement in resourceUIElements.Values)
        {
            Destroy(uiElement.gameObject);
        }
        resourceUIElements.Clear();
    }
}



