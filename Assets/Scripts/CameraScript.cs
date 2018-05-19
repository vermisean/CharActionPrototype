using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour 
{
	public Transform m_target;

	void Update () 
	{
		transform.LookAt(m_target);
	}
}
