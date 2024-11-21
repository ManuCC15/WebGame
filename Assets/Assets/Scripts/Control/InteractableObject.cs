using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public bool isResourceNode;
    public string resourceType;
    public int resourceAmountPerCycle = 1;
    public float gatherInterval = 5f;

    public bool isCraftingStation;
    public string requiredResource1;
    public int requiredAmount1;
    public string requiredResource2;
    public int requiredAmount2;
    public GameObject craftedPrefab;

    public Transform spawnLocation;

    private bool isPlayerGathering;
    private byte resourceGatherEventCode = 1; // Código de evento personalizado
    private PhotonView photonView;

    void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }

    // Iniciar recolección
    public void StartGathering()
    {
        if (isResourceNode)
        {
            isPlayerGathering = true;
            InvokeRepeating(nameof(GatherResource), 0f, gatherInterval);
        }
    }

    // Detener recolección
    public void StopGathering()
    {
        if (isResourceNode && isPlayerGathering)
        {
            isPlayerGathering = false;
            CancelInvoke(nameof(GatherResource));
        }
    }

    void GatherResource()
    {
        // Recolectar el recurso localmente
        InventoryManager.Instance.AddResource(resourceType, resourceAmountPerCycle);

        // Notificar a los demás jugadores sobre la recolección
        RaiseGatherResourceEvent();
    }

    void RaiseGatherResourceEvent()
    {
        // Los datos del evento (tipo de recurso y cantidad)
        object[] content = new object[] { resourceType, resourceAmountPerCycle };

        // RaiseEvent con Photon Realtime
        RaiseEventOptions options = new RaiseEventOptions { Receivers = ReceiverGroup.All }; // Enviar a todos los jugadores
        SendOptions sendOptions = new SendOptions { Reliability = true }; // Garantiza que el mensaje se reciba

        PhotonNetwork.RaiseEvent(resourceGatherEventCode, content, options, sendOptions);
    }

    // Craftear el objeto
    public void CraftItem()
    {
        if (isCraftingStation && InventoryManager.Instance.HasEnoughResource(requiredResource1, requiredAmount1)
            && InventoryManager.Instance.HasEnoughResource(requiredResource2, requiredAmount2))
        {
            InventoryManager.Instance.ConsumeResource(requiredResource1, requiredAmount1);
            InventoryManager.Instance.ConsumeResource(requiredResource2, requiredAmount2);

            // Instanciar el prefab y sincronizar con los demás jugadores
            photonView.RPC("CraftAndSpawnPrefab", RpcTarget.All);
        }
    }

    // RPC para craftear y generar el objeto para todos los jugadores
    [PunRPC]
    void CraftAndSpawnPrefab()
    {
        if (craftedPrefab != null)
        {
            // Generar el prefab en todos los clientes
            GameObject spawnedObject = Instantiate(craftedPrefab, spawnLocation.position, spawnLocation.rotation);

            // Obtener el PhotonView del objeto instanciado
            PhotonView spawnedObjectPhotonView = spawnedObject.GetComponent<PhotonView>();

            // Asegurar que todos los jugadores vean el objeto instanciado
            spawnedObjectPhotonView.RPC("OnPrefabCrafted", RpcTarget.All, spawnedObject.transform.position, spawnedObject.transform.rotation);
        }
    }

    // RPC para notificar a todos los jugadores sobre la creación del prefab
    [PunRPC]
    void OnPrefabCrafted(Vector3 position, Quaternion rotation)
    {
        // Instanciar el prefab en la posición y rotación especificada por el master
        if (craftedPrefab != null)
        {
            Instantiate(craftedPrefab, position, rotation);
        }
    }
}







