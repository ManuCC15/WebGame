using UnityEngine;
using Photon.Pun;
using TMPro;

public class FortressHealth : MonoBehaviourPunCallbacks
{
    public int maxHealth = 100;
    private int currentHealth;

    public GameObject canvas;
    public TextMeshProUGUI canvasMensage;

    public TextMeshProUGUI teamHealthUI; // Referencia al texto en la UI para la vida
    public string teamTag; // Etiqueta que indica el equipo de la fortaleza (ejemplo: "TeamA" o "TeamB")

    void Start()
    {
        // Inicializar la vida
        currentHealth = maxHealth;
        UpdateHealthUI();

        canvas.SetActive(false);
    }

    // Función para perder vida
    [PunRPC]
    public void TakeDamage(int damage)
    {
        Debug.Log("saquevida");
        if (currentHealth <= 0) return;

        currentHealth = Mathf.Max(currentHealth - damage, 0); // Reducir la vida sin pasar de 0
        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            FortressDestroyed();
        }
    }

    // Actualizar la UI de vida
    private void UpdateHealthUI()
    {
        if (teamHealthUI != null)
        {
            teamHealthUI.text = $"{teamTag} Health: {currentHealth}/{maxHealth}";
        }
    }

    // Acción cuando la fortaleza es destruida
    private void FortressDestroyed()
    {
        Debug.Log($"{teamTag} Fortress has been destroyed!");
        // Aquí puedes añadir lógica adicional, como terminar el juego o notificar a los jugadores.
        canvas.SetActive(true);
        canvasMensage.text = $"{teamTag} has win the game";
    }

    // Llamar a la función de daño sincronizada
    public void ApplyDamage(int damage)
    {
        photonView.RPC("TakeDamage", RpcTarget.All, damage);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("A") || collision.gameObject.CompareTag("B"))
        {
            Debug.Log("Soldado alcanzó la fortaleza");

            Soldier soldier = collision.GetComponent<Soldier>();
            if (soldier != null && PhotonNetwork.IsMasterClient)
            {
                ApplyDamage(10); // Llama al método RPC para sincronizar el daño
                PhotonNetwork.Destroy(collision.gameObject); 
            }
            else
            {
                // Si no eres el MasterClient, solicita al MasterClient que destruya el objeto
                photonView.RPC("RequestDestroy", RpcTarget.MasterClient, collision.gameObject);
            }
        }
    }

    [PunRPC]
    void RequestDestroy(GameObject gO)
    {
        if (PhotonNetwork.IsMasterClient && photonView != null)
        {
            PhotonNetwork.Destroy(gO);
        }
    }
}

