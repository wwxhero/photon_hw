using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JointsPoolBindLog : JointsPool
{
	static readonly string [] s_itemNames = {
		"MakeHuman default skeleton"
		, "male_genericMesh"
		, "short02Mesh"
		, "high-polyMesh"
		, "eyebrow009Mesh"
		, "eyelashes01Mesh"
		, "teeth_baseMesh"
		, "tongue01Mesh"
		, "shoes06Mesh"
		, "male_casualsuit01Mesh"
		, "root"
		, "spine05"
		, "pelvis.L"
		, "pelvis.R"
		, "spine04"
		, "upperleg01.L"
		, "upperleg01.R"
		, "spine03"
		, "upperleg02.L"
		, "upperleg02.R"
		, "spine02"
		, "lowerleg01.L"
		, "lowerleg01.R"
		, "breast.L"
		, "breast.R"
		, "spine01"
		, "lowerleg02.L"
		, "lowerleg02.R"
		, "clavicle.L"
		, "clavicle.R"
		, "neck01"
		, "foot.L"
		, "foot.R"
		, "shoulder01.L"
		, "shoulder01.R"
		, "neck02"
		, "toe1-1.L"
		, "toe2-1.L"
		, "toe3-1.L"
		, "toe4-1.L"
		, "toe5-1.L"
		, "toe1-1.R"
		, "toe2-1.R"
		, "toe3-1.R"
		, "toe4-1.R"
		, "toe5-1.R"
		, "upperarm01.L"
		, "upperarm01.R"
		, "neck03"
		, "toe1-2.L"
		, "toe2-2.L"
		, "toe3-2.L"
		, "toe4-2.L"
		, "toe5-2.L"
		, "toe1-2.R"
		, "toe2-2.R"
		, "toe3-2.R"
		, "toe4-2.R"
		, "toe5-2.R"
		, "upperarm02.L"
		, "upperarm02.R"
		, "head"
		, "toe2-3.L"
		, "toe3-3.L"
		, "toe4-3.L"
		, "toe5-3.L"
		, "toe2-3.R"
		, "toe3-3.R"
		, "toe4-3.R"
		, "toe5-3.R"
		, "lowerarm01.L"
		, "lowerarm01.R"
		, "jaw"
		, "levator02.L"
		, "levator02.R"
		, "special01"
		, "special03"
		, "special06.L"
		, "special06.R"
		, "temporalis01.L"
		, "temporalis01.R"
		, "temporalis02.L"
		, "temporalis02.R"
		, "lowerarm02.L"
		, "lowerarm02.R"
		, "special04"
		, "tongue00"
		, "levator03.L"
		, "levator03.R"
		, "oris04.L"
		, "oris04.R"
		, "oris06"
		, "levator06.L"
		, "levator06.R"
		, "special05.L"
		, "special05.R"
		, "oculi02.L"
		, "oculi02.R"
		, "risorius02.L"
		, "risorius02.R"
		, "wrist.L"
		, "wrist.R"
		, "oris02"
		, "oris06.L"
		, "oris06.R"
		, "tongue01"
		, "levator04.L"
		, "levator04.R"
		, "oris03.L"
		, "oris03.R"
		, "oris05"
		, "eye.L"
		, "orbicularis03.L"
		, "orbicularis04.L"
		, "eye.R"
		, "orbicularis03.R"
		, "orbicularis04.R"
		, "oculi01.L"
		, "oculi01.R"
		, "risorius03.L"
		, "risorius03.R"
		, "finger1-1.L"
		, "metacarpal1.L"
		, "metacarpal2.L"
		, "metacarpal3.L"
		, "metacarpal4.L"
		, "finger1-1.R"
		, "metacarpal1.R"
		, "metacarpal2.R"
		, "metacarpal3.R"
		, "metacarpal4.R"
		, "oris01"
		, "oris07.L"
		, "oris07.R"
		, "tongue02"
		, "tongue05.L"
		, "tongue05.R"
		, "levator05.L"
		, "levator05.R"
		, "finger1-2.L"
		, "finger2-1.L"
		, "finger3-1.L"
		, "finger4-1.L"
		, "finger5-1.L"
		, "finger1-2.R"
		, "finger2-1.R"
		, "finger3-1.R"
		, "finger4-1.R"
		, "finger5-1.R"
		, "tongue03"
		, "tongue06.L"
		, "tongue06.R"
		, "finger1-3.L"
		, "finger2-2.L"
		, "finger3-2.L"
		, "finger4-2.L"
		, "finger5-2.L"
		, "finger1-3.R"
		, "finger2-2.R"
		, "finger3-2.R"
		, "finger4-2.R"
		, "finger5-2.R"
		, "tongue04"
		, "tongue07.L"
		, "tongue07.R"
		, "finger2-3.L"
		, "finger3-3.L"
		, "finger4-3.L"
		, "finger5-3.L"
		, "finger2-3.R"
		, "finger3-3.R"
		, "finger4-3.R"
		, "finger5-3.R"
	};
	void Start()
	{
		Dictionary<string, Transform> name2transform = new Dictionary<string, Transform>();
		foreach (Transform j in m_joints)
		{
			name2transform[j.name] = j;
		}

		loggerSrvLib.Logger logger = new loggerSrvLib.Logger();

		logger.Create("Global_matrice.log");
		foreach(string name in s_itemNames)
		{
			logger.LogOut("    Item name: " + name + '\n');
			logger.LogOut("    Matrix value:" + '\n');
			Transform j = name2transform[name];
			if (null != j)
			{
				Matrix4x4 m = j.localToWorldMatrix;
				Matrix4x4 m_t = m.transpose;
				for (int k = 0; k < 4; k++)
				{
					logger.LogOut(string.Format("        {0,9:F4}, {1,9:F4}, {2,9:F4}, {3,9:F4}\n"
											 , m_t[k, 0], m_t[k, 1], m_t[k, 2], m_t[k, 3]));
				}

			}
			else
				logger.LogOut("            null");
		}
		logger.Close();

		logger.Create("Global_transform.log");
		foreach(string name in s_itemNames)
		{
			logger.LogOut("    Item name: " + name + '\n');
			Transform j = name2transform[name];
			if (null != j)
			{
				Vector3 p = j.position;
				logger.LogOut(string.Format("        {0,9:F4}, {1,9:F4}, {2,9:F4}\n"
											 , p[0], p[1], p[2]));
				Quaternion q = j.rotation;
				logger.LogOut(string.Format("        {0,9:F4}, {1,9:F4}, {2,9:F4}, {3,9:F4}\n"
											 , q.w, q.x, q.y, q.z));
				Vector3 s = j.lossyScale;
				logger.LogOut(string.Format("        {0,9:F4}, {1,9:F4}, {2,9:F4}\n"
											 , s[0], s[1], s[2]));
			}
			else
				logger.LogOut("            null");
		}
		logger.Close();

		logger.Create("Local_transform.log");
		foreach(string name in s_itemNames)
		{
			logger.LogOut("    Item name: " + name + '\n');
			Transform j = name2transform[name];
			if (null != j)
			{
				Vector3 p = j.localPosition;
				logger.LogOut(string.Format("        {0,9:F4}, {1,9:F4}, {2,9:F4}\n"
											 , p[0], p[1], p[2]));
				Quaternion q = j.localRotation;
				logger.LogOut(string.Format("        {0,9:F4}, {1,9:F4}, {2,9:F4}, {3,9:F4}\n"
											 , q.w, q.x, q.y, q.z));
				Vector3 s = j.localScale;
				logger.LogOut(string.Format("        {0,9:F4}, {1,9:F4}, {2,9:F4}\n"
											 , s[0], s[1], s[2]));

			}
			else
				logger.LogOut("            null");
		}
		logger.Close();


		logger = null;
	}

	private void Update()
	{
		foreach (Transform t in m_joints)
		{
			Matrix4x4 l2w = t.localToWorldMatrix;
			Matrix4x4 w2p = (null == t.parent ? Matrix4x4.identity : t.parent.worldToLocalMatrix);
			Matrix4x4 l2p = w2p * l2w;

			Matrix4x4 l2p_2 = Matrix4x4.TRS(t.localPosition, t.localRotation, t.localScale);
			Debug.Assert(l2p == l2p_2);
		}
	}

}