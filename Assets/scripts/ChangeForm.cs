using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ChangeForm : MonoBehaviour
{
    [Header("切换形态")]
    public GameObject changeSpite;// 切换形态的
    public GameObject player;
    public GameObject bora;
    public GameObject Frog; 
    public float changeTime = 15f;
    public float nowTime=0;
    public Image changebar;
    public bool ischange = false;//表示已经进入了变身状态
    public SpriteRenderer renderer2 ;
    public static ChangeForm instance;
    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (ischange)
        {
            nowTime = nowTime + Time.deltaTime;
        }
        if(nowTime >= changeTime)
        {
            ChangeToPlayer();
        }
        changebar.fillAmount =(changeTime- nowTime) / changeTime;
    }
    public void ChangeToPlayer()
    {
        if (Player.instance.State == Player.PlayerState.isPlayer) return;
        Player.instance.tag = "Player";
        renderer2.color = Color.white;//防止野猪蓄力变红无法变回白色
        Player.instance.isUp = false;
        Player.instance.uptime = 0;
        Player.instance.CanMove = true;
        nowTime = 0;
        ischange = false;
        Player.instance.State = Player.PlayerState.isPlayer;
        changeForm(player);

    }
    public void ChangeToBora()
    {
        if(Player.instance.State == Player.PlayerState.isBora) return;
        Player.instance.tag = "Bora";
        ischange = true;
        Player.instance.State = Player.PlayerState.isBora;
        changeForm(bora);
    }
    public void ChangeToFrog()
    {
        Player.instance.tag = "Frog";
        if (Player.instance.State == Player.PlayerState.isFrog) return;
        ischange = true;
        Player.instance.State = Player.PlayerState.isFrog;
        changeForm(Frog);
    }
    public void changeForm(GameObject targetObject)
    {
        Player.instance.rb.velocity = Vector2.zero;//切换形态时速度归零
        Player.instance.isCrouching = false;//切换形态时取消蹲下状态
        Player.instance.currentSpeed = Player.instance.moveSpeed;//切换形态时重置移动速度
        Player.instance.targetCanvas.gameObject.SetActive(false);
        changeSpite.gameObject.SetActive(true);
        Invoke("CloseChangeSprite", 1f); // 延迟1秒后调用CloseChangeSprite方法)
        // 检查目标物体是否为空
        if (targetObject == null)
        {
            Debug.LogError("目标物体不能为空！");
            return;
        }

        // 检查目标物体是否有父物体
        Transform parent = targetObject.transform.parent;
        if (parent == null)
        {
            Debug.LogError("目标物体没有父物体，无法执行操作！");
            return;
        }

        // 遍历父物体下的所有直接子物体
        foreach (Transform child in parent)
        {
            // 激活目标物体，关闭其他子物体
            child.gameObject.SetActive(child.gameObject == targetObject);
        }
    }
    public void CloseChangeSprite()
    {
        changeSpite.gameObject.SetActive(false);
    }
    

}
