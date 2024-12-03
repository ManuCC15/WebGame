using Photon.Pun;
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

    public List<Resource> resourcesTeamA = new List<Resource>(); // Recursos para el equipo A
    public List<Resource> resourcesTeamB = new List<Resource>(); // Recursos para el equipo B

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
    }

    public List<Resource> GetResourcesForTeam(string team)
    {
        return team == "A" ? resourcesTeamA : resourcesTeamB;
    }

    public void AddResource(string resourceName, string team, int amount)
    {
        List<Resource> resources = team == "A" ? resourcesTeamA : resourcesTeamB;
        Resource resource = resources.Find(r => r.name == resourceName);

        if (resource != null)
        {
            int newQuantity = Mathf.Clamp(resource.quantity + amount, 0, resource.maxQuantity);
            resource.quantity = newQuantity;

            // Sincronizar con todos los jugadores del equipo
            SyncResourceUpdate(team, resourceName, newQuantity);

            // Notificar cambios a la UI
            ResourceUpdated?.Invoke(team, resourceName, resource.quantity);
        }
    }

    public void ConsumeResource(string resourceName, string team, int amount)
    {
        List<Resource> resources = team == "A" ? resourcesTeamA : resourcesTeamB;
        Resource resource = resources.Find(r => r.name == resourceName);

        if (resource != null && resource.quantity >= amount)
        {
            int newQuantity = resource.quantity - amount;
            resource.quantity = newQuantity;

            // Sincronizar con todos los jugadores del equipo
            SyncResourceUpdate(team, resourceName, newQuantity);

            // Notificar cambios a la UI
            ResourceUpdated?.Invoke(team, resourceName, resource.quantity);
        }
    }

    public bool HasEnoughResource(string resourceName, string team, int requiredAmount)
    {
        List<Resource> resources = team == "A" ? resourcesTeamA : resourcesTeamB;
        Resource resource = resources.Find(r => r.name == resourceName);
        return resource != null && resource.quantity >= requiredAmount;
    }

    private void SyncResourceUpdate(string team, string resourceName, int newQuantity)
    {
        // Llamada RPC para actualizar a todos los jugadores
        PhotonView.Get(this).RPC("UpdateResourceOnAllClients", RpcTarget.All, team, resourceName, newQuantity);
    }

    // Este RPC se llama en todos los clientes para actualizar la UI
    [PunRPC]
    public void UpdateResourceOnAllClients(string team, string resourceName, int newQuantity)
    {
        ResourceUpdated?.Invoke(team, resourceName, newQuantity);
    }
}









