using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ScenarioControl_LogPlayBack))]
public class ScenarioControl_LogPlayBackEditor : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		//GUILayout.BeginHorizontal();
		//if (GUILayout.Button("Play", GUILayout.Width(60)))
		//	Debug.Log("Playing: " + target.name);
		//else if (GUILayout.Button("Pause", GUILayout.Width(100)))
		//	Debug.Log("Pause: " + target.name);
		//GUILayout.EndHorizontal();

	}
};