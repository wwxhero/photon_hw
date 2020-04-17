using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bolt.Samples.GettingStarted
{
	[BoltGlobalBehaviour("Tutorial1")]
	public class NetworkCallbacks : Bolt.GlobalEventListener
	{
		List<string> logMessages = new List<string>();

		public override void SceneLoadLocalDone(string scene)
		{
			// randomize a position
			var spawnPosition = new Vector3(Random.Range(-8, 8), 1, Random.Range(-8, 8));

			// instantiate cube
			BoltEntity entity_0 = BoltNetwork.Instantiate(BoltPrefabs.Joint, spawnPosition, Quaternion.identity);
			spawnPosition.x += 1f;
			BoltEntity entity_1 = BoltNetwork.Instantiate(BoltPrefabs.Joint, spawnPosition, Quaternion.identity);
			spawnPosition.x += 1f;
			BoltEntity entity_2 = BoltNetwork.Instantiate(BoltPrefabs.Joint, spawnPosition, Quaternion.identity);

            entity_1.SetParent(entity_0);
            entity_2.SetParent(entity_1);

            entity_0.gameObject.AddComponent<LoggerAvatar>();
		}

		public override void OnEvent(LogEvent evnt)
		{
			logMessages.Insert(0, evnt.Message);
		}

		void OnGUI()
		{
			// only display max the 5 latest log messages
			int maxMessages = Mathf.Min(5, logMessages.Count);

			GUILayout.BeginArea(new Rect(Screen.width / 2 - 200, Screen.height - 100, 400, 100), GUI.skin.box);

			for (int i = 0; i < maxMessages; ++i)
			{
				GUILayout.Label(logMessages[i]);
			}

			GUILayout.EndArea();
		}
	}
}