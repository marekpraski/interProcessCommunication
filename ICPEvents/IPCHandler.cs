using System;
using System.Text;

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
                setUpICPServer();
            else if (appType == SendReceiveApp.SENDER)
                setUpICPClient();
        }


        private void setUpICPServer()
        {
            this.server = new IPCServer(port);
            server.Start();
            server.dataReceived += messageReceived;
        }

        private void setUpICPClient()
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

        public void write(SendReceiveApp senderApp, object data)
        {
            CreateOrOpenMappedFile(data.ToString());
        }


        public void read()
        {
            ReadMemoryMappedFile();
        }

        //--Create or Open a memory mapped file<br>        //-----------------------------------------------------------------------------------       
        protected void CreateOrOpenMappedFile(string data)
        {
            byte[] dataBuffer = Encoding.UTF8.GetBytes(data);

            try
            {
                SharedMemory.BufferReadWrite buff = new SharedMemory.BufferReadWrite("testName", dataBuffer.Length);
                buff.Write(dataBuffer);
            }
            catch (Exception ex)
            {
                //-- Just trap this message just incase the memory file is not mapped
                string sMessage = ex.Message;
            }

        }


        //-----------------------------------------------------------------------------------
        //--Open the memory mapped file and read the contents
        //-----------------------------------------------------------------------------------

        protected object ReadMemoryMappedFile()
        {
            byte[] pointData = new byte[10];
            try
            {
                SharedMemory.BufferReadWrite buff = new SharedMemory.BufferReadWrite("testName");
                buff.Read(pointData);
                buff.Close();
            }
            catch (Exception ex)
            {
                //-- Just trap this message just incase the memory file is not mapped
                string sMessage = ex.Message;

            }
            string result = Encoding.ASCII.GetString(pointData);
            return result;
        }
    }
}
