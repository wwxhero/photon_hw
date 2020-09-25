using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bolt.Samples.GettingStarted;
using VehiControl = LoggerVeh;

public class MockVehControl : VehiControl {
	public float c_speedInKPH = 20;
	float c_speedInMPS = 0;
	Vector3 c_velInMPS = Vector3.zero;
	Vector3[] c_route = new Vector3[2]; //a simple line route
	float	c_routeLen;
	List<NetworkVehBehavior> m_vehi = new List<NetworkVehBehavior>();
	List<float> m_sLen = new List<float>();
	ScenarioControl m_sceneCtrl = null;
    enum KeyState { up, down};
    KeyState m_keyState = KeyState.up;
	// Use this for initialization
	void Start () {
		c_route[0] = transform.Find("Start").position;
		c_route[1] = transform.Find("End").position;
		Vector3 vec = c_route[1] - c_route[0];
		c_routeLen = vec.magnitude;
		c_speedInMPS = (c_speedInKPH * 1000/(60*60));
		c_velInMPS = c_speedInMPS * vec.normalized;
		GameObject scenario_obj = GameObject.FindGameObjectWithTag("scene");
		Debug.Assert(null != scenario_obj);
		m_sceneCtrl = scenario_obj.GetComponent<ScenarioControl>();
	}

	// Update is called once per frame
	void FixedUpdate()  {
		int i_removed = m_vehi.Count; //[i_removed, m_vehi.Count) will be removed
		int n_removed = 0;
		for (int i_veh = m_vehi.Count - 1; i_veh > -1 ; i_veh --)
		{
			m_vehi[i_veh].position += c_velInMPS * Time.fixedDeltaTime;
			m_sLen[i_veh] += c_speedInMPS * Time.fixedDeltaTime;
			if (m_sLen[i_veh] > c_routeLen)
			{
				i_removed --;
				n_removed ++;

				NetworkVehBehavior removing = m_vehi[i_veh];
				m_vehi[i_veh] = m_vehi[i_removed];
				m_vehi[i_removed] = removing;

				float temp_s = m_sLen[i_removed];
				m_sLen[i_removed] = m_sLen[i_veh];
				m_sLen[i_veh] = temp_s;

				Bolt.Samples.GettingStarted.GS_ServerCallbacks.DestroyVeh(m_sceneCtrl, removing);
			}
		}
		if (n_removed > 0)
		{
			m_vehi.RemoveRange(i_removed, n_removed);
			m_sLen.RemoveRange(i_removed, n_removed);
		}

        KeyState state_np = KeyState.up;
        if (Input.GetKeyDown(KeyCode.Alpha0))
        	state_np = KeyState.down;
        if (KeyState.down == m_keyState
        	&& KeyState.up == state_np)
		{
			Vector3 forward = c_route[1] - c_route[0];
			Quaternion rot = Quaternion.LookRotation(forward, Vector3.up);
			NetworkVehBehavior veh = Bolt.Samples.GettingStarted.GS_ServerCallbacks.CreateVeh(m_sceneCtrl, c_route[0], rot);
			m_vehi.Add(veh);
			m_sLen.Add(0);
		}
		m_keyState = state_np;
	}
}
