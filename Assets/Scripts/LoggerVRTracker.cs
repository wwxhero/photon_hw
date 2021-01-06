using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoggerVRTracker : LoggerObj {
	SteamVR_TrackedObjectEx m_tracker;
	readonly string [] c_fields = {"time", "frame"
								, "m11", "m12", "m13", "m14"
								, "m21", "m22", "m23", "m24"
								, "m31", "m32", "m33", "m34"
								, "v1", "v2", "v3"
								, "a1", "a2", "a3"
								, "t_res"};
	void Start()
	{
		base.Initialize(transform.name);
		string strHeader = c_fields[0];
		for (int i_f = 1; i_f < c_fields.Length; i_f ++)
			strHeader += string.Format(", {0}", c_fields[i_f]);
		strHeader += "\n";
		LogOutInPack(strHeader);
		m_tracker = GetComponent<SteamVR_TrackedObjectEx>();
	}

	public override void OnLogging(bool delayed)
	{
        if (null == m_tracker
            || !delayed) //not intialized
            return;
		var m = m_tracker.m_pose.mDeviceToAbsoluteTracking;
		var v = m_tracker.m_pose.vVelocity;
		var av = m_tracker.m_pose.vAngularVelocity;
		int res = (int)m_tracker.m_pose.eTrackingResult;
		string strLog = string.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}, {14}, {15}, {16}, {17}, {18}, {19}, {20}\n"
							, System.DateTime.Now.Ticks / System.TimeSpan.TicksPerMillisecond
							, Time.frameCount
							, m.m0, m.m1, m.m2, m.m3
							, m.m4, m.m5, m.m6, m.m7
							, m.m8, m.m9, m.m10, m.m11
							, v.v0, v.v1, v.v2
							, av.v0, av.v1, av.v2
							, res);
		LogOutInPack(strLog);
	}
}