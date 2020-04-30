
public class LocalJointId : Bolt.IProtocolToken
{
	public int pedId;
	public int jointId;

	public LocalJointId(int a_pedId, int a_jointId) {
		pedId = a_pedId;
		jointId = a_jointId;
	}

	public LocalJointId() {
		pedId = -1;
		jointId = -1;
	}


	public void Write(UdpKit.UdpPacket packet) {
		packet.WriteInt(pedId);
		packet.WriteInt(jointId);
	}

	public void Read(UdpKit.UdpPacket packet) {
		pedId = packet.ReadInt();
		jointId = packet.ReadInt();
	}
}