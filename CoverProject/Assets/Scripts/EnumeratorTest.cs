using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnumeratorTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
        StartCoroutine(Pe());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public IEnumerator Pe()
    {
        while(true)
        {
            Debug.Log(Time.deltaTime);
            yield return new WaitForSeconds(5);
        }

    }
}
