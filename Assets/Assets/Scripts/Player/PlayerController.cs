<<<<<<< Updated upstream
using System.Collections;
using System.Collections.Generic;
=======
//using UnityEngine;
//using Photon.Pun;

//public class PlayerController : MonoBehaviour
//{
//    public float moveSpeed = 5f; // Velocidad de movimiento
//    public Transform movePoint; // Punto de referencia para el movimiento
//    public LayerMask interactableLayer; // Capa de objetos interactuables
//    public LayerMask obstacles; // muros

//    private PhotonView photonView;
//    private InteractableObject currentInteractable; // Objeto interactuable actual

//    void Start()
//    {
//        movePoint.parent = null; // Independiza el punto de movimiento del jugador
//        photonView = GetComponent<PhotonView>();
//    }

//    void Update()
//    {
//        HandleMovement(); // Gestión del movimiento

//        HandleInteraction(); // Gestión de interacción
//    }

//    // Controla el movimiento en una grilla
//    void HandleMovement()
//    {
//        if (photonView.IsMine)
//        {
//            transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed * Time.deltaTime);

//            if (Vector3.Distance(transform.position, movePoint.position) <= .05f)
//            {
//                if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) == 1f)
//                {
//                    if (!Physics2D.OverlapCircle(movePoint.position + new Vector3(Input.GetAxisRaw("Horizontal"), 0f, 0f), .2f, obstacles))
//                    {
//                        movePoint.position += new Vector3(Input.GetAxisRaw("Horizontal"), 0f, 0f);
//                    }
//                }
//                else if (Mathf.Abs(Input.GetAxisRaw("Vertical")) == 1f)
//                {
//                    if (!Physics2D.OverlapCircle(movePoint.position + new Vector3(0f, Input.GetAxisRaw("Vertical"), 0f), .2f, obstacles))
//                    {
//                        movePoint.position += new Vector3(0f, Input.GetAxisRaw("Vertical"), 0f);
//                    }
//                }
//            }
//        }


//    }

//    // Controla las interacciones con objetos
//    void HandleInteraction()
//    {
//        if (Input.GetKeyDown(KeyCode.E))
//        {
//            Collider2D interactable = Physics2D.OverlapCircle(transform.position, 1f, interactableLayer);

//            if (interactable != null)
//            {
//                currentInteractable = interactable.GetComponent<InteractableObject>();

//                if (currentInteractable != null)
//                {
//                    if (currentInteractable.isResourceNode)
//                    {
//                        // Inicia la recolección de recursos
//                        currentInteractable.StartGathering();
//                    }
//                    else if (currentInteractable.isCraftingStation)
//                    {
//                        // Genera un prefab desde una estación de crafting
//                        currentInteractable.CraftItem();
//                    }
//                    //else if (currentInteractable.isPrefabSpawner)
//                    //{
//                    //    // Genera un prefab desde un spawner específico
//                    //    currentInteractable.SpawnPrefab();
//                    //}
//                }
//            }
//        }

//        if (Input.GetKeyUp(KeyCode.E) && currentInteractable != null)
//        {
//            if (currentInteractable.isResourceNode)
//            {
//                currentInteractable.StopGathering();
//            }
//            currentInteractable = null;
//        }
//    }

//private void OnDrawGizmosSelected()
//    {
//        // Visualizar el radio de interacción
//        Gizmos.color = Color.green;
//        Gizmos.DrawWireSphere(transform.position, 1f);
//    }
//}
>>>>>>> Stashed changes
using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviour
{
<<<<<<< Updated upstream

    public float moveSpeed = 5f;//Velocidad de transicion entre grillas
    public Transform movePoint;//Punto de referencia para moverse en la grilla
=======
    public int team; // Equipo del jugador (1 o 2)
    public float moveSpeed = 5f;
    public Transform movePoint;
    public LayerMask interactableLayer;
    public LayerMask obstacles;

    private PhotonView photonView;
>>>>>>> Stashed changes

    public LayerMask obstacles;//Layer de obstaculos en tilemap
    // Start is called before the first frame update
    void Start()
    {
<<<<<<< Updated upstream
        movePoint.parent = null;//Empieza encima del player y luego lo usamos para desplazarnos
=======
        movePoint.parent = null;
        photonView = GetComponent<PhotonView>();
>>>>>>> Stashed changes
    }

    // Update is called once per frame
    void Update()
    {
<<<<<<< Updated upstream
        if (GetComponent<PhotonView>().IsMine == true)
=======
        HandleMovement();
        HandleInteraction();
    }

    void HandleMovement()
    {
        if (photonView.IsMine)
>>>>>>> Stashed changes
        {
            //Movimiento
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
        }
<<<<<<< Updated upstream
    }
}
=======
    }

    void HandleInteraction()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Collider2D interactable = Physics2D.OverlapCircle(transform.position, 1f, interactableLayer);

            if (interactable != null)
            {
                var currentInteractable = interactable.GetComponent<InteractableObject>();

                if (currentInteractable != null)
                {
                    if (currentInteractable.isResourceNode)
                    {
                        currentInteractable.StartGathering();
                        InventoryManager.Instance.AddResource(team, currentInteractable.resourceName, 10); // Ejemplo
                    }
                    else if (currentInteractable.isCraftingStation)
                    {
                        if (InventoryManager.Instance.UseResource(team, "Wood", 5)) // Ejemplo
                        {
                            currentInteractable.CraftItem();
                        }
                        else
                        {
                            Debug.Log($"Equipo {team}: Recursos insuficientes");
                        }
                    }
                }
            }
        }
    }
}


>>>>>>> Stashed changes
