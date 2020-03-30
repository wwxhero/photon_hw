using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
public class loggerTesterCorrectness : MonoBehaviour {
	loggerSrvLib.Logger m_logger;
	Random m_rnd = new Random();
	List<string> m_lstStandard = new List<string>();
	// Use this for initialization
	void Start () {
		m_logger = new loggerSrvLib.Logger();
		m_logger.Create("test_verifying.txt");

	}

	// Update is called once per frame
	void Update () {
		int n = Random.Range(2, 4096);
		string item = new string('b', n);
				item = '[' + item + ']';
		m_logger.LogOut(item);
		m_lstStandard.Add(item);
	}

    void OnDestroy()  {
    	m_logger.Close();
    	m_logger = null;
    	FileStream fs = new FileStream("test_standard.txt"
										   , FileMode.CreateNew
										   , FileAccess.Write);
		BufferedStream buf = new BufferedStream(fs);
		foreach (string item in m_lstStandard)
		{
			byte[] bytes = Encoding.Unicode.GetBytes(item);
			buf.Write(bytes, 0, bytes.Length);
		}
		buf.Flush();
    }
}
