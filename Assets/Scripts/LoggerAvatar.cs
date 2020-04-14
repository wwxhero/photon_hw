using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoggerAvatar {
	loggerSrvLib.Logger m_logger;
	Transform m_tranform;
	List<Transform> m_lstTrans = new List<Transform>();
	readonly string [] c_fields = {"r_w", "r_x", "r_y", "r_z"
								, "t_x", "t_y", "t_z"};
	public void Initialize(Transform tran)
	{
		m_tranform = tran;
		m_logger = new loggerSrvLib.Logger();
		string name = string.Format("{0}.csv", m_tranform.name);
		m_logger.Create(name);
		Queue<Transform> queBFS = new Queue<Transform>();
		queBFS.Enqueue(m_tranform);
		//m_lstTrans.Add(m_tranform);
		string strHeader = string.Format("{0}.{1}, {0}.{2}, {0}.{3}, {0}.{4}, {0}.{5}, {0}.{6}, {0}.{7}"
										, m_tranform.name
										, c_fields[0], c_fields[1], c_fields[2], c_fields[3]
										, c_fields[4], c_fields[5], c_fields[6]);
		while (queBFS.Count > 0)
		{
			Transform p = queBFS.Dequeue();
			foreach (Transform c in p)
			{
				strHeader += string.Format(", {0}.{1}, {0}.{2}, {0}.{3}, {0}.{4}, {0}.{5}, {0}.{6}, {0}.{7}"
										, c.name
										, c_fields[0], c_fields[1], c_fields[2], c_fields[3]
										, c_fields[4], c_fields[5], c_fields[6]);
				queBFS.Enqueue(c);
				m_lstTrans.Add(c);
			}
		}
		strHeader += "\r\n";
		m_logger.LogOut(strHeader);
	}
	public void Logout()
	{
		Quaternion q = m_tranform.localRotation;
		Vector3 t = m_tranform.localPosition;
		string strItem = string.Format("{0,7:#.0000}, {1,7:#.0000}, {2,7:#.0000}, {3,7:#.0000}, {4,7:#.000}, {5,7:#.000}, {6,7:#.000}"
										, q.w, q.x, q.y, q.z
										, t.x, t.y, t.z);
		foreach (Transform tr in m_lstTrans)
		{
			q = tr.localRotation;
			t = tr.localPosition;
			strItem += string.Format(", {0,7:#.0000}, {1,7:#.0000}, {2,7:#.0000}, {3,7:#.0000}, {4,7:#.000}, {5,7:#.000}, {6,7:#.000}"
										, q.w, q.x, q.y, q.z
										, t.x, t.y, t.z);
		}
		strItem += "\r\n";
		m_logger.LogOut(strItem);
	}
	public void Close()
	{
		m_logger.Close();
		m_logger = null;
	}
}
