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
            try
            {
                InitializeComponent();
                this.tabControl1.TabPages.Clear();
                sm = SettingsManager.getInstance();

                this.cbShowInInputAssignment.Checked = sm.GetKernelSetting<bool>(ESettingsType.APPLICATION, NanoleafGUI_Plugin.NANOLEAF_SHOW_IN_INPUTASSIGNMENT);
                this.cbDiscover.Checked = sm.GetKernelSetting<bool>(ESettingsType.APPLICATION, NanoleafGUI_Plugin.NANOLEAF_DISCOVER);
                this.cbAutoRequestToken.Checked = sm.GetKernelSetting<bool>(ESettingsType.APPLICATION, NanoleafGUI_Plugin.NANOLEAF_AUTOREQUEST_TOKEN);
                this.cbAutoConnect.Checked = sm.GetKernelSetting<bool>(ESettingsType.APPLICATION, NanoleafGUI_Plugin.NANOLEAF_AUTOCONNECT);
                this.nudRefreshRate.Value = sm.GetKernelSetting<int>(ESettingsType.APPLICATION, NanoleafGUI_Plugin.NANOLEAF_REFRESH_RATE);

                this.lbDiscoveryState.Text = string.Format(T._("Discover: {0}"), sm.GetKernelSetting<string>(ESettingsType.APPLICATION, NanoleafGUI_Plugin.NANOLEAF_DISCOVER_STATE));
                updateControllerTabPages();

                sm.SettingChanged += SettingsManager_SettingChanged;
            }
            catch (Exception e)
            {
                NanoleafGUI_Plugin.Log.Error(e);
            }
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
            try
            {
                string jsonDiscoveredControllers = sm.GetKernelSetting<string>(ESettingsType.APPLICATION, NanoleafGUI_Plugin.NANOLEAF_DISCOVERED_CONTROLLERS);
                string jsonControllers = sm.GetKernelSetting<string>(ESettingsType.APPLICATION, NanoleafGUI_Plugin.NANOLEAF_CONTROLLERS);
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
            catch (Exception e)
            {
                NanoleafGUI_Plugin.Log.Error(e);
            }
        }
        private void updateControllerPanel(LumosTabPage tabPage)
        {
            try
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
                    this.pictureBox1.BackgroundImage = this.renderLayout(controller);
                }
            }
            catch (Exception e)
            {
                NanoleafGUI_Plugin.Log.Error(e);
            }
        }

        private void SettingsManager_SettingChanged(object sender, SettingChangedEventArgs args)
        {
            try
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
                catch (Exception e)
                {
                    log.ErrorOrDebug(string.Empty, e);
                }
                sm.SettingChanged += SettingsManager_SettingChanged;
            }
            catch (Exception e)
            {
                NanoleafGUI_Plugin.Log.Error(e);
            }
        }

        private void cbShowInInputAssignment_CheckedChanged(object sender, EventArgs e)
        {
            sm.SetKernelSetting(ESettingsType.APPLICATION, NanoleafGUI_Plugin.NANOLEAF_SHOW_IN_INPUTASSIGNMENT, this.cbShowInInputAssignment.Checked);
        }
        private void cbDiscover_CheckedChanged(object sender, EventArgs e)
        {
            sm.SetKernelSetting(ESettingsType.APPLICATION, NanoleafGUI_Plugin.NANOLEAF_DISCOVER, this.cbDiscover.Checked);
        }
        private void cbAutoRequestToken_CheckedChanged(object sender, EventArgs e)
        {
            sm.SetKernelSetting(ESettingsType.APPLICATION, NanoleafGUI_Plugin.NANOLEAF_AUTOREQUEST_TOKEN, this.cbAutoRequestToken.Checked);
        }
        private void cbAutoConnect_CheckedChanged(object sender, EventArgs e)
        {
            sm.SetKernelSetting(ESettingsType.APPLICATION, NanoleafGUI_Plugin.NANOLEAF_AUTOCONNECT, this.cbAutoConnect.Checked);
        }
        private void nudRefreshRate_ValueChanged(object sender, EventArgs e)
        {
            sm.SetKernelSetting(ESettingsType.APPLICATION, NanoleafGUI_Plugin.NANOLEAF_REFRESH_RATE, (int)this.nudRefreshRate.Value);
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            updateControllerPanel((LumosTabPage)tabControl1.SelectedTab);
        }

        private void btRequestToken_Click(object sender, EventArgs e)
        {
            try
            {
                if (tabControl1.SelectedTab?.Tag == null)
                    return;
                sm.SetKernelSetting(ESettingsType.APPLICATION, NanoleafGUI_Plugin.NANOLEAF_REQUEST_TOKEN, (string)((JToken)tabControl1.SelectedTab?.Tag)["IP"]);
                Task.Delay(2000).GetAwaiter();
            }
            catch (Exception ex)
            {
                NanoleafGUI_Plugin.Log.Error(ex);
            }
        }

        private void btAddController_Click(object sender, EventArgs e)
        {
            try
            {
                if (addControllerForm.ShowDialog() == DialogResult.OK)
                {
                    JObject obj = new JObject();
                    obj.Add("IP", addControllerForm.IP);
                    obj.Add("Token", addControllerForm.Token);
                    sm.SetKernelSetting(ESettingsType.APPLICATION, NanoleafGUI_Plugin.NANOLEAF_ADD_CONTROLLER, JsonConvert.SerializeObject(obj));
                }
            }
            catch (Exception ex)
            {
                NanoleafGUI_Plugin.Log.Error(ex);
            }
        }

        private Bitmap renderLayout(JToken controller)
        {
            try
            {
                var panels = controller["Panels"];
                int maxX = panels.Select(p => (int)p["X"]).Max();
                int maxY = panels.Select(p => (int)p["Y"]).Max();
                int minX = panels.Select(p => (int)p["X"]).Min();
                int minY = panels.Select(p => (int)p["Y"]).Min();
                int maxSize = panels.Select(p => (int)p["SideLength"]).Max();

                int penSize = 2;
                int width = maxX - minX;
                int height = maxY - minY;
                width += maxSize + penSize;
                height += maxSize + penSize;
                Bitmap bmp = new Bitmap(width, height);
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    Pen pen = new Pen(this.ForeColor, penSize);
                    Brush brush = new SolidBrush(this.ForeColor);
                    Font font = this.Font;
                    StringFormat stringFormat = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                    int i = 0;
                    foreach (var panel in panels)
                    {
                        i++;

                        int id = (int)panel["ID"];
                        int x = (int)panel["X"];
                        int y = (int)panel["Y"];
                        y = maxY - y;
                        x += penSize / 2;
                        y += penSize / 2;
                        int size = (int)panel["SideLength"];
                        int orientation = (int)panel["Orientation"];

                        string str = $"ID: {id}" + Environment.NewLine + $"Index: {i}" + Environment.NewLine + $"{orientation}°";

                        var shape = (int)panel["Shape"];
                        switch (shape)
                        {
                            //Square
                            case 2:
                            case 3:
                            case 4:
                                var rect = new RectangleF(x, y, size, size);
                                g.DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height);
                                g.DrawString(str, font, brush, rect, stringFormat);
                                if (shape == 3 || shape == 4)
                                {
                                    int xOffset = size / 2;
                                    int buttonSize = size / 16;
                                    int buttonOffset = size / 12;
                                    for (int j = 0; j < 6; j++)
                                    {
                                        g.FillRectangle(brush, rect.X + xOffset, rect.Bottom - (buttonSize + penSize), buttonSize, buttonSize);
                                        xOffset += buttonOffset;
                                    }
                                }
                                break;
                        }
                    }
                }
                return bmp;
            }
            catch (Exception e)
            {
                NanoleafGUI_Plugin.Log.Error(e);
            }
            return null;
        }
    }
}