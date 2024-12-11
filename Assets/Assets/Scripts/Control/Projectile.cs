using UnityEngine;
using Photon.Pun;

public class Projectile : MonoBehaviourPun
{
    public float speed = 10f;               // Velocidad del proyectil
    public int minDamage = 5;              // Da�o m�nimo
    public int maxDamage = 15;             // Da�o m�ximo
    public float lifetime = 5f;            // Duraci�n del proyectil antes de destruirse

    private Transform target;              // Objetivo del proyectil
    private string enemyTeamTag;           // Etiqueta del equipo enemigo

    void Start()
    {
        // Destruir el proyectil autom�ticamente despu�s de un tiempo
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        if (target != null)
        {
            // Mover el proyectil hacia el objetivo
            Vector3 direction = (target.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;
        }
        else
        {
            // Si no hay objetivo, destruir el proyectil
            Destroy(gameObject);
        }
    }

    public void Initialize(Transform target, string enemyTag)
    {
        this.target = target;
        this.enemyTeamTag = enemyTag;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Si colisiona con un enemigo del equipo contrario
        if (other.CompareTag(enemyTeamTag))
        {
            Soldier enemy = other.GetComponent<Soldier>();
            if (enemy != null)
            {
                int damage = Random.Range(minDamage, maxDamage);
                enemy.TakeDamage(damage);
                Debug.Log($"Proyectil impact� a {enemy.name} y le caus� {damage} de da�o.");
            }

            // Destruir el proyectil al impactar
            Destroy(gameObject);
        }
    }
}

