using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ChangeForm : MonoBehaviour
{
    [Header("�л���̬")]
    public GameObject changeSpite;// �л���̬��
    public GameObject player;
    public GameObject bora;
    public GameObject frog;
    public GameObject chameleon;
    public GameObject bat;
    public float changeTime = 15f;
    public float nowTime=0;
    public Image changebar;
    public bool ischange = false;//��ʾ�Ѿ������˱���״̬
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
        Player.instance.character.isImmuneToFallDamage = false;//�����ܵ������˺�
        Player.instance.tag = "Player";
        renderer2.color = Color.white;//��ֹҰ����������޷���ذ�ɫ
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
        if (Player.instance.State == Player.PlayerState.isFrog) return;
        Player.instance.character.isImmuneToFallDamage = true;//������̬���ߵ����˺�
        Player.instance.tag = "Frog";    
        ischange = true;
        Player.instance.State = Player.PlayerState.isFrog;
        changeForm(frog);

    }
    public void ChangeToChameleon()
    {
        if (Player.instance.State == Player.PlayerState.isChameleon) return;
        Player.instance.tag = "Chameleon";
        ischange = true;
        Player.instance.State = Player.PlayerState.isChameleon;
        changeForm(chameleon);
    }
    public void ChangeToBat()
    {  
        if (Player.instance.State == Player.PlayerState.isBat) return;
        Player.instance.tag = "Bat";
        ischange = true;
        Player.instance.State = Player.PlayerState.isBat;
        // ��������ʱ����Y��λ�ã�ʵ�ַ��ڿ��е�Ч��
        // ��ȡ��ǰλ��
        Vector3 currentPos = Player.instance.transform.position;
        // �ڵ�ǰY����������Ӹ߶ȣ��ɸ���ʵ�����������ֵ��������1.5f������
        float liftHeight = 1f;
        Player.instance.transform.position = new Vector3(currentPos.x, currentPos.y + liftHeight, currentPos.z);
        changeForm(bat);
    }
    public void changeForm(GameObject targetObject)
    {
        Player.instance.rb.velocity = Vector2.zero;//�л���̬ʱ�ٶȹ���
        Player.instance.isCrouching = false;//�л���̬ʱȡ������״̬
        Player.instance.currentSpeed = Player.instance.moveSpeed;//�л���̬ʱ�����ƶ��ٶ�
        Player.instance.targetCanvas.gameObject.SetActive(false);
        changeSpite.gameObject.SetActive(true);
        Invoke("CloseChangeSprite", 1f); // �ӳ�1������CloseChangeSprite����)
        // ���Ŀ�������Ƿ�Ϊ��
        if (targetObject == null)
        {
            Debug.LogError("Ŀ�����岻��Ϊ�գ�");
            return;
        }

        // ���Ŀ�������Ƿ��и�����
        Transform parent = targetObject.transform.parent;
        if (parent == null)
        {
            Debug.LogError("Ŀ������û�и����壬�޷�ִ�в�����");
            return;
        }

        // �����������µ�����ֱ��������
        foreach (Transform child in parent)
        {
            // ����Ŀ�����壬�ر�����������
            child.gameObject.SetActive(child.gameObject == targetObject);
        }
    }
    public void CloseChangeSprite()
    {
        changeSpite.gameObject.SetActive(false);
    }
    

}
