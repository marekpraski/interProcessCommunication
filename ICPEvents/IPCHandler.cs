

namespace IPC
{
    public enum SendReceiveApp { SENDER, RECEIVER}

    public class IPCHandler
    {
        private IPCServer server;
        private IPCClient client;
        private readonly int port = 100;

        public delegate void WriteDataEventHandler(object sender, IPCEventArgs args);
        public event WriteDataEventHandler dataWrittenToMemory;

        public IPCHandler(SendReceiveApp appType)
        {
            if (appType == SendReceiveApp.RECEIVER)
                setUpIPCServer();
            else if (appType == SendReceiveApp.SENDER)
                setUpIPCClient();
        }


        private void setUpIPCServer()
        {
            this.server = new IPCServer(port);
            server.Start();
            server.dataReceived += messageReceived;
        }

        private void setUpIPCClient()
        {
            this.client = new IPCClient(port);
        }

        public bool sendMessage(string message)
        {
            return this.client.sendMessage(message);
        }


        private void messageReceived(object sender, IPCEventArgs args)
        {
            onMessageReceived(args.data);
        }


        protected virtual void onMessageReceived(object data)
        {
            if (dataWrittenToMemory != null)
            {
                IPCEventArgs args = new IPCEventArgs();
                args.data = data;
                dataWrittenToMemory(this, args);
            }
        }
    }
}
