using Lumos.GUI.BaseWindow;
using Lumos.GUI.Settings;
using LumosLIB.Tools.I18n;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using org.dmxc.lumos.Kernel.Settings;
using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

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

        private async void btRequestToken_Click(object sender, EventArgs e)
        {
            try
            {
                if (tabControl1.SelectedTab?.Tag == null)
                    return;
                sm.SetKernelSetting(ESettingsType.APPLICATION, NanoleafGUI_Plugin.NANOLEAF_REQUEST_TOKEN, (string)((JToken)tabControl1.SelectedTab?.Tag)["IP"]);
                await Task.Delay(500);
                sm.SetKernelSetting(ESettingsType.APPLICATION, NanoleafGUI_Plugin.NANOLEAF_REQUEST_TOKEN, "");
            }
            catch (Exception ex)
            {
                NanoleafGUI_Plugin.Log.Error(ex);
            }
        }

        private void btRemoveController_Click(object sender, EventArgs e)
        {
            try
            {
                if (tabControl1.SelectedTab?.Tag == null)
                    return;
                if (MessageBox.Show("Do you want to remove this controller?", "Remove Controller?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    sm.SetKernelSetting(ESettingsType.APPLICATION, NanoleafGUI_Plugin.NANOLEAF_REMOVE_CONTROLLER, (string)((JToken)tabControl1.SelectedTab?.Tag)["IP"]);
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
                int globalOrientation = (int)controller["GlobalOrientation"];

                int penSize = 2;
                int width = maxX - minX;
                int height = maxY - minY;
                width += maxSize * 2 + penSize; // *2 because the width of the hexagons is 2 * SideLength
                height += maxSize * 2 + penSize; // *2 because the width of the hexagons is 2 * SideLength
                Bitmap bmp = new Bitmap(width, height);
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    Pen pen = new Pen(this.ForeColor, penSize);
                    Brush brush = new SolidBrush(this.ForeColor);
                    Font font = this.Font;
                    StringFormat stringFormat = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };

                    foreach (var (panel, i) in panels.Select((value, i) => (value, i)))
                    {
                        int id = (int)panel["ID"];
                        int x = (int)panel["X"];
                        int y = (int)panel["Y"];
                        y = maxY - y;
                        x += penSize / 2;
                        y += penSize / 2;
                        int size = (int)panel["SideLength"];
                        int orientation = (int)panel["Orientation"];
                        RectangleF rect_text;
                        string str = $"ID: {id}" + Environment.NewLine + $"Index: {i}" + Environment.NewLine + $"{orientation}°";
                        var shape = (int)panel["Shape"];
                        switch (shape)
                        {
                            //Canvas
                            case 2: // Square
                            case 3: // Controller (active)
                            case 4: // Controller (inactive)
                                RectangleF rect = new RectangleF(x, y, size, size);
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
                            //Shapes
                            case 7: //Hexagon
                                // Shift by maxSize because the Shapes return their center point
                                var x_hex = x + maxSize;
                                var y_hex = y + maxSize;

                                rect_text = new RectangleF(x_hex - size, y_hex - size, size * 2, size * 2);

                                //Create 6 points
                                var points_hex = new PointF[6];
                                for (int a = 0; a < 6; a++)
                                {
                                    points_hex[a] = new PointF(
                                        x_hex + size * (float)Math.Cos((a * 60 + orientation) * Math.PI / 180f),
                                        y_hex + size * (float)Math.Sin((a * 60 + orientation) * Math.PI / 180f));
                                }
                                g.DrawPolygon(pen, points_hex);
                                g.DrawString(str, font, brush, rect_text, stringFormat);
                                break;
                            case 8: //Large Triangle
                            case 9: //Small Triangle
                                // Shift by maxSize because the Shapes return their center point
                                var x_tri = x + maxSize;
                                var y_tri = y + maxSize;

                                rect_text = new RectangleF(x_tri - (size / 2), y_tri - (size / 2), size, size);
                                var localFont = font;
                                if (shape == 9) localFont = new Font("Microsoft Sans Serif", 7);

                                //Create 3 points
                                var points_tri = new PointF[3];
                                for (int a = 0; a < 3; a++)
                                {
                                    points_tri[a] = new PointF(
                                        x_tri + (float)(size / Math.Sqrt(3)) * (float)Math.Cos((a * 120 + 30 + orientation) * Math.PI / 180f),
                                        y_tri + (float)(size / Math.Sqrt(3)) * (float)Math.Sin((a * 120 + 30 + orientation) * Math.PI / 180f));
                                }
                                g.DrawPolygon(pen, points_tri);
                                g.DrawString(str, localFont, brush, rect_text, stringFormat);
                                break;
                            case 12: //Controller (active)
                                // Shift by maxSize because the Shapes return their center point
                                orientation = ((360 - orientation) - 120) % 360;

                                int x_offset = 0;
                                float y_offset = 0;

                                switch (orientation)
                                {

                                    case 0:
                                    case -360:
                                        x_offset = 9;
                                        y_offset = 4.5f;
                                        break;
                                    case 60: //tested
                                    case -300:
                                        x_offset = 0;
                                        y_offset = 9f;
                                        break;
                                    case 120:
                                    case -240:
                                        x_offset = -9;
                                        y_offset = 4.5f;
                                        break;
                                    case 180: //tested
                                    case -180:
                                        x_offset = -9;
                                        y_offset = -4.5f;
                                        break;
                                    case 240: //tested
                                    case -120:
                                        x_offset = 0;
                                        y_offset = -9f;
                                        break;
                                    case 300: //tested
                                    case -60:
                                        x_offset = 9;
                                        y_offset = -4.5f;
                                        break;
                                }


                                var x_ctrl = x + maxSize + x_offset;
                                var y_ctrl = y + maxSize + y_offset;


                                //Create 4 points
                                var points_ctrl = new PointF[4];

                                var temp_point = new PointF(
                                        x_ctrl + (float)(67 / Math.Sqrt(3)) * (float)Math.Cos((30 + orientation) * Math.PI / 180f),
                                        y_ctrl + (float)(67 / Math.Sqrt(3)) * (float)Math.Sin((30 + orientation) * Math.PI / 180f));

                                points_ctrl[3] = new PointF(
                                        x_ctrl + (float)(67 / Math.Sqrt(3)) * (float)Math.Cos((1 * 120 + 30 + orientation) * Math.PI / 180f),
                                        y_ctrl + (float)(67 / Math.Sqrt(3)) * (float)Math.Sin((1 * 120 + 30 + orientation) * Math.PI / 180f));
                                points_ctrl[0] = new PointF(
                                        x_ctrl + (float)(67 / Math.Sqrt(3)) * (float)Math.Cos((2 * 120 + 30 + orientation) * Math.PI / 180f),
                                        y_ctrl + (float)(67 / Math.Sqrt(3)) * (float)Math.Sin((2 * 120 + 30 + orientation) * Math.PI / 180f));

                                points_ctrl[1] = new PointF(
                                        points_ctrl[0].X + (float)((temp_point.X - points_ctrl[0].X) * 0.2),
                                        points_ctrl[0].Y + (float)((temp_point.Y - points_ctrl[0].Y) * 0.2));

                                points_ctrl[2] = new PointF(
                                        points_ctrl[3].X - (float)((points_ctrl[3].X - temp_point.X) * 0.2),
                                        points_ctrl[3].Y - (float)((points_ctrl[3].Y - temp_point.Y) * 0.2));

                                g.DrawPolygon(pen, points_ctrl);
                                break;
                        }
                    }
                }
                //int globalOrientation = (int)controller["GlobalOrientation"];
                //return RotateImage(bmp, globalOrientation);
                return bmp;
            }
            catch (Exception e)
            {
                NanoleafGUI_Plugin.Log.Error(e);
            }
            return null;
        }

        private Bitmap RotateImage(Bitmap bmp, float angle)
        {
            Bitmap rotatedImage = new Bitmap(bmp.Width, bmp.Height);
            rotatedImage.SetResolution(bmp.HorizontalResolution, bmp.VerticalResolution);

            using (Graphics g = Graphics.FromImage(rotatedImage))
            {
                // Set the rotation point to the center in the matrix
                g.TranslateTransform(bmp.Width / 2, bmp.Height / 2);
                // Rotate
                g.RotateTransform(angle);
                // Restore rotation point in the matrix
                g.TranslateTransform(-bmp.Width / 2, -bmp.Height / 2);
                // Draw the image on the bitmap
                g.DrawImage(bmp, new Point(0, 0));
            }

            return rotatedImage;
        }
    }
}