using RootMotion.FinalIK;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenarioControl_VRIKAccuEva : MonoBehaviour
{
	public GameObject m_refPhysical;
	public GameObject m_refHostIk;
	public Rect _opButtonRect;
	string[] s_title = { "test for variable joints", "log joints", "quit" };
	public RuntimeAnimatorController[] m_refControllers;
	int m_state = 0; //0: s_title[0]; 1: s_title[1]; 2: s_title[2]

	Transform corres(Transform root_t, Transform root_ik, Transform ref_ik)
	{
		string path = ref_ik.name;
		for (Transform t = ref_ik.parent; t != root_ik; t = t.parent)
		{
			path = t.name + '/' + path;
		}
		return root_t.Find(path);
	}

	void AssociatePhy_IK()
	{
		RootMotion.FinalIK.VRIK ik = m_refHostIk.AddComponent<RootMotion.FinalIK.VRIK>();
        ik.AutoDetectReferences();
        Transform targets_root = m_refPhysical.transform;
		Transform ik_root = m_refHostIk.transform;
		ik.solver.spine.headTarget = corres(targets_root, ik_root, ik.references.head);
		ik.solver.spine.pelvisTarget = corres(targets_root, ik_root, ik.references.pelvis);
		ik.solver.spine.pelvisPositionWeight = 1;
		ik.solver.spine.pelvisRotationWeight = 1;
		ik.solver.plantFeet = false;
		ik.solver.spine.maxRootAngle = 180f;
		ik.solver.leftArm.target = corres(targets_root, ik_root, ik.references.leftHand);
		ik.solver.leftArm.positionWeight = 1f;
		ik.solver.leftArm.rotationWeight = 1f;
		ik.solver.rightArm.target = corres(targets_root, ik_root, ik.references.rightHand);
		ik.solver.rightArm.positionWeight = 1f;
		ik.solver.rightArm.rotationWeight = 1f;
		ik.solver.leftLeg.target = corres(targets_root, ik_root, ik.references.leftToes);
		ik.solver.leftLeg.positionWeight = 1f;
		ik.solver.leftLeg.rotationWeight = 1f;
		//ik.solver.leftLeg.bendGoal = corres(targets_root, ik_root, ik.references.leftCalf);
		ik.solver.leftLeg.bendGoalWeight = 1f;
		ik.solver.rightLeg.target = corres(targets_root, ik_root, ik.references.rightToes);
		ik.solver.rightLeg.positionWeight = 1f;
		ik.solver.rightLeg.rotationWeight = 1f;
		//ik.solver.rightLeg.bendGoal = corres(targets_root, ik_root, ik.references.rightCalf);
		ik.solver.rightLeg.bendGoalWeight = 1f;

		var rootController = ik.references.root.GetComponent<VRIKRootController>();

		if (rootController == null)
			rootController = ik.references.root.gameObject.AddComponent<VRIKRootController>();
		rootController.Calibrate();


		ik.solver.spine.minHeadHeight = 0f;
		ik.solver.locomotion.weight = 0f;
		ik.LockSolver(false);
	}

	void OnGUI()
	{
		if (m_state < s_title.Length)
		{
			if (GUI.Button(_opButtonRect, s_title[m_state]))
			{
				GameObject[] gos = { m_refPhysical, m_refHostIk };
				if (0 == m_state)
				{
					AssociatePhy_IK();
					Animator anim = m_refPhysical.GetComponent<Animator>();
					anim.runtimeAnimatorController = m_refControllers[m_state];
					anim.enabled = true;
					foreach (GameObject go in gos)
					{
						JointsPoolVarTest joints = go.GetComponent<JointsPoolVarTest>();
						joints.Test4VariableJoints();
					}
					m_state++;
				}
				else if (1 == m_state)
				{
					Dictionary<string, Object> names = new Dictionary<string, Object>();
					DebugLog.Warning("Variable Joints: {");
					for (int i_go = 1; i_go < gos.Length; i_go++)
					{
						GameObject go = gos[i_go];
						JointsPoolVarTest joints = go.GetComponent<JointsPoolVarTest>();
						foreach (Transform j in joints.VarJoints())
						{
							DebugLog.InfoFormat("\t{0}:\t{1}", i_go, j.name);
							names[j.name] = null;
						}
					}
					string[] names_arr = new string[names.Count];
					int n_name = 0;

					foreach (var name in names)
					{
						names_arr[n_name++] = name.Key;
						DebugLog.InfoFormat("\tU:\t{0}", name.Key);
					}
					DebugLog.Warning("}");


					foreach (GameObject go in gos)
					{
						LoggerAvatar logger = go.AddComponent<LoggerAvatar>();
						logger.Initialize(names_arr, false);
						LoggerAvatar_s logger_s = go.AddComponent<LoggerAvatar_s>();
						logger_s.Initialize(names_arr, false);
						LoggerAvatar_l logger_l = go.AddComponent<LoggerAvatar_l>();
						logger_l.Initialize(names_arr, false);
					}
					Animator anim = m_refPhysical.GetComponent<Animator>();
					anim.runtimeAnimatorController = m_refControllers[m_state];
					m_state++;
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
