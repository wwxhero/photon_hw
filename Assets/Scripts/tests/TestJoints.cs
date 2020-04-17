using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestJoints : Bolt.EntityBehaviour<IJointState> {

	// Use this for initialization
	private List<Transform> m_lstChain = new List<Transform>()
	public override void Attached()
	{
		state.SetTransforms(state.JointTransform, transform);
		if (null == transform.parent)
		{
			Queue<Transform> queBFS = new Queue<Transform>();
			queBFS.Enqueue(transform);
			while (queBFS.Count > 0)
			{
				Transform p = queBFS.Dequeue();
				m_lstChain.Add(p);
				foreach (Transform c in p)
				{
					queBFS.Enqueue(c);
				}
			}
		}
	}


	// Update is called once per frame
	void Update () {
		if (!entity.IsAttached
		 || !entity.IsOwner
		 || !(m_lstChain.Count > 0))
			return;

		Transform refTran = null;
		for (int nKey = (int)KeyCode.Alpha0; nKey < (int)KeyCode.Alpha9 + 1; nKey ++)
		{
			if (Input.GetKeyDown((KeyCode)nKey))
			{
				refTran = m_lstChain[nKey - (int)KeyCode.Alpha0];
			}
		}
		if (null != refTran)
		{
			const float c_angleDelta = 5;
            refTran.Rotate(new Vector3(0, c_angleDelta, 0));
		}
	}

}
