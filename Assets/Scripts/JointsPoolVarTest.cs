using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JointsPoolVarTest : JointsPool
{
	class Transform2
	{
		Quaternion m_q0;
		Vector3 m_p0;
		Transform m_target;
		public Transform target
		{
			get { return m_target; }
		}
		bool m_changed = false;
		public Transform2(Transform target)
		{
			m_target = target;
			m_p0 = target.localPosition;
			m_q0 = target.localRotation;
		}
		public void CheckChanged()
		{
			if (m_changed)
				return;
			else
			{
				Quaternion q_n = m_target.localRotation;
				const float epsilon_q_l = 1f-.001f;
				const float epsilon_q_r = 1f+.001f;
				float cos_q = q_n.x * m_q0.x
							+ q_n.y * m_q0.y
							+ q_n.z * m_q0.z
							+ q_n.w * m_q0.w;
				bool equal_q = (cos_q < epsilon_q_r && cos_q > epsilon_q_l);
				Vector3 p_n = m_target.localPosition;
				const float epsilon_p = 0.001f;
				bool equal_p = ((p_n - m_p0).magnitude < epsilon_p);
				m_changed = !(equal_p && equal_q);
			}
		}
		public bool Changed()
		{
			return m_changed;
		}

	};
	Transform2 [] m_joints0;

	public void Test4VariableJoints()
	{
		m_joints0 = new Transform2[m_joints.Count];
		for (int i = 0; i < m_joints.Count; i ++)
		{
			m_joints0[i] = new Transform2(m_joints[i]);
		}
	}

	public List<Transform> VarJoints()
	{
		List<Transform> varJoints = new List<Transform>();
		foreach (Transform2 t2 in m_joints0)
		{
			if (t2.Changed())
				varJoints.Add(t2.target);
		}
		return varJoints;
	}

	void Update()
	{
		bool testing4VariableJoints = (null != m_joints0);
		if (testing4VariableJoints)
		{
			for (int i = 0; i < m_joints0.Length; i ++)
			{
				m_joints0[i].CheckChanged();
			}
		}
	}
};