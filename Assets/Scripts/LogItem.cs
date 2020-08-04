using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;
using Id2Name = System.Collections.Generic.Dictionary<int, string>;
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
	public enum LogType {ped = 0, veh, total};
	public LogType type;
	public int nFrame;
	public double ticks; //in millisecond
	public Transform_log [] transforms;
	static bool c_debug = false;
	static int m_nId = 0;
	static readonly string c_filesuffix = ".csv";
	public delegate bool ParseRowTransform(BufferedStream buff, Transform_log[] transforms, int n_trans);

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

	static bool ParseRow4Ped_rt(BufferedStream buff, Transform_log[] transforms, int n_trans)
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
			transforms[i_tran].scl.x = 1;
			transforms[i_tran].scl.y = 1;
			transforms[i_tran].scl.z = 1;
		}
		return valid_parse; //todo: parse an array of transforms from buf
	}

	static bool ParseRow4Ped_s(BufferedStream buff, Transform_log[] transforms, int n_trans)
	{
		bool valid_parse = true;
		float [] v = new float[3];
		for (int i_tran = 0
			; i_tran < n_trans
			&& valid_parse
			; i_tran ++)
		{
			for (int i = 0
				; i < 3
				&& valid_parse
				; i ++)
			{
				string field;
				NextField(buff, out field);
				valid_parse = float.TryParse(field, out v[i]);
			}
			transforms[i_tran].ori.w = 1;
			transforms[i_tran].ori.x = 0;
			transforms[i_tran].ori.y = 0;
			transforms[i_tran].ori.z = 0;
			transforms[i_tran].pos.x = 0;
			transforms[i_tran].pos.y = 0;
			transforms[i_tran].pos.z = 0;
			transforms[i_tran].scl.x = v[0];
			transforms[i_tran].scl.y = v[1];
			transforms[i_tran].scl.z = v[2];
		}
		return valid_parse; //todo: parse an array of transforms from buf
	}

	static bool ParseRow4Veh(BufferedStream buff, Transform_log[] transforms, int n_trans)
	{
		Debug.Assert(1 == n_trans);
		bool valid_parse = true;
		float [] v = new float[7];

		for (int i = 0
			; i < 7
			&& valid_parse
			; i ++)
		{
			string field;
			NextField(buff, out field);
			valid_parse = float.TryParse(field, out v[i]);
		}
		transforms[0].ori.w = v[0];
		transforms[0].ori.x = v[1];
		transforms[0].ori.y = v[2];
		transforms[0].ori.z = v[3];
		transforms[0].pos.x = v[4];
		transforms[0].pos.y = v[5];
		transforms[0].pos.z = v[6];
		transforms[0].scl.x = 1;
		transforms[0].scl.y = 1;
		transforms[0].scl.z = 1;

		return valid_parse; //todo: parse an array of transforms from buf
	}

	static int Parse4Veh(string name, List<Id2Item> records)
	{
		string path = name + c_filesuffix;
        string[] dummy = null;
        return ParseTable(path, ParseRow4Veh, records, 7, true, LogType.veh, ref dummy);
		//each vehicle has an id, vehicles are aggregated in this log file
	}

	static int ParseTable(string path, ParseRowTransform parser_row_trans , List<Id2Item> records, int u_joint, bool aggregated_entities, LogType type, ref string[] joint_names)
	{
		List<LogItem> rawRecords = new List<LogItem>();

		FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
		BufferedStream buffer = new BufferedStream(fs);
		string strLine = "";
		bool read = NextLine(buffer, ref strLine);
		Debug.Assert(read);
		DebugLog.InfoFormat("Header: {0}", strLine);

		int n_fields = 0;
		int i_start = 0;
		int n_cnt = strLine.Length;
		int n_skip_fields = aggregated_entities ? 3 : 2;

		bool retrive_joint_names = (null != joint_names);
		List<string> joint_names_2 = retrive_joint_names ? new List<string>() : null;
		while (n_cnt > 0)
		{
			int i_start_m = i_start;
			int i = strLine.IndexOf(',', i_start, n_cnt);
			int n_skip = 0;
			if (i > 0)
			{
				int i_start_prime = i + 1;
				n_skip = i_start_prime - i_start;
				n_cnt = n_cnt - n_skip;
				i_start = i_start_prime;
			}
			else
			{
				n_cnt = 0;
			}

			if (retrive_joint_names
				&& (n_skip_fields < n_fields)
				&& 0 == ((n_fields - n_skip_fields) % u_joint))
			{
				int i_dot = strLine.LastIndexOf('.', i_start, n_skip);
                string name = strLine.Substring(i_start_m, i_dot - i_start_m);
                joint_names_2.Add(name.Trim());
			}

			n_fields ++;
		}

        if (retrive_joint_names)
            joint_names = joint_names_2.ToArray();

		int n_joints_u = n_fields - n_skip_fields;
		int n_joints = n_joints_u / u_joint;
		Debug.Assert(n_joints_u == n_joints * u_joint);

		int nFrame = 0;
		double ticks = 0;
		int id_base = m_nId;
		Transform_log [] transforms = null;

		int nItem = 0;

		while (read)
		{
			transforms = new Transform_log[n_joints];
			int id_offset = 0;
			read = ParseDouble(buffer, out ticks)
				&& ParseInt(buffer, out nFrame)
				&& (!aggregated_entities || ParseInt(buffer, out id_offset))
				&& parser_row_trans(buffer, transforms, n_joints);
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
										  id = id_base + id_offset
										, type = type
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
		return nItem;
	}

	static void Parse4Ped(string name, List<Id2Item> records, ref string[] joint_names)
	{
		List<Id2Item> records_RT = records;
		List<Id2Item> records_S = new List<Id2Item>();
		string path_rt = name + c_filesuffix;
		string path_s = name + "_s" + c_filesuffix;
		ParseTable(path_rt, ParseRow4Ped_rt, records_RT, 7, false, LogType.ped, ref joint_names);
		if (File.Exists(path_s))
		{
            string[] dummy = null;
            ParseTable(path_s, ParseRow4Ped_s, records_S, 3, false, LogType.ped, ref dummy);
			Id2Item id2Item = new Id2Item();
			records_S.Add(id2Item);
			id2Item[m_nId] = new LogItem{
									  id = m_nId
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
				int n_l_s = (records_S[i_S])[m_nId].nFrame;
				int n_r_s = (records_S[i_S + 1])[m_nId].nFrame;
				int n_rt = (records_RT[i_RT])[m_nId].nFrame;
				if (n_r_s <= n_rt) //n_r_s <= n_rt
					i_S ++;
				Debug.Assert((n_rt = (records_RT[i_RT])[m_nId].nFrame) < (n_r_s = (records_S[i_S + 1])[m_nId].nFrame)
							&& (n_l_s = (records_S[i_S])[m_nId].nFrame) <= (n_rt = (records_RT[i_RT])[m_nId].nFrame));
				Transform_log [] trans_dst = (records[i_RT])[m_nId].transforms;
				Transform_log [] trans_src_rt = (records_RT[i_RT])[m_nId].transforms;
				Transform_log [] trans_src_s = (records_S[i_S])[m_nId].transforms;
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
	}

	static int N_Frame(Id2Item id2item)
	{
		Debug.Assert(id2item.Count > 0);
		foreach (var id_item in id2item)
			return id_item.Value.nFrame;
        return -1;
	}

	static void JointRecord(List<Id2Item> records, List<Id2Item> records_prime)
	{
		int i_rc = 0;
		int i_rc_prime = 0;
		int n_frame = N_Frame(records[0]);
		int n_frame_prime = 0;
		for (
			; i_rc_prime < records_prime.Count
			 && n_frame > (n_frame_prime = N_Frame(records_prime[i_rc_prime]))
			; i_rc_prime ++)
		{
			Id2Item record_prime = records_prime[i_rc_prime];
			records.Insert(i_rc_prime, record_prime);
		}

		while (i_rc < records.Count
			   && i_rc_prime < records_prime.Count)
		{
			Id2Item record = records[i_rc];
			Id2Item record_prime = records_prime[i_rc_prime];
			if (n_frame == n_frame_prime)
			{
				foreach (var id2item_prime in record_prime)
				{
					record[id2item_prime.Key] = id2item_prime.Value;
				}
				i_rc ++;
				if (i_rc < records.Count)
					n_frame = N_Frame(records[i_rc]);
				i_rc_prime ++;
				if (i_rc_prime < records_prime.Count)
					n_frame_prime = N_Frame(records_prime[i_rc_prime]);
			}
			else if (n_frame < n_frame_prime)
			{
				i_rc ++;
				if (i_rc < records.Count)
					n_frame = N_Frame(records[i_rc]);
			}
			else
			{
				i_rc_prime ++;
				if (i_rc_prime < records_prime.Count)
					n_frame_prime = N_Frame(records_prime[i_rc_prime]);
			}
		}

		while (i_rc_prime < records_prime.Count)
		{
			Id2Item record_prime = records_prime[i_rc_prime];
			records.Add(record_prime);
			i_rc_prime++;
		}
	}

	public static void Parse(LogType type
							, string name
							, List<Id2Item> records
							, Id2Name id2name
							, ref string[] joint_names
							, ref int nFrameBase
							, ref int nFrameMax
							, bool debug = false)
	{
		int n_records = records.Count;
		List<Id2Item> records_prime = (n_records > 0)
										? new List<Id2Item>()
										: records;
		switch(type)
		{
			case LogType.ped:
			{
				int id = m_nId;
				Parse4Ped(name, records_prime, ref joint_names);
				id2name[id] = name;
				m_nId ++;
				break;
			}
			case LogType.veh:
			{
				m_nId += Parse4Veh(name, records_prime);
				break;
			}
		}

		if (records != records_prime)
			JointRecord(records, records_prime);

		if (debug)
		{
			for (int i_rc = 0; i_rc < records.Count; i_rc ++)
			{
				int nFrame = -1;
				foreach (var id2item in records[i_rc])
				{
					if (nFrame < 0)
					{
						nFrame = id2item.Value.nFrame;
					}
					else
					{
						Debug.Assert(nFrame == id2item.Value.nFrame);
					}
				}
			}
		}

		int nFrameBase_prime = N_Frame(records[0]);
		int nFrameMax_prime = records.Count - 1 + nFrameBase_prime;
		if (nFrameBase > nFrameBase_prime)
			nFrameBase = nFrameBase_prime;
		if (nFrameMax < nFrameMax_prime)
			nFrameMax = nFrameMax_prime;
	}
};


