//using UnityEngine;
//using TMPro;
//using System.Collections.Generic;

//public class UIManager : MonoBehaviour
//{
//    public static UIManager Instance;

//    public Transform resourceUIParent;

//    public TextMeshProUGUI resourceUITextPrefab;
//    private Dictionary<string, TextMeshProUGUI> resourceUIElements = new Dictionary<string, TextMeshProUGUI>();

//    void Awake()
//    {
//        if (Instance == null)
//        {
//            Instance = this;
//        }
//        else
//        {
//            Destroy(gameObject);
//        }
//    }

//    void Start()
//    {
//        // Suscribir al evento de actualización de recursos
//        InventoryManager.Instance.ResourceUpdated += UpdateResourceUI;

//        // Inicializar la UI
//        ClearResourceUI();

//        foreach (var resource in InventoryManager.Instance.resources)
//        {
//            CreateResourceUI(resource.name, resource.quantity);
//        }
//    }

//    void CreateResourceUI(string resourceName, int initialQuantity)
//    {
//        if (!resourceUIElements.ContainsKey(resourceName)) // Evitar duplicados
//        {
//            TextMeshProUGUI resourceText = Instantiate(resourceUITextPrefab);
//            resourceText.transform.SetParent(resourceUIParent, false);

//            // Asignar el texto inicial
//            resourceText.text = $"{resourceName}: {initialQuantity}";

//            // Guardar el UI del recurso
//            resourceUIElements.Add(resourceName, resourceText);
//        }
//    }

//    // Actualizar la UI cuando cambian los recursos
//    void UpdateResourceUI(string resourceName, int newQuantity)
//    {
//        if (resourceUIElements.TryGetValue(resourceName, out TextMeshProUGUI resourceText))
//        {
//            resourceText.text = $"{resourceName}: {newQuantity}";
//        }
//        else
//        {
//            Debug.LogWarning($"Intento de actualizar un recurso inexistente: {resourceName}");
//        }
//    }

//    void ClearResourceUI()
//    {
//        foreach (var uiElement in resourceUIElements.Values)
//        {
//            Destroy(uiElement.gameObject);
//        }
//        resourceUIElements.Clear();
//    }
//}
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static InventoryManager;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public Transform team1UIParent;  // Canvas del Equipo 1
    public Transform team2UIParent;  // Canvas del Equipo 2

    public TextMeshProUGUI resourceUITextPrefab;
    private Dictionary<string, TextMeshProUGUI> team1UIElements = new Dictionary<string, TextMeshProUGUI>();
    private Dictionary<string, TextMeshProUGUI> team2UIElements = new Dictionary<string, TextMeshProUGUI>();


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

        team1UIElements = new Dictionary<string, TextMeshProUGUI>();
        team2UIElements = new Dictionary<string, TextMeshProUGUI>();
    }

    void Start()
    {
        // Crear UI para cada equipo
        CreateResourceUI(1); // Equipo 1
        CreateResourceUI(2); // Equipo 2

        // Suscribir al evento de actualización de recursos
        InventoryManager.Instance.ResourceUpdated += UpdateResourceUI;
    }

    void InitializeUI()
    {
        Debug.Log("Inicializando UI para Equipo 1 y Equipo 2");

        foreach (var resource in InventoryManager.Instance.resourcesTeam1)
        {
            Debug.Log($"Creando UI para recurso {resource.name} en Equipo 1");
            CreateResourceUI(1);
        }

        foreach (var resource in InventoryManager.Instance.resourcesTeam2)
        {
            Debug.Log($"Creando UI para recurso {resource.name} en Equipo 2");
            CreateResourceUI(2);
        }
    }

    void CreateResourceUI(int teamNumber)
    {
        List<Resource> resources = teamNumber == 1 ? InventoryManager.Instance.resourcesTeam1 : InventoryManager.Instance.resourcesTeam2;
        Transform uiParent = teamNumber == 1 ? team1UIParent : team2UIParent;

        foreach (var resource in resources)
        {
            // Crear un UI para cada recurso
            TextMeshProUGUI resourceText = Instantiate(resourceUITextPrefab);
            resourceText.transform.SetParent(uiParent, false);

            // Asignar el texto inicial del recurso
            resourceText.text = $"{resource.name}: {resource.quantity}";

            // Guardar el UI del recurso en el diccionario correspondiente
            if (teamNumber == 1)
            {
                team1UIElements.Add(resource.name, resourceText);
            }
            else
            {
                team2UIElements.Add(resource.name, resourceText);
            }
        }
    }

    void UpdateResourceUI(int teamNumber, string resourceName, int newQuantity)
    {
        // Verificar el equipo y actualizar la UI correspondiente
        if (teamNumber == 1 && team1UIElements.ContainsKey(resourceName))
        {
            team1UIElements[resourceName].text = $"{resourceName}: {newQuantity}";
        }
        else if (teamNumber == 2 && team2UIElements.ContainsKey(resourceName))
        {
            team2UIElements[resourceName].text = $"{resourceName}: {newQuantity}";
        }
    }

}




