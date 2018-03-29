﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovment : MonoBehaviour {
    public float moveSpeed = 5f;
    public bool isSneak;

    private CapsuleCollider cs;
    private CharacterController cc;
    Animator anim;
    Vector3 playerForward;

	// Use this for initialization
	void Start () {
        cc = GetComponent<CharacterController>();
        cs = GetComponent<CapsuleCollider>();
        isSneak = false;
        anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () 
    {
        Vector3 dir = new Vector3(JoystickController._instance.GetiputDirection.normalized.x, 0, JoystickController._instance.GetiputDirection.normalized.y);
        transform.position = transform.position + dir * moveSpeed * Time.deltaTime;
        transform.rotation = Quaternion.LookRotation(playerForward);
        if (JoystickController._instance.GetiputDirection.normalized.x!=0f||JoystickController._instance.GetiputDirection.normalized.y!=0f)
        {
            playerForward = new Vector3(JoystickController._instance.GetiputDirection.normalized.x, 0,JoystickController._instance.GetiputDirection.normalized.y);
            anim.SetBool("IsMove",true);
        }
        else
        {
            anim.SetBool("IsMove", false);
        }

        if (isSneak)
        {
            moveSpeed = 1f;
            cs.enabled = true;
            cc.enabled = false;
            anim.SetBool(HashIDs.playerSneakHash, true);

        }
        else{
            moveSpeed = 2f;
            cc.enabled = true;
            cs.enabled = false;
            anim.SetBool(HashIDs.playerSneakHash, false);
        }
    }
}
