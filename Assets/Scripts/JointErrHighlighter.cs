using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JointErrHighlighter : MonoBehaviour {
	[Range(0.1f, 2*Mathf.PI)] public float m_errNormMax = 2 * Mathf.PI;
	List<Mesh>			m_meshes = new List<Mesh>();
	List<ErrorTr[]>		m_jointErr = new List<ErrorTr[]>();
	readonly Color 		c_clrErr0 = Color.blue;
	readonly Color 		c_clrErr1 = Color.red;

	void UpdateVerticeClr(Mesh mesh, ErrorTr[] joints_err)
	{
		BoneWeight[] joint_wt = mesh.boneWeights;
		Color32[] verts_clr = mesh.colors32;
		Debug.Assert(joint_wt.Length == verts_clr.Length);
		for (int i_vert = 0; i_vert < verts_clr.Length; i_vert ++)
		{
			BoneWeight bw = joint_wt[i_vert];
			float [] w  = {bw.weight0
						 , bw.weight1
						 , bw.weight2
						 , bw.weight3};
			float [] e = {joints_err[bw.boneIndex0].Error
						, joints_err[bw.boneIndex1].Error
						, joints_err[bw.boneIndex2].Error
						, joints_err[bw.boneIndex3].Error};
			float e_sigma = ( w[0] * e[0]
						+ w[1] * e[1]
						+ w[2] * e[2]
						+ w[3] * e[3]) / m_errNormMax;
			verts_clr[i_vert] = Color32.Lerp(Color.blue, Color.red, e_sigma);
		}
		mesh.colors32 = verts_clr;
	}

	void Start()
	{
		foreach (Transform t_child in transform)
		{
			SkinnedMeshRenderer renderer = null;
			Mesh mesh = null;
			Transform [] joints = null;
			BoneWeight [] bws = null;

			if (null != (renderer = t_child.gameObject.GetComponent<SkinnedMeshRenderer>())
				&& null != (mesh = renderer.sharedMesh)
				&& null != (joints = renderer.bones)
				&& null != (bws = mesh.boneWeights))
			{
				// Create a material with transparent diffuse shader
				Material material = new Material(Shader.Find("Avatar_Vis/Vertex_Color"));
				renderer.material = material;
				Debug.Assert(bws.Length == mesh.vertices.Length);
				Color32[] clrs = new Color32[bws.Length];
				mesh.colors32 = clrs;
				ErrorTr [] error_tr = new ErrorTr[joints.Length];
				for (int i = 0; i < joints.Length; i ++)
				{
					var error_tr_i = joints[i].gameObject.GetComponent<ErrorTr>();
					if (null == error_tr_i)
						error_tr_i = joints[i].gameObject.AddComponent<ErrorTr>();
					error_tr[i] = error_tr_i;
				}
				m_meshes.Add(mesh);
				m_jointErr.Add(error_tr);
				UpdateVerticeClr(mesh, error_tr);
			}
		}
	}

	void Update()
	{
		Debug.Assert(m_meshes.Count == m_jointErr.Count);

		for (int i_mesh = 0; i_mesh < m_meshes.Count; i_mesh ++)
		{
			Mesh mesh = m_meshes[i_mesh];
			ErrorTr[] joint_err = m_jointErr[i_mesh];
			UpdateVerticeClr(mesh, joint_err);
		}
	}
}