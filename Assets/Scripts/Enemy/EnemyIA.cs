
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Linq;

public class EnemyAI : MonoBehaviour
{
    [System.Serializable]
    public enum AIState
    {
        Idle,
        Patrol,
        Chase,
        Lost
    }

    [Header("Componentes")]
    [SerializeField] private NavMeshAgent navMeshAgent;
    [SerializeField] private Transform playerTransform;

    // ANIMAÇÃO: Referência ao componente Animator para controlar as animações do inimigo
    [SerializeField] private Animator animator;

    [Header("Configurações de Visão")]
    [SerializeField] private float fieldOfViewDistance = 15f;
    [SerializeField] private float fieldOfViewAngle = 45f;
    [SerializeField] private LayerMask visionObstacleMask;

    [Header("Configurações de Patrulha")]
    [SerializeField] private string navigationPointTag = "NavigationPoint";
    [SerializeField] private float patrolWaitTime = 2f;
    [SerializeField] private float maxDistanceToPoint = 10f;
    [SerializeField] private float pointSearchRadius = 50f;

    [Header("Configurações de Perseguição")]
    [SerializeField] private float chaseSpeed = 3f;
    [SerializeField] private float patrolSpeed = 1.5f;
    [SerializeField] private float lostTargetTimeout = 3f;

    private List<Transform> patrolPoints = new List<Transform>();
    private int currentPatrolIndex = -1;
    private float patrolWaitTimer = 0f;
    private AIState currentState = AIState.Idle;
    private AIState previousState = AIState.Idle;
    private float lostTargetTimer = 0f;

    private void Start()
    {
        if (navMeshAgent == null)
            navMeshAgent = GetComponent<NavMeshAgent>();

        if (playerTransform == null)
            playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;

        // ANIMAÇÃO: Se não houver Animator atribuído no Inspector, tenta pegar do gameObject
       // if (animator == null)
            //animator = GetComponent<Animator>();

        InitializePatrolPoints();
        ChangeState(AIState.Patrol);
    }

    private void Update()
    {
        if (playerTransform == null || navMeshAgent == null)
            return;

        bool playerInSight = IsPlayerInFieldOfView();

        if (playerInSight)
        {
            lostTargetTimer = 0f;
            ChangeState(AIState.Chase);
        }
        else if (currentState == AIState.Chase)
        {
            lostTargetTimer += Time.deltaTime;
            if (lostTargetTimer >= lostTargetTimeout)
            {
                ChangeState(AIState.Lost);
            }
        }

        switch (currentState)
        {
            case AIState.Patrol:
                HandlePatrolState();
                break;
            case AIState.Chase:
                HandleChaseState();
                break;
            case AIState.Lost:
                HandleLostState();
                break;
        }

        // ANIMAÇÃO: Atualiza os parâmetros do Animator com os valores atuais
       // UpdateAnimator();
    }

    private void InitializePatrolPoints()
    {
        patrolPoints.Clear();
        GameObject[] allPoints = GameObject.FindGameObjectsWithTag(navigationPointTag);

        Vector3 myPosition = transform.position;

        foreach (GameObject point in allPoints)
        {
            if (Vector3.Distance(point.transform.position, myPosition) <= pointSearchRadius)
            {
                if (IsPointOnSameArea(myPosition, point.transform.position))
                {
                    patrolPoints.Add(point.transform);
                }
            }
        }

        if (patrolPoints.Count > 0)
        {
            currentPatrolIndex = 0;
        }
    }

    private bool IsPointOnSameArea(Vector3 posA, Vector3 posB)
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(posA, out hit, 2f, NavMesh.AllAreas) &&
            NavMesh.SamplePosition(posB, out hit, 2f, NavMesh.AllAreas))
        {
            return true;
        }
        return false;
    }

    private void HandlePatrolState()
    {
        if (patrolPoints.Count == 0)
        {
            navMeshAgent.velocity = Vector3.zero;
            return;
        }

        navMeshAgent.speed = patrolSpeed;

        if (navMeshAgent.remainingDistance <= maxDistanceToPoint && !navMeshAgent.hasPath || navMeshAgent.velocity.sqrMagnitude == 0f)
        {
            patrolWaitTimer += Time.deltaTime;

            if (patrolWaitTimer >= patrolWaitTime)
            {
                patrolWaitTimer = 0f;
                MoveToNextPatrolPoint();
            }
        }
    }

    private void HandleChaseState()
    {
        navMeshAgent.speed = chaseSpeed;
        navMeshAgent.SetDestination(playerTransform.position);
    }

    private void HandleLostState()
    {
        navMeshAgent.speed = patrolSpeed;

        if (!navMeshAgent.hasPath || navMeshAgent.remainingDistance <= maxDistanceToPoint)
        {
            ChangeState(AIState.Patrol);
        }
    }

    private bool IsPlayerInFieldOfView()
    {
        if (playerTransform == null)
            return false;

        Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer > fieldOfViewDistance)
            return false;

        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        if (angleToPlayer > fieldOfViewAngle / 2f)
            return false;

        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, directionToPlayer, distanceToPlayer, visionObstacleMask))
            return false;

        return true;
    }

    private void MoveToNextPatrolPoint()
    {
        if (patrolPoints.Count == 0)
            return;

        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Count;
        navMeshAgent.SetDestination(patrolPoints[currentPatrolIndex].position);
    }

    private void ChangeState(AIState newState)
    {
        if (currentState == newState)
            return;

        previousState = currentState;
        currentState = newState;

        switch (newState)
        {
            case AIState.Patrol:
                patrolWaitTimer = 0f;
                if (currentPatrolIndex < 0 && patrolPoints.Count > 0)
                {
                    currentPatrolIndex = 0;
                }
                if (currentPatrolIndex >= 0)
                {
                    navMeshAgent.SetDestination(patrolPoints[currentPatrolIndex].position);
                }
                break;

            case AIState.Chase:
                break;

            case AIState.Lost:
                if (currentPatrolIndex < 0 && patrolPoints.Count > 0)
                {
                    MoveToNextPatrolPoint();
                }
                break;
        }
    }

    /// <summary>
    /// ANIMAÇÃO: Este método atualiza os parâmetros do Animator
    /// Para usar: Crie um parâmetro Float chamado "Speed" e um Integer chamado "State" no seu Animator
    /// Speed: Velocidade de movimento do inimigo (0 = parado, 5+ = correndo)
    /// State: Estado atual do inimigo (0=Idle, 1=Patrol, 2=Chase, 3=Lost)
    /// </summary>
    private void UpdateAnimator()
    {
        // ANIMAÇÃO: Verifica se o Animator existe antes de usar
        //if (animator == null)
        //    return;

        // ANIMAÇÃO: Calcula a velocidade atual do movimento (magnitude do vetor de velocidade)
        //float speed = navMeshAgent.velocity.magnitude;

        // ANIMAÇÃO: Envia a velocidade para o Animator controlar o blend de animações (parado/caminhando/correndo)
       // animator.SetFloat("Speed", speed);

        // ANIMAÇÃO: Envia o estado atual como inteiro para o Animator controlar qual animação deve tocar
        //animator.SetInteger("State", (int)currentState);
    }

    public AIState GetCurrentState()
    {
        return currentState;
    }

    public void AddPatrolPoint(Transform point)
    {
        if (point != null && !patrolPoints.Contains(point))
        {
            patrolPoints.Add(point);
        }
    }

    public void RemovePatrolPoint(Transform point)
    {
        patrolPoints.Remove(point);
    }

    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying)
            return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, fieldOfViewDistance);

        Vector3 fovLineA = Quaternion.AngleAxis(fieldOfViewAngle / 2f, transform.up) * transform.forward * fieldOfViewDistance;
        Vector3 fovLineB = Quaternion.AngleAxis(-fieldOfViewAngle / 2f, transform.up) * transform.forward * fieldOfViewDistance;

        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + fovLineA);
        Gizmos.DrawLine(transform.position, transform.position + fovLineB);

        if (patrolPoints.Count > 0)
        {
            Gizmos.color = Color.blue;
            foreach (Transform point in patrolPoints)
            {
                if (point != null)
                {
                    Gizmos.DrawWireCube(point.position, Vector3.one * 0.5f);
                }
            }
        }

        if (currentPatrolIndex >= 0 && currentPatrolIndex < patrolPoints.Count && patrolPoints[currentPatrolIndex] != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, patrolPoints[currentPatrolIndex].position);
        }

        if (playerTransform != null && IsPlayerInFieldOfView())
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, playerTransform.position);
        }
    }
}