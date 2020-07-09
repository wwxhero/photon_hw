
public class LocalVehId : Bolt.IProtocolToken
{
	public int id;


	public void Write(UdpKit.UdpPacket packet) {
		packet.WriteInt(id);
	}

	public void Read(UdpKit.UdpPacket packet) {
		id = packet.ReadInt();
	}
}