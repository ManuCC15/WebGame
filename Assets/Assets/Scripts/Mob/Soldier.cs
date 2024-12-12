using System.Collections;
using UnityEngine;
using Photon.Pun;


public class Soldier : MonoBehaviour
{
    public float speed = 5f;
    public int maxHealth = 100;
    [SerializeField]private int currentHealth;
    public float attackRange = 1.5f;  // Distancia a la que comienza el combate
    private float attackCooldown = 1f;
    private bool canAttack = true;
    public string teamTag;  // "TeamA" o "TeamB"

    private PhotonView photonView;
    public GameObject targetPosition;
    private bool isDead = false;

    private Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
        photonView = GetComponent<PhotonView>();
        //if (photonView != null)
        //{
        //    // Si el objeto no tiene propietario (es huérfano), transfiere la propiedad al cliente actual
        //    if (photonView.Owner == null || photonView.Owner.IsInactive)
        //    {
        //        if (PhotonNetwork.IsMasterClient)
        //        {
        //            photonView.TransferOwnership(PhotonNetwork.LocalPlayer);
        //            Debug.Log($"{gameObject.name} no tenía propietario. Transferido al cliente actual.");
        //        }
        //    }
        //}

        currentHealth = maxHealth;

        if (photonView.IsMine)
        {
            Debug.Log($"{gameObject.name} es controlado por este cliente.");
        }
        else
        {
            Debug.Log($"{gameObject.name} NO es controlado por este cliente.");
        }
    }

    void Update()
    {
        if (!photonView.IsMine || isDead) return;  // No se mueve ni hace nada si está muerto

        // Movimiento hacia el punto objetivo
        MoveToTarget();

        // Si el soldado tiene un PhotonView y es el suyo propio, puede atacar
        if (photonView.IsMine && canAttack)
        {
            Debug.Log("entre1 - Está controlado por este cliente");
            LookForEnemies();
        }
    }

    // Mover al objetivo
    void MoveToTarget()
    {
        if (targetPosition != null && targetPosition.transform.position != transform.position)
        {
            Vector3 direction = (targetPosition.transform.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;
        }
        else
        {
            Debug.Log("No se ha asignado un objetivo o ya hemos llegado.");
        }
    }

    // Verificar si hay enemigos cerca para iniciar combate
    void LookForEnemies()
    {

        // Buscar un enemigo dentro del rango de ataque
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, attackRange);
        foreach (var enemy in hitEnemies)
        {
            Debug.Log($"Objeto detectado: {enemy.gameObject.name}");

            if (enemy.CompareTag(teamTag)) continue;  // Evitar atacar a aliados

            Soldier enemySoldier = enemy.GetComponent<Soldier>();
            if (enemySoldier != null && enemySoldier.teamTag != teamTag && canAttack)
            {
                Debug.Log("¡Enemigo encontrado! Iniciando ataque.");
                StartCoroutine(Attack(enemySoldier));
                break; // Solo ataca al primer enemigo detectado
            }
        }
    }

    // Iniciar un ataque contra un enemigo
    private IEnumerator Attack(Soldier enemySoldier)
    {
        if (enemySoldier == null || enemySoldier.isDead) yield break;

        canAttack = false;

        // Activar la animación de ataque
        animator.SetTrigger("Attacking");

        // Aplicar daño
        int damage = Random.Range(5, 15);
        enemySoldier.TakeDamage(damage);

        Debug.Log($"{gameObject.name} atacó a {enemySoldier.gameObject.name} con {damage} de daño.");

        yield return new WaitForSeconds(attackCooldown);
        //canAttack = true; 
    }


    // Recibir daño
    [PunRPC]
    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        photonView.RPC("SyncHealth", RpcTarget.All, currentHealth);
        Debug.Log($"{gameObject.name} recibió {damage} de daño. Salud restante: {currentHealth}");


    }

    public void OnAttackingAnimationEnd()
    {
        Debug.Log("Animación de ataque finalizada");
        canAttack = true;  // Permite el siguiente disparo
    }

    [PunRPC]
    public void SyncHealth(int newHealth)
    {
        currentHealth = newHealth;
        Debug.Log($"La salud de {gameObject.name} se sincronizó: {currentHealth}");
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Matar al soldado
    private void Die()
    {
        if (isDead) return; // Evita que el método se ejecute dos veces
        isDead = true;

        Debug.Log($"{gameObject.name} ha muerto.");

        // Asegúrate de que el MasterClient maneja la destrucción
        if (PhotonNetwork.IsMasterClient)
        {
            if (photonView != null && photonView.IsMine)
            {
                PhotonNetwork.Destroy(gameObject);
            }
        }
        else
        {
            // Si no eres el MasterClient, solicita al MasterClient que destruya el objeto
            photonView.RPC("RequestDestroy", RpcTarget.MasterClient);
        }
    }

    // RPC para que el MasterClient destruya el objeto
    [PunRPC]
    void RequestDestroy()
    {
        PhotonView photonView = GetComponent<PhotonView>();

        if (photonView.IsMine || PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }

}





