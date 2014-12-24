namespace DoodleEmpires.Engine.Net
{
    public class Packet_PlayerJoined : IPacket
    {
        PlayerInfo _info;

        public PlayerInfo Info
        {
            get { return _info; }
            set { _info = value; }
        }

        public Packet_PlayerJoined()
        {
            _info = null;
            SendToAll = true;
        }

        public Packet_PlayerJoined(PlayerInfo info)
        {
            _info = info;
            SendToAll = true;
        }
    }
}
