using UdpKit;
using UdpKit.Platform.Photon;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Bolt.Samples.GettingStarted
{
	[BoltGlobalBehaviour(BoltNetworkModes.Server, "Tutorial1")]
	public class GS_ServerCallbacks : Bolt.GlobalEventListener
	{
		public override void Connected(BoltConnection connection)
		{
			var log = LogEvent.Create();
			log.Message = string.Format("{0} connected with token {1}", connection.RemoteEndPoint, connection.ConnectToken);
			log.Send();
		}

		public override void Disconnected(BoltConnection connection)
		{
			var log = LogEvent.Create();
			log.Message = string.Format("{0} disconnected", connection.RemoteEndPoint);
			log.Send();
		}

		public override void BoltShutdownBegin(AddCallback registerDoneCallback, UdpConnectionDisconnectReason disconnectReason)
		{
			registerDoneCallback(() =>
			{
				DebugLog.InfoFormat("Server Shutdown Done with Reason: {0}", disconnectReason);
				SceneManager.LoadScene(0);
			});
		}

		public override void SessionCreated(UdpSession session)
		{
			DebugLog.Warning("Session created");

			var photonSession = session as PhotonSession;

			LogPhotonSession(photonSession);
		}

		public static void LogPhotonSession(PhotonSession photonSession)
		{
			if (photonSession != null)
			{
				DebugLog.Warning(photonSession.HostName);
				DebugLog.Warning(photonSession.IsOpen);
				DebugLog.Warning(photonSession.IsVisible);

				foreach(var key in photonSession.Properties.Keys)
				{
					DebugLog.WarningFormat("{0} = {1}", key, photonSession.Properties[key]);
				}
			}
		}

		public static void CreateNetworkingVeh(int vid, Vector3 pos, Quaternion rot)
		{
			var vid_n = new LocalVehId
			{
				id = vid
			};
			BoltNetwork.Instantiate(BoltPrefabs.Veh_n
											, vid_n
											, pos
											, rot);
		}

		public static void DestroyNetworkingVeh(int vid)
		{
			foreach (var entity in BoltNetwork.SceneObjects)
			{
				if (entity.StateIs<IVehState>())
				{
					NetworkVehBehavior veh_n = entity.gameObject.GetComponent<NetworkVehBehavior>();
					if (veh_n.id_n == vid)
					{
						BoltNetwork.Destroy(entity);
						break;
					}
				}
			}
		}
	}
}