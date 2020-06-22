using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogGOPed : LogGO {
	public override void Play (LogItem item)
	{
		Debug.LogFormat("Updating pedestrian: {0}", item.id);
	}
}
