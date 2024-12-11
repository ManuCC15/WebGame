using System.Collections;
using UnityEngine;
using Photon.Pun;

public class Archer : MonoBehaviourPun
{
    public GameObject projectilePrefab;  // Prefab del proyectil
    public Transform firePoint;          // Punto desde donde se disparan los proyectiles
    public float attackRange = 5f;       // Rango de ataque
    public string teamTag;               // Etiqueta del equipo del arquero ("TeamA" o "TeamB")
    public int maxBattlePhases = 2;      // M�ximo de fases antes de destruir al arquero

    private bool canAttack = true;
    private int battlePhasesParticipated = 0;
    private Animator animator;           // Referencia al componente Animator

    void Start()
    {
        animator = GetComponent<Animator>();

        PhotonGameManager.Instance.OnBattlePhaseStart += OnBattlePhaseStart;  // Subscribirse al evento
    }

    void OnDestroy()
    {
        PhotonGameManager.Instance.OnBattlePhaseStart -= OnBattlePhaseStart;  // Desubscribirse del evento
    }

    void Update()
    {
        if (canAttack)
        {
            LookForEnemies();
        }
    }

    // Verificar si hay enemigos cerca para atacar
    private void LookForEnemies()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, attackRange);

        foreach (var enemy in hitEnemies)
        {
            if (enemy.CompareTag(teamTag)) continue;  // No atacar a aliados

            Soldier enemySoldier = enemy.GetComponent<Soldier>();
            if (enemySoldier != null)
            {
                StartCoroutine(Attack(enemySoldier.transform));
                return;
            }
        }
    }

    // Disparar un proyectil hacia un enemigo
    private IEnumerator Attack(Transform target)
    {
        if (!canAttack) yield break;

        canAttack = false;

        // Activar la animaci�n de disparo
        animator.SetTrigger("Shoot");

        // Esperar hasta que termine la animaci�n
        //yield return new WaitForSeconds(GetShootAnimationDuration());
        yield return new WaitUntil(() => !canAttack);

        // Instanciar el proyectil despu�s de la animaci�n
        GameObject projectile = PhotonNetwork.Instantiate(projectilePrefab.name, firePoint.position, Quaternion.identity);
        Projectile projectileScript = projectile.GetComponent<Projectile>();
        projectileScript.Initialize(target, teamTag == "A" ? "B" : "A");

        //canAttack = true;
    }

    public void OnShootAnimationEnd()
    {
        canAttack = true;  // Permite el siguiente disparo
    }

    // M�todo para verificar si la animaci�n de disparo ha terminado
    //private float GetShootAnimationDuration()
    //{
    //    // Obtener la duraci�n de la animaci�n "Shoot" desde el Animator
    //    AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
    //    if (stateInfo.IsName("ShootArcherA"))
    //    {
    //        return stateInfo.length;
    //    }
    //    return 0f;  // Si no se est� ejecutando "Shoot", no se espera nada
    //}

    // Evento que se llama al inicio de una fase de batalla
    private void OnBattlePhaseStart()
    {
        battlePhasesParticipated++;

        Debug.Log($"{gameObject.name}: Fases de batalla participadas: {battlePhasesParticipated}");

        if (battlePhasesParticipated >= maxBattlePhases)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Visualizar el rango de ataque
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}



