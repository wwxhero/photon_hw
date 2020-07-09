using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Net;
using System.Net.Sockets;
using Bolt.Samples.GettingStarted;
using Veh = System.Collections.Generic.KeyValuePair<int, UnityEngine.GameObject>;

public class ScenarioControl : MonoBehaviour
{
	public GameObject [] m_vehsPrefab;
	public GameObject m_pedPrefab;
	public GameObject m_camInspectorPrefab;
	public GameObject m_mockTrackersPrefab;
	GameObject m_trackers;
	Camera m_egoInspector;
	Quaternion c_qOffsetVeh = new Quaternion(Mathf.Sin(-Mathf.PI/4), 0, 0, Mathf.Cos(-Mathf.PI/4));
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

		public void Apply(RootMotion.FinalIK.VRIK ped, bool notifyNetwork = true)
		{
			float s_h = height / height0;
			float s_w = width / (width0 * s_h);
			ped.references.root.localScale = new Vector3(s_h, s_h, s_h);
			ped.references.leftShoulder.localScale = new Vector3(1f, s_w, 1f);
			ped.references.rightShoulder.localScale = new Vector3(1f, s_w, 1f);
			if (notifyNetwork)
				NetworkCallbacks.ScaleJoints(new Transform[]{ped.references.root
													, ped.references.leftShoulder
													, ped.references.rightShoulder});
			LoggerAvatar_s logger = ped.gameObject.GetComponent<LoggerAvatar_s>();
			logger.LogOut();
		}

		public void Apply(GameObject mockPhysic, RootMotion.FinalIK.VRIK ped, bool notifyNetwork = true)
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
			if (notifyNetwork)
				NetworkCallbacks.ScaleJoints(new Transform[]{ped.references.root
													, ped.references.leftShoulder
													, ped.references.rightShoulder});
			LoggerAvatar_s logger = ped.gameObject.GetComponent<LoggerAvatar_s>();
			logger.LogOut();
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
	public class ConfVehical
	{
		public float Width = 2.06f;
		public float Height = 1.55f;
		public float Depth = 5.15f;
	};
	public class ConfMap
	{
		Vector3 center;
		float width, height, depth;
		public ConfMap(Transform t)
		{
			Transform bcube = t.Find("Bcube");
			Debug.Assert(null != bcube);
			if (null == bcube)
				throw new Exception("no bounding cube defined for map!");
			center = bcube.localPosition;
			Vector3 size = bcube.lossyScale;
			width = size.x;
			height = size.y;
			depth = size.z;
		}
		public float Height
		{
			get { return height; }
		}
		public float Width
		{
			get { return width; }
		}
		public float Depth
		{
			get { return depth; }
		}
		public Vector3 Center
		{
			get { return center; }
		}
	};
	public class InspectorHelper
	{
		struct BBOX
		{
			public Vector3 center;
			public float halfWidth, halfHeight, halfDepth;
		};
		BBOX m_bbox;
		public enum ObjType { Host, Ego, Map };
		ObjType m_type;
		Transform m_target;
		public InspectorHelper(Transform target, ConfAvatar conf)
		{
			m_type = ObjType.Ego;
			m_target = target;
			Vector3 s_l = target.localScale;
			m_bbox.center = new Vector3(0, (conf.Height * 0.5f) / s_l.y, 0);
			m_bbox.halfWidth = conf.Width * 0.5f;
			m_bbox.halfHeight = conf.Height * 0.5f;
			m_bbox.halfDepth = conf.Depth * 0.5f;
		}

		public InspectorHelper(Transform target, ConfVehical conf)
		{
			m_type = ObjType.Host;
			m_target = target;
			m_bbox.center = new Vector3(0, 0, conf.Height * 0.5f);
			m_bbox.halfWidth = conf.Width * 0.5f;
			m_bbox.halfHeight = conf.Height * 0.5f;
			m_bbox.halfDepth = conf.Depth * 0.5f;
		}

		public InspectorHelper(Transform target, ConfMap conf)
		{
			m_type = ObjType.Map;
			m_target = target;
			m_bbox.center = conf.Center;
			m_bbox.halfWidth = conf.Width * 0.5f;
			m_bbox.halfHeight = conf.Height * 0.5f;
			m_bbox.halfDepth = conf.Depth * 0.5f;
		}
		public enum Direction { front = 0, up, right, back, down, left };

		public void Apply(Camera cam, Direction dir)
		{
			//fixme: put camera in the specific direction of the target
			float[] camSize = {
				  Mathf.Max(m_bbox.halfWidth, m_bbox.halfHeight)
				, Mathf.Max(m_bbox.halfWidth, m_bbox.halfDepth)
				, Mathf.Max(m_bbox.halfHeight, m_bbox.halfDepth)
				, Mathf.Max(m_bbox.halfWidth, m_bbox.halfHeight)
				, Mathf.Max(m_bbox.halfWidth, m_bbox.halfDepth)
				, Mathf.Max(m_bbox.halfHeight, m_bbox.halfDepth)
			};
			int host_mask = 1 << (int)LAYER.host_dynamic;
			int ego_mask = 1 << (int)LAYER.ego_dynamic;
			int static_mask = 1 << (int)LAYER.scene_static;
			int dyn_mask = 1 << (int)LAYER.peer_dynamic;
			int dyn_marker_mask = 1 << (int)LAYER.marker_dynamic;

			if (ObjType.Host == m_type)
				cam.cullingMask = host_mask | ego_mask;
			else if (ObjType.Ego == m_type)
				cam.cullingMask = ego_mask;
			else // ObjType.Map == m_type
				cam.cullingMask = static_mask | dyn_marker_mask;

			cam.orthographic = true;
			cam.orthographicSize = camSize[(int)dir];

			cam.transform.parent = m_target;
			float c_distance = 100f;
			Vector3[] t_l = null;
			Vector3 u_l;
			if (ObjType.Host == m_type)
			{
				t_l = new Vector3[] {
						  new Vector3(0, -1, 0)
						, new Vector3(0,  0, 1)
						, new Vector3(1, 0, 0)
						, new Vector3(0, 1, 0)
						, new Vector3(0,  0, -1)
						, new Vector3(-1, 0, 0)
					};
				u_l = new Vector3(0, 0, 1);
			}
			else if (ObjType.Ego == m_type)
			{
				t_l = new Vector3[] {
						  new Vector3(0, 0, 1)
						, new Vector3(0, 1, 0)
						, new Vector3(1, 0, 0)
						, new Vector3(0, 0, -1)
						, new Vector3(0, -1, 0)
						, new Vector3(-1, 0, 0)
					};
				u_l = new Vector3(0, 1, 0);
			}
			else //if (ObjType.Map == m_type)
			{
				t_l = new Vector3[] {
						  new Vector3(0, 1, 0)
						, new Vector3(0, 1, 0)
						, new Vector3(0, 1, 0)
						, new Vector3(0, 1, 0)
						, new Vector3(0, 1, 0)
						, new Vector3(0, 1, 0)
					};
				u_l = new Vector3(0, 0, -1);
			}

			Quaternion r_l = Quaternion.LookRotation(-t_l[(int)dir], u_l);
			Vector3 p_l = t_l[(int)dir] * c_distance + m_bbox.center;
			cam.transform.localPosition = p_l;
			cam.transform.localRotation = r_l;
		}
	};
	[HideInInspector] public ConfAvatar m_confAvatar;
	ConfVehical m_confVehicle = new ConfVehical(); //fixme: driving vehicle size is hardcoded
	ConfMap m_confMap;
	bool m_debug = true;
	bool m_mockIp = true;
	bool m_mockVeh = true;
	[HideInInspector] public GameObject m_ownPed;
	[HideInInspector] public int m_ownPedId;
	[HideInInspector] public Dictionary<int, GameObject> m_Peds = new Dictionary<int, GameObject>();
	[HideInInspector] public static string[] s_lstNetworkingJoints = {
									"root",				"upperleg01.R",		"lowerleg01.L",		"spine02",
									"foot.R",  			"clavicle.R",		"neck02",			"upperarm01.R",
									"lowerarm01.L", 	"wrist.L", 			"upperleg01.L", 	"spine04",
									"lowerleg01.R",		"foot.L",			"clavicle.L",		"toe2-1.L",
									"upperarm01.L",		"head",				"lowerarm01.R",		"wrist.R",
									"toe2-1.R",			"upperleg02.L",		"upperleg02.R"
								};
	Dictionary<int, GameObject> m_Vehs = new Dictionary<int, GameObject>();
	int s_vehId = 0;
	// Use this for initialization
	public void Initialize(bool isServer)
	{
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
									DebugLog.InfoFormat("CreatePed({0}, {1}, {2}, {3})\n"
										, ownPed.ToString()
										, name_ped_attr.Value
										, p.ToString()
										, r.ToString());
								}
								Quaternion q = Quaternion.Euler(r);
								GameObject ped = Instantiate(m_pedPrefab, p, q);
								ped.name = name_ped_attr.Value;
								LoggerAvatar logger = ped.AddComponent<LoggerAvatar>();
								logger.Initialize(s_lstNetworkingJoints, true);
								LoggerAvatar_s logger_s = ped.AddComponent<LoggerAvatar_s>();
								logger_s.Initialize(s_lstNetworkingJoints, true);
								if (ownPed)
								{
									m_ownPed = ped;
									m_ownPedId = idPed;

									RootMotion.FinalIK.VRIK ik = ped.AddComponent<RootMotion.FinalIK.VRIK>();
									ped.AddComponent<RootMotion.FinalIK.VRIKBackup>();
									ik.AutoDetectReferences();

									bool mockTracking = (null != m_mockTrackersPrefab);
									GameObject steamVR = GameObject.Find("[CameraRig]");
									Debug.Assert(null != steamVR);
									SteamVR_Manager mgr = steamVR.GetComponent<SteamVR_Manager>();
									mgr.Avatar = ped;
									mgr.DEF_MOCKSTEAM = mockTracking;
									if (mockTracking)
									{
										m_trackers = Instantiate(m_mockTrackersPrefab, p, q);
										RootMotion.Demos.VRIKCalibrationController caliCtrl = ped.AddComponent<RootMotion.Demos.VRIKCalibrationController>();
										caliCtrl.ik = ik;
										MockPhysics trackers_mp = m_trackers.GetComponent<MockPhysics>();
										Debug.Assert((int)MockPhysics.Mount.total == trackers_mp.m_trackersMt.Length);
										caliCtrl.headTracker = trackers_mp.m_trackersMt[(int)MockPhysics.Mount.head];
										caliCtrl.bodyTracker = trackers_mp.m_trackersMt[(int)MockPhysics.Mount.body];
										caliCtrl.leftHandTracker = trackers_mp.m_trackersMt[(int)MockPhysics.Mount.lh];
										caliCtrl.rightHandTracker = trackers_mp.m_trackersMt[(int)MockPhysics.Mount.rh];
										caliCtrl.leftFootTracker = trackers_mp.m_trackersMt[(int)MockPhysics.Mount.lf];
										caliCtrl.rightFootTracker = trackers_mp.m_trackersMt[(int)MockPhysics.Mount.rf];
										m_confAvatar.Apply(m_trackers, ik, false);
									}
									else
									{
										m_trackers = steamVR;
										Debug.Assert(null != m_confAvatar);
										m_confAvatar.Apply(ik, false);
									}
									setLayer(ped, LAYER.ego_dynamic);
									setLayer(m_trackers, LAYER.ego_dynamic);
									//no matter driver or pedestrain, by default, inspector is on avatar
									InspectorHelper inspector = new InspectorHelper(ped.transform, m_confAvatar);
									m_egoInspector = Instantiate(m_camInspectorPrefab).GetComponent<Camera>();
									inspector.Apply(m_egoInspector, InspectorHelper.Direction.front);
								}
								m_Peds[idPed] = ped;
							}
						}
					}
				}
				//m_confMap = new ConfMap(transform);
			}
			setLayer(gameObject, LAYER.scene_static);
			m_confMap = new ConfMap(transform);
		}
		catch (System.IO.FileNotFoundException)
		{
			DebugLog.Error("scene load failed!");
		}
		catch(ArgumentNullException)
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

		transform.Find("MockVehControl").gameObject.SetActive(isServer && m_mockVeh);
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


	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space)
			&& null != m_mockTrackersPrefab)
		{
			Animator player = m_trackers.GetComponent<Animator>();
			player.enabled = !(player.enabled);
		}
	}

	public void adjustInspector(InspectorHelper.Direction dir, InspectorHelper.ObjType type)
	{
		InspectorHelper inspector = null;
		if (type == InspectorHelper.ObjType.Host)
			inspector = new InspectorHelper(m_ownPed.transform.parent, m_confVehicle);
		else if (type == InspectorHelper.ObjType.Ego)
			inspector = new InspectorHelper(m_ownPed.transform, m_confAvatar);
		else if (type == InspectorHelper.ObjType.Map)
			inspector = new InspectorHelper(transform, m_confMap);
		inspector.Apply(m_egoInspector, dir);
	}

	public void viewInspec()
	{
		m_egoInspector.gameObject.SetActive(true);
		adjustInspector(ScenarioControl.InspectorHelper.Direction.front, InspectorHelper.ObjType.Ego);
	}

	public void viewHmd()
	{
		m_egoInspector.gameObject.SetActive(false);
	}

	public void viewMap()
	{
		m_egoInspector.gameObject.SetActive(true);
		adjustInspector(ScenarioControl.InspectorHelper.Direction.up, InspectorHelper.ObjType.Map);
	}

	public Veh CreateVeh(Vector3 pos, Quaternion rot)
	{
		//fixme: make it networked
		Debug.Assert(m_vehsPrefab.Length > 0);
		int id = s_vehId ++;

		m_Vehs[id] = (Instantiate(m_vehsPrefab[id % m_vehsPrefab.Length], pos, rot * c_qOffsetVeh));
		Veh veh = new Veh(id, m_Vehs[id]);
		return veh;
	}

	public void DeleteVeh(int vid)
	{
		GameObject veh = null;
		bool find = m_Vehs.TryGetValue(vid, out veh);
		Debug.Assert(find);
		if (find)
		{
			Destroy(veh);
			m_Vehs.Remove(vid);
		}
	}
}
