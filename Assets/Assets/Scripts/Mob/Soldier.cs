using System.Collections;
using UnityEngine;
using Photon.Pun;

public class Soldier : MonoBehaviour
{
    public int speed;
    public int minDamage = 5; // Daño mínimo
    public int maxDamage = 15; // Daño máximo
    public int maxHealth = 100; // Salud máxima
    private int currentHealth;

    public string teamTag; // Etiqueta del equipo (TeamA o TeamB)

    private PhotonView photonView;

    void Start()
    {
        photonView = GetComponent<PhotonView>();
        currentHealth = maxHealth;
    }

    void Update()
    {
        transform.position += new Vector3(-speed * Time.deltaTime, 0f, 0f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Colisión con el castillo
        if (collision.gameObject.CompareTag("Castle"))
        {
            if (teamTag == "TeamB")
            {
                // Lógica para dañar el castillo del equipo A
                Debug.Log("Soldado de TeamB dañó el castillo de TeamA.");
            }
        }
        else if (collision.gameObject.CompareTag("Castle2"))
        {
            if (teamTag == "TeamA")
            {
                // Lógica para dañar el castillo del equipo B
                Debug.Log("Soldado de TeamA dañó el castillo de TeamB.");
            }
        }
        // Colisión con otro soldado
        else if (collision.gameObject.CompareTag("TeamA") || collision.gameObject.CompareTag("TeamB"))
        {
            if (collision.gameObject.CompareTag(teamTag)) return; // Ignorar si son del mismo equipo

            // Iniciar combate
            Soldier enemySoldier = collision.GetComponent<Soldier>();
            if (enemySoldier != null)
            {
                Debug.Log("ataque"+teamTag);

                StartCoroutine(Attack(enemySoldier));
            }
        }
    }

    private IEnumerator Attack(Soldier enemySoldier)
    {
        while (enemySoldier != null && enemySoldier.currentHealth > 0)
        {
            int damage = Random.Range(5, 15); // Daño aleatorio
            enemySoldier.TakeDamage(damage);

            Debug.Log($"{gameObject.name} atacó a {enemySoldier.gameObject.name} con {damage} de daño.");

            yield return new WaitForSeconds(1f); // Intervalo entre golpes
        }
    }

    [PunRPC]
    public void TakeDamage(int damage)
    {
        if (!photonView.IsMine) return; // Asegúrate de que solo el propietario reduce la salud.

        currentHealth -= damage;
        Debug.Log($"{gameObject.name} recibió {damage} de daño. Salud restante: {currentHealth}");
        photonView.RPC("SyncHealth", RpcTarget.All, currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }



    [PunRPC]
    public void SyncHealth(int newHealth)
    {
        currentHealth = newHealth;
        Debug.Log($"La salud del soldado se sincronizó: {currentHealth}");
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} ha muerto.");
        PhotonNetwork.Destroy(gameObject);
    }
}

