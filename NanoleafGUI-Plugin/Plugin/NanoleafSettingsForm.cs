using Lumos.GUI.BaseWindow;
using Lumos.GUI.Settings;
using LumosControls.Controls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using org.dmxc.lumos.Kernel.Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LumosLIB.Tools.I18n;

namespace NanoleafGUI_Plugin
{
    public partial class NanoleafSettingsForm : LumosForm
    {
        private SettingsManager sm;
        private AddControllerForm addControllerForm = new AddControllerForm();
        public NanoleafSettingsForm()
        {
            InitializeComponent();
            this.tabControl1.TabPages.Clear();
            sm = SettingsManager.getInstance();

            this.cbShowInInputAssignment.Checked = sm.getSetting<bool>(ESettingsType.APPLICATION, NanoleafGUI_Plugin.NANOLEAF_SHOW_IN_INPUTASSIGNMENT);
            this.cbDiscover.Checked = sm.getSetting<bool>(ESettingsType.APPLICATION, NanoleafGUI_Plugin.NANOLEAF_DISCOVER);
            this.cbAutoRequestToken.Checked = sm.getSetting<bool>(ESettingsType.APPLICATION, NanoleafGUI_Plugin.NANOLEAF_AUTOREQUEST_TOKEN);
            this.cbAutoConnect.Checked = sm.getSetting<bool>(ESettingsType.APPLICATION, NanoleafGUI_Plugin.NANOLEAF_AUTOCONNECT);
            this.nudRefreshRate.Value = sm.getSetting<int>(ESettingsType.APPLICATION, NanoleafGUI_Plugin.NANOLEAF_REFRESH_RATE);

            this.lbDiscoveryState.Text = string.Format(T._("Discover: {0}"), sm.getSetting<string>(ESettingsType.APPLICATION, NanoleafGUI_Plugin.NANOLEAF_DISCOVER_STATE));
            updateControllerTabPages();

            sm.SettingChanged += SettingsManager_SettingChanged;
        }
        ~NanoleafSettingsForm()
        {
            sm.SettingChanged -= SettingsManager_SettingChanged;
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            sm.SettingChanged -= SettingsManager_SettingChanged;
        }

        private void updateControllerTabPages()
        {
            string jsonDiscoveredControllers = sm.getSetting<string>(ESettingsType.APPLICATION, NanoleafGUI_Plugin.NANOLEAF_DISCOVERED_CONTROLLERS);
            string jsonControllers = sm.getSetting<string>(ESettingsType.APPLICATION, NanoleafGUI_Plugin.NANOLEAF_CONTROLLERS);
            JArray objDiscoveredControllers = JsonConvert.DeserializeObject(jsonDiscoveredControllers) as JArray;
            JArray objControllers = JsonConvert.DeserializeObject(jsonControllers) as JArray;
            tabControl1.TabPages.Clear();
            if (objControllers != null)
                foreach (var controller in objControllers.Children())
                {
                    string name = (string)controller["Name"];

                        tabControl1.TabPages.Add(new LumosTabPage(name) { Tag = controller });
                }
            if (objDiscoveredControllers != null)
                foreach (var controller in objDiscoveredControllers.Children())
                {
                    if (objControllers.Any(c => ((string)c["IP"]).Equals((string)controller["IP"])))
                        continue;

                    string name = (string)controller["Name"];
                    if (0 == (from System.Windows.Forms.TabPage tab in tabControl1.TabPages
                                                                        where tab.Name.Equals(name)
                              select tab).Count())
                        tabControl1.TabPages.Add(new LumosTabPage(name) { Tag = controller });
                }

            updateControllerPanel((LumosTabPage)tabControl1.SelectedTab);
        }
        private void updateControllerPanel(LumosTabPage tabPage)
        {
            if (tabPage?.Tag is JToken controller)
            {
                this.tbIP.Text = (string)controller["IP"];
                this.tbName.Text = (string)controller["Name"];

                this.tbAuthToken.Text = (string)controller["Auth_token"];
                this.tbModel.Text = (string)controller["Model"];
                this.tbNS.Text = (string)controller["SerialNumber"];
                this.tbHW.Text = (string)controller["HardwareVersion"];
                this.tbFW.Text = (string)controller["FirmwareVersion"];
            }
        }

        private void SettingsManager_SettingChanged(object sender, SettingChangedEventArgs args)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => SettingsManager_SettingChanged(sender, args)));
                return;
            }
            sm.SettingChanged -= SettingsManager_SettingChanged;
            try
            {
                switch (args.SettingsPath)
                {
                    case NanoleafGUI_Plugin.NANOLEAF_SHOW_IN_INPUTASSIGNMENT:
                        this.cbShowInInputAssignment.Checked = (bool)args.NewValue;
                        break;
                    case NanoleafGUI_Plugin.NANOLEAF_DISCOVER:
                        this.cbDiscover.Checked = (bool)args.NewValue;
                        break;
                    case NanoleafGUI_Plugin.NANOLEAF_AUTOREQUEST_TOKEN:
                        this.cbAutoRequestToken.Checked = (bool)args.NewValue;
                        break;
                    case NanoleafGUI_Plugin.NANOLEAF_AUTOCONNECT:
                        this.cbAutoConnect.Checked = (bool)args.NewValue;
                        break;
                    case NanoleafGUI_Plugin.NANOLEAF_REFRESH_RATE:
                        this.nudRefreshRate.Value = (int)args.NewValue;
                        break;
                    case NanoleafGUI_Plugin.NANOLEAF_DISCOVER_STATE:
                        this.lbDiscoveryState.Text = string.Format(T._("Discover: {0}"), (string)args.NewValue);
                        break;
                    case NanoleafGUI_Plugin.NANOLEAF_DISCOVERED_CONTROLLERS:
                    case NanoleafGUI_Plugin.NANOLEAF_CONTROLLERS:
                        this.updateControllerTabPages();
                        break;
                }
            }
            catch(Exception e)
            {
                log.ErrorOrDebug(string.Empty, e);
            }
            sm.SettingChanged += SettingsManager_SettingChanged;
        }

        private void cbShowInInputAssignment_CheckedChanged(object sender, EventArgs e)
        {
            sm.setSetting(ESettingsType.APPLICATION, NanoleafGUI_Plugin.NANOLEAF_SHOW_IN_INPUTASSIGNMENT, this.cbShowInInputAssignment.Checked);
        }
        private void cbDiscover_CheckedChanged(object sender, EventArgs e)
        {
            sm.setSetting(ESettingsType.APPLICATION, NanoleafGUI_Plugin.NANOLEAF_DISCOVER, this.cbDiscover.Checked);
        }
        private void cbAutoRequestToken_CheckedChanged(object sender, EventArgs e)
        {
            sm.setSetting(ESettingsType.APPLICATION, NanoleafGUI_Plugin.NANOLEAF_AUTOREQUEST_TOKEN, this.cbAutoRequestToken.Checked);
        }
        private void cbAutoConnect_CheckedChanged(object sender, EventArgs e)
        {
            sm.setSetting(ESettingsType.APPLICATION, NanoleafGUI_Plugin.NANOLEAF_AUTOCONNECT, this.cbAutoConnect.Checked);
        }
        private void nudRefreshRate_ValueChanged(object sender, EventArgs e)
        {
            sm.setSetting(ESettingsType.APPLICATION, NanoleafGUI_Plugin.NANOLEAF_REFRESH_RATE, (int)this.nudRefreshRate.Value);
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            updateControllerPanel((LumosTabPage)tabControl1.SelectedTab);
        }

        private void btRequestToken_Click(object sender, EventArgs e)
        {
            sm.setSetting(ESettingsType.APPLICATION, NanoleafGUI_Plugin.NANOLEAF_REQUEST_TOKEN, (string)((JToken)tabControl1.SelectedTab.Tag)["IP"]);
            Task.Delay(2000).GetAwaiter();
        }

        private void btAddController_Click(object sender, EventArgs e)
        {
            if(addControllerForm.ShowDialog() == DialogResult.OK)
            {
                JObject obj = new JObject();
                obj.Add("IP", addControllerForm.IP);
                obj.Add("Token", addControllerForm.Token);
                sm.setSetting(ESettingsType.APPLICATION, NanoleafGUI_Plugin.NANOLEAF_ADD_CONTROLLER, JsonConvert.SerializeObject(obj));
            }
        }
    }
}
