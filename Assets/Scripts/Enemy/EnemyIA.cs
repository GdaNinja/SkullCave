using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyIA : MonoBehaviour
{
    [Header("IA Navigation Settings")]
    public float minDistanceToFollow = 1.2f;  // Distância mínima para seguir o jogador
    public float minDistanceToPoint = 1.0f;   // Distância mínima para considerar que chegou no ponto de patrulha
    public float timeToStartNavigation = 3.0f; // Tempo antes de voltar a patrulhar
    public float spdOfNavigation = 1.0f;      // Velocidade de patrulha
    public float spdOfFollow = 3.0f;          // Velocidade ao perseguir o jogador

    [Header("IA Vision Settings")]
    public Transform eyes;                    // Ponto de origem da visão (geralmente na cabeça)
    public float visionRadius = 10f;          // Distância máxima de visão
    public float visionAngle = 45f;           // Ângulo de visão
    public float timeOfNavigation = 3.0f;     // Tempo que o inimigo continua procurando após perder o jogador

    [Header("Animation Settings")]
    public string animFollow = "Follow";      // Nome do parâmetro de animação de perseguição
    public string animNavigation = "Navigation"; // Nome do parâmetro de animação de patrulha
    public string animIdle = "Idle";          // Nome do parâmetro de animação de idle
    public string animSpotPlayer = "SpotPlayer"; // Nome da animação de ver o player pela primeira vez
    public float spotAnimationDuration = 2f;  // Duração da animação de spotting antes de começar a perseguição

    private float timerNav;
    private int pointIndex;
    private bool canSeePlayer = false;
    private bool hasSpottedPlayer = false;    // Controle para não repetir a animação de spotting
    private bool isSpotting = false;          // Indica se está atualmente fazendo a animação de spotting

    private Animator anim;
    private Transform target;
    private NavMeshAgent navMesh;
    private GameObject[] navigationPoints;

    void Start()
    {
        anim = GetComponent<Animator>();
        navMesh = GetComponent<NavMeshAgent>();
        target = GameObject.FindGameObjectWithTag("Player").transform;

        // Encontra todos os pontos de patrulha
        navigationPoints = GameObject.FindGameObjectsWithTag("NavigationPoint");
        pointIndex = GetRandomPointIndex();
    }

    void Update()
    {
        // Timer reduz enquanto não vê o player
        timerNav = Mathf.Clamp(timerNav - Time.deltaTime, 0, timeOfNavigation);

        canSeePlayer = CanSeePlayer();

        if (isSpotting) return; // Pausa o comportamento enquanto faz animação de spotting

        if (canSeePlayer || timerNav > 0)
            Follow();
        else
            Patrol();
    }

    private bool CanSeePlayer()
    {
        if (target == null) return false;

        Vector3 directionToTarget = (target.position - eyes.position).normalized;

        // Verifica se o jogador está dentro do ângulo de visão
        if (Vector3.Angle(eyes.forward, directionToTarget) < visionAngle / 2)
        {
            RaycastHit hit;

            // Raycast para detectar o jogador, ignorando obstáculos
            if (Physics.Raycast(eyes.position, directionToTarget, out hit, visionRadius))
            {
                if (hit.transform.CompareTag("Player"))
                {
                    timerNav = timeOfNavigation; // Reseta o tempo de busca

                    // Se o inimigo acabou de ver o jogador pela primeira vez
                    if (!hasSpottedPlayer)
                    {
                        hasSpottedPlayer = true;
                        StartCoroutine(SpotPlayerRoutine());
                    }

                    return true;
                }
            }
        }

        // Se não está vendo o jogador mais, reseta o estado
        hasSpottedPlayer = false;
        return false;
    }

    private IEnumerator SpotPlayerRoutine()
    {
        isSpotting = true;
        navMesh.enabled = false;
        anim.SetTrigger(animSpotPlayer); // Toca animação de spotting

        yield return new WaitForSeconds(spotAnimationDuration);

        isSpotting = false;
        navMesh.enabled = true;
    }

    private void Follow()
    {
        if (isSpotting) return;

        navMesh.enabled = true;
        navMesh.speed = spdOfFollow;

        float distance = Vector3.Distance(transform.position, target.position);

        if (distance > minDistanceToFollow)
        {
            anim.SetBool(animFollow, true);
            anim.SetBool(animNavigation, false);
            anim.SetBool(animIdle, false);

            navMesh.SetDestination(target.position);
        }
        else
        {
            anim.SetBool(animFollow, false);
            anim.SetBool(animIdle, true);
            transform.LookAt(new Vector3(target.position.x, transform.position.y, target.position.z));
        }
    }

    private void Patrol()
    {
        if (isSpotting) return;

        navMesh.speed = spdOfNavigation;
        anim.SetBool(animFollow, false);
        anim.SetBool(animNavigation, true);
        anim.SetBool(animIdle, false);

        if (navigationPoints.Length == 0) return;

        float distance = Vector3.Distance(transform.position, navigationPoints[pointIndex].transform.position);

        if (distance > minDistanceToPoint)
        {
            navMesh.enabled = true;
            navMesh.SetDestination(navigationPoints[pointIndex].transform.position);
        }
        else
        {
            StartCoroutine(StartNavigation());
        }
    }

    private int GetRandomPointIndex()
    {
        if (navigationPoints.Length == 0) return 0;

        int i = UnityEngine.Random.Range(0, navigationPoints.Length);

        if (i == pointIndex)
            return GetRandomPointIndex();

        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            var otherIA = enemy.GetComponent<EnemyIA>();
            if (otherIA != null && otherIA.pointIndex == i)
                return GetRandomPointIndex();
        }

        return i;
    }

    private IEnumerator StartNavigation()
    {
        navMesh.enabled = false;
        anim.SetBool(animNavigation, false);

        yield return new WaitForSeconds(timeToStartNavigation);

        pointIndex = GetRandomPointIndex();
        navMesh.enabled = true;
        navMesh.SetDestination(navigationPoints[pointIndex].transform.position);
        anim.SetBool(animNavigation, true);
    }

    void OnDrawGizmosSelected()
    {
        if (eyes == null) return;

        Gizmos.color = canSeePlayer ? Color.blue : Color.red;
        Quaternion leftRayRotation = Quaternion.AngleAxis(-visionAngle / 2, transform.up);
        Quaternion rightRayRotation = Quaternion.AngleAxis(visionAngle / 2, transform.up);

        Vector3 leftRayDirection = leftRayRotation * transform.forward;
        Vector3 rightRayDirection = rightRayRotation * transform.forward;

        Gizmos.DrawRay(eyes.position, leftRayDirection * visionRadius);
        Gizmos.DrawRay(eyes.position, rightRayDirection * visionRadius);
    }
}
