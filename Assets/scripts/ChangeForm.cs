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
    public GameObject Frog; 
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
        Player.instance.tag = "Frog";
        if (Player.instance.State == Player.PlayerState.isFrog) return;
        ischange = true;
        Player.instance.State = Player.PlayerState.isFrog;
        changeForm(Frog);
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
