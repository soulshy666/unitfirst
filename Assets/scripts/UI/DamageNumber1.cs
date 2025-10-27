using DamageNumbersPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageNumber1 : MonoBehaviour
{
    public DamageNumberMesh damageNumber1;
    public DamageNumberMesh damageNumber2;
    public static DamageNumber1 instance;
    // Start is called before the first frame update
    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ShowDamamgeNumber1(Character character ,float damageAmount)
    {
        DamageNumber newDamageNumber = damageNumber1.Spawn(character.transform.position, damageAmount);
        newDamageNumber.SetFollowedTarget(character.transform);//¸úËæ±»¹¥»÷µÄÈË
    }
    public void ShowDamamgeNumber2(Character character, float damageAmount)
    {
        DamageNumber newDamageNumber = damageNumber2.Spawn(character.transform.position, damageAmount);
        newDamageNumber.SetFollowedTarget(character.transform);

    }
}
