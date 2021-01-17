using System;
using System.Windows.Forms;
using IPC;


namespace InterProcessCommunication
{
    public partial class Sender : Form
    {
        IPCHandler ipcHandler;
        public Sender()
        {
            InitializeComponent();
            setupThisForm();
        }

        private void setupThisForm()
        {
            ipcHandler = new IPCHandler(SendReceiveApp.SENDER);
        }


        private void button1_Click(object sender, EventArgs e)
        {
            ipcHandler.sendMessage(tbToSend.Text);
        }

    }
}
