namespace TFFtest
{
    partial class Compares
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
            this.track = new System.Windows.Forms.TrackBar();
            this.panel1 = new System.Windows.Forms.Panel();
            this.fav = new System.Windows.Forms.ListBox();
            this.Delete = new System.Windows.Forms.Button();
            this.AddBtn = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.Tabs = new System.Windows.Forms.TabControl();
            this.button2 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.track)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // track
            // 
            this.track.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.track.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.track.Location = new System.Drawing.Point(0, 462);
            this.track.Name = "track";
            this.track.Size = new System.Drawing.Size(853, 45);
            this.track.TabIndex = 3;
            this.track.ValueChanged += new System.EventHandler(this.track_ValueChanged);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.fav);
            this.panel1.Controls.Add(this.Delete);
            this.panel1.Controls.Add(this.AddBtn);
            this.panel1.Controls.Add(this.button1);
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(757, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(96, 462);
            this.panel1.TabIndex = 6;
            // 
            // fav
            // 
            this.fav.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fav.FormattingEnabled = true;
            this.fav.Location = new System.Drawing.Point(0, 0);
            this.fav.Name = "fav";
            this.fav.Size = new System.Drawing.Size(96, 253);
            this.fav.TabIndex = 6;
            this.fav.MouseClick += new System.Windows.Forms.MouseEventHandler(this.fav_MouseClick);
            // 
            // Delete
            // 
            this.Delete.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.Delete.Location = new System.Drawing.Point(0, 253);
            this.Delete.Name = "Delete";
            this.Delete.Size = new System.Drawing.Size(96, 23);
            this.Delete.TabIndex = 8;
            this.Delete.Text = "Delete";
            this.Delete.UseVisualStyleBackColor = true;
            this.Delete.Click += new System.EventHandler(this.Delete_Click);
            // 
            // AddBtn
            // 
            this.AddBtn.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.AddBtn.Location = new System.Drawing.Point(0, 276);
            this.AddBtn.Name = "AddBtn";
            this.AddBtn.Size = new System.Drawing.Size(96, 23);
            this.AddBtn.TabIndex = 7;
            this.AddBtn.Text = "Add";
            this.AddBtn.UseVisualStyleBackColor = true;
            this.AddBtn.Click += new System.EventHandler(this.AddBtn_Click);
            // 
            // button1
            // 
            this.button1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.button1.Location = new System.Drawing.Point(0, 299);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(96, 23);
            this.button1.TabIndex = 10;
            this.button1.Text = "Add Time";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.textBox1);
            this.panel2.Controls.Add(this.button2);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 322);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(96, 140);
            this.panel2.TabIndex = 9;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 47);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "label3";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "label2";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "label1";
            // 
            // Tabs
            // 
            this.Tabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Tabs.Location = new System.Drawing.Point(0, 0);
            this.Tabs.Name = "Tabs";
            this.Tabs.SelectedIndex = 0;
            this.Tabs.Size = new System.Drawing.Size(757, 462);
            this.Tabs.TabIndex = 8;
            this.Tabs.Selected += new System.Windows.Forms.TabControlEventHandler(this.Tabs_Selected);
            this.Tabs.TabIndexChanged += new System.EventHandler(this.Tabs_TabIndexChanged);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(12, 111);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 3;
            this.button2.Text = "Save shot";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(12, 81);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(75, 20);
            this.textBox1.TabIndex = 4;
            // 
            // Compares
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(853, 507);
            this.Controls.Add(this.Tabs);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.track);
            this.KeyPreview = true;
            this.Name = "Compares";
            this.Text = "Compares";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Compares_FormClosed);
            this.Shown += new System.EventHandler(this.Compares_Shown);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Compares_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.track)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TrackBar track;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ListBox fav;
        private System.Windows.Forms.Button Delete;
        private System.Windows.Forms.Button AddBtn;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabControl Tabs;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox textBox1;
    }
}