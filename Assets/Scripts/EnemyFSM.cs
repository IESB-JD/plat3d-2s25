using UnityEngine;
using UnityEngine.AI;

public class EnemyFSM : MonoBehaviour
{
    [System.Serializable]
    public enum EnemyState
    {
        Idle,
        Patrol,
        Chase,
        Attack,
        Escape,
        Die
    }
    
    [Header("Estado Atual")]
    public EnemyState currentState = EnemyState.Idle;

    [Header("Configurações de Detecção")]
    [SerializeField] private float engageRange = 10f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float losePlayerRange = 15f;
    [SerializeField] private LayerMask playerLayerMask = 1;

    [Header("Configurações de Vida")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float escapeHealthPercentage = 0.25f;
    private float currentHealth;

    [Header("Configurações de Movimento")]
    [SerializeField] private float patrolRadius = 8f;
    [SerializeField] private float chaseSpeed = 6f;
    [SerializeField] private float patrolSpeed = 3f;
    [SerializeField] private float escapeSpeed = 8f;

    [Header("Configurações de Tempo")]
    [SerializeField] private float idleTime = 3f;
    [SerializeField] private float patrolWaitTime = 2f;
    [SerializeField] private float attackCooldown = 1.5f;

    [Header("Configurações de Ataque")]
    [SerializeField] private float attackDamage = 25f;
    [SerializeField] private float attackAnimationTime = 1f;

    // Componentes
    private NavMeshAgent agent;
    private Transform player;
    private Animator animator;

    // Variáveis de controle
    private float stateTimer;
    private Vector3 startingPosition;
    private Vector3 patrolDestination;
    private bool hasPatrolDestination;
    private float lastAttackTime;
    private bool isDead;

    // Propriedades públicas
    public float HealthPercentage => currentHealth / maxHealth;
    public bool IsPlayerInEngageRange => Vector3.Distance(transform.position, player.position) <= engageRange;

    public bool IsPlayerInAttackRange => Vector3.Distance(transform.position, player.position) <= attackRange;
    public bool ShouldEscape => HealthPercentage <= escapeHealthPercentage && !isDead;

    void Start()
    {
        // Inicialização dos componentes
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        
        // Encontrar o jogador
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;

        // Configurações iniciais
        currentHealth = maxHealth;
        startingPosition = transform.position;
        stateTimer = 0f;
        hasPatrolDestination = false;
        
        // Configurar NavMeshAgent
        if (agent != null)
        {
            agent.speed = patrolSpeed;
            agent.stoppingDistance = 0.5f;
        }

        // Iniciar no estado Idle
        ChangeState(EnemyState.Idle);
    }

    void Update()
    {
        if (isDead) return;

        // Verificar se deve escapar (prioridade máxima)
        if (ShouldEscape && currentState != EnemyState.Escape)
        {
            ChangeState(EnemyState.Escape);
            return;
        }

        // Atualizar timer do estado atual
        stateTimer += Time.deltaTime;

        // Executar lógica do estado atual
        switch (currentState)
        {
            case EnemyState.Idle:
                HandleIdleState();
                break;
            case EnemyState.Patrol:
                HandlePatrolState();
                break;
            case EnemyState.Chase:
                HandleChaseState();
                break;
            case EnemyState.Attack:
                HandleAttackState();
                break;
            case EnemyState.Escape:
                HandleEscapeState();
                break;
        }
    }

    private void HandleIdleState()
    {
        // Parar movimento
        if (agent != null)
            agent.SetDestination(transform.position);

        // Verificar se jogador entrou na área de engage
        if (player != null && IsPlayerInEngageRange)
        {
            ChangeState(EnemyState.Chase);
            return;
        }

        // Após tempo de idle, ir para patrol
        if (stateTimer >= idleTime)
        {
            ChangeState(EnemyState.Patrol);
        }
    }

    private void HandlePatrolState()
    {
        // Verificar se jogador entrou na área de engage
        if (player != null && IsPlayerInEngageRange)
        {
            ChangeState(EnemyState.Chase);
            return;
        }

        // Se não tem destino de patrol ou chegou no destino
        if (!hasPatrolDestination || (agent != null && agent.remainingDistance < 0.5f))
        {
            if (stateTimer >= patrolWaitTime)
            {
                SetRandomPatrolDestination();
            }
        }
    }

    private void HandleChaseState()
    {
        if (player == null)
        {
            ChangeState(EnemyState.Idle);
            return;
        }

        // Se jogador está muito longe, voltar para idle
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer > losePlayerRange)
        {
            ChangeState(EnemyState.Idle);
            return;
        }

        // Se jogador está na faixa de ataque
        if (IsPlayerInAttackRange)
        {
            ChangeState(EnemyState.Attack);
            return;
        }

        // Perseguir jogador
        if (agent != null)
        {
            agent.SetDestination(player.position);
        }
    }

    private void HandleAttackState()
    {
        if (player == null)
        {
            ChangeState(EnemyState.Idle);
            return;
        }

        // Parar movimento durante ataque
        if (agent != null)
            agent.SetDestination(transform.position);

        // Se jogador saiu da faixa de ataque
        if (!IsPlayerInAttackRange)
        {
            ChangeState(EnemyState.Chase);
            return;
        }

        // Executar ataque se o cooldown passou
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            PerformAttack();
            lastAttackTime = Time.time;
        }

        // Voltar para chase após animação de ataque
        if (stateTimer >= attackAnimationTime)
        {
            ChangeState(EnemyState.Chase);
        }
    }

    private void HandleEscapeState()
    {
        // Fugir do jogador
        if (player != null && agent != null)
        {
            Vector3 escapeDirection = (transform.position - player.position).normalized;
            Vector3 escapeDestination = transform.position + escapeDirection * patrolRadius;
            
            // Verificar se a posição é válida no NavMesh
            NavMeshHit hit;
            if (NavMesh.SamplePosition(escapeDestination, out hit, patrolRadius, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
            }
        }

        // Se recuperou vida suficiente, voltar para chase se jogador ainda estiver próximo
        if (!ShouldEscape)
        {
            if (player != null && IsPlayerInEngageRange)
                ChangeState(EnemyState.Chase);
            else
                ChangeState(EnemyState.Idle);
        }
    }

    private void ChangeState(EnemyState newState)
    {
        // Sair do estado atual
        OnExitState(currentState);

        // Mudar para novo estado
        currentState = newState;
        stateTimer = 0f;

        // Entrar no novo estado
        OnEnterState(newState);
    }

    private void OnEnterState(EnemyState state)
    {
        switch (state)
        {
            case EnemyState.Idle:
                if (agent != null)
                    agent.speed = patrolSpeed;
                SetAnimationTrigger("Idle");
                break;
            
            case EnemyState.Patrol:
                if (agent != null)
                    agent.speed = patrolSpeed;
                hasPatrolDestination = false;
                SetAnimationTrigger("Walk");
                break;
            
            case EnemyState.Chase:
                if (agent != null)
                    agent.speed = chaseSpeed;
                SetAnimationTrigger("Run");
                break;
            
            case EnemyState.Attack:
                SetAnimationTrigger("Attack");
                break;
            
            case EnemyState.Escape:
                if (agent != null)
                    agent.speed = escapeSpeed;
                SetAnimationTrigger("Run");
                break;
        }
    }

    private void OnExitState(EnemyState state)
    {
        // Lógica específica ao sair de cada estado
        switch (state)
        {
            case EnemyState.Attack:
                // Reset do trigger de ataque
                if (animator != null)
                    animator.ResetTrigger("Attack");
                break;
        }
    }

    private void SetRandomPatrolDestination()
    {
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        randomDirection += startingPosition;
        randomDirection.y = transform.position.y; // Manter na mesma altura

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, patrolRadius, NavMesh.AllAreas))
        {
            patrolDestination = hit.position;
            if (agent != null)
            {
                agent.SetDestination(patrolDestination);
                hasPatrolDestination = true;
            }
        }
    }

    private bool CanSeePlayer()
    {
        if (player == null) return false;

        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Raycast para verificar se há obstáculos
        RaycastHit hit;
        if (Physics.Raycast(transform.position, directionToPlayer, out hit, distanceToPlayer))
        {
            return hit.collider.CompareTag("Player");
        }

        return false;
    }

    private void PerformAttack()
    {
        if (player == null) return;

        // Aplicar dano ao jogador (assumindo que tem um componente de saúde)
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(attackDamage);
        }

        Debug.Log($"Inimigo atacou o jogador causando {attackDamage} de dano!");
    }

    private void SetAnimationTrigger(string triggerName)
    {
        if (animator != null)
        {
            animator.SetTrigger(triggerName);
        }
    }

    // Método público para receber dano
    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Método público para curar
    public void Heal(float healAmount)
    {
        if (isDead) return;

        currentHealth += healAmount;
        currentHealth = Mathf.Min(maxHealth, currentHealth);
    }

    private void Die()
    {
        isDead = true;
        currentState = EnemyState.Die;
        
        if (agent != null)
        {
            agent.enabled = false;
        }

        SetAnimationTrigger("Die");
        
        // Desabilitar colliders
        Collider[] colliders = GetComponents<Collider>();
        foreach (Collider col in colliders)
        {
            col.enabled = false;
        }

        Debug.Log("Inimigo morreu!");
    }

    // Gizmos para visualizar as áreas no editor
    void OnDrawGizmosSelected()
    {
        // Área de Engage (amarela)
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, engageRange);

        // Área de Ataque (vermelha)
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, attackRange);

        // Área de Patrol (azul)
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(startingPosition, patrolRadius);

        // Linha para o destino de patrol
        if (hasPatrolDestination)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, patrolDestination);
            Gizmos.DrawSphere(patrolDestination, 0.5f);
        }
    }
}