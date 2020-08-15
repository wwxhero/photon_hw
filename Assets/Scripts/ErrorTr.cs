using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ErrorTr : MonoBehaviour {
	public float m_ErrorDisplay;
	float m_Error = -1f;
	ErrorTr m_parent;
	public float Error {
		get {
            if (m_Error < 0)
            {
                m_ErrorDisplay = (null == m_parent) ? 0 : m_parent.Error;
                if (null == m_parent)
                    return 0;
                else
                    return m_parent.Error;
            }
            else
            {
                m_ErrorDisplay = m_Error;
                return m_Error;
            }
		}
		set {
			m_Error = value;
		}
	}

	void Start()
	{
		Transform parent_t = transform.parent;
		while (null != parent_t
			&& transform.root != parent_t
			&& null == m_parent)
		{
			m_parent = parent_t.gameObject.GetComponent<ErrorTr>();
			parent_t = parent_t.parent;
		}
	}
}