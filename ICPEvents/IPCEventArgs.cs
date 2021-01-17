using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IPC
{
    public class IPCEventArgs : EventArgs
    {
        public SendReceiveApp senderApp;
        public object data;
    }
}
