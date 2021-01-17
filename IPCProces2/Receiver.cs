using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using IPC;


namespace IPCProces2
{
    public partial class Receiver : Form
    {
        IPCHandler communicator;
        public Receiver()
        {
            InitializeComponent();
            setupThisForm();
        }

        private void setupThisForm()
        {
            communicator = new IPCHandler(SendReceiveApp.RECEIVER);
            communicator.dataWrittenToMemory += Communicator_dataWrittenToMemory;
        }

        public delegate void displayTextDelegate(string text);

        private void Communicator_dataWrittenToMemory(object sender, IPCEventArgs args)
        {
            displayText(args.data.ToString());
        }

        private void displayText(string text) 
        {
            if (this.InvokeRequired)
            {
                Invoke(new displayTextDelegate(displayText), text);
            }
            else
            {
                tbReceived.Text = text;
            }
        }

    }
}
