using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoggerVeh : LoggerObj {
	readonly string [] c_fields = {"time", "frame", "id"
								, "r_w", "r_x", "r_y", "r_z"
								, "t_x", "t_y", "t_z"};
	public Dictionary<int, LocalVehBehavior> m_refVehs;
	public void Initialize(ScenarioControl ctrl)
	{
		base.Initialize("vehi");
		string strHeader = c_fields[0];
		for (int i_f = 1; i_f < c_fields.Length; i_f ++)
			strHeader += string.Format(", {0}", c_fields[i_f]);
		strHeader += "\n";
		LogOutInPack(strHeader);
		m_refVehs = ctrl.m_Vehs;
	}

	public override void OnLogging()
	{
        if (null == m_refVehs) //not intialized
            return;
        string strLog = "";
		foreach (var reg_v in m_refVehs)
		{
			Quaternion rot = reg_v.Value.rotation;
			Vector3 pos = reg_v.Value.position;
			strLog += string.Format("{0}, {1}, {2}, {3,7:#.0000}, {4,7:#.0000}, {5,7:#.0000}, {6,7:#.0000}, {7,7:#.000}, {8,7:#.000}, {9,7:#.000}\n"
								, System.DateTime.Now.Ticks / System.TimeSpan.TicksPerMillisecond
								, Time.frameCount
								, reg_v.Key
								, rot.w, rot.x, rot.y, rot.z
								, pos.x, pos.y, pos.z);
		}
		LogOutInPack(strLog);
	}


}
