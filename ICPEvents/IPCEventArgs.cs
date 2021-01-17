using System;

namespace IPC
{
    public class IPCEventArgs : EventArgs
    {
        public SendReceiveApp senderApp;
        public object data;
    }
}
