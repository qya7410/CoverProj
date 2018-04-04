using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    public float speed;
    public float delayDestroy;
    public float damage;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.position += transform.forward*Time.deltaTime*speed;
        Destroy(this.gameObject, delayDestroy);
	}


	// private void OnTriggerEnter(Collider other)
	// {
    //     if (other.tag == Tags.player)
    //     {
    //         other.GetComponent<PlayerHealth>().TakeDamage(damage);
    //         Destroy(this.gameObject);
    //     }
	// }
    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.tag == Tags.obstacle)
        {
            Destroy(this.gameObject);
        }

        if (other.collider.tag == Tags.player)
        {
             other.collider.GetComponent<PlayerHealth>().TakeDamage(damage);
            Destroy(this.gameObject);
        }
        
    }
}
