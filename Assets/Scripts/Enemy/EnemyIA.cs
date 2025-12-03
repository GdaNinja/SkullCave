using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    [System.Serializable]
    public enum AIState
    {
        Idle,
        Patrol,
        Chase,
        Lost,
        JumpScareImage
    }

    [Header("Componentes")]
    [SerializeField] private NavMeshAgent navMeshAgent;
    [SerializeField] private Transform playerTransform;

    [Header("Animação")]
    [Tooltip("Animator do inimigo - será buscado automaticamente se não atribuído")]
    [SerializeField] private Animator animator;

    [Header("Configurações de Visão")]
    [SerializeField] private float fieldOfViewDistance = 15f;
    [SerializeField] private float fieldOfViewAngle = 45f;
    [SerializeField] private LayerMask visionObstacleMask;

    [Header("Configurações de Patrulha")]
    [SerializeField] private string navigationPointTag = "NavigationPoint";
    [SerializeField] private float patrolWaitTime = 2f;
    [SerializeField] private float maxDistanceToPoint = 2f; // CORRIGIDO: Era 10, agora é 2
    [SerializeField] private float pointSearchRadius = 50f;

    [Header("Configurações de Perseguição")]
    [SerializeField] private float chaseSpeed = 3f;
    [SerializeField] private float patrolSpeed = 1.5f;
    [SerializeField] private float lostTargetTimeout = 3f;

    [Header("Configurações de JumpScare")]
    [Tooltip("NÃO PREENCHER - Será encontrado automaticamente via JumpScareManager")]
    public AudioSource JumpScareSound;
    [Tooltip("NÃO PREENCHER - Será encontrado automaticamente via JumpScareManager")]
    public GameObject JumpScareImage;
    [SerializeField] private float jumpScareWaitTime = 5f;

    private List<Transform> patrolPoints = new List<Transform>();
    private int currentPatrolIndex = -1;
    private float patrolWaitTimer = 0f;
    private AIState currentState = AIState.Idle;
    private AIState previousState = AIState.Idle;
    private float lostTargetTimer = 0f;
    private bool patrolPointsInitialized = false; // NOVO: Flag para evitar reinicialização

    private void Start()
    {
        // Inicializa o NavMeshAgent
        if (navMeshAgent == null)
            navMeshAgent = GetComponent<NavMeshAgent>();

        // Busca o player na cena
        if (playerTransform == null)
            playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;

        // ANIMAÇÃO: Busca o Animator automaticamente se não foi atribuído
        if (animator == null)
        {
            animator = GetComponent<Animator>();
            if (animator == null)
            {
                // Tenta buscar em objetos filhos (caso o modelo 3D esteja em um filho)
                animator = GetComponentInChildren<Animator>();
            }
            
            if (animator == null)
            {
                Debug.LogWarning("[EnemyAI] Animator não encontrado! As animações não funcionarão.");
            }
            else
            {
                Debug.Log("[EnemyAI] Animator encontrado automaticamente!");
            }
        }

        // Busca automaticamente as referências do JumpScare via Manager
        JumpScareManager manager = JumpScareManager.Instance;
        if (manager != null)
        {
            JumpScareImage = manager.jumpScareImage;
            JumpScareSound = manager.jumpScareSound;
        }
        else
        {
            Debug.LogError("[EnemyAI] JumpScareManager não encontrado na cena! Adicione o script JumpScareManager.");
        }

        // Inicializa os pontos de patrulha
        InitializePatrolPoints();
        
        // Inicia o estado de patrulha
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

        // ANIMAÇÃO: Atualiza as animações a cada frame
        UpdateAnimator();
    }

    private void InitializePatrolPoints()
    {
        // CORREÇÃO: Só inicializa uma vez para evitar mudanças na lista durante o jogo
        if (patrolPointsInitialized && patrolPoints.Count > 0)
        {
            Debug.Log("[EnemyAI] Pontos de patrulha já foram inicializados.");
            return;
        }

        patrolPoints.Clear();
        GameObject[] allPoints = GameObject.FindGameObjectsWithTag(navigationPointTag);

        if (allPoints.Length == 0)
        {
            Debug.LogWarning($"[EnemyAI] Nenhum ponto com tag '{navigationPointTag}' foi encontrado!");
            return;
        }

        Vector3 myPosition = transform.position;

        // MELHORIA: Ordena os pontos por distância antes de adicionar
        List<GameObject> sortedPoints = allPoints
            .Where(point => Vector3.Distance(point.transform.position, myPosition) <= pointSearchRadius)
            .Where(point => IsPointOnSameArea(myPosition, point.transform.position))
            .OrderBy(point => Vector3.Distance(point.transform.position, myPosition))
            .ToList();

        foreach (GameObject point in sortedPoints)
        {
            patrolPoints.Add(point.transform);
        }

        if (patrolPoints.Count > 0)
        {
            currentPatrolIndex = 0;
            patrolPointsInitialized = true;
            Debug.Log($"[EnemyAI] {patrolPoints.Count} pontos de patrulha encontrados e ordenados!");
        }
        else
        {
            Debug.LogWarning("[EnemyAI] Nenhum ponto de patrulha válido foi encontrado próximo ao inimigo!");
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

        // CORREÇÃO: Verifica se está perto do ponto de destino com mais precisão
        float distanceToTarget = Vector3.Distance(transform.position, patrolPoints[currentPatrolIndex].position);
        
        // Se chegou no ponto (distância menor que maxDistanceToPoint)
        if (distanceToTarget <= maxDistanceToPoint)
        {
            // Incrementa o timer de espera
            patrolWaitTimer += Time.deltaTime;

            // Se esperou tempo suficiente, vai para o próximo ponto
            if (patrolWaitTimer >= patrolWaitTime)
            {
                patrolWaitTimer = 0f;
                MoveToNextPatrolPoint();
            }
        }
        else
        {
            // NOVO: Se ainda não chegou, garante que está indo para o ponto correto
            // Isso evita que ele "perca" o destino no meio do caminho
            if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance > maxDistanceToPoint)
            {
                // Se o caminho foi calculado mas ele está parado/travado, recalcula
                if (navMeshAgent.velocity.sqrMagnitude < 0.01f && navMeshAgent.hasPath)
                {
                    navMeshAgent.SetDestination(patrolPoints[currentPatrolIndex].position);
                }
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

        // Move para o próximo índice (loop circular)
        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Count;
        
        // MELHORIA: Log para debug
        Debug.Log($"[EnemyAI] Indo para ponto {currentPatrolIndex + 1}/{patrolPoints.Count}: {patrolPoints[currentPatrolIndex].name}");
        
        // Define o destino
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
                if (currentPatrolIndex >= 0 && currentPatrolIndex < patrolPoints.Count)
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
    /// Para usar: Crie 3 parâmetros BOOL no seu Animator:
    /// - "isIdle": true quando parado nos pontos
    /// - "isWalking": true quando patrulhando
    /// - "isRunning": true quando perseguindo o player
    /// </summary>
    private void UpdateAnimator()
    {
        // ANIMAÇÃO: Verifica se o Animator existe antes de usar
        if (animator == null)
            return;

        // ANIMAÇÃO: Calcula a velocidade atual do movimento
        float currentSpeed = navMeshAgent.velocity.magnitude;
        
        // Define um threshold para considerar que está parado
        bool isMoving = currentSpeed > 0.1f;

        // ANIMAÇÃO: Reseta todos os estados para false
        animator.SetBool("isIdle", false);
        animator.SetBool("isWalking", false);
        animator.SetBool("isRunning", false);

        // ANIMAÇÃO: Define qual animação deve tocar baseado no estado atual
        switch (currentState)
        {
            case AIState.Idle:
                // Parado
                animator.SetBool("isIdle", true);
                break;

            case AIState.Patrol:
                // Patrulhando
                if (isMoving)
                {
                    animator.SetBool("isWalking", true);
                }
                else
                {
                    // Parado no ponto esperando
                    animator.SetBool("isIdle", true);
                }
                break;

            case AIState.Chase:
                // Perseguindo (correndo)
                animator.SetBool("isRunning", true);
                break;

            case AIState.Lost:
                // Voltando para patrulha (andando)
                if (isMoving)
                {
                    animator.SetBool("isWalking", true);
                }
                else
                {
                    animator.SetBool("isIdle", true);
                }
                break;
        }
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

    /// <summary>
    /// Executa o jumpscare: ativa a imagem, toca o som e carrega a cena de Game Over
    /// </summary>
    public void PlayJumpScareImage()
    {
        Debug.Log("[EnemyAI] PlayJumpScareImage chamado!");
        
        // Usa o JumpScareManager para garantir que sempre funcione
        JumpScareManager manager = JumpScareManager.Instance;
        if (manager != null)
        {
            manager.TriggerJumpScare();
        }
        else
        {
            // Fallback para o método antigo
            if (JumpScareImage != null)
            {
                JumpScareImage.SetActive(true);
                Debug.Log("[EnemyAI] JumpScareImage ativado!");
            }
            else
            {
                Debug.LogError("[EnemyAI] JumpScareImage não foi atribuído!");
            }

            if (JumpScareSound != null)
            {
                JumpScareSound.Play();
                Debug.Log("[EnemyAI] JumpScareSound tocando!");
            }
            else
            {
                Debug.LogWarning("[EnemyAI] JumpScareSound não foi atribuído!");
            }

            StartCoroutine(LoadSceneOnSeconds(jumpScareWaitTime));
        }
    }

    private IEnumerator LoadSceneOnSeconds(float time)
    {
        Debug.Log($"[EnemyAI] Aguardando {time} segundos para carregar GameOver...");
        yield return new WaitForSeconds(time);
        Debug.Log("[EnemyAI] Carregando cena GameOver...");
        SceneManager.LoadScene("GameOver");
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
    private void OnCollisionEnter(Collision collision)
{
    if (collision.gameObject.CompareTag("Player"))
    {
        Debug.Log("[EnemyAI] Colidiu com o player!");
        PlayJumpScareImage();
    }
}
}