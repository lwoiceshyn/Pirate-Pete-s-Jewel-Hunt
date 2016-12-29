namespace WindowsFormsApplication1
{
    partial class myGame
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
            this.components = new System.ComponentModel.Container();
            this.serialPort = new System.IO.Ports.SerialPort(this.components);
            this.cmbPortName = new System.Windows.Forms.ComboBox();
            this.butPortState = new System.Windows.Forms.Button();
            this.eventTimer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // serialPort
            // 
            this.serialPort.BaudRate = 128000;
            this.serialPort.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.serialPort1_DataReceived);
            // 
            // cmbPortName
            // 
            this.cmbPortName.BackColor = System.Drawing.SystemColors.Window;
            this.cmbPortName.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbPortName.ForeColor = System.Drawing.Color.Magenta;
            this.cmbPortName.FormattingEnabled = true;
            this.cmbPortName.Location = new System.Drawing.Point(16, 6);
            this.cmbPortName.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cmbPortName.Name = "cmbPortName";
            this.cmbPortName.Size = new System.Drawing.Size(99, 26);
            this.cmbPortName.TabIndex = 1;
            // 
            // butPortState
            // 
            this.butPortState.Font = new System.Drawing.Font("Arial", 10F);
            this.butPortState.ForeColor = System.Drawing.Color.Magenta;
            this.butPortState.Location = new System.Drawing.Point(124, 7);
            this.butPortState.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.butPortState.Name = "butPortState";
            this.butPortState.Size = new System.Drawing.Size(119, 28);
            this.butPortState.TabIndex = 2;
            this.butPortState.Text = "Connect";
            this.butPortState.UseVisualStyleBackColor = true;
            this.butPortState.Click += new System.EventHandler(this.butPortState_Click);
            // 
            // eventTimer
            // 
            this.eventTimer.Interval = 1;
            this.eventTimer.Tick += new System.EventHandler(this.eventTimer_Tick);
            // 
            // myGame
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlText;
            this.ClientSize = new System.Drawing.Size(892, 658);
            this.Controls.Add(this.butPortState);
            this.Controls.Add(this.cmbPortName);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "myGame";
            this.Text = "Pirate Pete\'s Jewel Hunt";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.IO.Ports.SerialPort serialPort;
        private System.Windows.Forms.ComboBox cmbPortName;
        private System.Windows.Forms.Button butPortState;
        private System.Windows.Forms.Timer eventTimer;
    }
}

