﻿//======= Copyright (c) Valve Corporation, All rights reserved. ===============
//
// Purpose: For controlling in-game objects with tracked devices.
//
//=============================================================================

using UnityEngine;
using Valve.VR;

public class SteamVR_TrackedObject : MonoBehaviour
{
	public enum EIndex
	{
		None = -1,
		Hmd = (int)OpenVR.k_unTrackedDeviceIndex_Hmd,
		Device1,
		Device2,
		Device3,
		Device4,
		Device5,
		Device6,
		Device7,
		Device8,
		Device9,
		Device10,
		Device11,
		Device12,
		Device13,
		Device14,
		Device15
	}

	public EIndex index;

	[Tooltip("If not set, relative to parent")]
	public Transform origin;

	public bool isValid { get; protected set; }

	private bool m_lock = false;
	private Vector3 m_posAct = new Vector3(0, 0, 0);
	private Quaternion m_rotAct = Quaternion.identity;
	public bool Lock(bool lok)
	{
		bool lock_prev = m_lock;
		m_lock = (lok || (index == EIndex.None)); //a fake tracker is alway locked
		if (m_lock)
		{
			transform.position = m_posDft;
			transform.rotation = m_rotDft;
		}
		else
		{
			transform.position = m_posAct;
			transform.rotation = m_rotAct;
		}
		return lock_prev;
	}
	private Vector3 m_posDft = new Vector3(0, 0, 0);
    private Quaternion m_rotDft = Quaternion.identity;
	public void SetDft(Vector3 p, Quaternion r)
	{
		m_posDft = p;
		m_rotDft = r;
	}
	public void Lock(Vector3 p, Quaternion r)
	{
		m_lock = true;
		transform.position = p;
		transform.rotation = r;
	}

	public bool Locked()
	{
		bool actLock = (transform.position == m_posDft
					&& transform.rotation == m_rotDft);
		return m_lock || actLock;
	}

	private void OnNewPoses(TrackedDevicePose_t[] poses)
	{
		if (index == EIndex.None)
			return;

		var i = (int)index;

		isValid = false;
		if (poses.Length <= i)
			return;

		if (!poses[i].bDeviceIsConnected)
			return;

		if (!poses[i].bPoseIsValid)
			return;

		Vector3 pos_m = transform.position;
		Quaternion rot_m = transform.rotation;
		isValid = true;

		var pose = new SteamVR_Utils.RigidTransform(poses[i].mDeviceToAbsoluteTracking);



		if (origin != null)
		{
			transform.position = origin.transform.TransformPoint(pose.pos);
			transform.rotation = origin.rotation * pose.rot;
		}
		else
		{
			transform.localPosition = pose.pos;
			transform.localRotation = pose.rot;
		}

		m_posAct = transform.position;
		m_rotAct = transform.rotation;
		if (m_lock)
		{
			transform.position = pos_m;
			transform.rotation = rot_m;
		}
	}

	protected SteamVR_Events.Action newPosesAction;

	protected SteamVR_TrackedObject()
	{
		newPosesAction = SteamVR_Events.NewPosesAction(OnNewPoses);
	}

	void OnEnable()
	{
		var render = SteamVR_Render.instance;
		if (render == null)
		{
			enabled = false;
			return;
		}

		newPosesAction.enabled = true;
	}

	void OnDisable()
	{
		newPosesAction.enabled = false;
		isValid = false;
	}

	public void SetDeviceIndex(int index)
	{
		if (System.Enum.IsDefined(typeof(EIndex), index))
			this.index = (EIndex)index;
	}
}

