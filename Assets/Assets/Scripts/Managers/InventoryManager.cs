using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
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

    // Diccionario de recursos dividido por equipo
    private Dictionary<string, List<Resource>> teamResources = new Dictionary<string, List<Resource>>();

    public delegate void OnResourceUpdated(string team, string resourceName, int newQuantity);
    public event OnResourceUpdated ResourceUpdated;

    private const byte UpdateResourceEventCode = 1; // Código para sincronizar los recursos

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

        // Inicializar recursos para equipos
        teamResources["A"] = new List<Resource>();
        teamResources["B"] = new List<Resource>();
    }

    void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEventReceived;
    }

    void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEventReceived;
    }

    private void OnEventReceived(EventData photonEvent)
    {
        if (photonEvent.Code == UpdateResourceEventCode)
        {
            object[] data = (object[])photonEvent.CustomData;
            string team = (string)data[0];
            string resourceName = (string)data[1];
            int newQuantity = (int)data[2];

            List<Resource> resources = GetResourcesForTeam(team);
            Resource resource = resources.Find(r => r.name == resourceName);
            if (resource != null)
            {
                resource.quantity = newQuantity;

                // Notificar a la UI
                ResourceUpdated?.Invoke(team, resourceName, resource.quantity);
            }
        }
    }

    private void SyncResourceUpdate(string team, string resourceName, int newQuantity)
    {
        object[] content = new object[] { team, resourceName, newQuantity };

        RaiseEventOptions options = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        SendOptions sendOptions = new SendOptions { Reliability = true };

        PhotonNetwork.RaiseEvent(UpdateResourceEventCode, content, options, sendOptions);
    }

    public void AddResource(string resourceName, string team, int amount)
    {
        List<Resource> resources = GetResourcesForTeam(team);
        Resource resource = resources.Find(r => r.name == resourceName);

        if (resource != null)
        {
            int newQuantity = Mathf.Clamp(resource.quantity + amount, 0, resource.maxQuantity);
            resource.quantity = newQuantity;

            // Notificar cambios a la UI
            ResourceUpdated?.Invoke(team, resourceName, resource.quantity);

            // Sincronizar con todos los jugadores
            SyncResourceUpdate(team, resourceName, newQuantity);
        }
    }

    public void ConsumeResource(string resourceName, string team, int amount)
    {
        List<Resource> resources = GetResourcesForTeam(team);
        Resource resource = resources.Find(r => r.name == resourceName);

        if (resource != null && resource.quantity >= amount)
        {
            int newQuantity = resource.quantity - amount;
            resource.quantity = newQuantity;

            // Notificar cambios a la UI
            ResourceUpdated?.Invoke(team, resourceName, resource.quantity);

            // Sincronizar con todos los jugadores
            SyncResourceUpdate(team, resourceName, newQuantity);
        }
    }

    public bool HasEnoughResource(string resourceName, string team, int requiredAmount)
    {
        List<Resource> resources = GetResourcesForTeam(team);
        Resource resource = resources.Find(r => r.name == resourceName);
        return resource != null && resource.quantity >= requiredAmount;
    }

    public int GetResourceQuantity(string resourceName, string team)
    {
        List<Resource> resources = GetResourcesForTeam(team);
        Resource resource = resources.Find(r => r.name == resourceName);
        return resource != null ? resource.quantity : 0;
    }

    public List<Resource> GetResourcesForTeam(string team)
    {
        if (teamResources.TryGetValue(team, out List<Resource> resources))
        {
            return resources;
        }
        else
        {
            Debug.LogWarning($"No se encontraron recursos para el equipo: {team}");
            return new List<Resource>();
        }
    }

    public void InitializeTeamResources(string team, List<Resource> initialResources)
    {
        if (teamResources.ContainsKey(team))
        {
            teamResources[team] = initialResources;
        }
        else
        {
            teamResources.Add(team, initialResources);
        }
    }
}








