using System;
using System.Windows.Forms;
using IPC;


namespace InterProcessCommunication
{
    public partial class Sender : Form
    {
        IPCHandler communicator;
        public Sender()
        {
            InitializeComponent();
            setupThisForm();
        }

        private void setupThisForm()
        {
            communicator = new IPCHandler(SendReceiveApp.SENDER);
        }


        private void button1_Click(object sender, EventArgs e)
        {
            communicator.write(SendReceiveApp.SENDER, tbToSend.Text);
            //communicator.sendMessage(tbToSend.Text);
        }

    }
}
