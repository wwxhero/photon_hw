using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LogItem_n = System.Collections.Generic.Dictionary<int, LogItem>;

public class LogPlayBack : MonoBehaviour {
	public string [] m_logFiles;
	public LogItem.LogType [] m_logTypes;
    [RangeAttribute(0, 300)]
    public int m_nFrame = 0;
    public bool m_play = false;

    LogItem_n [] m_records;
	int c_nFrameBase = 0;
	HashSet<int> m_rc = new HashSet<int>();
	HashSet<int> m_rcPrime = new HashSet<int>();
	// Use this for initialization
	void Start () {
		Debug.Assert(m_logFiles.Length == m_logTypes.Length);
		for (int log_i = 0; log_i < m_logFiles.Length; log_i ++)
			LogItem.Parse(m_logTypes[log_i], m_logFiles[log_i], m_records);
	}

	// Update is called once per frame
	void Update () {
		if (m_play)
		{
			m_nFrame ++;
			m_play = UpdateFrame();
            if (!m_play)
                m_nFrame --;
		}
	}

	void UpdateObject(LogItem item)
	{
		Debug.Assert(false); //not yet done!!!
	}

	void DeleteObject(int id)
	{
		Debug.Assert(false); //not yet done!!!
	}

	void CreateObject(LogItem item)
	{
		Debug.Assert(false); //to be completed
	}

	bool UpdateFrame()
	{
		int i_offset = m_nFrame - c_nFrameBase;
		if (i_offset < 0)
			return true;
		else if(null != m_records 
            && m_nFrame < m_records.Length)
		{
			LogItem_n items = m_records[m_nFrame];
			foreach (KeyValuePair<int, LogItem> pair in items)
				m_rcPrime.Add(pair.Key);

			HashSet<int> Ids_update = new HashSet<int>(m_rc);
			Ids_update.IntersectWith(m_rcPrime);
			foreach (int id_update in Ids_update)
				UpdateObject(items[id_update]);

			HashSet<int> Ids_delete = new HashSet<int>(m_rc);
			Ids_delete.ExceptWith(m_rcPrime);
			foreach (int id_delete in Ids_delete)
				DeleteObject(id_delete);

			HashSet<int> Ids_new = new HashSet<int>(m_rcPrime);
			Ids_new.ExceptWith(m_rc);
			foreach (int id_new in Ids_new)
				CreateObject(items[id_new]);

			m_rc = new HashSet<int>(m_rcPrime);
			m_rcPrime.Clear();
			return true;
		}
		else
			return false;
	}
};



