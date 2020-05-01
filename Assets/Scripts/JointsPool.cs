using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JointsPool : MonoBehaviour
{
	const bool m_debug = true;
	[HideInInspector] public List<Transform> m_joints = new List<Transform>();
	class Node_d
	{
		Transform m_this;
		int m_iNextChild = 0;
		public Node_d(Transform t)
		{
			m_this = t;
		}
		public string name
		{
			get { return m_this.name; }
		}
		public Vector3 pos
		{
			get { return m_this.position; }
		}
		public Quaternion rot
		{
			get { return m_this.rotation; }
		}
		public Transform transform
		{
			get { return m_this; }
		}
		public Node_d nextChild()
		{
			if (m_iNextChild < m_this.childCount)
			{
				Transform child_t = m_this.GetChild(m_iNextChild++);
				return new Node_d(child_t);
			}
			else
				return null;
		}
	};

	static void LogTreeNode(Stack<Node_d> dfs)
	{
		string item = "";
		for (int i = 0; i < dfs.Count; i++)
			item += "\t";
		item += dfs.Peek().name;
		DebugLog.Warning(item);
	}

	public delegate void Enter(Transform this_t);
	public delegate void Leave(Transform this_t);
	public static void Traverse_d(Transform root_t, Enter onEnter, Leave onLeave)
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


	// Use this for initialization
	void Awake()
	{
        Traverse_d(transform
                , (Transform this_t) => { m_joints.Add(this_t); }
                , (Transform this_t) => { }
                );
	}


}
