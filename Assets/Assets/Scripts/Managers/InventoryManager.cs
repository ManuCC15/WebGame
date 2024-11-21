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

    public List<Resource> resources = new List<Resource>();

    public delegate void OnResourceUpdated(string resourceName, int newQuantity);
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
            string resourceName = (string)data[0];
            int newQuantity = (int)data[1];

            Resource resource = resources.Find(r => r.name == resourceName);
            if (resource != null)
            {
                resource.quantity = newQuantity;

                // Notificar a la UI
                ResourceUpdated?.Invoke(resourceName, resource.quantity);
            }
        }
    }

    private void SyncResourceUpdate(string resourceName, int newQuantity)
    {
        object[] content = new object[] { resourceName, newQuantity };

        RaiseEventOptions options = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        SendOptions sendOptions = new SendOptions { Reliability = true };

        PhotonNetwork.RaiseEvent(UpdateResourceEventCode, content, options, sendOptions);
    }

    public void AddResource(string resourceName, int amount)
    {
        Resource resource = resources.Find(r => r.name == resourceName);

        if (resource != null)
        {
            int newQuantity = Mathf.Clamp(resource.quantity + amount, 0, resource.maxQuantity);
            resource.quantity = newQuantity;

            // Notificar cambios a la UI
            ResourceUpdated?.Invoke(resourceName, resource.quantity);

            // Sincronizar con todos los jugadores
            SyncResourceUpdate(resourceName, newQuantity);
        }
    }

    public void ConsumeResource(string resourceName, int amount)
    {
        Resource resource = resources.Find(r => r.name == resourceName);

        if (resource != null && resource.quantity >= amount)
        {
            int newQuantity = resource.quantity - amount;
            resource.quantity = newQuantity;

            // Notificar cambios a la UI
            ResourceUpdated?.Invoke(resourceName, resource.quantity);

            // Sincronizar con todos los jugadores
            SyncResourceUpdate(resourceName, newQuantity);
        }
    }

    public bool HasEnoughResource(string resourceName, int requiredAmount)
    {
        Resource resource = resources.Find(r => r.name == resourceName);
        return resource != null && resource.quantity >= requiredAmount;
    }

    public int GetResourceQuantity(string resourceName)
    {
        Resource resource = resources.Find(r => r.name == resourceName);
        return resource != null ? resource.quantity : 0;
    }
}





