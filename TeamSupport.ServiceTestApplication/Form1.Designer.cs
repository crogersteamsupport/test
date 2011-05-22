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
      this.timerEmails = new System.Windows.Forms.Timer(this.components);
      this.btnOnStart = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // btnOnStart
      // 
      this.btnOnStart.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.btnOnStart.Font = new System.Drawing.Font("Trebuchet MS", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnOnStart.ForeColor = System.Drawing.Color.Green;
      this.btnOnStart.Location = new System.Drawing.Point(12, 12);
      this.btnOnStart.Name = "btnOnStart";
      this.btnOnStart.Size = new System.Drawing.Size(264, 208);
      this.btnOnStart.TabIndex = 21;
      this.btnOnStart.Text = "Start";
      this.btnOnStart.UseVisualStyleBackColor = true;
      this.btnOnStart.Click += new System.EventHandler(this.btnOnStart_Click);
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(288, 232);
      this.Controls.Add(this.btnOnStart);
      this.Name = "Form1";
      this.Text = "Email Test";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
      this.Load += new System.EventHandler(this.Form1_Load);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Timer timerEmails;
    private System.Windows.Forms.Button btnOnStart;
  }
}

