using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using DoodleEmpires.Engine.Net;
using System.IO;
using System.Threading;

namespace DoodleServer
{
    public partial class frm_main : Form
    {
        GameServer _server;
        Thread _serverThread;

        OpenFileDialog _loadDialog;
        SaveFileDialog _saveDialog;

        Queue<string> _commandStack = new Queue<string>();
        int _commandPointer = -1;

        public frm_main()
        {
            InitializeComponent();

            Console.SetOut(new TextBoxWriter(this));

            _loadDialog = new OpenFileDialog();
            _loadDialog.Filter = "Doodle Empires Map|*.dem";

            _saveDialog = new SaveFileDialog();
            _saveDialog.Filter = "Doodle Empires Map|*.dem";
        }

        public void AppendTextBox(string value)
        {
            if (InvokeRequired)
            {
                this.BeginInvoke(new Action<string>(AppendTextBox), new object[] { value });
                return;
            }
            rtb_output.Text += value;

            rtb_output.SelectionStart = rtb_output.Text.Length;
            rtb_output.ScrollToCaret();
        }

        private void btn_launch_Click(object sender, EventArgs e)
        {
            _server = new GameServer();
            
            _serverThread = new Thread(StartServer);
            _serverThread.Start();
        }

        private void StartServer()
        {
            _server.Run(new string[]{txt_serverName.Text, nud_port.Value.ToString()});
        }

        private void btn_perform_Click(object sender, EventArgs e)
        {
            string command = txt_commandIn.Text.Trim().ToLower();
            _commandPointer = -1;
            _commandStack.Enqueue(command);

            txt_commandIn.Text = "";

            DialogResult dResult;
            switch (command)
            {
                case "save":
                    dResult = _saveDialog.ShowDialog();

                    if (dResult == DialogResult.OK)
                    {
                        _server.Save(_saveDialog.OpenFile());
                    }
                    break;
                case "load":
                    dResult = _loadDialog.ShowDialog();

                    if (dResult == DialogResult.OK)
                    {
                        _server.Load(_loadDialog.OpenFile());
                    }
                    break;
            }
        }

        private void txt_commandIn_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up)
            {
                _commandPointer++;
                _commandPointer = _commandPointer > _commandStack.Count - 1 ? _commandStack.Count - 1 : _commandPointer;
                txt_commandIn.Text = _commandStack.ElementAt(_commandPointer);
                e.Handled = true;
            }

            if (e.KeyCode == Keys.Down)
            {
                _commandPointer--;
                _commandPointer = _commandPointer < 0 ? 0 : _commandPointer;
                txt_commandIn.Text = _commandStack.ElementAt(_commandPointer);
                e.Handled = true;
            }
            if (e.KeyCode == Keys.Enter)
            {
                btn_perform_Click(this, EventArgs.Empty);
                e.Handled = true;
            }
        }
    }

    public class TextBoxWriter : TextWriter
    {
        frm_main _output;

        public TextBoxWriter(frm_main form)
        {
            _output = form;
        }
        
        public override void Write(char value)
        {
            base.Write(value);
            _output.AppendTextBox(value.ToString());
        }

        public override Encoding Encoding
        {
            get { return System.Text.Encoding.UTF8; }
        }
    }

}
