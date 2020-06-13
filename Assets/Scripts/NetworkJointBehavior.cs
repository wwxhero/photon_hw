using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkJointBehavior : Bolt.EntityBehaviour<IJointState> {

	const bool m_debug = true;
	Transform m_tranLocal;
	// Use this for initialization
	public override void Attached()
	{
		state.SetTransforms(state.JointTransform, transform);
		var jointId = (LocalJointId)entity.AttachToken;
		if (m_debug)
			DebugLog.InfoFormat("binding start:({0}, {1})", jointId.pedId, jointId.jointId);
		GameObject scenario_obj = GameObject.FindGameObjectWithTag("scene");
		Debug.Assert(null != scenario_obj);
		ScenarioControl scenario_ctrl = scenario_obj.GetComponent<ScenarioControl>();
		GameObject ped = scenario_ctrl.m_Peds[jointId.pedId];
		JointsPool joints = ped.GetComponent<JointsPool>();
		Debug.Assert(joints.m_joints.Count > jointId.jointId);
		m_tranLocal = joints.m_joints[jointId.jointId];
		if (m_debug)
			DebugLog.InfoFormat("binding end:({0}, {1})", ped.name, m_tranLocal.name);
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
