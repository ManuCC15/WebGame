//using Photon.Pun;
//using UnityEngine;

////nodos de recursos (por ejemplo, árboles o minerales que se pueden recolectar)
////y estaciones de crafteo (lugares donde los jugadores pueden fabricar objetos combinando recursos).
//public class InteractableObject : MonoBehaviour
//{
//    public bool isResourceNode;// Indica si este objeto es un nodo de recursos (algo que puede ser recolectado).
//    public string resourceType;//Define el tipo de recurso que el nodo proporciona (por ejemplo, "madera", "piedra").
//    public int resourceAmountPerCycle = 1;//La cantidad de recursos que se otorgan.
//    public float gatherInterval = 5f;//Tiempo en segundos entre cada ciclo de recolección.

//    public bool isCraftingStation;//Indica si este objeto es una estación de crafteo.
//    public string requiredResource1;//tipo de recurso
//    public int requiredAmount1;//cantidad del recurso
//    public string requiredResource2;//tipo de recurso
//    public int requiredAmount2;//cantidad del recurso
//    public GameObject craftedPrefab;//El objeto a craftear

//    public Transform spawnLocation;//Posición donde aparecerá el objeto fabricado.

//    private bool isPlayerGathering;
//    private PhotonView photonView;

//    void Awake()
//    {
//        photonView = GetComponent<PhotonView>();
//    }

//    public void StartGathering()//recolecta recursos si es un nodo
//    {
//        if (isResourceNode)
//        {
//            isPlayerGathering = true;
//            InvokeRepeating(nameof(GatherResource), 0f, gatherInterval);
//        }
//    }

//    public void StopGathering()//detiene la recoleccion recursos si es un nodo
//    {
//        if (isResourceNode && isPlayerGathering)
//        {
//            isPlayerGathering = false;
//            CancelInvoke(nameof(GatherResource));
//        }
//    }

//    void GatherResource()//Agrega el nodo al inventario
//    {
//        InventoryManager.Instance.AddResource(resourceType, resourceAmountPerCycle);
//    }

//    public void CraftItem()//Crea un objeto si es una estacion de crafteo
//    {
//        if (isCraftingStation && InventoryManager.Instance.HasEnoughResource(requiredResource1, requiredAmount1)
//            && InventoryManager.Instance.HasEnoughResource(requiredResource2, requiredAmount2))
//        {
//            InventoryManager.Instance.ConsumeResource(requiredResource1, requiredAmount1);
//            InventoryManager.Instance.ConsumeResource(requiredResource2, requiredAmount2);

//            photonView.RPC("CraftAndSpawnPrefab", RpcTarget.All);
//        }
//    }

//    [PunRPC]
//    void CraftAndSpawnPrefab()//Sincroniza con todos el objeto
//    {
//        if (craftedPrefab != null)
//        {
//            Instantiate(craftedPrefab, spawnLocation.position, spawnLocation.rotation);
//        }
//    }
//}
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public bool isCraftingStation;
    public bool isResourceNode;

    public string resourceName;

    public void StartGathering()
    {
        Debug.Log($"Recolectando recurso: {resourceName}");
    }

    public void CraftItem()
    {
        Debug.Log("Objeto creado en estación de crafting.");
    }
}







