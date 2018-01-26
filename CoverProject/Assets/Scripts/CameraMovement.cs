using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {
    public float cameraSpeed = 5f;
    private Transform player;
    private Vector3 offset= Vector3.zero;
	// Use this for initialization
	void Start () {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        offset=transform.position- player.position;
	}
	
	// Update is called once per frame
	void Update () 
    {
        this.transform.position = Vector3.Lerp(transform.position, player.position + offset, cameraSpeed * Time.deltaTime);
	}
}
