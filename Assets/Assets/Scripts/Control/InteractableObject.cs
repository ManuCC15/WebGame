using Photon.Pun;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public bool isResourceNode; // Define si es un nodo de recursos
    public string resourceType; // Tipo de recurso
    public int resourceAmountPerCycle = 1; // Recursos por ciclo
    public float gatherInterval = 5f; // Intervalo de recolección

    public bool isCraftingStation; // Define si es una estación de crafting
    public string requiredResource1;
    public int requiredAmount1;
    public string requiredResource2;
    public int requiredAmount2;
    public GameObject craftedPrefab;

    //public bool isPrefabSpawner; // Define si es un spawner de prefabs
    //public GameObject prefabToSpawn; // Prefab que debe generarse
    public Transform spawnLocation; // Lugar donde se generará el prefab

    private bool isPlayerGathering;

    private PhotonView photonView;

    private void Start()
    {
        photonView = GetComponent<PhotonView>();
    }

    // Inicio de recolección
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
        InventoryManager.Instance.AddResource(resourceType, resourceAmountPerCycle);
    }

    // Craftear un objeto
    [Photon.Pun.PunRPC]
    public void CraftItemRPC()
    {
        if (isCraftingStation && InventoryManager.Instance.HasEnoughResource(requiredResource1, requiredAmount1)
            && InventoryManager.Instance.HasEnoughResource(requiredResource2, requiredAmount2))
        {
            InventoryManager.Instance.ConsumeResource(requiredResource1, requiredAmount1);
            InventoryManager.Instance.ConsumeResource(requiredResource2, requiredAmount2);

            if (craftedPrefab != null)
            {
                SpawnPrefab();
            }
        }
    }

    public void CraftItem()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            CraftItemRPC();  // Llamada RPC en el cliente master
        }
        else
        {
            photonView.RPC("CraftItemRPC", RpcTarget.MasterClient);  // Llamada RPC en otros clientes
        }
    }


    // Generar un prefab
    public void SpawnPrefab()
    {
        Instantiate(craftedPrefab, spawnLocation.position, spawnLocation.rotation);
    }
}

