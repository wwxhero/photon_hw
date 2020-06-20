using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Transform_log
{
	Vector3 pos;
	Quaternion ori;
};

public class LogItem
{
	public int id;
	public enum LogType {ped, veh};
	public LogType type;
	public int nFrame;
	public double ticks; //in millisecond
	public Transform_log [] transforms;

	static int s_id = 0;
	static void Parse4Veh(string path, Dictionary<int, LogItem> [] records)
	{
		Debug.Assert(false);
	}

	static void Parse4Ped(string path, Dictionary<int, LogItem> [] records)
	{
		List<LogItem> rawRecords = new List<LogItem>();

	}

	public static void Parse(LogType type, string path, Dictionary<int, LogItem> [] records)
	{
		switch(type)
		{
			case LogType.ped:
			{
				Parse4Ped(path, records);
				break;
			}
			case LogType.veh:
			{
				Parse4Veh(path, records);
				break;
			}
		}
		s_id ++;
	}
};


