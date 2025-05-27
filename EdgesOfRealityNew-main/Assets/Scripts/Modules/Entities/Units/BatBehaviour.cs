using UnityEngine;
using Metroidvania.Combat;
using Metroidvania.Characters;
using Metroidvania.Entities;

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
    [SerializeField] private float preAttackDelay = 0.5f; // Задержка перед началом атаки


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

    private Transform target;
    private Transform patrolTarget;
    private float life;
    private int facing = 1;
    private float waitTimer;
    private float attackTimer;
    private bool firstAttackDone, secondAttackDone;
    private float preAttackTimer;
    private bool preAttackStarted;

    private enum State { Patrol, Chase, Attack, Wait }
    private State state = State.Patrol;

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
    }

    private void FixedUpdate()
    {
        DetectPlayer();
        StateMachine();
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
        if (Vector2.Distance(transform.position, patrolTarget.position) < 0.3f)
        {
            patrolTarget = patrolTarget == pointA ? pointB : pointA;
        }

        MoveTowards(patrolTarget.position, moveSpeed);
    }

    private void MoveTowards(Vector2 targetPos, float speed)
    {
        Vector2 dir = (targetPos - (Vector2)transform.position).normalized/2;
        rb.linearVelocity = dir * speed;
        Flip((int)Mathf.Sign(dir.x));
    }

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
        if (!target)
        {
            state = State.Patrol;
            return;
        }

        if (!preAttackStarted)
        {
            preAttackStarted = true;
            preAttackTimer = preAttackDelay;
            rb.linearVelocity = Vector2.zero; // Останавливаемся перед атакой
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
            DoAttack(attack1Damage, attack1Force, retreat1Force);
            firstAttackDone = true;
        }

        if (!secondAttackDone && attackTimer >= attackDelay)
        {
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
        LookAtTarget();

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
        life -= hitData.damage;
        if (life <= 0)
        {
            anim.SetTrigger("Die");
            Destroy(gameObject, 1f);
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
