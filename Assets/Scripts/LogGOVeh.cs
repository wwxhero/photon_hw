using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogGOVeh : LogGO {
	readonly Quaternion c_qOffsetVeh = new Quaternion(Mathf.Sin(-Mathf.PI / 4), 0, 0, Mathf.Cos(-Mathf.PI / 4));
	public override void Play (LogItem item)
	{
		Debug.Assert(item.transforms.Length == 1);
		transform.rotation = item.transforms[0].ori * c_qOffsetVeh;
		transform.position = item.transforms[0].pos;
	}
}
