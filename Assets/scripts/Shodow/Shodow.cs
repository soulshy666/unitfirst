using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shodow : MonoBehaviour
{
    public Transform player;
    public SpriteRenderer playerSprite;
    public  SpriteRenderer thisSprite;
    public  Color color;
    // Start is called before the first frame update
    [Header("时间控制参数")]
    public float activeStart;//开始显示的时间
    public float activetime;//显示时间

    [Header("不透明度")]
    private float alpha = 1;
    public float alphaset;
    public float alphasMultiplier;

    public void OnEnable()
    {
       // player = GameObject.FindGameObjectWithTag("Player").transform;
        thisSprite = GetComponent<SpriteRenderer>();
        playerSprite = player.GetComponent<SpriteRenderer>();
        alpha = alphaset;
        thisSprite.sprite = playerSprite.sprite;
        transform.position = player.position;
        thisSprite.flipX = playerSprite.flipX;
        transform.rotation = player.rotation;
        activeStart = Time.time;

    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        alpha *= alphasMultiplier;
        if(Player.instance.isAttack3)
        {
            color = new Color(0.5f, 0.5f, 1, alpha);
            thisSprite.color = color;
        }
        else
        {
            color = new Color(1, 1, 1, alpha);
            thisSprite.color = color;
        }
        if(Player.instance.isDeshRight)
        {
            thisSprite.flipX = false;
        }
        if(!Player.instance.isDeshRight)
        {
            thisSprite.flipX = true;
        }

        if (Time.time > activeStart + activetime)
        {
            ShadowPool.instance.ReturnPool(this.gameObject);
        }

    }
}
