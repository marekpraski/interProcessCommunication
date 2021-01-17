using System;
using System.Net;
using System.Net.Sockets;
using System.Text;


namespace IPC
{
    /// <summary>
    /// Represents the client side application.
    /// </summary>
    internal class IPCClient
    {
        private Socket clientSocket;
        private int port;

        internal IPCClient(int port)
        {
            this.port = port;
        }

        internal bool setupClient()
        {
            try
            {
                clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                // Change IPAddress.Loopback to a remote IP to connect to a remote host.
                clientSocket.Connect(IPAddress.Loopback, port);
                return true;
            }
            catch (SocketException ex)
            {
                return false;
            }
        }

        internal bool sendMessage(string message)
        {
            try
            {
                clientSocket.Send(Encoding.ASCII.GetBytes(message));
                return true;
            }
            catch
            {
                return false;
            }
        }

        internal void closeClient()
        {
            clientSocket.Close();
            Environment.Exit(0);
        }
    }
}
