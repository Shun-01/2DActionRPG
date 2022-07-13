using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField]
    private PlayerController player;

    [SerializeField]
    private Slider hpSlider;

    [SerializeField]
    private Slider staminaSlider;

    public GameObject dialogueBox;

    public Text dialogueText;

    private string[] dialogueLines;

    private int currentLine;

    private bool justStarted;

    public GameObject statusPanel;
    
    [SerializeField]
    private Text hpText, stText, atText;

    [SerializeField]
    private WeaponController weapon;

    private int totalEXP, currentLv;

    [SerializeField,Tooltip("ïKóvåoå±íl")]
    private int[] requiredEXP;

    [SerializeField]
    private GameObject levelUpText;

    [SerializeField]
    private Canvas canvas;

    [SerializeField]
    private GameObject gameoverText, restartText;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    private void Start()
    {
        if (PlayerPrefs.HasKey("MaxHp"))
        {
            LoadStatus();
        }
    }
    private void Update()
    {
        if (dialogueBox.activeInHierarchy) //ï∂éöëóÇË
        {
            SoundManager.instance.PlaySE(4);
            if (Input.GetMouseButtonUp(1))
            {
                if (!justStarted)
                {
                    currentLine++;
                    if(currentLine >= dialogueLines.Length)
                    {
                        dialogueBox.SetActive(false);
                    }
                    else
                    {
                        dialogueText.text = dialogueLines[currentLine];
                    }
                }
                else
                {
                    justStarted = false;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            ShowStatusPanel();
        }
    }
    public void UpdateHpUI()
    {
        hpSlider.maxValue = player.maxHp;
        hpSlider.value = player.currentHp;
    }
    public void UpdateStaminaUI()
    {
        staminaSlider.maxValue = player.maxStamina;
        staminaSlider.value = player.currentStamina;
    }
    public void ShowDialogue(string[] lines)
    {
        dialogueLines = lines;
        currentLine = 0;

        dialogueText.text = dialogueLines[currentLine];
        dialogueBox.SetActive(true);

        justStarted = true;
    }
    public void ShowDialogueChange(bool x)
    {
        dialogueBox.SetActive(x);
    }
    public void Load()
    {
        SceneManager.LoadScene("Main");
    }

    public void ShowStatusPanel()
    {
        statusPanel.SetActive(true);

        Time.timeScale = 0f;

        StatusUpdate();
    }
    public void CloseStatusPanel()
    {
        statusPanel.SetActive(false);

        Time.timeScale = 1f;
    }
    public void StatusUpdate()
    {
        hpText.text = "ê∂ñΩóÕ : " + player.maxHp;
        stText.text = "éùãvóÕ : " + player.maxStamina;
        atText.text = "çUåÇóÕ : " + weapon.attackDamage;
    }
    public void AddExp(int exp)
    {
        if(requiredEXP.Length <= currentLv)
        {
            return;
        }

        totalEXP += exp;

        if(totalEXP >= requiredEXP[currentLv])
        {
            currentLv++;

            player.maxHp += 5;
            player.maxStamina += 5;
            weapon.attackDamage += 2;

            GameObject levelUp = Instantiate(levelUpText, player.transform.position, Quaternion.identity);
            levelUp.transform.SetParent(player.transform);
            //levelUp.transform.localPosition = player.transform.position + new Vector3(0, 50, 0);
        }
    }
    public void SaveStatus()
    {
        PlayerPrefs.SetInt("MaxHp", player.maxHp);
        PlayerPrefs.SetFloat("MaxSt", player.maxStamina);
        PlayerPrefs.SetInt("At", weapon.attackDamage);
        PlayerPrefs.SetInt("Level", currentLv);
        PlayerPrefs.SetInt("Exp", totalEXP);
    }
    public void LoadStatus()
    {
        player.maxHp = PlayerPrefs.GetInt("MaxHp");
        player.maxStamina = PlayerPrefs.GetFloat("MaxSt");
        weapon.attackDamage = PlayerPrefs.GetInt("At");
        currentLv = PlayerPrefs.GetInt("Level");
        totalEXP = PlayerPrefs.GetInt("Exp");
    }
    public void ShowDeathText()
    {
        gameoverText.SetActive(true);
        restartText.SetActive(true);
    }
}
