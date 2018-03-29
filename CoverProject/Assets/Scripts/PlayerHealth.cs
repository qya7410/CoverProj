using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour {

    public float hp = 100;
    private Animator anim;
	// Use this for initialization
	void Start () {
        anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void TakeDamage(float damage)
    {
        if(hp<=0)
        {
            anim.SetTrigger(HashIDs.playerDeadHash);
        }
        else
        {
            hp -= damage;
        }
    }
}
