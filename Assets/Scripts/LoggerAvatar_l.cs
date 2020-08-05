using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoggerAvatar_l : LoggerObj {
	List<Transform> m_lstTrans = new List<Transform>();
	bool m_local;
	readonly string [] c_fields = {"time", "frame"};
	public void Initialize(string[] joints, bool local)
	{
		m_local = local;
		HashSet<string> names = new HashSet<string>(joints);
		string name = transform.name + "_l";
		base.Initialize(name);
		string strHeader = string.Format("{0}, {1}", c_fields[0], c_fields[1]);

		strHeader += string.Format(", {0}"
												, transform.name);
		m_lstTrans.Add(transform);
		JointsPool.Traverse_d(
			  transform
			, (Transform this_t) => {
					if (names.Contains(this_t.name))
					{
						strHeader += string.Format(", {0}"
												, this_t.name);
						m_lstTrans.Add(this_t);
					}
				}
			, (Transform this_t) => { }
			);
		strHeader += "\n";
		LogOutInPack(strHeader);

		LogOut();
	}

	void LogOut()
	{
		bool initialized = (null != m_logger);
		if (!initialized)
			return;
		Vector3 s;
		string strItem = string.Format("{0}, {1}", System.DateTime.Now.Ticks / System.TimeSpan.TicksPerMillisecond, Time.frameCount);
		for (int i = 0; i < m_lstTrans.Count; i ++)
		{
			Transform t_this = m_lstTrans[i];
			Transform t_root = transform;
			float len = 0;
			while (t_this != t_root)
			{
				len += t_this.localPosition.magnitude;
				t_this = t_this.parent;
			}
			strItem += string.Format(", {0,7:#.0000}"
										, len);
		}
		strItem += "\n";
		LogOutInPack(strItem);
	}

    public override void OnLogging(bool delayed)
	{
		//do nothing for scaling information
		//, scaling logging happens only on occations
		//, rather than per-frame
	}
}