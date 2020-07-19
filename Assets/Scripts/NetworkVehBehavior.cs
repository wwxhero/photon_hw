using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkVehBehavior : Bolt.EntityBehaviour<IVehState> {

	const bool m_debug = true;
	LocalVehBehavior m_localVeh;
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
			bool local_exists = m_scenarioCtrl.m_Vehs.TryGetValue(vid.id, out m_localVeh);
			Debug.Assert(local_exists);
		}
		else
			m_localVeh = m_scenarioCtrl.CreateLocalVeh(transform.position, transform.rotation, id);

		
	}

	public override void Detached()
	{
		if (entity.IsOwner)
		{
			m_localVeh = null;
		}
		else
		{
			m_scenarioCtrl.DeleteLocalVeh(id);
			m_localVeh = null;
		}
		base.Detached();
	}

	public Vector3 position
	{
		get { return m_localVeh.position; }
		set { m_localVeh.position = value; }
	}

	public Quaternion rotation
	{
		get { return m_localVeh.rotation; }
		set { m_localVeh.rotation = value; }
	}

	public GameObject localGameObject
	{
		get { return m_localVeh.gameObject; }
	}

	// Update is called once per frame
	void Update ()
	{
		if (entity.IsAttached)
		{
			if (entity.IsOwner)
			{
				transform.position = m_localVeh.position;
				transform.rotation = m_localVeh.rotation;
			}
			else
			{
				m_localVeh.position = transform.position;
				m_localVeh.rotation = transform.rotation;
			}
		}
	}

}
