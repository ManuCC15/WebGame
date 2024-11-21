using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f; // Velocidad de transición entre grillas
    public Transform movePoint; // Punto de referencia para moverse en la grilla

    public LayerMask obstacles; // Layer de obstáculos en tilemap
    public LayerMask interactableLayer; // Layer de objetos interactuables

    public GameObject prefabToSpawn; // Prefab a spawnear
    public GameObject spawnOfPrefab; // Prefab a spawnear

    private PhotonView photonView; // Referencia al PhotonView

    void Start()
    {
        movePoint.parent = null; // Empieza encima del player y luego lo usamos para desplazarnos
        photonView = GetComponent<PhotonView>(); // Obtener el PhotonView
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            // Movimiento
            transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, movePoint.position) <= .05f)
            {
                if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) == 1f)
                {
                    if (!Physics2D.OverlapCircle(movePoint.position + new Vector3(Input.GetAxisRaw("Horizontal"), 0f, 0f), .2f, obstacles))
                    {
                        movePoint.position += new Vector3(Input.GetAxisRaw("Horizontal"), 0f, 0f);
                    }
                }
                else if (Mathf.Abs(Input.GetAxisRaw("Vertical")) == 1f)
                {
                    if (!Physics2D.OverlapCircle(movePoint.position + new Vector3(0f, Input.GetAxisRaw("Vertical"), 0f), .2f, obstacles))
                    {
                        movePoint.position += new Vector3(0f, Input.GetAxisRaw("Vertical"), 0f);
                    }
                }
            }

            // Interacción
            if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("xd");

                Interactuar();
            }
        }
    }

    void Interactuar()
    {
        // Detectar objetos interactuables cercanos
        Collider2D interactable = Physics2D.OverlapCircle(transform.position, 1f, interactableLayer);

        if (interactable != null)
        {
            // Lógica de interacción (spawnear prefab)
            Debug.Log("pase pa");

            photonView.RPC("SpawnPrefab", RpcTarget.All, spawnOfPrefab.transform.position);

        }
    }

    [PunRPC]
    void SpawnPrefab(Vector3 spawnPosition)
    {
        // Instanciar el prefab en todos los clientes
        Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
        Debug.Log("siuuu");

    }

    private void OnDrawGizmosSelected()
    {
        // Visualizar el radio de interacción en el editor
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 1f);
    }
}

