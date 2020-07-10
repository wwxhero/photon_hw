using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Veh = System.Collections.Generic.KeyValuePair<int, UnityEngine.GameObject>;

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
		id_n = vid.id;
		GameObject scenario_obj = GameObject.FindGameObjectWithTag("scene");
		Debug.Assert(null != scenario_obj);
		ScenarioControl scenario_ctrl = scenario_obj.GetComponent<ScenarioControl>();
		GameObject veh = null;
		if (entity.IsOwner)
		{
			bool local_exists = scenario_ctrl.m_Vehs.TryGetValue(vid.id, out veh);
			Debug.Assert(local_exists);
			m_tranLocal = veh.transform;
		}
		else
		{
			Veh veh2 = scenario_ctrl.CreateLocalVeh(transform.position, transform.rotation, id_n);
			m_tranLocal = veh2.Value.transform;
		}
	}
	public override void Detached()
	{
		if (entity.IsOwner)
		{
			m_tranLocal = null;
		}
		else
		{
			GameObject scenario_obj = GameObject.FindGameObjectWithTag("scene");
			Debug.Assert(null != scenario_obj);
			ScenarioControl scenario_ctrl = scenario_obj.GetComponent<ScenarioControl>();
			scenario_ctrl.DeleteLocalVeh(id_n);
			m_tranLocal = null;
		}
		base.Detached();
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
