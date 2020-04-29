using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Net;
using System.Net.Sockets;

public class ScenarioControl : MonoBehaviour
{
	public enum LAYER { scene_static = 8, peer_dynamic, host_dynamic, ego_dynamic, marker_dynamic };
	public class ConfAvatar
	{
		public float Height
		{
			get { return height; }
			set { height = value; }
		}
		public float WingSpan
		{
			get { return width; }
			set { width = value; }
		}
		public float Width
		{
			get { return width + (2 * hand0) * width / width0; }
		}
		public float Depth
		{
			get { return depth; }
		}
		private float height;
		private float width;
		private float depth;
		private float perceptualHeight;
		private const float height0 = 1.77f;
		private const float width0 = 1.40f;
		private const float hand0 = 0.25f;

		public ConfAvatar(float a_height, float a_width, float a_depth)
		{
			height = a_height;
			width = a_width;
			depth = a_depth;
			perceptualHeight = height;
		}

		public float ScaleInv(float dh)
		{
			float h_prime = perceptualHeight + dh;
			float s_h_inv = perceptualHeight / h_prime;
			perceptualHeight = h_prime;
			return s_h_inv;
		}

		public void Apply(RootMotion.FinalIK.VRIK ped)
		{
			float s_h = height / height0;
			float s_w = width / (width0 * s_h);
			ped.references.root.localScale = new Vector3(s_h, s_h, s_h);
			ped.references.leftShoulder.localScale = new Vector3(1f, s_w, 1f);
			ped.references.rightShoulder.localScale = new Vector3(1f, s_w, 1f);
		}

		public void Apply(GameObject mockPhysic, RootMotion.FinalIK.VRIK ped)
		{
			float s_h = height / height0;
			float s_w = width / (width0 * s_h);
			MockPhysics mp = mockPhysic.GetComponent<MockPhysics>();
			ped.references.root.localScale = new Vector3(s_h, s_h, s_h);
			ped.references.leftShoulder.localScale = new Vector3(1f, s_w, 1f);
			ped.references.rightShoulder.localScale = new Vector3(1f, s_w, 1f);
			mp.transform.localScale = new Vector3(s_h, s_h, s_h);
			mp.m_leftShoulder.localScale = new Vector3(1f, s_w, 1f);
			mp.m_rightShoulder.localScale = new Vector3(1f, s_w, 1f);
		}

		public static string s_mockIp = "mockIp";
		public static string s_pedestrian = "pedestrian";
		public static string s_ip = "ip";
		public static string s_name = "name";
		public static string s_avatar = "avatar";
		public static string s_height = "height";
		public static string s_width = "width";
		public static string s_depth = "depth";

		public void DbgLog()
		{

			string log = string.Format("height:{0}\twidth:{1}\tdepth:{2}"
							, height, width, depth);
			DebugLog.Warning(log);
		}
	};
	public ConfAvatar m_confAvatar;
	public GameObject m_pedPrefab;
	bool m_debug = true;
	bool m_mockIp = false;
	[HideInInspector] public GameObject m_ownPed;
	[HideInInspector] public int m_ownPedId;
	[HideInInspector] public Dictionary<int, GameObject> m_Peds = new Dictionary<int, GameObject>();
	[HideInInspector] public string[] m_lstNetworkingJoints = {
									"root",				"upperleg01.R",		"lowerleg01.L",		"spine02",
									"foot.R",  			"clavicle.R",		"neck02",			"upperarm01.R",
									"lowerarm01.L", 	"wrist.L", 			"upperleg01.L", 	"spine04",
									"lowerleg01.R",		"foot.L",			"clavicle.L",		"toe2-1.L",
									"upperarm01.L",		"head",				"lowerarm01.R",		"wrist.R",
									"toe2-1.R"
								};
	// Use this for initialization
	public void LoadLocalAvatar()
	{
		GameObject scenario_obj = GameObject.FindGameObjectWithTag("scene");
		Debug.Assert(null != scenario_obj);
		ScenarioControl scenario_ctrl = scenario_obj.GetComponent<ScenarioControl>();
		try
		{
			XmlDocument scene = new XmlDocument();
			scene.Load("SceneDistri.xml");
			XmlNode root = scene.DocumentElement;
			HashSet<int> localIps = new HashSet<int>();
			if (m_mockIp)
			{
				XmlElement e_mIp = (XmlElement)root;
				XmlAttribute mip_attr = e_mIp.GetAttributeNode(ConfAvatar.s_mockIp);
				IPAddress ip = IPAddress.Parse(mip_attr.Value);
				localIps.Add(ip.GetHashCode());
			}
			else
			{
				var host = Dns.GetHostEntry(Dns.GetHostName());
				foreach (var ip in host.AddressList)
				{
					if (ip.AddressFamily == AddressFamily.InterNetwork)
					{
						localIps.Add(ip.GetHashCode());
					}
				}
				if (!(localIps.Count > 0))
					throw new Exception("No network adapters with an IPv4 address in the system!");
			}
			XmlNodeList children = root.ChildNodes;
			for (int i_node = 0; i_node < children.Count; i_node++)
			{
				XmlNode n_child = children[i_node];
				if (ConfAvatar.s_avatar == n_child.Name)
				{
					XmlElement e_avatar = (XmlElement)n_child;
					XmlAttribute height_attr = e_avatar.GetAttributeNode(ConfAvatar.s_height);
					XmlAttribute width_attr = e_avatar.GetAttributeNode(ConfAvatar.s_width);
					XmlAttribute depth_attr = e_avatar.GetAttributeNode(ConfAvatar.s_depth);
					float height = float.Parse(height_attr.Value);
					float width = float.Parse(width_attr.Value);
					float depth = float.Parse(depth_attr.Value);
					m_confAvatar = new ConfAvatar(height, width, depth);
					XmlNodeList children_avatar = n_child.ChildNodes;
					if (null != children_avatar)
					{
						for (int i = 0; i < children_avatar.Count; i++)
						{
							XmlNode child_avatar = children_avatar[i];
							if (ConfAvatar.s_pedestrian == child_avatar.Name)
							{
								XmlElement e_ped = (XmlElement)child_avatar;
								XmlAttribute ip_ped_attr = e_ped.GetAttributeNode(ConfAvatar.s_ip);
								XmlAttribute name_ped_attr = e_ped.GetAttributeNode(ConfAvatar.s_name);
								XmlElement telElement = (XmlElement)child_avatar;
								string[] name = { "x", "y", "z", "i", "j", "k" };
								float[] val = new float[name.Length];
								for (int i_attr = 0; i_attr < name.Length; i_attr++)
								{
									XmlElement v_text = telElement[name[i_attr]];
									val[i_attr] = float.Parse(v_text.InnerXml);
								}
								Vector3 p = new Vector3(val[0], val[1], val[2]);
								Vector3 r = new Vector3(val[3], val[4], val[5]);
								int idPed = IPAddress.Parse(ip_ped_attr.Value).GetHashCode();
								bool ownPed = (localIps.Contains(idPed));
								if (m_debug)
								{
									DebugLog.Format("CreatePed({0}, {1}, {2}, {3})\n"
										, ownPed.ToString()
										, name_ped_attr.Value
										, p.ToString()
										, r.ToString());
								}
								Quaternion q = Quaternion.Euler(r);
								GameObject ped = Instantiate(m_pedPrefab, p, q);
								ped.name = name_ped_attr.Value;
								if (ownPed)
								{
									m_ownPed = ped;
									m_ownPedId = idPed;
								}
								m_Peds[idPed] = ped;
							}
						}
					}
				}
				//m_confMap = new ConfMap(transform);
			}
			setLayer(gameObject, LAYER.scene_static);
		}
		catch (System.IO.FileNotFoundException)
		{
			DebugLog.Error("scene load failed!");
		}
		catch(ArgumentNullException e)
		{
			DebugLog.Error("IP is not defined for the pedestrian!");
		}
		catch(FormatException e)
		{
			string strError = "FormatException caught!!!";
			strError += string.Format("Source : {0}", e.Source);
			strError += string.Format("Message : {0}", e.Message);
			DebugLog.Error(strError);
		}
		catch (Exception e)
		{
			DebugLog.Error("scene load failed!");
			DebugLog.Error(e.Message);
		}
		if (m_debug)
			m_confAvatar.DbgLog();

	}


	public void setLayer(GameObject o, LAYER l)
	{
		Queue<Transform> bfs = new Queue<Transform>();
		bfs.Enqueue(o.transform);
		while (bfs.Count > 0)
		{
			Transform t = bfs.Dequeue();
			t.gameObject.layer = (int)l;
			foreach (Transform t_c in t)
				bfs.Enqueue(t_c);
		}
	}
}
