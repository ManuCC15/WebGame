using System.Collections.Generic;
using UnityEngine;
using TMPro;

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
        InventoryManager.Instance.ResourceUpdated += UpdateResourceUI;

        // Limpia la UI antes de generar nuevos elementos
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
            // Instanciar el prefab sin asignar el padre directamente
            TextMeshProUGUI resourceText = Instantiate(resourceUITextPrefab);

            // Asignar el padre explícitamente después de instanciar
            resourceText.transform.SetParent(resourceUIParent, false);

            // Configurar el texto inicial
            resourceText.text = $"{resourceName}: {initialQuantity}";

            // Agregar al diccionario para control futuro
            resourceUIElements.Add(resourceName, resourceText);
        }
    }


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


