using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour {

    PlayerMovment playerMovment;
    public float hp = 100;
    public bool isPlayerDead;
    private Animator anim;
	// Use this for initialization
	void Start () 
    {
        isPlayerDead=false;
        anim = GetComponent<Animator>();
        playerMovment =GetComponent<PlayerMovment>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void TakeDamage(float damage)
    {
        if(hp<=0)
        {
            isPlayerDead=true;
            playerMovment.enabled =false;
            anim.SetTrigger(HashIDs.playerDeadHash);
        }
        else
        {
            hp -= damage;
        }
    }
}
