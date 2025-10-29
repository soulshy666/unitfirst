using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject shell;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            shell.gameObject.SetActive(false);
            Player.instance.isDiscard = false;
        }
    }
    public void DestroyAfterAnimation()//放在死亡动画中，当作事件触发
    {
        shell.gameObject.SetActive(false);
    }
}
