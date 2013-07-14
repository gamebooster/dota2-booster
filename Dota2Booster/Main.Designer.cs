namespace Dota2Booster {
  partial class Main {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      if (disposing && (components != null)) {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      this.components = new System.ComponentModel.Container();
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
      this.distanceUpDown = new System.Windows.Forms.NumericUpDown();
      this.statusLabel = new System.Windows.Forms.Label();
      this.searchTimer = new System.Windows.Forms.Timer(this.components);
      this.checkTimer = new System.Windows.Forms.Timer(this.components);
      this.usageTooltip = new System.Windows.Forms.ToolTip(this.components);
      this.fovUpDown = new System.Windows.Forms.NumericUpDown();
      this.rangeCheckbox = new System.Windows.Forms.CheckBox();
      this.linkLabel = new System.Windows.Forms.LinkLabel();
      ((System.ComponentModel.ISupportInitialize)(this.distanceUpDown)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.fovUpDown)).BeginInit();
      this.SuspendLayout();
      // 
      // distanceUpDown
      // 
      this.distanceUpDown.Location = new System.Drawing.Point(12, 6);
      this.distanceUpDown.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
      this.distanceUpDown.Name = "distanceUpDown";
      this.distanceUpDown.Size = new System.Drawing.Size(138, 20);
      this.distanceUpDown.TabIndex = 0;
      this.usageTooltip.SetToolTip(this.distanceUpDown, "Camera Distance Value");
      this.distanceUpDown.Value = new decimal(new int[] {
            1134,
            0,
            0,
            0});
      this.distanceUpDown.ValueChanged += new System.EventHandler(this.DistanceValueChanged);
      // 
      // statusLabel
      // 
      this.statusLabel.AutoSize = true;
      this.statusLabel.Location = new System.Drawing.Point(10, 11);
      this.statusLabel.Name = "statusLabel";
      this.statusLabel.Size = new System.Drawing.Size(0, 13);
      this.statusLabel.TabIndex = 2;
      // 
      // searchTimer
      // 
      this.searchTimer.Interval = 1000;
      this.searchTimer.Tick += new System.EventHandler(this.SearchTimerTick);
      // 
      // checkTimer
      // 
      this.checkTimer.Interval = 200;
      this.checkTimer.Tick += new System.EventHandler(this.CheckTimerTick);
      // 
      // fovUpDown
      // 
      this.fovUpDown.Location = new System.Drawing.Point(13, 33);
      this.fovUpDown.Name = "fovUpDown";
      this.fovUpDown.Size = new System.Drawing.Size(137, 20);
      this.fovUpDown.TabIndex = 6;
      this.usageTooltip.SetToolTip(this.fovUpDown, "Field of View Value");
      this.fovUpDown.ValueChanged += new System.EventHandler(this.fovUpDown_ValueChanged);
      // 
      // rangeCheckbox
      // 
      this.rangeCheckbox.AutoSize = true;
      this.rangeCheckbox.Location = new System.Drawing.Point(13, 59);
      this.rangeCheckbox.Name = "rangeCheckbox";
      this.rangeCheckbox.Size = new System.Drawing.Size(118, 17);
      this.rangeCheckbox.TabIndex = 7;
      this.rangeCheckbox.Text = "dota_range_display";
      this.usageTooltip.SetToolTip(this.rangeCheckbox, "Toggle range display");
      this.rangeCheckbox.UseVisualStyleBackColor = true;
      this.rangeCheckbox.CheckedChanged += new System.EventHandler(this.rangeCheckbox_CheckedChanged);
      // 
      // linkLabel
      // 
      this.linkLabel.AutoSize = true;
      this.linkLabel.Location = new System.Drawing.Point(9, 91);
      this.linkLabel.Name = "linkLabel";
      this.linkLabel.Size = new System.Drawing.Size(138, 13);
      this.linkLabel.TabIndex = 5;
      this.linkLabel.TabStop = true;
      this.linkLabel.Text = "gamebooster/dota2-booster";
      this.linkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkLabelLinkClicked);
      // 
      // Main
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(160, 116);
      this.Controls.Add(this.rangeCheckbox);
      this.Controls.Add(this.fovUpDown);
      this.Controls.Add(this.linkLabel);
      this.Controls.Add(this.statusLabel);
      this.Controls.Add(this.distanceUpDown);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.MaximizeBox = false;
      this.MaximumSize = new System.Drawing.Size(200, 200);
      this.MinimizeBox = false;
      this.Name = "Main";
      this.ShowIcon = false;
      this.Text = "Dota 2 Booster";
      this.usageTooltip.SetToolTip(this, "gamebooster.github.com");
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Main_FormClosing);
      ((System.ComponentModel.ISupportInitialize)(this.distanceUpDown)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.fovUpDown)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.NumericUpDown distanceUpDown;
    private System.Windows.Forms.Label statusLabel;
    private System.Windows.Forms.Timer searchTimer;
    private System.Windows.Forms.Timer checkTimer;
    private System.Windows.Forms.ToolTip usageTooltip;
    private System.Windows.Forms.LinkLabel linkLabel;
    private System.Windows.Forms.NumericUpDown fovUpDown;
    private System.Windows.Forms.CheckBox rangeCheckbox;
  }
}

