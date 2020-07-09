using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkVehBehavior : Bolt.EntityBehaviour<IVehState> {

	const bool m_debug = true;
	Transform m_tranLocal;
	public int id_n;
	// Use this for initialization
	public override void Attached()
	{
		state.SetTransforms(state.VehTransform, transform);
		var vid = (LocalVehId)entity.AttachToken;
		if (m_debug)
			DebugLog.InfoFormat("veh binding start:({0})", vid.id);
		GameObject scenario_obj = GameObject.FindGameObjectWithTag("scene");
		Debug.Assert(null != scenario_obj);
		ScenarioControl scenario_ctrl = scenario_obj.GetComponent<ScenarioControl>();
		GameObject veh = null;
		bool bind_exists = scenario_ctrl.m_Vehs.TryGetValue(vid.id, out veh);
		Debug.Assert(bind_exists);
		if (bind_exists)
			m_tranLocal = veh.transform;
	}


	// Update is called once per frame
	void Update ()
	{
		if (entity.IsAttached)
		{
			if (entity.IsOwner)
			{
				transform.position = m_tranLocal.position;
				transform.rotation = m_tranLocal.rotation;
			}
			else
			{
				m_tranLocal.position = transform.position;
				m_tranLocal.rotation = transform.rotation;
			}
		}
	}

}
