﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Id2Item = System.Collections.Generic.Dictionary<int, LogItem>;
using Id2GO = System.Collections.Generic.Dictionary<int, LogGO>;

public class LogPlayBack : MonoBehaviour {
	public string [] m_logFiles;
	public LogItem.LogType [] m_logTypes;
	public GameObject [] m_prefabs;
	[RangeAttribute(0, 12000)]
	public int m_nFrame = 0;
	public bool m_play = false;

	List<Id2Item> m_records = new List<Id2Item>();
	int c_nFrameBase = 0;
	HashSet<int> m_rc = new HashSet<int>();
	HashSet<int> m_rcPrime = new HashSet<int>();
	Id2GO m_id2go = new Id2GO();
	// Use this for initialization
	void Start () {
		Debug.Assert(m_logFiles.Length == m_logTypes.Length);
		for (int log_i = 0; log_i < m_logFiles.Length; log_i ++)
			LogItem.Parse(m_logTypes[log_i], m_logFiles[log_i], m_records);

	}

	// Update is called once per frame
	void Update () {
		bool played = UpdateFrame();
		m_play = m_play && played;
		if (m_play)
			m_nFrame ++;
	}

	void UpdateObject(LogItem item)
	{
		LogGO go = null;
		if (m_id2go.TryGetValue(item.id, out go))
			go.Play(item);
	}

	void DeleteObject(int id)
	{
		LogGO go = null;
		if (m_id2go.TryGetValue(id, out go))
		{
			Object.Destroy(go.gameObject);
			m_id2go.Remove(id);
		}
	}

	void CreateObject(LogItem item)
	{
		GameObject obj = Instantiate(m_prefabs[item.id], item.transforms[0].pos, item.transforms[0].ori);
		LogGO go = null;
		if (LogItem.LogType.ped == item.type)
		{
			LogGOPed ped = obj.AddComponent<LogGOPed>();
			go = ped;
			ped.Initialize();
		}
		else
			go = obj.AddComponent<LogGOVeh>();
		go.Play(item);
		m_id2go[item.id] = go;
	}

	bool UpdateFrame()
	{
		int i_offset = m_nFrame - c_nFrameBase;
		if (i_offset < 0)
			return true;
		else if(null != m_records
			&& m_nFrame < m_records.Count)
		{
			Id2Item items = m_records[m_nFrame];
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



