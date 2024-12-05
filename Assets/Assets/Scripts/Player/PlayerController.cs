using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f; // Velocidad de movimiento
    public Transform movePoint; // Punto de referencia para el movimiento
    public LayerMask interactableLayer; // Capa de objetos interactuables
    public LayerMask obstacles; // muros

    private PhotonView photonView;
    private InteractableObject currentInteractable; // Objeto interactuable actual

    void Start()
    {
        movePoint.parent = null; // Independiza el punto de movimiento del jugador
        photonView = GetComponent<PhotonView>();
    }

    void Update()
    {
        HandleMovement(); // Gestión del movimiento

        HandleInteraction(); // Gestión de interacción
    }

    // Controla el movimiento en una grilla
    void HandleMovement()
    {
        if (photonView.IsMine)
        {
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

          
    }

    // Controla las interacciones con objetos
    void HandleInteraction()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Collider2D interactable = Physics2D.OverlapCircle(transform.position, 1f, interactableLayer);

            if (interactable != null)
            {
                currentInteractable = interactable.GetComponent<InteractableObject>();

                if (currentInteractable != null)
                {
                    if (currentInteractable != null && currentInteractable.isResourceNode)
                    {
                        // Inicia la recolección de recursos solo si no se está recolectando ya
                        if (!currentInteractable.IsGathering())
                        {
                            currentInteractable.StartGathering();
                        }
                    }
                    else if (currentInteractable.isCraftingStation)
                    {
                        // Genera un prefab desde una estación de crafting
                        currentInteractable.CraftItem();
                    }
                    else if (currentInteractable.isCraftingSoldier)
                    {
                        // Genera un prefab desde un spawner específico
                        currentInteractable.StoreSoldier();
                    }
                }
            }
        }

        if (Input.GetKeyUp(KeyCode.E) && currentInteractable != null)
        {
            if (currentInteractable.isResourceNode)
            {
                currentInteractable.StopGathering();
            }
            currentInteractable = null;
        }
    }

private void OnDrawGizmosSelected()
    {
        // Visualizar el radio de interacción
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 1f);
    }
}

