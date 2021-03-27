using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Lumos.GUI.BaseWindow;
using Lumos.GUI.Settings;
using LumosLIB.Tools.I18n;
using org.dmxc.lumos.Kernel.Settings;

namespace NanoleafGUI_Plugin
{
    public partial class AddControllerForm : LumosForm
    {
        public string IP
        {
            get { return this.tbIP.Text; }
        }
        public string Token
        {
            get { return this.tbToken.Text; }
        }

        public AddControllerForm()
        {
            InitializeComponent();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            this.tbIP.Text = null;
            this.tbToken.Text = null;
        }
        private void btAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.tbIP.Text))
            {
                MessageBox.Show(this, T._("IP can't be empty"), T._("Check IP"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else if (!this.tbIP.Text.ValidateIPv4())
            {
                MessageBox.Show(this, T._("IP invalide!"), T._("Check IP"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            bool autoRequestToken = SettingsManager.getInstance().getSetting<bool>(ESettingsType.APPLICATION, NanoleafGUI_Plugin.NANOLEAF_AUTOREQUEST_TOKEN);
            if (string.IsNullOrWhiteSpace(this.tbToken.Text) && autoRequestToken)
            {
                if (MessageBox.Show(this, T._("Please Hold the Powerbutton til you see a Visual Feedback on the Controller(5 - 7)s"), T._("Start Controller Pairing-Mode"), MessageBoxButtons.OK, MessageBoxIcon.Information) == DialogResult.OK)
                {
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            else
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
    }
}
