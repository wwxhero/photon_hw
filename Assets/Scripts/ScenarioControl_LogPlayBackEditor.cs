using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ScenarioControl_LogPlayBack))]
public class ScenarioControl_LogPlayBackEditor : Editor
{
	SerializedProperty playProp;
	SerializedProperty nFrameMaxProp;
	SerializedProperty nFrameMinProp;
	SerializedProperty nFrameProp;

	void OnEnable()
	{
		// Setup the SerializedProperties.
		playProp = serializedObject.FindProperty ("m_play");
		nFrameMinProp = serializedObject.FindProperty("c_nFrameBase");
		nFrameMaxProp = serializedObject.FindProperty("c_nFrameMax");
		nFrameProp = serializedObject.FindProperty("m_nFrame");
	}
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		GUILayout.BeginHorizontal();
		string [] btns = {"Play", "Pause"};
		int i_btn = playProp.boolValue ? 1 : 0;
		if (GUILayout.Button(btns[i_btn], GUILayout.Width(60)))
		{
			playProp.boolValue = !playProp.boolValue;
			serializedObject.ApplyModifiedProperties ();
		}
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		int nFrame = EditorGUILayout.IntSlider("Frame"
								, nFrameProp.intValue
								, nFrameMinProp.intValue
								, nFrameMaxProp.intValue);
		if (nFrame != nFrameProp.intValue)
		{
			nFrameProp.intValue = nFrame;
			serializedObject.ApplyModifiedProperties();
		}
		GUILayout.EndHorizontal();
	}
};