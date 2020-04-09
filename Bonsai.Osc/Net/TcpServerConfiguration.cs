﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Bonsai.Osc.Net
{
    public class TcpServerConfiguration : TransportConfiguration
    {
        public int Port { get; set; }

        public bool NoDelay { get; set; }

        public bool AllowNatTraversal { get; set; }

        internal override ITransport CreateTransport()
        {
            var listener = new TcpListener(IPAddress.Loopback, Port);
            listener.AllowNatTraversal(AllowNatTraversal);
            listener.Start();
            return new TcpServerTransport(listener, NoDelay);
        }
    }
}
