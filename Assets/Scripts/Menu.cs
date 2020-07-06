using System;
using System.Collections;
using Bolt.Matchmaking;
using Bolt.Photon;
using UdpKit;
using UdpKit.Platform;
using UnityEngine;

namespace Bolt.Samples.GettingStarted
{
	public class Menu : Bolt.GlobalEventListener
	{
		private bool _showGui = true;
		private Coroutine _timerRoutine;
		//private string c_sessionId = "d885c66f-5474-4239-a25f-a2a3ad7f9c8b";
		private void Awake()
		{
			Application.targetFrameRate = 60;
			BoltLauncher.SetUdpPlatform(new PhotonPlatform());
		}

		private void OnGUI()
		{
			if (!_showGui) { return; }

			GUILayout.BeginArea(new Rect(10, 10, Screen.width - 20, Screen.height - 20));

			if (GUILayout.Button("Start Server", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)))
			{
				// START SERVER
				BoltLauncher.StartServer();
			}

			if (GUILayout.Button("Start Client", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)))
			{
				// START CLIENT
				BoltLauncher.StartClient();
			}

			GUILayout.EndArea();
		}

		public override void BoltStartBegin()
		{
			_showGui = false;
			BoltNetwork.RegisterTokenClass<PhotonRoomProperties>();
			BoltNetwork.RegisterTokenClass<LocalJointId>();
		}

		public override void BoltStartDone()
		{
			if (BoltNetwork.IsServer)
			{
				string matchName = Guid.NewGuid().ToString(); // c_sessionId;

                var props = new PhotonRoomProperties();

                props.IsOpen = true;
                props.IsVisible = true;

                props["type"] = "game01";
                props["map"] = "Tutorial1";

                BoltMatchmaking.CreateSession(
					sessionID: matchName,
					sceneToLoad: "Tutorial1",
					token: props
				);
			}

			if (BoltNetwork.IsClient)
			{
                // This will start a server after 10secs of wait
                // if no server was found
                _timerRoutine = StartCoroutine(ShutdownAndStartServer());
            }
		}

		public override void BoltShutdownBegin(AddCallback registerDoneCallback)
		{
			registerDoneCallback(() => {
				BoltLauncher.StartServer();
			});
		}

		public override void SessionListUpdated(Map<Guid, UdpSession> sessionList)
		{
			if (_timerRoutine != null)  {
				StopCoroutine(_timerRoutine);
				_timerRoutine = null;
			}

			DebugLog.InfoFormat("Session list updated: {0} total sessions", sessionList.Count);

			foreach (var session in sessionList)
			{
				UdpSession photonSession = session.Value as UdpSession;

				if (photonSession.Source == UdpSessionSource.Photon)
				{
					var s = photonSession as UdpKit.Platform.Photon.PhotonSession;
                    //if (s.HostName == c_sessionId)
                    {
                        BoltNetwork.Connect(photonSession);
                        DebugLog.WarningFormat("Connect to session:{0}", photonSession);
                    }
                    GS_ServerCallbacks.LogPhotonSession(s);

				}
			}
		}

		// Utils

		private static IEnumerator ShutdownAndStartServer(int timeout = 10)
		{
			yield return new WaitForSeconds(timeout);

			BoltLog.Warn("No server found, restarting");
			DebugLog.Warning("No server found, restarting");
			BoltNetwork.ShutdownImmediate();
		}
	}
}