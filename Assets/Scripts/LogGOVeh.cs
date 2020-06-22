using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogGOVeh : LogGO {
	public override void Play (LogItem item)
	{
		Debug.LogFormat("Updating vehicle: {0}", item.id);
	}
}
