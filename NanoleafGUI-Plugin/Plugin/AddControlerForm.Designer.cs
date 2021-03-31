
using LumosControls.Controls;

namespace NanoleafGUI_Plugin
{
    partial class AddControllerForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddControllerForm));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tbToken = new LumosControls.Controls.WatermarkTextBox_v2();
            this.lbToken = new LumosControls.Controls.LumosLabel();
            this.lbIP = new LumosControls.Controls.LumosLabel();
            this.btAdd = new LumosControls.Controls.LumosButton();
            this.btCancel = new LumosControls.Controls.LumosButton();
            this.tbIP = new LumosControls.Controls.WatermarkTextBox_v2();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 140F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 141F));
            this.tableLayoutPanel1.Controls.Add(this.tbToken, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.lbToken, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.lbIP, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.btAdd, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.btCancel, 3, 2);
            this.tableLayoutPanel1.Controls.Add(this.tbIP, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 42F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(551, 116);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // tbToken
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.tbToken, 3);
            this.tbToken.Dock = System.Windows.Forms.DockStyle.Top;
            this.tbToken.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.tbToken.Location = new System.Drawing.Point(54, 40);
            this.tbToken.Name = "tbToken";
            this.tbToken.Size = new System.Drawing.Size(494, 20);
            this.tbToken.TabIndex = 11;
            this.tbToken.Watermark = "(optional)";
            this.tbToken.WatermarkColor = System.Drawing.SystemColors.GrayText;
            this.tbToken.WatermarkStyle = LumosControls.Controls.EShowWatermark.Empty;
            // 
            // lbToken
            // 
            this.lbToken.AutoSize = true;
            this.lbToken.Dock = System.Windows.Forms.DockStyle.Top;
            this.lbToken.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.lbToken.Location = new System.Drawing.Point(3, 40);
            this.lbToken.Margin = new System.Windows.Forms.Padding(3);
            this.lbToken.Name = "lbToken";
            this.lbToken.Size = new System.Drawing.Size(48, 16);
            this.lbToken.TabIndex = 9;
            this.lbToken.Text = "Token:";
            // 
            // lbIP
            // 
            this.lbIP.AutoSize = true;
            this.lbIP.Dock = System.Windows.Forms.DockStyle.Top;
            this.lbIP.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.lbIP.Location = new System.Drawing.Point(3, 3);
            this.lbIP.Margin = new System.Windows.Forms.Padding(3);
            this.lbIP.Name = "lbIP";
            this.lbIP.Size = new System.Drawing.Size(48, 16);
            this.lbIP.TabIndex = 7;
            this.lbIP.Text = "IP:";
            // 
            // btAdd
            // 
            this.btAdd.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btAdd.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.btAdd.Location = new System.Drawing.Point(273, 77);
            this.btAdd.Name = "btAdd";
            this.btAdd.Size = new System.Drawing.Size(134, 36);
            this.btAdd.TabIndex = 0;
            this.btAdd.Text = "Add";
            this.btAdd.UseVisualStyleBackColor = true;
            this.btAdd.Click += new System.EventHandler(this.btAdd_Click);
            // 
            // btCancel
            // 
            this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btCancel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.btCancel.Location = new System.Drawing.Point(413, 77);
            this.btCancel.Name = "btCancel";
            this.btCancel.Size = new System.Drawing.Size(135, 36);
            this.btCancel.TabIndex = 1;
            this.btCancel.Text = "Cancel";
            this.btCancel.UseVisualStyleBackColor = true;
            // 
            // tbIP
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.tbIP, 3);
            this.tbIP.Dock = System.Windows.Forms.DockStyle.Top;
            this.tbIP.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.tbIP.Location = new System.Drawing.Point(54, 3);
            this.tbIP.Name = "tbIP";
            this.tbIP.Size = new System.Drawing.Size(494, 20);
            this.tbIP.TabIndex = 10;
            this.tbIP.Watermark = "192.168.1.201";
            this.tbIP.WatermarkColor = System.Drawing.SystemColors.GrayText;
            this.tbIP.WatermarkStyle = LumosControls.Controls.EShowWatermark.Empty;
            // 
            // AddControllerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btCancel;
            this.ClientSize = new System.Drawing.Size(551, 116);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "AddControllerForm";
            this.Text = "Add Controller";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private WatermarkTextBox_v2 tbToken;
        private LumosLabel lbToken;
        private LumosLabel lbIP;
        private LumosButton btAdd;
        private LumosButton btCancel;
        private WatermarkTextBox_v2 tbIP;
    }
}