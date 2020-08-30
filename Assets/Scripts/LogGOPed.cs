using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogGOPed : LogGO {

	Transform [] m_joints;
	bool m_localjoint = true;
	public void Initialize(bool localJoint)
	{
		HashSet<string> names = new HashSet<string>(ScenarioControl_LogPlayBack.s_lstNetworkingJoints);
		m_joints = new Transform[ScenarioControl_LogPlayBack.s_lstNetworkingJoints.Length + 1];
		int i_joint = 0;
		m_joints[i_joint] = transform;
		i_joint ++;
		JointsPool.Traverse_d(transform
			, (Transform this_t) => {
                    string name = this_t.name.Trim();
					if (names.Contains(name))
					{
						m_joints[i_joint ++] = this_t;
					}
				}
			, (Transform this_t) => {
				});
		m_localjoint = localJoint;
	}
	public override void Play (LogItem item)
	{
		Debug.Assert(item.transforms.Length == m_joints.Length);
		if (m_localjoint)
		{
			for (int i = 0; i < m_joints.Length; i ++)
			{
				m_joints[i].localPosition = item.transforms[i].pos;
				m_joints[i].localRotation = item.transforms[i].ori;
				m_joints[i].localScale = item.transforms[i].scl;
			}
		}
		else
		{
			for (int i = 0; i < m_joints.Length; i ++)
			{
				m_joints[i].position = item.transforms[i].pos;
				m_joints[i].rotation = item.transforms[i].ori;
				//m_joints[i].scale = item.transforms[i].scl;
			}
		}
	}
}
