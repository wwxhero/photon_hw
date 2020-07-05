using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;
using Id2Item = System.Collections.Generic.Dictionary<int, LogItem>;

public struct Transform_log
{
	public Vector3 pos;
	public Quaternion ori;
	public Vector3 scl;
};

public class LogItem
{
	public int id;
	public enum LogType {ped, veh};
	public LogType type;
	public int nFrame;
	public double ticks; //in millisecond
	public Transform_log [] transforms;
	static bool c_debug = false;
	static int s_idStatic = 0;

	static bool NextLine(BufferedStream buff, ref string line)
	{
		line = "";
		char[] char_16 = null;
		byte[] bytes_16 = new byte[2];
		bool eof = false;
		bool eol = ( eof = (0 == buff.Read(bytes_16, 0, 2))
					|| '\n' == (char_16 = Encoding.Unicode.GetChars(bytes_16))[0]
					|| '\r' == char_16[0]);
		while (!eol)
		{
			line += char_16[0];
			eol = ( eof = (0 == buff.Read(bytes_16, 0, 2))
					|| '\n' == (char_16 = Encoding.Unicode.GetChars(bytes_16))[0]
					|| '\r' == char_16[0]);
		}

		if (null != char_16
			&& '\r' == char_16[0])
			buff.Read(bytes_16, 0, 2);

		return eof;
	}

	static void NextField(BufferedStream buff, out string field)
	{
		char[] char_16 = null;
		byte[] bytes_16 = new byte[2];
		bool eof = false;
		bool end_of_field = ( eof = (0 == buff.Read(bytes_16, 0, 2))
					|| ',' == (char_16 = Encoding.Unicode.GetChars(bytes_16))[0]
					|| '\r' == char_16[0]
					|| '\n' == char_16[0]);
		field = "";
		while(!eof
			&& !end_of_field)
		{
			field += char_16[0];
			end_of_field = ( eof = (0 == buff.Read(bytes_16, 0, 2))
					|| ',' == (char_16 = Encoding.Unicode.GetChars(bytes_16))[0]
					|| '\r' == char_16[0]
					|| '\n' == char_16[0]);
		}
		if (null != char_16
			&& '\r' == char_16[0])
			buff.Read(bytes_16, 0, 2);
	}

	static bool ParseInt(BufferedStream buff, out int value)
	{
		string field;
		NextField(buff, out field);
		return int.TryParse(field, out value);
	}

	static bool ParseDouble(BufferedStream buff, out double value)
	{
		string field;
		NextField(buff, out field);
		return double.TryParse(field, out value);
	}

	static bool ParseTransforms(BufferedStream buff, Transform_log[] transforms, int n_trans)
	{
		bool valid_parse = true;
		float [] v = new float[7];
		for (int i_tran = 0
			; i_tran < n_trans
			&& valid_parse
			; i_tran ++)
		{
			for (int i = 0
				; i < 7
				&& valid_parse
				; i ++)
			{
				string field;
				NextField(buff, out field);
				valid_parse = float.TryParse(field, out v[i]);
			}
			transforms[i_tran].ori.w = v[0];
			transforms[i_tran].ori.x = v[1];
			transforms[i_tran].ori.y = v[2];
			transforms[i_tran].ori.z = v[3];
			transforms[i_tran].pos.x = v[4];
			transforms[i_tran].pos.y = v[5];
			transforms[i_tran].pos.z = v[6];
		}
		return valid_parse; //todo: parse an array of transforms from buf
	}

	static void Parse4Veh(string path, List<Id2Item> records)
	{
		Debug.Assert(false);
	}

	static void Parse4Ped_RT(string path, List<Id2Item> records)
	{
		int n_joints = ScenarioControl.s_lstNetworkingJoints.Length + 1; //+1 for entity

		List<LogItem> rawRecords = new List<LogItem>();

		FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
		BufferedStream buffer = new BufferedStream(fs);
		string strLine = "";
		bool read = NextLine(buffer, ref strLine);
		Debug.Assert(read);
		DebugLog.InfoFormat("Header: {0}", strLine);

		int nFrame = 0;
		double ticks = 0;
		Transform_log [] transforms = null;

		int nItem = 0;
		while (read)
		{
			transforms = new Transform_log[n_joints];
			read = ParseDouble(buffer, out ticks)
				&& ParseInt(buffer, out nFrame)
				&& ParseTransforms(buffer, transforms, n_joints);
			if (read)
			{
				if (c_debug)
				{
					string strItem = string.Format("===>{0} {1}", nFrame, ticks);
					for (int i_joint = 0; i_joint < n_joints; i_joint ++)
					{
						Quaternion q = transforms[i_joint].ori;
						Vector3 t = transforms[i_joint].pos;
						strItem += string.Format(", {0,7:#.0000}, {1,7:#.0000}, {2,7:#.0000}, {3,7:#.0000}, {4,7:#.000}, {5,7:#.000}, {6,7:#.000}"
											, q.w, q.x, q.y, q.z
											, t.x, t.y, t.z);
					}
					DebugLog.InfoFormat(strItem);
				}
				LogItem item = new LogItem {
										  id = s_idStatic
										, type = LogType.ped
										, nFrame = nFrame
										, ticks = ticks
										, transforms = transforms
										};
				Id2Item id2item = null;
				if (records.Count > nItem)
					id2item = records[nItem];
				else
				{
					id2item = new Id2Item();
					records.Add(id2item);
				}
				id2item[item.id] = item;
			}
			nItem ++;
		}
	}

	static bool Parse4Ped_S(string path, List<Id2Item> records)
	{
		return false;
	}

	static void Parse4Ped(string path, List<Id2Item> records)
	{
		List<Id2Item> records_RT = records;
		List<Id2Item> records_S = new List<Id2Item>();
		Parse4Ped_RT(path, records_RT);
		if (Parse4Ped_S(path, records_S))
		{
			Id2Item id2Item = new Id2Item();
			records_S.Add(id2Item);
			id2Item[s_idStatic] = new LogItem{
									  id = s_idStatic
									, type = LogType.ped
									, nFrame = int.MaxValue
									, ticks = int.MaxValue
									, transforms = null
								};
			int i_RT = 0;
			int i_S = 0;
			while (i_RT < records_RT.Count
				&& i_S < records_S.Count - 1)
			{
				int n_l_s = (records_S[i_S])[s_idStatic].nFrame;
				int n_r_s = (records_S[i_S + 1])[s_idStatic].nFrame;
				int n_rt = (records_RT[i_RT])[s_idStatic].nFrame;
				if (n_r_s <= n_rt) //n_r_s <= n_rt
					i_S ++;
				Debug.Assert((n_rt = (records_RT[i_RT])[s_idStatic].nFrame) < (n_r_s = (records_S[i_S + 1])[s_idStatic].nFrame)
							&& (n_l_s = (records_S[i_S])[s_idStatic].nFrame) <= (n_rt = (records_RT[i_RT])[s_idStatic].nFrame));
				Transform_log [] trans_dst = (records[i_RT])[s_idStatic].transforms;
				Transform_log [] trans_src_rt = (records_RT[i_RT])[s_idStatic].transforms;
				Transform_log [] trans_src_s = (records_S[i_S])[s_idStatic].transforms;
				Debug.Assert(trans_dst.Length == trans_src_s.Length
							&& trans_dst.Length == trans_src_rt.Length);
				for (int i_tran = 0; i_tran < trans_dst.Length; i_tran ++)
				{
					trans_dst[i_tran].pos = trans_src_rt[i_tran].pos;
					trans_dst[i_tran].ori = trans_src_rt[i_tran].ori;
					trans_dst[i_tran].scl = trans_src_s[i_tran].scl;
				}
				i_RT ++;
			}
		}
		s_idStatic ++;

	}

	public static void Parse(LogType type, string path, List<Id2Item> records)
	{
		switch(type)
		{
			case LogType.ped:
			{
				Parse4Ped(path, records);
				break;
			}
			case LogType.veh:
			{
				Parse4Veh(path, records);
				break;
			}
		}
	}
};


