using UnityEngine;
using Photon.Pun;
using TMPro;
using System;

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
        if(teamTag == "A" && currentHealth <= 0)
        {
            UIManager.Instance.FortressDestroyedA();
        }
        else if (teamTag == "B" && currentHealth <= 0)
        {
            UIManager.Instance.FortressDestroyedB();
        }
    }

    // Llamar a la función de daño sincronizada
    public void ApplyDamage(int damage)
    {
        photonView.RPC("TakeDamage", RpcTarget.All, damage);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("A") || collision.gameObject.CompareTag("B")) // Si colisiona con un soldado
        {
            Debug.Log("pegue");
            TakeDamage(10); // Inflige 10 puntos de daño

            Destroy(collision.gameObject);
        }
    }

    public float GetCurrentLife()
    {
        return currentHealth;
    }
}

