using RootMotion.FinalIK;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenarioControl_VRIKAccuEva_mockVR : MonoBehaviour {

	public GameObject m_refPhysical;
	public GameObject m_refHostIk;
	public Rect _opButtonRect;
	string [] s_title = {"test for variable joints", "log joints", "quit"};
	public RuntimeAnimatorController[] m_refControllers;
	int m_state = 0;



	void OnGUI()
	{
		if (m_state < s_title.Length)
		{
			if (GUI.Button(_opButtonRect, s_title[m_state]))
			{
				GameObject [] gos = {m_refPhysical, m_refHostIk};
				if (0 == m_state)
				{
					RootMotion.FinalIK.VRIK ik = m_refHostIk.GetComponent<RootMotion.FinalIK.VRIK>();
					MockPhysics trackers_mp = m_refPhysical.GetComponent<MockPhysics>();
					Debug.Assert((int)MockPhysics.Mount.total == trackers_mp.m_trackersMt.Length);
					VRIKCalibrator2.Calibrate(ik
						, trackers_mp.m_trackersMt[(int)MockPhysics.Mount.head]
						, trackers_mp.m_trackersMt[(int)MockPhysics.Mount.body]
						, trackers_mp.m_trackersMt[(int)MockPhysics.Mount.lh]
						, trackers_mp.m_trackersMt[(int)MockPhysics.Mount.rh]
						, trackers_mp.m_trackersMt[(int)MockPhysics.Mount.lf]
						, trackers_mp.m_trackersMt[(int)MockPhysics.Mount.rf]);
					Animator anim = m_refPhysical.GetComponent<Animator>();
					anim.runtimeAnimatorController = m_refControllers[m_state];
					anim.enabled = true;
					foreach (GameObject go in gos)
					{
						JointsPoolVarTest joints = go.GetComponent<JointsPoolVarTest>();
						joints.Test4VariableJoints();
					}
					m_state ++;
				}
				else if(1 == m_state)
				{
					Dictionary<string, Object> names = new Dictionary<string, Object>();
					DebugLog.Warning("Variable Joints: {");
					for (int i_go = 0; i_go < gos.Length; i_go ++)
					{
						GameObject go = gos[i_go];
						JointsPoolVarTest joints = go.GetComponent<JointsPoolVarTest>();
						foreach (Transform j in joints.VarJoints())
						{
							DebugLog.Format("\t{0}:\t{1}", i_go, j.name);
							names[j.name] = null;
						}
					}
					string [] names_arr = new string[names.Count];
					int n_name = 0;

					foreach (var name in names)
					{
						names_arr[n_name ++] = name.Key;
						DebugLog.Format("\tU:\t{0}", name.Key);
					}
					DebugLog.Warning("}");


					foreach (GameObject go in gos)
					{
						LoggerAvatar logger = go.GetComponent<LoggerAvatar>();
						logger.Initialize(names_arr, false);
					}

					Animator anim = m_refPhysical.GetComponent<Animator>();
					anim.runtimeAnimatorController = m_refControllers[m_state];
					m_state ++;
				}
				else
				{
					Application.Quit();
					m_state++;
				}
			}
		}

	}
}
