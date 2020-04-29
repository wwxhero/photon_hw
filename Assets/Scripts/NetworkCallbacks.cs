using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bolt.Samples.GettingStarted
{
	[BoltGlobalBehaviour("Tutorial1")]
	//FHWA ScenarioControl Corresponding class
	public class NetworkCallbacks : Bolt.GlobalEventListener
	{
		List<string> logMessages = new List<string>();
		bool m_debug = true;

		class Node_d
		{
			Transform m_this;
			int m_iNextChild = 0;
			public Node_d(Transform t)
			{
				m_this = t;
			}
			public string name {
				get {return m_this.name;}
			}
			public Vector3 pos {
				get { return m_this.position; }
			}
			public Quaternion rot {
				get { return m_this.rotation; }
			}
			public Transform transform {
				get { return m_this; }
			}
			public Node_d nextChild()
			{
				if (m_iNextChild < m_this.childCount)
				{
					Transform child_t = m_this.GetChild(m_iNextChild ++);
					return new Node_d(child_t);
				}
				else
					return null;
			}
		};

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

		void LogTreeNode(Stack<Node_d> dfs)
		{
			string item = "";
			for (int i = 0; i < dfs.Count; i ++)
				item += "\t";
			item += dfs.Peek().name;
			DebugLog.Warning(item);
		}

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

		delegate void Enter(Transform this_t);
		delegate void Leave(Transform this_t);
		void Traverse_d(Transform root_t, Enter onEnter, Leave onLeave)
		{
			Stack<Node_d> dfs_stk = new Stack<Node_d>();
			Node_d root_n = new Node_d(root_t);
			dfs_stk.Push(root_n);
			onEnter(root_n.transform);
			if (m_debug)
				LogTreeNode(dfs_stk);
			while (dfs_stk.Count > 0)
			{
				Node_d n_p = dfs_stk.Peek();
				Node_d n_c = n_p.nextChild();
				if (null == n_c)
				{
					dfs_stk.Pop();
					onLeave(n_p.transform);
				}
				else
				{
					dfs_stk.Push(n_c);
					onEnter(n_c.transform);
					if (m_debug)
						LogTreeNode(dfs_stk);
				}
			}
		}

		public override void SceneLoadLocalDone(string a_scene)
		{
			GameObject scenario_obj = GameObject.FindGameObjectWithTag("scene");
			Debug.Assert(null != scenario_obj);
			ScenarioControl scenario_ctrl = scenario_obj.GetComponent<ScenarioControl>();
			scenario_ctrl.LoadLocalAvatar();

			Transform root_t = scenario_ctrl.m_ownPed.transform;
			Stack<Pair<Transform, BoltEntity>> bind_stk = new Stack<Pair<Transform, BoltEntity>>();
			BoltEntity root_e = BoltNetwork.Instantiate(BoltPrefabs.Joint
													, root_t.position
													, root_t.rotation);
			root_e.transform.name = root_t.name;
			bind_stk.Push(new Pair<Transform, BoltEntity>(root_t, root_e));
			HashSet<string> names = new HashSet<string>(scenario_ctrl.m_lstNetworkingJoints);

			Traverse_d(root_t
					, (Transform this_t) =>
						{
							if (names.Contains(this_t.name))
							{
								BoltEntity e_p = bind_stk.Peek().Second;
								BoltEntity e_c = BoltNetwork.Instantiate(BoltPrefabs.Joint
													, this_t.position
													, this_t.rotation);
								e_c.transform.name = this_t.name;
								e_c.SetParent(e_p);
								bind_stk.Push(new Pair<Transform, BoltEntity>(this_t, e_c));
							}
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


	}
}