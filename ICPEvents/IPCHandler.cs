using System;
using System.Threading;
using System.IO;
using System.Text;

namespace IPC
{
    public enum SendReceiveApp { SENDER, RECEIVER}

    public class IPCHandler
    {
        private object data;
        private SendReceiveApp senderApp;
        bool IsmutexCreated;
        Mutex mutex;
        public int readPosition { get; set; }
        public int writePosition { get; set; }
        SharedMemory.BufferReadWrite buff;


        private IPCServer server;
        private IPCClient client;
        private readonly int port = 100;

        //1. define a delegate
        //2. define an event based on that delegate
        public delegate void WriteDataEventHandler(object sender, IPCEventArgs args);
        public event WriteDataEventHandler dataWrittenToMemory;

        public IPCHandler(SendReceiveApp appType)
        {
            if (appType == SendReceiveApp.RECEIVER)
                setUpICPServer();
            else if (appType == SendReceiveApp.SENDER)
                setUpICPClient();
        }

        //3. raise the event, tzn tworzę metodę virtual, która najpierw sprawdza, czy event nie jest null, i jeżeli nie jest to wysyła odpowiednie dane przypisane do eventu
        //tzn obiekt, który wysyła ten event oraz argumenty


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
            ReadMemoryMappedFile();
        }


        public void initializeMutex()
        {
            //this.mutex = new Mutex(true, "NonPersisterMemoryMappedFilemutex", out this.IsmutexCreated);
        }


        public void read()
        {
            this.data = ReadMemoryMappedFile();
        }

        //--Create or Open a memory mapped file<br>        //-----------------------------------------------------------------------------------       
        protected void CreateOrOpenMappedFile(string data)
        {
            //char[] dataBuffer = data.ToCharArray();
            byte[] dataBuffer = Encoding.ASCII.GetBytes(data);

            try
            {
                //MemoryMappedFile mmf = null;
                //if (assertMmfExists())
                //{
                //    mmf = MemoryMappedFile.OpenExisting("sharedName");
                //    mmf.Dispose();
                //}

                //    mmf = MemoryMappedFile.CreateNew("sharedName", 4096);
                ////{
                //    using (MemoryMappedViewStream stream = mmf.CreateViewStream())
                //    {
                //        stream.Position = 0;
                //        BinaryWriter writer = new BinaryWriter(stream);
                //        writer.Write(dataBuffer);
                //    }
                //}
                


                buff = new SharedMemory.BufferReadWrite("testName", dataBuffer.Length);

                buff.Write(dataBuffer);

                //Mutex mutex = Mutex.OpenExisting("NonPersisterMemoryMappedFilemutex");
                //mutex.WaitOne();

                //char[] pointData = new char[10];
                //using (MemoryMappedViewStream stream = mmf.CreateViewStream())
                //{
                //    int i = 0;
                //    BinaryReader reader = new BinaryReader(stream);
                //    while (stream.Position != pointData.Length)
                //    {
                //        char p = reader.ReadChar();
                //        pointData[i] = p;
                //        i++;
                //    }
                //}



                //this.mutex.ReleaseMutex();
            }
            catch (Exception ex)
            {
                //-- Just trap this message just incase the memory file is not mapped
                string sMessage = ex.Message;
            }

        }

        private bool assertMmfExists()
        {
            try
            {
                //MemoryMappedFile mmf = MemoryMappedFile.OpenExisting("sharedName");
                return true;
            }
            catch (Exception)
            {
                return false;
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
                
                //MemoryMappedFile mmf = MemoryMappedFile.OpenExisting("sharedName");
                ////{
                //    using (MemoryMappedViewStream stream = mmf.CreateViewStream())
                //    {
                //        int i = 0;
                //        BinaryReader reader = new BinaryReader(stream);
                //        while (stream.Position != pointData.Length)
                //        {
                //            pointData[i] = reader.ReadChar();
                //            i++;
                //        }
                //    }
                    
                    //mmf.Dispose();
                //}
                    buff = new SharedMemory.BufferReadWrite("testName");
                    //Mutex mutex = Mutex.OpenExisting("NonPersisterMemoryMappedFilemutex");
                    //mutex.WaitOne();

                    buff.Read(pointData);
                    //mutex.ReleaseMutex();
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
