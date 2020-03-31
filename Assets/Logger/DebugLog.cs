
using System;

class DebugLog
{
	private loggerSrvLib.Logger m_logger;
	private static DebugLog g_dbgLogger = new DebugLog();
	public static void Format(string format, params object[] args)
	{
		string strPrefix = "Info:\t";
		string strInfo = string.Format(format, args);
		string item = strPrefix + strInfo + "\n";
		g_dbgLogger.m_logger.LogOut(item);
		g_dbgLogger.m_logger.Dump();
	}

	public static void Warning(object message)
	{
		string strPrefix = "Warning:\t";
		string strInfo = message.ToString();
		string item = strPrefix + strInfo + "\n";
		g_dbgLogger.m_logger.LogOut(item);
		g_dbgLogger.m_logger.Dump();
	}

	public static void WarningFormat(string format, params object[] args)
	{
		string strPrefix = "Warning:\t";
		string strInfo = string.Format(format, args);
		string item = strPrefix + strInfo + "\n";
		g_dbgLogger.m_logger.LogOut(item);
		g_dbgLogger.m_logger.Dump();
	}

	DebugLog()
	{
		m_logger = new loggerSrvLib.Logger();
		m_logger.Create("Debug.log");
	}

	~DebugLog()
	{
		m_logger.Close();
		m_logger = null;
	}

};

