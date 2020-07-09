using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bolt.Samples.GettingStarted;
using Veh = System.Collections.Generic.KeyValuePair<int, UnityEngine.GameObject>;

public class MockVehControl : MonoBehaviour {
	public float c_speedInKPH = 20;
	float c_speedInMPS = 0;
	Vector3 c_velInMPS = Vector3.zero;
	Vector3[] c_route = new Vector3[2]; //a simple line route
	float	c_routeLen;
	List<Veh> m_vehi = new List<Veh>();
	List<float> m_sLen = new List<float>();
	ScenarioControl m_sceneCtrl = null;
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
	void Update () {
		int i_removed = m_vehi.Count; //[i_removed, m_vehi.Count) will be removed
		int n_removed = 0;
		for (int i_veh = m_vehi.Count - 1; i_veh > -1 ; i_veh --)
		{
			m_vehi[i_veh].Value.transform.position += c_velInMPS * Time.deltaTime;
			m_sLen[i_veh] += c_speedInMPS * Time.deltaTime;
			if (m_sLen[i_veh] > c_routeLen)
			{
				i_removed --;
				n_removed ++;

				Veh removing = m_vehi[i_veh];
				m_vehi[i_veh] = m_vehi[i_removed];
				m_vehi[i_removed] = removing;

				float temp_s = m_sLen[i_removed];
				m_sLen[i_removed] = m_sLen[i_veh];
				m_sLen[i_veh] = temp_s;

				m_sceneCtrl.DeleteVeh(removing.Key);
			}
		}
		if (n_removed > 0)
		{
			m_vehi.RemoveRange(i_removed, n_removed);
			m_sLen.RemoveRange(i_removed, n_removed);
		}

		if (Input.GetKeyDown(KeyCode.Tab))
		{
			Vector3 forward = c_route[1] - c_route[0];
			Quaternion rot = Quaternion.LookRotation(forward, Vector3.up);
			Veh veh = m_sceneCtrl.CreateVeh(c_route[0], rot);
			m_vehi.Add(veh);
			m_sLen.Add(0);
		}
	}
}
