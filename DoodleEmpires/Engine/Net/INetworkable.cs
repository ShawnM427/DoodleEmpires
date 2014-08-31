using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoodleEmpires.Engine.Net
{
    public interface INetworkable
    {
        INetworkable Read(NetIncomingMessage msg);

        void Write(NetOutgoingMessage msg);
    }
}
