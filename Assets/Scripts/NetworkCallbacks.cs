using System.Collections;
using System.Collections.Generic;
using UdpKit;
using UnityEngine;

namespace Bolt.Samples.GettingStarted
{
	[BoltGlobalBehaviour("Tutorial1")]

	public class NetworkCallbacks : Bolt.GlobalEventListener
	{
		List<string> logMessages = new List<string>();
		const bool m_debug = true;
		public class Pair<T, U>
		{
			public Pair()
			{
			}

			public Pair(T first, U second)
			{
				this.First = first;
				this.Second = second;
			}

			public T First { get; set; }
			public U Second { get; set; }
		};

		void LogTreeTransformRecur(Transform p, int indent = 0)
		{
			string item = "";
			for (int i = 0; i < indent; i++)
				item += "\t";
			item += p.name;
			DebugLog.Warning(item);
			int indent_prime = indent + 1;
			foreach (Transform c in p)
				LogTreeTransformRecur(c, indent_prime);
		}

		void LogTreeNode2(BoltEntity root, int indent = 0)
		{
			LogTreeTransformRecur(root.transform);
		}

		public override void SceneLoadLocalDone(string a_scene)
		{
			GameObject scenario_obj = GameObject.FindGameObjectWithTag("scene");
			Debug.Assert(null != scenario_obj);
			ScenarioControl scenario_ctrl = scenario_obj.GetComponent<ScenarioControl>();
			scenario_ctrl.Initialize(BoltNetwork.IsServer);
			int pedId = scenario_ctrl.m_ownPedId;
			int jointId = 0;

			Transform root_t = scenario_ctrl.m_ownPed.transform;
			Stack<Pair<Transform, BoltEntity>> bind_stk = new Stack<Pair<Transform, BoltEntity>>();
			var root_tok = new LocalJointId
			{
				pedId = pedId,
				jointId = jointId
			};
			BoltEntity root_e = BoltNetwork.Instantiate(BoltPrefabs.Joint_n
													, root_tok
													, root_t.position
													, root_t.rotation);
			root_e.transform.name = root_t.name;
			bind_stk.Push(new Pair<Transform, BoltEntity>(root_t, root_e));
			HashSet<string> names = new HashSet<string>(ScenarioControl.s_lstNetworkingJoints);

			JointsPool.Traverse_d(root_t
					, (Transform this_t) =>
						{
							if (names.Contains(this_t.name))
							{
								BoltEntity e_p = bind_stk.Peek().Second;
								var e_tok = new LocalJointId
								{
									pedId = pedId,
									jointId = jointId
								};
								BoltEntity e_c = BoltNetwork.Instantiate(BoltPrefabs.Joint_n
													, e_tok
													, this_t.position
													, this_t.rotation);
								e_c.transform.name = this_t.name;
								e_c.SetParent(e_p);
								bind_stk.Push(new Pair<Transform, BoltEntity>(this_t, e_c));
							}
							jointId ++;
						}
					, (Transform this_t) =>
						{
							if (bind_stk.Peek().First == this_t)
								bind_stk.Pop();
						}
					);
			if (m_debug)
				LogTreeNode2(root_e);

		}

		public override void OnEvent(LogEvent evnt)
		{
			logMessages.Insert(0, evnt.Message);
		}

		public override void OnEvent(JointScaleEvent evnt)
		{
			GameObject scenario_obj = GameObject.FindGameObjectWithTag("scene");
			Debug.Assert(null != scenario_obj);
			ScenarioControl scenario_ctrl = scenario_obj.GetComponent<ScenarioControl>();
			GameObject ped = scenario_ctrl.m_Peds[evnt.pedId];
			JointsPool joints = ped.GetComponent<JointsPool>();
			Debug.Assert(joints.m_joints.Count > evnt.jointId);
			joints.m_joints[evnt.jointId].localScale = evnt.scale;
		}

		void OnGUI()
		{
			// only display max the 5 latest log messages
			int maxMessages = Mathf.Min(5, logMessages.Count);

			GUILayout.BeginArea(new Rect(Screen.width / 2 - 200, Screen.height - 100, 400, 100), GUI.skin.box);

			for (int i = 0; i < maxMessages; ++i)
			{
				GUILayout.Label(logMessages[i]);
			}

			GUILayout.EndArea();
		}

		public static void ScaleJoints(Transform[] localJoints)
		{
			GameObject scenario_obj = GameObject.FindGameObjectWithTag("scene");
			Debug.Assert(null != scenario_obj);
			ScenarioControl scenario_ctrl = scenario_obj.GetComponent<ScenarioControl>();
			int pedId = scenario_ctrl.m_ownPedId;
			int jointId = 0;

			Transform root_t = scenario_ctrl.m_ownPed.transform;

			HashSet<Transform> joints = new HashSet<Transform>(localJoints);

			JointsPool.Traverse_d(root_t
					, (Transform this_t) =>
						{
							if (joints.Contains(this_t))
							{
								var scale_event = JointScaleEvent.Create(GlobalTargets.Others);
								scale_event.pedId = pedId;
								scale_event.jointId = jointId;
								scale_event.scale = this_t.localScale;
								scale_event.Send();
							}
							jointId ++;
						}
					, (Transform this_t) =>
						{
						}
					);
		}

		public override void BoltShutdownBegin(AddCallback registerDoneCallback, UdpConnectionDisconnectReason disconnectReason)
		{
			foreach (var entity in BoltNetwork.Entities)
            {
                BoltNetwork.Destroy(entity.gameObject);
            }
            base.BoltShutdownBegin(registerDoneCallback, disconnectReason);           
        }


	}
}