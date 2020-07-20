using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalVehBehavior : MonoBehaviour {

    // Use this for initialization
    readonly Quaternion c_qOffsetVeh = new Quaternion(Mathf.Sin(-Mathf.PI / 4), 0, 0, Mathf.Cos(-Mathf.PI / 4));
    readonly Quaternion c_qOffsetVehInv = new Quaternion(Mathf.Sin(Mathf.PI / 4), 0, 0, Mathf.Cos(Mathf.PI / 4));
    int m_id = -1;

    public Vector3 position
    {
        get { return transform.position; }
        set { transform.position = value; }
    }

    public Quaternion rotation
    {
        get { return transform.rotation * c_qOffsetVehInv; }
        set { transform.rotation = value * c_qOffsetVeh; }
    }

    public int id
    {
        get { return m_id; }
        set { m_id = value; }
    }

}
