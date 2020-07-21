using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoggerAvatar_s : LoggerObj {
	List<Transform> m_lstTrans = new List<Transform>();
	bool m_local;
	readonly string [] c_fields = {"time", "frame"
								, "x", "y", "z"};
	public void Initialize(string[] joints, bool local)
	{
		m_local = local;
		HashSet<string> names = new HashSet<string>(joints);
		string name = transform.name + "_s";
		base.Initialize(name);
		string strHeader = string.Format("{0}, {1}", c_fields[0], c_fields[1]);

		strHeader += string.Format(", {0}.{1}, {0}.{2}, {0}.{3}"
												, transform.name
												, c_fields[2], c_fields[3], c_fields[4]);
		m_lstTrans.Add(transform);
		JointsPool.Traverse_d(
			  transform
			, (Transform this_t) => {
					if (names.Contains(this_t.name))
					{
						strHeader += string.Format(", {0}.{1}, {0}.{2}, {0}.{3}"
												, this_t.name
												, c_fields[2], c_fields[3], c_fields[4]);
						m_lstTrans.Add(this_t);
					}
				}
			, (Transform this_t) => { }
			);
		strHeader += "\n";
		LogOutInPack(strHeader);
	}

	public void LogOut()
	{
		bool initialized = (null != m_logger);
		if (!initialized)
			return;
		Vector3 s;
		string strItem = string.Format("{0}, {1}", System.DateTime.Now.Ticks / System.TimeSpan.TicksPerMillisecond, Time.frameCount);
		for (int i = 0; i < m_lstTrans.Count; i ++)
		{
			if (m_local)
			{
				s = m_lstTrans[i].localScale;
			}
			else
			{
                s = m_lstTrans[i].lossyScale;
			}
			strItem += string.Format(", {0,7:#.0000}, {1,7:#.0000}, {2,7:#.0000}"
										, s.x, s.y, s.z);
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