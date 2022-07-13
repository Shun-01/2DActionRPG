using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator enemyAnim;

    [SerializeField]
    private float moveSpeed, waitTime, walkTime;

    private float waitCounter, moveCounter;

    private Vector2 moveDir;

    [SerializeField]
    private BoxCollider2D area;

    [SerializeField,Tooltip("プレイヤーを追いかける？")]
    private bool chase;

    private bool isChasing;

    [SerializeField]
    private float chaseSpeed, rangeToChase;

    private Transform target;

    [SerializeField]
    private float attackInterval;

    [SerializeField]
    private int attackDamage;

    [SerializeField]
    private float maxHp;
    private float currentHp;

    private bool isKnockingBack;

    [SerializeField]
    private float knockBackTime, knockBackForce;

    private float knockBackCounter;

    private Vector2 knockDir;

    [SerializeField]
    private GameObject portion;

    [SerializeField]
    private float portionDropChance;

    [SerializeField]
    private GameObject blood;

    [SerializeField]
    private int exp;

    [SerializeField]
    private Image hpImage;

    private Flash flash;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        enemyAnim = GetComponent<Animator>();
        flash = GetComponent<Flash>();

        waitCounter = waitTime;

        target = GameObject.FindGameObjectWithTag("Player").transform;

        currentHp = maxHp;
    }

    void Update()
    {
        if (isKnockingBack)
        {
            if(knockBackCounter > 0)
            {
                knockBackCounter -= Time.deltaTime;
                rb.velocity = knockBackForce * knockDir;
            }
            else
            {
                rb.velocity = Vector2.zero;
                isKnockingBack = false;
            }
            return;
        }

        if (!isChasing)
        {
            if (waitCounter > 0)
            {
                waitCounter -= Time.deltaTime;
                rb.velocity = Vector2.zero;

                if (waitCounter <= 0)
                {
                    moveCounter = walkTime;

                    enemyAnim.SetBool("Moving", true);

                    moveDir = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
                    moveDir.Normalize();
                }
            }//止まっているときのコード
            else
            {
                moveCounter -= Time.deltaTime;

                rb.velocity = moveDir * moveSpeed;

                if (moveCounter <= 0)
                {
                    enemyAnim.SetBool("Moving", false);

                    waitCounter = waitTime;
                }
            }//動いているときのコード

            if (chase)
            {
                if(Vector3.Distance(transform.position, target.transform.position) < rangeToChase)
                {
                    isChasing = true;
                }
            }
        }
        else
        {
            if(waitCounter > 0)
            {
                waitCounter -= Time.deltaTime;
                rb.velocity = Vector2.zero;

                if(waitCounter <= 0)
                {
                    enemyAnim.SetBool("Moving", true);
                }
            }
            else
            {
                moveDir = target.transform.position - transform.position;
                moveDir.Normalize();

                rb.velocity = moveDir * chaseSpeed;
            }

            if (Vector3.Distance(transform.position, target.transform.position) > rangeToChase)
            {
                isChasing = false;

                waitCounter = waitTime;

                enemyAnim.SetBool("Moving", false);
            }
        }

        transform.position = new Vector3(Mathf.Clamp(transform.position.x, area.bounds.min.x + 1, area.bounds.max.x - 1),
            Mathf.Clamp(transform.position.y, area.bounds.min.y + 1, area.bounds.max.y - 1), transform.position.z); //動ける範囲の制限
    }

    private void OnCollisionEnter2D(Collision2D collision) //playerとぶつかったとき
    {
        if(collision.gameObject.tag == "Player")
        {
            if (isChasing)
            {
                PlayerController player = collision.gameObject.GetComponent<PlayerController>();

                player.KnockBack(transform.position);
                player.DamagePlayer(attackDamage);

                waitCounter = attackInterval;

                enemyAnim.SetBool("Moving", false);
            }
        }
    }
    public void KnockBack(Vector3 position)
    {
        isKnockingBack = true;
        knockBackCounter = knockBackTime;

        knockDir = transform.position - position;
        knockDir.Normalize();

        enemyAnim.SetBool("Moving", false);
    }
    public void TakeDamage(int damage, Vector3 position)
    {
        currentHp -= damage;

        UpdateHpImage();

        flash.PlayFeedback();

        if(currentHp <= 0)
        {
            Instantiate(blood, transform.position, transform.rotation);

            GameManager.instance.AddExp(exp);

            SoundManager.instance.PlaySE(1);
            if (Random.Range(0, 100) < portionDropChance && portion != null)
            {
                Instantiate(portion, transform.position, transform.rotation);
            }

            Destroy(gameObject);
        }

        KnockBack(position);
    }
    private void UpdateHpImage()
    {
        hpImage.fillAmount = currentHp / maxHp;
    }
}
