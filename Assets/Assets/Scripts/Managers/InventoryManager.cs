//using Photon.Pun;
//using Photon.Realtime;
//using ExitGames.Client.Photon;
//using System.Collections.Generic;
//using UnityEngine;
////Se encarga de añadir, consumir y consultar recursos
////mientras asegura que las actualizaciones sean sincronizadas
////en todos los clientes conectados.
//public class InventoryManager : MonoBehaviour
//{
//    public static InventoryManager Instance;

//    [System.Serializable]
//    public class Resource
//    {
//        public string name;
//        public int quantity;
//        public int maxQuantity;
//    }

//    public List<Resource> resources = new List<Resource>();//Contiene todos los recursos del inventario. Cada recurso es un objeto de tipo Resource.

//    public delegate void OnResourceUpdated(string resourceName, int newQuantity);
//    public event OnResourceUpdated ResourceUpdated;

//    private const byte UpdateResourceEventCode = 1; // Código para sincronizar los recursos

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

//    void OnEnable()
//    {
//        PhotonNetwork.NetworkingClient.EventReceived += OnEventReceived;
//    }

//    void OnDisable()
//    {
//        PhotonNetwork.NetworkingClient.EventReceived -= OnEventReceived;
//    }

//    private void OnEventReceived(EventData photonEvent)
//    {
//        if (photonEvent.Code == UpdateResourceEventCode)
//        {
//            object[] data = (object[])photonEvent.CustomData;
//            string resourceName = (string)data[0];
//            int newQuantity = (int)data[1];

//            Resource resource = resources.Find(r => r.name == resourceName);
//            if (resource != null)
//            {
//                resource.quantity = newQuantity;

//                // Notificar a la UI
//                ResourceUpdated?.Invoke(resourceName, resource.quantity);
//            }
//        }
//    }

//    private void SyncResourceUpdate(string resourceName, int newQuantity)
//    {
//        object[] content = new object[] { resourceName, newQuantity };

//        RaiseEventOptions options = new RaiseEventOptions { Receivers = ReceiverGroup.All };
//        SendOptions sendOptions = new SendOptions { Reliability = true };

//        PhotonNetwork.RaiseEvent(UpdateResourceEventCode, content, options, sendOptions);
//    }

//    public void AddResource(string resourceName, int amount)
//    {
//        Resource resource = resources.Find(r => r.name == resourceName);

//        if (resource != null)
//        {
//            int newQuantity = Mathf.Clamp(resource.quantity + amount, 0, resource.maxQuantity);
//            resource.quantity = newQuantity;

//            // Notificar cambios a la UI
//            ResourceUpdated?.Invoke(resourceName, resource.quantity);

//            // Sincronizar con todos los jugadores
//            SyncResourceUpdate(resourceName, newQuantity);
//        }
//    }

//    public void ConsumeResource(string resourceName, int amount)
//    {
//        Resource resource = resources.Find(r => r.name == resourceName);

//        if (resource != null && resource.quantity >= amount)
//        {
//            int newQuantity = resource.quantity - amount;
//            resource.quantity = newQuantity;

//            // Notificar cambios a la UI
//            ResourceUpdated?.Invoke(resourceName, resource.quantity);

//            // Sincronizar con todos los jugadores
//            SyncResourceUpdate(resourceName, newQuantity);
//        }
//    }

//    public bool HasEnoughResource(string resourceName, int requiredAmount)
//    {
//        Resource resource = resources.Find(r => r.name == resourceName);
//        return resource != null && resource.quantity >= requiredAmount;
//    }

//    public int GetResourceQuantity(string resourceName)
//    {
//        Resource resource = resources.Find(r => r.name == resourceName);
//        return resource != null ? resource.quantity : 0;
//    }
//}

using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public class Resource
    {
        public string name;
        public int quantity;
        public int maxQuantity;
    }

    public List<Resource> resourcesTeam1 = new List<Resource>(); // Recursos de equipo 1
    public List<Resource> resourcesTeam2 = new List<Resource>(); // Recursos de equipo 2

    private Dictionary<int, Dictionary<string, int>> teamResources = new Dictionary<int, Dictionary<string, int>>()
    {
        { 1, new Dictionary<string, int>() }, // Equipo 1
        { 2, new Dictionary<string, int>() }  // Equipo 2
    };

    public event Action<int, string, int> ResourceUpdated;

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

    public void AddResource(int teamNumber, string resourceName, int quantity)
    {
        List<Resource> targetList = teamNumber == 1 ? resourcesTeam1 : resourcesTeam2;
        Resource resource = targetList.Find(r => r.name == resourceName);

        if (resource != null)
        {
            resource.quantity += quantity;
            ResourceUpdated?.Invoke(teamNumber, resourceName, resource.quantity); // Invocar el evento con tres parámetros
        }
    }

    public bool UseResource(int team, string resourceName, int quantity)
    {
        if (teamResources[team].ContainsKey(resourceName) && teamResources[team][resourceName] >= quantity)
        {
            teamResources[team][resourceName] -= quantity;

            ResourceUpdated?.Invoke(team, resourceName, teamResources[team][resourceName]);
            return true;
        }
        return false;
    }

    public Dictionary<string, int> GetTeamResources(int team)
    {
        return teamResources[team];
    }
}






