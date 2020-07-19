using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalVehBehavior : MonoBehaviour {

	// Use this for initialization
	Quaternion c_qOffsetVeh = new Quaternion(Mathf.Sin(-Mathf.PI/4), 0, 0, Mathf.Cos(-Mathf.PI/4));
    Quaternion c_qOffsetVehInv = new Quaternion(Mathf.Sin(Mathf.PI / 4), 0, 0, Mathf.Cos(Mathf.PI / 4));
    void Start () {

	}

	public Vector3 position
	{
		get { return transform.position; }
		set { transform.position = value; }
	}

	public Quaternion rotation
	{
		get { return transform.rotation * c_qOffsetVehInv; }
		set { transform.rotation = value * c_qOffsetVeh; }
	}
}
