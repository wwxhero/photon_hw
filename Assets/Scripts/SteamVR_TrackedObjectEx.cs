//======= Copyright (c) Valve Corporation, All rights reserved. ===============
//
// Purpose: For controlling in-game objects with tracked devices.
//
//=============================================================================

using UnityEngine;
using Valve.VR;

public class SteamVR_TrackedObjectEx : SteamVR_TrackedObject
{
    LoggerObj m_logger;
    int m_cntFrame = 0;
    public SteamVR_TrackedObjectEx()
    {
        newPosesAction = SteamVR_Events.NewPosesAction(OnNewPoses);
    }

    private void Start()
    {
        m_logger = GetComponent<LoggerObj>();
    }

    private void OnNewPoses(TrackedDevicePose_t[] poses)
    {
        Debug.Assert(0 == m_cntFrame
                    || 1 == Time.frameCount - m_cntFrame);
        //make sure this function is called once perframe
        m_cntFrame = Time.frameCount;
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
    }

}

