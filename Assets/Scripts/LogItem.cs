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

	public static void Parse(LogType type, string path, Dictionary<int, LogItem> [] records)
	{
		Debug.Assert(false); //to be completed!!!
	}
};


