using UnityEngine;
using Metroidvania.Combat;
using Metroidvania.Characters;
using Metroidvania.Entities;
using System.Collections;

public class BatBehaviour : MonoBehaviour, IHittableTarget
{
    [Header("Stats")]
    [SerializeField] private float maxLife = 10f;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float chaseSpeed = 4.5f;
    [SerializeField] private float attackDistance = 1f;
    [SerializeField] private float waitDuration = 1.5f;

    [Header("Attack")]
    [SerializeField] private int attack1Damage = 1;
    [SerializeField] private float attack1Force = 2f;
    [SerializeField] private float retreat1Force = 1f;
    [SerializeField] private float attackDelay = 0.3f;
    [SerializeField] private int attack2Damage = 1;
    [SerializeField] private float attack2Force = 3f;
    [SerializeField] private float retreat2Force = 2f;
    [SerializeField] private Vector2 attackBoxSize = new Vector2(1f, 1f);
    [SerializeField] private Vector2 attackBoxOffset = new Vector2(1f, 0f);
    [SerializeField] private LayerMask attackLayerMask;
    [SerializeField] private float preAttackDelay = 0.5f;

    [Header("Detection")]
    [SerializeField] private float detectionRadius = 6f;
    [SerializeField] private LayerMask charactersLayer;

    [Header("Patrol Points")]
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;

    [Header("Components")]
    [SerializeField] private Animator anim;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Collider2D bodyCollider;
    [SerializeField] private CharacterBase characterBase;

    [Header("Health Bar")]
    [SerializeField] private GameObject healthBarPrefab;
    private EnemyHealthBar healthBarInstance;

    private Transform target;
    private Transform patrolTarget;
    private float life;
    private int facing = 1;
    private float waitTimer;
    private float attackTimer;
    private bool firstAttackDone, secondAttackDone;
    private float preAttackTimer;
    private bool preAttackStarted;
    private bool isHurt = false;


    private enum State { Patrol, Chase, Attack, Wait }
    private State state = State.Patrol;

    private int currentAnimState = -1;

    private void SetAnimState(int value)
    {
        if (currentAnimState == value) return;
        currentAnimState = value;
        anim.SetInteger("State", value);
    }

private void Awake()
{
    if (!rb) rb = GetComponent<Rigidbody2D>();
    if (!anim) anim = GetComponent<Animator>();
    if (!bodyCollider) bodyCollider = GetComponent<Collider2D>();
    if (!characterBase) characterBase = GetComponent<CharacterBase>();

    rb.gravityScale = 0;
    rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    life = maxLife;
    patrolTarget = pointA;

    if (healthBarPrefab != null)
    {
        GameObject hb = Instantiate(healthBarPrefab, transform);
        hb.transform.localPosition = new Vector3(0, 0.8f, 0); // Adjust height as needed
        healthBarInstance = hb.GetComponent<EnemyHealthBar>();
        if (healthBarInstance != null)
        {
            healthBarInstance.Initialize(maxLife);
            healthBarInstance.SetHealth(life);
            Debug.Log("Health bar instantiated and initialized for Bat.");
        }
        else
        {
            Debug.LogError("Health bar prefab does not have EnemyHealthBar component.");
        }
    }
    else
    {
        Debug.LogError("Health bar prefab is not assigned in BatBehaviour.");
    }
}

    private void FixedUpdate()
    {
        if (isHurt)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        DetectPlayer();
        StateMachine();
        UpdateFacingDirection();
    }

    private void StateMachine()
    {
        switch (state)
        {
            case State.Patrol:
                Patrol();
                if (target) state = State.Chase;
                break;

            case State.Chase:
                if (!target)
                {
                    state = State.Patrol;
                    return;
                }

                MoveTowards(target.position, chaseSpeed);
                if (Vector2.Distance(transform.position, target.position) <= attackDistance)
                {
                    attackTimer = 0;
                    firstAttackDone = false;
                    secondAttackDone = false;
                    preAttackStarted = false;
                    state = State.Attack;
                }
                break;

            case State.Attack:
                Attack();
                break;

            case State.Wait:
                rb.linearVelocity = Vector2.zero;
                SetAnimState(1); // Idle
                waitTimer -= Time.deltaTime;
                if (waitTimer <= 0)
                {
                    state = State.Patrol;
                    patrolTarget = GetClosestPatrolPoint();
                }
                break;
        }
    }

    private void Patrol()
    {
        SetAnimState(0); // Fly
        if (Vector2.Distance(transform.position, patrolTarget.position) < 0.3f)
        {
            patrolTarget = patrolTarget == pointA ? pointB : pointA;
        }

        MoveTowards(patrolTarget.position, moveSpeed);
    }

    private void MoveTowards(Vector2 targetPos, float speed)
    {
        Vector2 dir = (targetPos - (Vector2)transform.position).normalized / 2;
        rb.linearVelocity = dir * speed;
    }

    private void UpdateFacingDirection()
    {
        if (Mathf.Abs(rb.linearVelocity.x) > 0.1f)
        {
            Flip((int)Mathf.Sign(rb.linearVelocity.x));
        }
        // Fix health bar scale to prevent flipping
        if (healthBarInstance != null)
        {
            Vector3 scale = healthBarInstance.transform.localScale;
            scale.x = Mathf.Abs(scale.x);
            healthBarInstance.transform.localScale = scale;
        }
    }

    // Removed LateUpdate to avoid redundant scaling updates and potential conflicts

    private void Flip(int dir)
    {
        if (dir != 0 && dir != facing)
        {
            facing = dir;
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * facing;
            transform.localScale = scale;
        }
    }

    private void DetectPlayer()
    {
        if (state == State.Attack || target) return;

        var hits = Physics2D.OverlapCircleAll(transform.position, detectionRadius, charactersLayer);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                target = hit.transform;
                break;
            }
        }
    }

    private void Attack()
    {
        if (isHurt)
        {
            // ���������� �����
            firstAttackDone = false;
            secondAttackDone = false;
            preAttackStarted = false;
            state = State.Wait;
            waitTimer = waitDuration;
            return;
        }

        if (!target)
        {
            state = State.Patrol;
            return;
        }

        if (!preAttackStarted)
        {
            preAttackStarted = true;
            preAttackTimer = preAttackDelay;
            SetAnimState(1); // Idle ����� ������
            return;
        }

        if (preAttackTimer > 0)
        {
            preAttackTimer -= Time.deltaTime;
            return;
        }

        attackTimer += Time.deltaTime;

        if (!firstAttackDone)
        {
            SetAnimState(2); // Attack1
            DoAttack(attack1Damage, attack1Force, retreat1Force);
            firstAttackDone = true;
            return;
        }

        if (!secondAttackDone && attackTimer >= attackDelay)
        {
            SetAnimState(3); // Attack2
            DoAttack(attack2Damage, attack2Force, retreat2Force);
            secondAttackDone = true;
            waitTimer = waitDuration;
            preAttackStarted = false;
            state = State.Wait;
        }
    }

    private Collider2D[] _hits = new Collider2D[4];

    private void DoAttack(float damage, float force, float retreatForce)
    {
        //LookAtTarget();

        Vector2 attackCenter = (Vector2)transform.position + attackBoxOffset * facing;
        Vector2 knockback = new Vector2(force * facing, 1f);

        int hitCount = Physics2D.OverlapBoxNonAlloc(attackCenter, attackBoxSize, 0f, _hits, attackLayerMask);

        for (int i = 0; i < hitCount; i++)
        {
            var character = _hits[i].GetComponent<CharacterBase>();
            if (character != null)
            {
                character.OnTakeHit(new EntityHitData(damage, knockback));
            }
        }

        rb.linearVelocity = new Vector2(-retreatForce * facing, 0f);
    }

    private void LookAtTarget()
    {
        if (target)
        {
            Flip(target.position.x > transform.position.x ? 1 : -1);
        }
    }

public void OnTakeHit(CharacterHitData hitData)
{
    if (isHurt) return;

    life -= hitData.damage;

    if (life <= 0)
    {
        SetAnimState(5); // Die
        Destroy(gameObject, 2f); // ����������� ����� �� �����������
    }
    else
    {
        Vector2 hurtForce = CalculateHurtForce(hitData);
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(hurtForce, ForceMode2D.Impulse);

        StartCoroutine(PlayHurtAndInterrupt());
    }

    if (healthBarInstance != null)
    {
        healthBarInstance.SetHealth(life); // Update health bar on damage
    }
}

    private Vector2 CalculateHurtForce(CharacterHitData hitData)
    {
        if (hitData.character == null)
            return Vector2.zero;

        float facingFromAttacker = Mathf.Sign(transform.position.x - hitData.character.transform.position.x);
        Vector2 baseForce = new Vector2(1.5f * facingFromAttacker, 1.5f);
        float forceMultiplier = hitData.force * Random.Range(1f, 1.5f);

        return baseForce.normalized * forceMultiplier;
    }

    private IEnumerator PlayHurtAndInterrupt()
    {
        isHurt = true;

        rb.linearVelocity = Vector2.zero;
        SetAnimState(4); // Hurt

        firstAttackDone = false;
        secondAttackDone = false;
        preAttackStarted = false;

        state = State.Wait;
        waitTimer = waitDuration;

        yield return new WaitForSeconds(0.1f); // ����������� ������������ �������� Hurt

        isHurt = false;

        if (target && Vector2.Distance(transform.position, target.position) <= detectionRadius)
        {
            state = State.Chase;
        }
        else
        {
            target = null;
            state = State.Patrol;
            patrolTarget = GetClosestPatrolPoint();
        }
    }

    private Transform GetClosestPatrolPoint()
    {
        float distA = Vector2.Distance(transform.position, pointA.position);
        float distB = Vector2.Distance(transform.position, pointB.position);
        return distA < distB ? pointA : pointB;
    }


#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        Gizmos.color = Color.cyan;
        if (pointA != null) Gizmos.DrawWireSphere(pointA.position, 0.2f);
        if (pointB != null) Gizmos.DrawWireSphere(pointB.position, 0.2f);

        Gizmos.color = Color.red;
        Vector2 attackCenter = Application.isPlaying
            ? (Vector2)transform.position + attackBoxOffset * facing
            : (Vector2)transform.position + attackBoxOffset;
        Gizmos.DrawWireCube(attackCenter, attackBoxSize);
    }
#endif
}
