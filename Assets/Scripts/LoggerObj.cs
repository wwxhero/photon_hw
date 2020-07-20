using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LoggerObj : MonoBehaviour {
	protected readonly int c_logPackCap = 512;
	protected loggerSrvLib.Logger m_logger;

	protected void Initialize(string name)
	{
		m_logger = new loggerSrvLib.Logger();
		string file = string.Format("{0}.csv", name);
		m_logger.Create(file);
	}

	void OnDestroy()
	{
		if (null != m_logger)
			m_logger.Close();
		m_logger = null;
	}

	protected void LogOutInPack(string item)
	{
		int n = item.Length;
		int L = c_logPackCap;
		for (int i = 0; i < n; i += L)
		{
			string item_i = item.Substring(i, Mathf.Min(n-i, L));
			m_logger.LogOut(item_i);
		}
	}

	public abstract void OnLogging();

	void LateUpdate()
	{
		//bool even_frm = (0 == (Time.frameCount & 0x01));
		//if (even_frm)
			OnLogging();
	}
}