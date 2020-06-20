using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoggerAvatar : MonoBehaviour {
	int c_logPackCap = 512;
	loggerSrvLib.Logger m_logger;
	List<Transform> m_lstTrans = new List<Transform>();
	bool m_local;
	readonly string [] c_fields = {"time", "frame"
								, "r_w", "r_x", "r_y", "r_z"
								, "t_x", "t_y", "t_z"};
	public void Initialize(string[] joints, bool local)
	{
		m_local = local;
		HashSet<string> names = new HashSet<string>(joints);
		m_logger = new loggerSrvLib.Logger();
		string name = string.Format("{0}.csv", transform.name);
		m_logger.Create(name);
		string strHeader = string.Format("{0}, {1}", c_fields[0], c_fields[1]);

		strHeader += string.Format(", {0}.{1}, {0}.{2}, {0}.{3}, {0}.{4}, {0}.{5}, {0}.{6}, {0}.{7}"
												, transform.name
												, c_fields[2], c_fields[3], c_fields[4], c_fields[5]
												, c_fields[6], c_fields[7], c_fields[8]);
		m_lstTrans.Add(transform);
		JointsPool.Traverse_d(
			  transform
			, (Transform this_t) => {
					if (names.Contains(this_t.name))
					{
						strHeader += string.Format(", {0}.{1}, {0}.{2}, {0}.{3}, {0}.{4}, {0}.{5}, {0}.{6}, {0}.{7}"
												, this_t.name
												, c_fields[2], c_fields[3], c_fields[4], c_fields[5]
												, c_fields[6], c_fields[7], c_fields[8]);
						m_lstTrans.Add(this_t);
					}
				}
			, (Transform this_t) => { }
			);
		strHeader += "\n";
		LogOutInPack(strHeader);
	}

	void Update()
	{
		bool initialized = (null != m_logger);
		if (!initialized)
			return;
		Quaternion q;
		Vector3 t;
		if (m_local)
		{
			q = m_lstTrans[0].localRotation;
			t = m_lstTrans[0].localPosition;
		}
		else
		{
			q = m_lstTrans[0].rotation;
			t = m_lstTrans[0].position;
		}
		string strItem = string.Format("{0}, {1}", System.DateTime.Now.Ticks / System.TimeSpan.TicksPerMillisecond, Time.frameCount);
		for (int i = 0; i < m_lstTrans.Count; i ++)
		{
			if (m_local)
			{
				q = m_lstTrans[i].localRotation;
				t = m_lstTrans[i].localPosition;
			}
			else
			{
				q = m_lstTrans[i].rotation;
				t = m_lstTrans[i].position;
			}
			strItem += string.Format(", {0,7:#.0000}, {1,7:#.0000}, {2,7:#.0000}, {3,7:#.0000}, {4,7:#.000}, {5,7:#.000}, {6,7:#.000}"
										, q.w, q.x, q.y, q.z
										, t.x, t.y, t.z);
		}
		strItem += "\n";
		LogOutInPack(strItem);
	}

	void OnDestroy()
	{
		if (null != m_logger)
			m_logger.Close();
		m_logger = null;
	}

	void LogOutInPack(string item)
	{
		int n = item.Length;
		int L = c_logPackCap;
		for (int i = 0; i < n; i += L)
		{
			string item_i = item.Substring(i, Mathf.Min(n-i, L));
			m_logger.LogOut(item_i);
		}
	}
}
