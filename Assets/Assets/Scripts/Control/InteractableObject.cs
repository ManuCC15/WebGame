using Photon.Pun;
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
    private PhotonView photonView;

    void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }

    public void StartGathering()
    {
        if (isResourceNode)
        {
            isPlayerGathering = true;
            InvokeRepeating(nameof(GatherResource), 0f, gatherInterval);
        }
    }

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

    public void CraftItem()
    {
        if (isCraftingStation && InventoryManager.Instance.HasEnoughResource(requiredResource1, requiredAmount1)
            && InventoryManager.Instance.HasEnoughResource(requiredResource2, requiredAmount2))
        {
            InventoryManager.Instance.ConsumeResource(requiredResource1, requiredAmount1);
            InventoryManager.Instance.ConsumeResource(requiredResource2, requiredAmount2);

            photonView.RPC("CraftAndSpawnPrefab", RpcTarget.All);
        }
    }

    [PunRPC]
    void CraftAndSpawnPrefab()
    {
        if (craftedPrefab != null)
        {
            Instantiate(craftedPrefab, spawnLocation.position, spawnLocation.rotation);
        }
    }
}








