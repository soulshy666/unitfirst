using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bar : MonoBehaviour
{
    // Start is called before the first frame update
    public Image heathGreen;
    public Image heathRed;
    public Image heathYellow;
    public Text healthtext;
    public Attack attack;
    public Character character;
    public float Current;//临时记录血条
    [Header("耐力")]
    public Image StaminaBar;
    public float MaxStamina = 100f;
    public float CurrentStamina;
    public float current;//临时记录耐力值
    public static Bar instance;

    void Awake()
    {
        instance = this;
        CurrentStamina = MaxStamina;
    }
    void Start()
    {

    }

    void Update()
    {
        ChangeBar();
        ChangeStamina();
        if (Current > character.CurrentHealth)
        {
            Current -= Time.deltaTime*10;
            if(Current < character.CurrentHealth)
            {
                Current = character.CurrentHealth;
            }
        }
        else
        {
            Current = character.CurrentHealth;
        }
        // 直接显示当前值/最大值
        // 用 "0" 确保显示为整数（自动四舍五入）
        healthtext.text = $"{Player.instance.character.CurrentHealth:0}/{Player.instance.character.HealthMax:0}";
    }

    public void ChangeBar()
    {
        heathGreen.fillAmount = character.CurrentHealth / character.HealthMax;
        Invoke("ChangeBarRed", 2f);
    }
    public void ChangeBarRed()
    {
        heathRed.fillAmount = Current / character.HealthMax;
    }
    public void ChangeStamina()
    {
        if ( CurrentStamina < MaxStamina)
        {
            CurrentStamina += Time.deltaTime * 20;
        }
        StaminaBar.fillAmount = CurrentStamina / MaxStamina;
    }
}