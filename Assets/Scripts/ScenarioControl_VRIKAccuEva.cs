using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenarioControl_VRIKAccuEva : MonoBehaviour {

	public GameObject m_refPhysical;
	public GameObject m_refHostIk;
	// Use this for initialization
	void Start () {
		string[] lstNetworkingJoints = {
									"root",				"upperleg01.R",		"lowerleg01.L",		"spine02",
									"foot.R",  			"clavicle.R",		"neck02",			"upperarm01.R",
									"lowerarm01.L", 	"wrist.L", 			"upperleg01.L", 	"spine04",
									"lowerleg01.R",		"foot.L",			"clavicle.L",		"toe2-1.L",
									"upperarm01.L",		"head",				"lowerarm01.R",		"wrist.R",
									"toe2-1.R",			"upperleg02.L",		"upperleg02.R"
								};
		GameObject [] gos = {m_refPhysical, m_refHostIk};
		foreach (GameObject go in gos)
		{
			LoggerAvatar logger = go.GetComponent<LoggerAvatar>();
			logger.Initialize(lstNetworkingJoints);
		}
	}

	// Update is called once per frame
	void Update () {

	}
}
