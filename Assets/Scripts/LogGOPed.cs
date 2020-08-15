using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogGOPed : LogGO {
	Transform [] m_joints;
	ErrorTr [] m_jointerrs;
	bool m_localjoint = true;
	public void Initialize(bool localJoint)
	{
		HashSet<string> names = new HashSet<string>(ScenarioControl_LogPlayBack.s_lstNetworkingJoints);
		m_joints = new Transform[ScenarioControl_LogPlayBack.s_lstNetworkingJoints.Length + 1];
		m_jointerrs = new ErrorTr[ScenarioControl_LogPlayBack.s_lstNetworkingJoints.Length + 1];
		int i_joint = 0;
		m_joints[i_joint] = transform;
		ErrorTr err_joint = transform.gameObject.GetComponent<ErrorTr>();
		if (null == err_joint)
			err_joint = transform.gameObject.AddComponent<ErrorTr>();
		m_jointerrs[i_joint] = err_joint;
		i_joint ++;
		JointsPool.Traverse_d(transform
			, (Transform this_t) => {
					string name = this_t.name.Trim();
					if (names.Contains(name))
					{
						m_joints[i_joint] = this_t;
						err_joint = this_t.gameObject.GetComponent<ErrorTr>();
						if (null == err_joint)
							err_joint = this_t.gameObject.AddComponent<ErrorTr>();
						m_jointerrs[i_joint] = err_joint;
						i_joint ++;
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
				m_jointerrs[i].Error = item.transforms[i].err;
			}
		}
		else
		{
			for (int i = 0; i < m_joints.Length; i ++)
			{
				m_joints[i].position = item.transforms[i].pos;
				m_joints[i].rotation = item.transforms[i].ori;
				Debug.Assert(2.0f == item.transforms[i].scl.x
							&& 2.0f == item.transforms[i].scl.y
							&& 2.0f == item.transforms[i].scl.z);
				//m_joints[i].lossyScale = item.transforms[i].scl;
				m_jointerrs[i].Error = item.transforms[i].err;
			}
		}
	}
}
