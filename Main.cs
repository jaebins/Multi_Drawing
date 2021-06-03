using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SharedDrawing
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private void but_CreateRoom_Click(object sender, EventArgs e)
        {
            new ServerConnect("", true);
        }

        private void but_JoinRoom_Click(object sender, EventArgs e)
        {
            new ServerConnect(InputIp.Text, false);
        }
    }
}
