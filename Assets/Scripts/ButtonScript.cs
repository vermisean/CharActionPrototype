using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonScript : MonoBehaviour 
{
	public float yPerSecond = 0.0005f;
	public float maxY = 2.25f;

	private bool isStoodOn = false;
	private float currentY;
	private float totalY;

	void Start () 
	{
		totalY = this.transform.position.y;
		currentY = totalY;
		//maxY = currentY - 
	}

	void Update () 
	{
		if(isStoodOn)
		{
			if(this.transform.position.y >= -maxY)
				this.transform.position -= new Vector3(0.0f, yPerSecond * Time.deltaTime, 0.0f);
		}
		else
		{
			if(this.transform.position.y <= totalY)
				this.transform.position += new Vector3(0.0f, yPerSecond * Time.deltaTime, 0.0f);
		}
	}

	void OnTriggerEnter(Collider col)
	{
		if(col.gameObject.tag == "Player")
		{
			isStoodOn = true;
		}
	}

	void OnTriggerExit(Collider col)
	{
		if(col.gameObject.tag == "Player")
		{
			isStoodOn = false;
		}
	}
}
