
using System;

public class DebugLog
{
	private loggerSrvLib.Logger m_logger;
	private static DebugLog g_dbgLogger = new DebugLog();

	private static void Out(object msg, string prefix = "")
	{
		string item = prefix + msg + "\n";
		g_dbgLogger.m_logger.LogOut(item);
		g_dbgLogger.m_logger.Dump();
	}

	private static void OutFormat(string format, params object[] args)
	{
		string msg = string.Format(format, args);
		string item = msg + "\n";
		g_dbgLogger.m_logger.LogOut(item);
		g_dbgLogger.m_logger.Dump();
	}

	public static void Info(object info)
	{
		string strPrefix = "Info:\t";
		Out(info, strPrefix);
	}

	public static void InfoFormat(string format, params object[] args)
	{
		string strPrefix = "Info:\t";
		format = strPrefix + format;
		OutFormat(format, args);
	}

	public static void Error(object message)
	{
		string strPrefix = "Error:\t";
		Out(message, strPrefix);
	}

	public static void ErrorFormat(string format, params object[] args)
	{
		string strPrefix = "Error:\t";
		format = strPrefix + format;
		OutFormat(format, args);
	}

	public static void Warning(object message)
	{
		string strPrefix = "Warning:\t";
		Out(message, strPrefix);
	}

	public static void WarningFormat(string format, params object[] args)
	{
		string strPrefix = "Warning:\t";
		format = strPrefix + format;
		OutFormat(format, args);
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

