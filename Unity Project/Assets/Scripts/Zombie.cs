using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
public class Zombie : MonoBehaviour
{
    [Header("Stats")]
    public float speed = 10f;
    public float attackRange = 1.5f;
    public float attackCooldown = 1f;
    public float damage = 10f;
    private float m_Distance;

    [Header("References")]
    public Transform target;      
    public CapsuleCollider rightHand;
    public CapsuleCollider leftHand;
    private CapsuleCollider mainCollider;
    private Animator animator;
    private AudioSource audioSource;
    private NavMeshAgent m_Agent;

    [Header("SFX Clips")]
    //public AudioClip[] chaseSounds;
    //public AudioClip[] attackSounds;
    //public AudioClip[] deathSounds;

    private float lastAttackTime;
    private bool isDead = false;

    void Awake()
    {
        m_Agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        mainCollider = GetComponent<CapsuleCollider>();

        if (target == null)
        {
            var playerGO = GameObject.FindWithTag("Player");
            if (playerGO != null)
                target = playerGO.transform;
            else
                Debug.LogError("Zombie: no object with tag 'Player'!");
        }

        animator.SetInteger("ChaseVariant", Random.Range(0, 9));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other != null && other.gameObject.layer == LayerMask.NameToLayer("Player Body"))
        {
            var health = other.GetComponentInParent<Health>();
            if (health != null)
            {
                health.TakeDamage(damage);
                // zwuk na udyr
            }
            //canHit = false;
        }
    }

    void Update()
    {

        if (isDead) return;

        m_Distance = Vector3.Distance(m_Agent.transform.position, target.position);
        if (m_Distance < attackRange)
        {
            m_Agent.isStopped = true;
            //animator.SetBool("Attack", true);
            TryAttack();
        }
        else
        {
            rightHand.enabled = false;
            leftHand.enabled = false;

            m_Agent.isStopped = false;
            //animator.SetBool("Attack", false);
            m_Agent.destination = target.position;
        }
    }

    private void OnAnimatorMove()
    {
        transform.position += animator.deltaPosition;
        transform.rotation *= animator.deltaRotation;

        //if (isDead) return;

        ////if (animator.GetBool("Attack") == false)
        //if (true)
        //{
        //    m_Agent.speed = (animator.deltaPosition / Time.deltaTime).magnitude*speed;
        //}
    }

    private void ChasePlayer(float dist)
    {
        //animator.SetFloat("Speed", speed);
        //if (!audioSource.isPlaying)
        //    PlayRandom(chaseSounds, loop: true);

        Vector3 dir = (target.position - transform.position).normalized;
        transform.position += dir * speed * Time.deltaTime;
        transform.LookAt(target);
    }

    private void TryAttack()
    {
        audioSource.Stop();

        if (Time.time - lastAttackTime < attackCooldown) return;
        lastAttackTime = Time.time;

        animator.SetInteger("AttackVariant", Random.Range(0, 7));
        animator.SetTrigger("Attack");

        //PlayRandom(attackSounds, loop: false);

        rightHand.enabled = true;
        leftHand.enabled = true;
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        m_Agent.updatePosition = false;
        m_Agent.updateRotation = false;
        m_Agent.isStopped = true;
        m_Agent.enabled = false;

        animator.applyRootMotion = true;

        animator.SetInteger("DeathVariant", Random.Range(0, 2));
        animator.SetTrigger("Death");
        //PlayRandom(deathSounds, loop: false);
                
        rightHand.enabled = false;
        leftHand.enabled = false;

        Destroy(gameObject, 10f);
    }

    private void PlayRandom(AudioClip[] clips, bool loop)
    {
        if (clips.Length == 0) return;
        audioSource.clip = clips[Random.Range(0, clips.Length)];
        audioSource.loop = loop;
        audioSource.Play();
    }
}

