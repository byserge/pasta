namespace Pasta.Screenshot
{
    partial class EditorForm
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
            this.effectsToolStrip = new System.Windows.Forms.ToolStrip();
            this.actionsToolStrip = new System.Windows.Forms.ToolStrip();
            this.SuspendLayout();
            // 
            // effectsToolStrip
            // 
            this.effectsToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.effectsToolStrip.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.effectsToolStrip.Location = new System.Drawing.Point(0, 0);
            this.effectsToolStrip.Name = "effectsToolStrip";
            this.effectsToolStrip.Size = new System.Drawing.Size(284, 25);
            this.effectsToolStrip.TabIndex = 0;
            this.effectsToolStrip.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.effectsToolStrip_ItemClicked);
            // 
            // actionsToolStrip
            // 
            this.actionsToolStrip.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.actionsToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.actionsToolStrip.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.actionsToolStrip.Location = new System.Drawing.Point(0, 236);
            this.actionsToolStrip.Name = "actionsToolStrip";
            this.actionsToolStrip.Size = new System.Drawing.Size(284, 25);
            this.actionsToolStrip.TabIndex = 1;
            this.actionsToolStrip.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.actionsToolStrip_ItemClicked);
            // 
            // EditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.actionsToolStrip);
            this.Controls.Add(this.effectsToolStrip);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "EditorForm";
            this.Text = "Form1";
            this.TopMost = true;
            this.Deactivate += new System.EventHandler(this.EditorForm_Deactivate);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.EditorForm_FormClosed);
            this.Load += new System.EventHandler(this.EditorForm_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.EditorForm_Paint);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.EditorForm_KeyDown);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.EditorForm_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.EditorForm_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.EditorForm_MouseUp);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

		#endregion

		private System.Windows.Forms.ToolStrip effectsToolStrip;
        private System.Windows.Forms.ToolStrip actionsToolStrip;
    }
}

