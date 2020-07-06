using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogGOPed : LogGO {

	Transform [] m_joints;
	public void Initialize()
	{
		HashSet<string> names = new HashSet<string>(ScenarioControl.s_lstNetworkingJoints);
		m_joints = new Transform[ScenarioControl.s_lstNetworkingJoints.Length + 1];
		int i_joint = 0;
		m_joints[i_joint] = transform;
		i_joint ++;
		JointsPool.Traverse_d(transform
			, (Transform this_t) => {
					if (names.Contains(this_t.name))
					{
						m_joints[i_joint ++] = this_t;
					}
				}
			, (Transform this_t) => {
				});
	}
	public override void Play (LogItem item)
	{
		Debug.Assert(item.transforms.Length == m_joints.Length);
		for (int i = 0; i < m_joints.Length; i ++)
		{
			m_joints[i].localPosition = item.transforms[i].pos;
			m_joints[i].localRotation = item.transforms[i].ori;
			m_joints[i].localScale = item.transforms[i].scl;
		}
	}
}
