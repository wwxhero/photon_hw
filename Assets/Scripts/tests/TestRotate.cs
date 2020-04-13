using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRotate : Bolt.EntityBehaviour<IJointState> {

	// Use this for initialization
	public Transform [] m_refTran;

	public override void Attached()
	{
		state.SetTransforms(state.JointTransform, transform);
	}

	void Start () {

	}

	// Update is called once per frame
	void Update () {
		if (!entity.IsAttached
		 || !entity.IsOwner
		 ||	null == m_refTran)
			return;

		Transform refTran = null;
		if (true)
		{
			if (Input.GetKeyDown(KeyCode.Alpha0))
				refTran = m_refTran[0];
			else if (Input.GetKeyDown(KeyCode.Alpha1))
				refTran = m_refTran[1];
			else if (Input.GetKeyDown(KeyCode.Alpha2))
				refTran = m_refTran[2];
		}
		if (null != refTran)
		{
			const float c_angleDelta = 5;
            refTran.Rotate(new Vector3(0, c_angleDelta, 0));
		}

	}
}
