namespace DoodleEmpires.Engine.Net
{
    public class Packet_Sample : IPacket
    {
        PlayerInfo _info;
        int _value1;
        double _value2;
        bool _value3;

        public PlayerInfo Info
        {
            get { return _info; }
            set { _info = value; }
        }
        public int Value1
        {
            get { return _value1; }
            set { _value1 = value; }
        }
        public double Value2
        {
            get { return _value2; }
            set { _value2 = value; }
        }
        public bool Value3
        {
            get { return _value3; }
            set { _value3 = value; }
        }

        public Packet_Sample()
        {
            _info = null;
        }
    }
}
