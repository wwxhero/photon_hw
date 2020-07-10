using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkVehBehavior : Bolt.EntityBehaviour<IVehState> {

	const bool m_debug = true;
	Transform m_tranLocal;
	ScenarioControl m_scenarioCtrl;
	int m_id;
	public int id
	{
		get { return m_id; }
	}

	public void Awake()
	{
		GameObject scenario_obj = GameObject.FindGameObjectWithTag("scene");
		Debug.Assert(null != scenario_obj);
		m_scenarioCtrl = scenario_obj.GetComponent<ScenarioControl>();
	}

	// Use this for initialization
	public override void Attached()
	{
		state.SetTransforms(state.VehTransform, transform);
		var vid = (LocalVehId)entity.AttachToken;
		if (m_debug)
			DebugLog.InfoFormat("veh binding start:({0})", vid.id);
		m_id = vid.id;
		GameObject veh = null;
		if (entity.IsOwner)
		{
			bool local_exists = m_scenarioCtrl.m_Vehs.TryGetValue(vid.id, out veh);
			Debug.Assert(local_exists);
		}
		else
			veh = m_scenarioCtrl.CreateLocalVeh(transform.position, transform.rotation, id);

		Debug.Assert(null != veh);
		m_tranLocal = veh.transform;
	}

	public override void Detached()
	{
		if (entity.IsOwner)
		{
			m_tranLocal = null;
		}
		else
		{
			m_scenarioCtrl.DeleteLocalVeh(id);
			m_tranLocal = null;
		}
		base.Detached();
	}

	public Vector3 position
	{
		get { return m_tranLocal.position; }
		set { m_tranLocal.position = value; }
	}

	public Quaternion rotation
	{
		get { return m_tranLocal.rotation; }
		set { m_tranLocal.rotation = value; }
	}

	public GameObject localGameObject
	{
		get { return m_tranLocal.gameObject; }
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
