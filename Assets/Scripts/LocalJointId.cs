
public class LocalJointId : Bolt.IProtocolToken
{
	public int pedId;
	public int jointId;

	public void Write(UdpKit.UdpPacket packet) {
		packet.WriteInt(pedId);
		packet.WriteInt(jointId);
	}

	public void Read(UdpKit.UdpPacket packet) {
		pedId = packet.ReadInt();
		jointId = packet.ReadInt();
	}
}