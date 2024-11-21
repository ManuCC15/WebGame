using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour, IPunObservable
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

    private PhotonView photonView;

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

        photonView = GetComponent<PhotonView>();  // Obtener PhotonView
    }

    public void AddResource(string resourceName, int amount)
    {
        if (!photonView.IsMine) return; // Solo el dueño del PhotonView puede modificar sus recursos

        Resource resource = resources.Find(r => r.name == resourceName);

        if (resource != null)
        {
            int newQuantity = Mathf.Clamp(resource.quantity + amount, 0, resource.maxQuantity);
            int addedAmount = newQuantity - resource.quantity;
            resource.quantity = newQuantity;

            // Notificar cambios si es necesario
            if (addedAmount > 0 && ResourceUpdated != null)
            {
                ResourceUpdated.Invoke(resourceName, resource.quantity);
            }
        }
    }

    public void ConsumeResource(string resourceName, int amount)
    {
        //if (!photonView.IsMine) return; // Solo el dueño del PhotonView puede consumir recursos

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

    public int GetResourceQuantity(string resourceName)
    {
        Resource resource = resources.Find(r => r.name == resourceName);
        return resource != null ? resource.quantity : 0;
    }

    // Sincronización de recursos entre jugadores
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Enviar los recursos al otro jugador
            foreach (var resource in resources)
            {
                stream.SendNext(resource.quantity);
            }
        }
        else
        {
            // Recibir los recursos del otro jugador
            for (int i = 0; i < resources.Count; i++)
            {
                resources[i].quantity = (int)stream.ReceiveNext();
            }

            // Notificar a la UI para actualizar
            foreach (var resource in resources)
            {
                ResourceUpdated?.Invoke(resource.name, resource.quantity);
            }
        }
    }
}




