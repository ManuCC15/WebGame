using UnityEngine;
using TMPro;
using System.Collections.Generic;
using Photon.Pun;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public Transform resourceUIParentTeamA; // Contenedor de UI para el Equipo A
    public Transform resourceUIParentTeamB; // Contenedor de UI para el Equipo B

    public TextMeshProUGUI resourceUITextPrefab; // Prefab del texto para los recursos
    private Dictionary<string, TextMeshProUGUI> resourceUIElementsTeamA = new Dictionary<string, TextMeshProUGUI>();
    private Dictionary<string, TextMeshProUGUI> resourceUIElementsTeamB = new Dictionary<string, TextMeshProUGUI>();

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
        string team = GetPlayerTeam(); // Obtiene el equipo del jugador
        Debug.Log($"Equipo del jugador: {team}");

        // Si el jugador es del equipo A, muestra la UI del equipo A y oculta la del equipo B
        if (team == "A")
        {
            resourceUIParentTeamA.gameObject.SetActive(true);
            resourceUIParentTeamB.gameObject.SetActive(false);
            InitializeTeamResources("A", InventoryManager.Instance.GetResourcesForTeam("A"));
        }
        // Si el jugador es del equipo B, muestra la UI del equipo B y oculta la del equipo A
        else if (team == "B")
        {
            resourceUIParentTeamA.gameObject.SetActive(false);
            resourceUIParentTeamB.gameObject.SetActive(true);
            InitializeTeamResources("B", InventoryManager.Instance.GetResourcesForTeam("B"));
        }
        else
        {
            Debug.LogWarning("Equipo del jugador no identificado.");
        }

        // Suscribirse al evento de actualización de recursos
        InventoryManager.Instance.ResourceUpdated += UpdateResourceUI;
    }

    void InitializeTeamResources(string team, List<InventoryManager.Resource> resources)
    {
        foreach (var resource in resources)
        {
            CreateResourceUI(team, resource.name, resource.quantity);
        }
    }

    void CreateResourceUI(string team, string resourceName, int initialQuantity)
    {
        Dictionary<string, TextMeshProUGUI> resourceUIElements;
        Transform parent;

        if (team == "A")
        {
            resourceUIElements = resourceUIElementsTeamA;
            parent = resourceUIParentTeamA;
        }
        else if (team == "B")
        {
            resourceUIElements = resourceUIElementsTeamB;
            parent = resourceUIParentTeamB;
        }
        else
        {
            Debug.LogWarning($"Equipo desconocido: {team}");
            return;
        }

        if (!resourceUIElements.ContainsKey(resourceName)) // Evitar duplicados
        {
            TextMeshProUGUI resourceText = Instantiate(resourceUITextPrefab, parent, false);

            // Asignar el texto inicial
            resourceText.text = $"{resourceName}: {initialQuantity}";

            // Guardar el elemento UI del recurso
            resourceUIElements.Add(resourceName, resourceText);
        }
    }

    void UpdateResourceUI(string team, string resourceName, int newQuantity)
    {
        Dictionary<string, TextMeshProUGUI> resourceUIElements = team == "A" ? resourceUIElementsTeamA : resourceUIElementsTeamB;

        if (resourceUIElements.TryGetValue(resourceName, out TextMeshProUGUI resourceText))
        {
            resourceText.text = $"{resourceName}: {newQuantity}";
        }
        else
        {
            Debug.LogWarning($"Intento de actualizar un recurso inexistente: {resourceName} en el equipo {team}");
        }
    }

    void ClearResourceUI()
    {
        foreach (var uiElement in resourceUIElementsTeamA.Values)
        {
            Destroy(uiElement.gameObject);
        }
        resourceUIElementsTeamA.Clear();

        foreach (var uiElement in resourceUIElementsTeamB.Values)
        {
            Destroy(uiElement.gameObject);
        }
        resourceUIElementsTeamB.Clear();
    }

    private string GetPlayerTeam()
    {
        // Obtén el equipo del jugador desde las propiedades personalizadas de Photon
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("Team", out object team))
        {
            return team as string;
        }
        return null;
    }
}








