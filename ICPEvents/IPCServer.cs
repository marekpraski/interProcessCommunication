using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace IPC
{
    internal class IPCServer
    {
        private Socket serverSocket, clientSocket;
        private byte[] buffer;
        private int port;
        public delegate void DataReceivedEventHandler(object sender, IPCEventArgs args);
        public event DataReceivedEventHandler dataReceived;

        internal IPCServer(int port)
        {
            this.port = port;
        }

        internal void Start()
        {
            try
            {
                // Create server socket and listen on any local interface.
                serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                serverSocket.Bind(new IPEndPoint(IPAddress.Any, port));
                serverSocket.Listen(1);

                serverSocket.BeginAccept(AcceptCallback, null);
            }
            catch (SocketException ex)
            {
                ShowErrorDialog(ex.Message);
            }
            catch (ObjectDisposedException ex)
            {
                ShowErrorDialog(ex.Message);
            }


            // Followed by periods to indicate work.
            //Console.WriteLine("Listening for connections on port " + port + "...");
            //clientSocket = serverSocket.Accept();
            //Console.WriteLine("Client Connected");
            //Console.WriteLine("Waiting for data...");
            //buffer = new byte[clientSocket.ReceiveBufferSize];
            //ReceiveData();
            //Console.WriteLine("Server loop ended. Press enter to exit.");
            //Console.ReadLine();
            //serverSocket.Close();
        }


        private void AcceptCallback(IAsyncResult AR)
        {
            try
            {
                clientSocket = serverSocket.EndAccept(AR);
                buffer = new byte[clientSocket.ReceiveBufferSize];

                // Send a message to the newly connected client.
                var sendData = Encoding.ASCII.GetBytes("Hello");
                clientSocket.BeginSend(sendData, 0, sendData.Length, SocketFlags.None, SendCallback, null);
                // Listen for client data.
                clientSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, ReceiveCallback, null);
                // Continue listening for clients.
                serverSocket.BeginAccept(AcceptCallback, null);
            }
            catch (SocketException ex)
            {
                ShowErrorDialog(ex.Message);
            }
            catch (ObjectDisposedException ex)
            {
                ShowErrorDialog(ex.Message);
            }
        }

        private void SendCallback(IAsyncResult AR)
        {
            try
            {
                clientSocket.EndSend(AR);
            }
            catch (SocketException ex)
            {
                ShowErrorDialog(ex.Message);
            }
            catch (ObjectDisposedException ex)
            {
                ShowErrorDialog(ex.Message);
            }
        }

        private void ReceiveCallback(IAsyncResult AR)
        {
            try
            {
                // Socket exception will raise here when client closes, as this sample does not
                // demonstrate graceful disconnects for the sake of simplicity.
                int received = clientSocket.EndReceive(AR);

                if (received == 0)
                {
                    return;
                }

                // The received data is deserialized in the PersonPackage ctor.
                string message = Encoding.ASCII.GetString(buffer);
                onDataReceived(message);

                // Start receiving data again.
                clientSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, ReceiveCallback, null);
            }
            // Avoid Pokemon exception handling in cases like these.
            catch (SocketException ex)
            {
                ShowErrorDialog(ex.Message);
            }
            catch (ObjectDisposedException ex)
            {
                ShowErrorDialog(ex.Message);
            }
        }

        /// <summary>
        /// Provides a thread safe way to add a row to the data grid.
        /// </summary>
        //private void SubmitPersonToDataGrid(PersonPackage person)
        //{
        //    Invoke((Action)delegate
        //    {
        //        dataGridView.Rows.Add(person.Name, person.Age, person.IsMale);
        //    });
        //}

        private static void ShowErrorDialog(string message)
        {
            MessageBox.Show(message);
        }






        //private void ReceiveData()
        //{
        //    while (clientSocket.Connected)
        //    {
        //        int received = clientSocket.Receive(buffer);

        //        if (received == 0) // Assume the client has disconnected.
        //        {
        //            //Console.WriteLine("Client Disconnected");
        //            break;
        //        }

        //        // Shrink the buffer so we don't get null chars in the text.
        //        Array.Resize(ref buffer, received);
        //        string receivedMsg = Encoding.ASCII.GetString(buffer);
        //        // Reset the buffer.
        //        Array.Resize(ref buffer, clientSocket.ReceiveBufferSize);
        //        onDataReceived(receivedMsg);
        //    }

        //    // Assume the client has disconnected and start listening again for connections.
        //    //Console.WriteLine("\nListening again...");
        //    clientSocket = serverSocket.Accept();
        //    //Console.WriteLine("Client Connected");
        //    //Console.WriteLine("Waiting for data...");
        //    ReceiveData();
        //}

        protected virtual void onDataReceived(string receivedMsg)
        {
            if (dataReceived != null)
            {
                IPCEventArgs args = new IPCEventArgs();
                args.data = receivedMsg;
                dataReceived(this, args);
            }
        }
    }
}
