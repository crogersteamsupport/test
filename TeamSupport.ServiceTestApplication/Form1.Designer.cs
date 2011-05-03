namespace TeamSupport.ServiceTestApplication
{
  partial class Form1
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
      this.button12 = new System.Windows.Forms.Button();
      this.button1 = new System.Windows.Forms.Button();
      this.btnSLA = new System.Windows.Forms.Button();
      this.timerEmails = new System.Windows.Forms.Timer(this.components);
      this.button2 = new System.Windows.Forms.Button();
      this.listBox1 = new System.Windows.Forms.ListBox();
      this.button3 = new System.Windows.Forms.Button();
      this.btnOnStart = new System.Windows.Forms.Button();
      this.btnOnStop = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // button12
      // 
      this.button12.Location = new System.Drawing.Point(10, 12);
      this.button12.Name = "button12";
      this.button12.Size = new System.Drawing.Size(120, 23);
      this.button12.TabIndex = 14;
      this.button12.Text = "Process Email Queue";
      this.button12.UseVisualStyleBackColor = true;
      this.button12.Click += new System.EventHandler(this.button12_Click);
      // 
      // button1
      // 
      this.button1.Location = new System.Drawing.Point(136, 12);
      this.button1.Name = "button1";
      this.button1.Size = new System.Drawing.Size(137, 23);
      this.button1.TabIndex = 16;
      this.button1.Text = "Sync TS and Muroc";
      this.button1.UseVisualStyleBackColor = true;
      this.button1.Click += new System.EventHandler(this.button1_Click);
      // 
      // btnSLA
      // 
      this.btnSLA.Location = new System.Drawing.Point(279, 12);
      this.btnSLA.Name = "btnSLA";
      this.btnSLA.Size = new System.Drawing.Size(105, 23);
      this.btnSLA.TabIndex = 17;
      this.btnSLA.Text = "Process SLA";
      this.btnSLA.UseVisualStyleBackColor = true;
      this.btnSLA.Click += new System.EventHandler(this.btnSLA_Click);
      // 
      // timerEmails
      // 
      this.timerEmails.Interval = 10000;
      this.timerEmails.Tick += new System.EventHandler(this.timerEmails_Tick);
      // 
      // button2
      // 
      this.button2.Location = new System.Drawing.Point(12, 41);
      this.button2.Name = "button2";
      this.button2.Size = new System.Drawing.Size(75, 23);
      this.button2.TabIndex = 18;
      this.button2.Text = "IIS Logs";
      this.button2.UseVisualStyleBackColor = true;
      this.button2.Click += new System.EventHandler(this.button2_Click);
      // 
      // listBox1
      // 
      this.listBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.listBox1.FormattingEnabled = true;
      this.listBox1.Location = new System.Drawing.Point(10, 106);
      this.listBox1.Name = "listBox1";
      this.listBox1.Size = new System.Drawing.Size(374, 420);
      this.listBox1.TabIndex = 19;
      // 
      // button3
      // 
      this.button3.Location = new System.Drawing.Point(93, 41);
      this.button3.Name = "button3";
      this.button3.Size = new System.Drawing.Size(107, 23);
      this.button3.TabIndex = 20;
      this.button3.Text = "Loop IIS Logs";
      this.button3.UseVisualStyleBackColor = true;
      this.button3.Click += new System.EventHandler(this.button3_Click);
      // 
      // btnOnStart
      // 
      this.btnOnStart.Location = new System.Drawing.Point(12, 70);
      this.btnOnStart.Name = "btnOnStart";
      this.btnOnStart.Size = new System.Drawing.Size(75, 23);
      this.btnOnStart.TabIndex = 21;
      this.btnOnStart.Text = "On Start";
      this.btnOnStart.UseVisualStyleBackColor = true;
      this.btnOnStart.Click += new System.EventHandler(this.btnOnStart_Click);
      // 
      // btnOnStop
      // 
      this.btnOnStop.Location = new System.Drawing.Point(93, 70);
      this.btnOnStop.Name = "btnOnStop";
      this.btnOnStop.Size = new System.Drawing.Size(75, 23);
      this.btnOnStop.TabIndex = 22;
      this.btnOnStop.Text = "On Stop";
      this.btnOnStop.UseVisualStyleBackColor = true;
      this.btnOnStop.Click += new System.EventHandler(this.btnOnStop_Click);
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(395, 535);
      this.Controls.Add(this.btnOnStop);
      this.Controls.Add(this.btnOnStart);
      this.Controls.Add(this.button3);
      this.Controls.Add(this.listBox1);
      this.Controls.Add(this.button2);
      this.Controls.Add(this.btnSLA);
      this.Controls.Add(this.button1);
      this.Controls.Add(this.button12);
      this.Name = "Form1";
      this.Text = "Email Test";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
      this.Load += new System.EventHandler(this.Form1_Load);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button button12;
    private System.Windows.Forms.Button button1;
    private System.Windows.Forms.Button btnSLA;
    private System.Windows.Forms.Timer timerEmails;
    private System.Windows.Forms.Button button2;
    private System.Windows.Forms.ListBox listBox1;
    private System.Windows.Forms.Button button3;
    private System.Windows.Forms.Button btnOnStart;
    private System.Windows.Forms.Button btnOnStop;
  }
}

