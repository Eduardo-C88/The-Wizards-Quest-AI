using UnityEngine;
using UnityEngine.AI;

public class RangedEnemy : MonoBehaviour
{
    public GameObject projectilePrefab;      // Prefab do projetil (bola de neve)
    public Transform shootingPoint;          // Ponto de onde o projetil será lançado
    public float shootingRange = 10f;        // Distância para começar a atirar
    public float fireRate = 2f;              // Intervalo entre disparos
    private float nextFireTime = 0f;

    private NavMeshAgent agent;
    private Transform player;

    [Header("Enemy Stats")]
    public float range = 1000f;              // Distância máxima de perseguição ao jogador

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // Verifica se o NavMeshAgent está ativo e ajusta a velocidade
        if (agent != null)
        {
            agent.isStopped = false;
            agent.speed = 3.5f; // Define a velocidade base do inimigo
        }
    }

    void Update()
    {
        if (player == null || agent == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Se o jogador está dentro da distância máxima de perseguição
        if (distanceToPlayer <= range)
        {
            // Se o jogador está dentro do alcance de tiro, atira; caso contrário, persegue o jogador
            if (distanceToPlayer <= shootingRange)
            {
                agent.isStopped = true; // Para o inimigo para que ele possa atirar
                if (Time.time >= nextFireTime)
                {
                    ShootProjectile();
                    nextFireTime = Time.time + 1f / fireRate;
                }
            }
            else
            {
                // Continua perseguindo o jogador se ele estiver fora do alcance de tiro
                agent.isStopped = false;
                agent.SetDestination(player.position);
            }
        }
        else
        {
            // Para o inimigo se o jogador estiver fora do alcance de perseguição
            agent.isStopped = true;
        }
    }

    private void ShootProjectile()
    {
        GameObject projectile = Instantiate(projectilePrefab, shootingPoint.position, Quaternion.identity);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();

        // Define a direção horizontal e adiciona uma pequena componente vertical para criar um arco sutil
        Vector3 direction = (player.position - shootingPoint.position).normalized;

        // Ajusta a força do lançamento e uma leve elevação para o arco
        float launchForce = 40f;     // Velocidade horizontal do projétil
        float slightArc = 0.6f;        // Pequena elevação para o arco, ajuste conforme necessário

        // Combina a direção horizontal com uma leve elevação
        Vector3 launchVelocity = direction * launchForce + Vector3.up * slightArc;
        rb.velocity = launchVelocity; // Define a velocidade inicial do projétil
    }
}
