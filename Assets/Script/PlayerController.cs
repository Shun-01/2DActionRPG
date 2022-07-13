using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField, Tooltip("移動速度")]
    private int moveSpeed;

    [SerializeField]
    private Animator playerAnim;

    public Rigidbody2D rb;

    [SerializeField]
    private Animator weaponAnim;

    public int maxHp;
    [System.NonSerialized]
    public int currentHp;

    public float maxStamina, staminaRecoverySpeed;
    [System.NonSerialized]
    public float currentStamina;

    [SerializeField]
    private float dashSpeed, dashCooldown, dashCost;

    private float dashCounter, activeMoveSpeed;

    private bool isKnockingBack;
    private Vector2 knockDir;
    
    [SerializeField]
    private float knockbackTime, knockbackForce;
    private float knockbackCounter;

    [SerializeField]
    private float invincibilityTime;
    private float invincibilityCounter;

    private Flash flash;

    void Start()
    {
        currentHp = maxHp;
        invincibilityCounter = invincibilityTime;

        GameManager.instance.UpdateHpUI();

        activeMoveSpeed = moveSpeed;

        currentStamina = maxStamina;
        GameManager.instance.UpdateStaminaUI();

        flash = GetComponent<Flash>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.statusPanel.activeInHierarchy)
        {
            return;
        }//ステータス画面のときは動かせない

        if(invincibilityCounter > 0)
        {
            invincibilityCounter -= Time.deltaTime;
        }

        if (isKnockingBack)
        {
            knockbackCounter -= Time.deltaTime;
            rb.velocity = knockDir * knockbackForce;

            if(knockbackCounter <= 0)
            {
                isKnockingBack = false;
            }
            else
            {
                return;
            }
        }

        rb.velocity = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized * activeMoveSpeed;

        if (rb.velocity != Vector2.zero)
        {
            if (Input.GetAxisRaw("Horizontal") != 0)
            {
                if (Input.GetAxisRaw("Horizontal") > 0)
                {
                    playerAnim.SetFloat("X", 1f);
                    playerAnim.SetFloat("Y", 0);

                    weaponAnim.SetFloat("X", 1f);
                    weaponAnim.SetFloat("Y", 0);
                }
                else
                {
                    playerAnim.SetFloat("X", -1f);
                    playerAnim.SetFloat("Y", 0);

                    weaponAnim.SetFloat("X", -1f);
                    weaponAnim.SetFloat("Y", 0);
                }
            }
            else if (Input.GetAxisRaw("Vertical") > 0)
            {
                playerAnim.SetFloat("X", 0);
                playerAnim.SetFloat("Y", 1f);

                weaponAnim.SetFloat("X", 0);
                weaponAnim.SetFloat("Y", 1f);
            }
            else
            {
                playerAnim.SetFloat("X", 0);
                playerAnim.SetFloat("Y", -1f);

                weaponAnim.SetFloat("X", 0);
                weaponAnim.SetFloat("Y", -1f);
            }
        } //移動と体の向き

        if (Input.GetMouseButtonDown(0))
        {
            weaponAnim.SetTrigger("Attack"); //攻撃ボタン
        }

        if(dashCounter <= 0)
        {
            if(Input.GetKeyDown(KeyCode.Space) && currentStamina > dashCost)
            {
                activeMoveSpeed = dashSpeed;
                dashCounter = dashCooldown;

                currentStamina -= dashCost;

                GameManager.instance.UpdateStaminaUI();
            }
        }
        else
        {
            dashCounter -= Time.deltaTime;

            if(dashCounter <= 0)
            {
                activeMoveSpeed = moveSpeed;
            }
        }

        currentStamina = Mathf.Clamp(currentStamina + staminaRecoverySpeed * Time.deltaTime, 0, maxStamina);
        GameManager.instance.UpdateStaminaUI();
    }
    /// <summary>
    /// 吹き飛ばし用の関数
    /// </summary>
    /// <param name="position"></param>
    public void KnockBack(Vector3 position)
    {
        knockbackCounter = knockbackTime;
        isKnockingBack = true;

        knockDir = transform.position - position;
        knockDir.Normalize();
    }
    public void DamagePlayer(int damage)
    {
        if(invincibilityCounter <= 0)
        {
            flash.PlayFeedback();

            currentHp = Mathf.Clamp(currentHp - damage, 0, maxHp);

            invincibilityCounter = invincibilityTime;

            SoundManager.instance.PlaySE(2);

            if (currentHp == 0)
            {
                gameObject.SetActive(false);
                SoundManager.instance.PlaySE(0);
                GameManager.instance.ShowDeathText();
            }
        }
        GameManager.instance.UpdateHpUI();
    }

    private void OnTriggerEnter2D(Collider2D collision) //回復ポーションを取ったとき
    {
        if(collision.tag == "Portion" && maxHp != currentHp && collision.GetComponent<ItemController>().waitTime<= 0)
        {
            SoundManager.instance.PlaySE(1);
            ItemController item = collision.GetComponent<ItemController>();

            currentHp = Mathf.Clamp(currentHp + item.recoveryValue, 0, maxHp);

            GameManager.instance.UpdateHpUI();

            Destroy(collision.gameObject);
        }
    }
}
