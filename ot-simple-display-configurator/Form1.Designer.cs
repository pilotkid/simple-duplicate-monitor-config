namespace ot_simple_display_configurator
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            monitors_detected_label = new Label();
            todoLabel = new Label();
            fix_multi_monitor_button = new Button();
            monitor_duplication_mode_label = new Label();
            reload = new Button();
            valid_resolutions_label = new Label();
            timer1 = new System.Windows.Forms.Timer(components);
            refresh_number = new Label();
            groupBox1 = new GroupBox();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // monitors_detected_label
            // 
            monitors_detected_label.AutoSize = true;
            monitors_detected_label.Font = new Font("Satoshi Black", 15.7499981F, FontStyle.Bold, GraphicsUnit.Point, 0);
            monitors_detected_label.Location = new Point(147, 9);
            monitors_detected_label.Name = "monitors_detected_label";
            monitors_detected_label.Size = new Size(243, 26);
            monitors_detected_label.TabIndex = 0;
            monitors_detected_label.Text = "Monitors Detected = X";
            monitors_detected_label.Click += label1_Click;
            // 
            // todoLabel
            // 
            todoLabel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            todoLabel.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            todoLabel.Location = new Point(24, 32);
            todoLabel.Name = "todoLabel";
            todoLabel.Size = new Size(659, 207);
            todoLabel.TabIndex = 2;
            todoLabel.Text = "Todo Text";
            // 
            // fix_multi_monitor_button
            // 
            fix_multi_monitor_button.BackColor = Color.Green;
            fix_multi_monitor_button.FlatStyle = FlatStyle.Popup;
            fix_multi_monitor_button.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            fix_multi_monitor_button.ForeColor = Color.White;
            fix_multi_monitor_button.Location = new Point(39, 67);
            fix_multi_monitor_button.Name = "fix_multi_monitor_button";
            fix_multi_monitor_button.Size = new Size(95, 26);
            fix_multi_monitor_button.TabIndex = 4;
            fix_multi_monitor_button.Text = "Fix";
            fix_multi_monitor_button.UseVisualStyleBackColor = false;
            fix_multi_monitor_button.Click += fix_multi_monitor_button_Click;
            // 
            // monitor_duplication_mode_label
            // 
            monitor_duplication_mode_label.AutoSize = true;
            monitor_duplication_mode_label.Font = new Font("Satoshi Black", 15.7499981F, FontStyle.Bold, GraphicsUnit.Point, 0);
            monitor_duplication_mode_label.Location = new Point(147, 67);
            monitor_duplication_mode_label.Name = "monitor_duplication_mode_label";
            monitor_duplication_mode_label.Size = new Size(306, 26);
            monitor_duplication_mode_label.TabIndex = 3;
            monitor_duplication_mode_label.Text = "Monitor Duplication Mode = ";
            // 
            // reload
            // 
            reload.Location = new Point(713, 12);
            reload.Name = "reload";
            reload.Size = new Size(75, 23);
            reload.TabIndex = 5;
            reload.Text = "Refresh";
            reload.UseVisualStyleBackColor = true;
            reload.Click += reload_Click;
            // 
            // valid_resolutions_label
            // 
            valid_resolutions_label.AutoSize = true;
            valid_resolutions_label.Font = new Font("Satoshi Black", 15.7499981F, FontStyle.Bold, GraphicsUnit.Point, 0);
            valid_resolutions_label.Location = new Point(147, 124);
            valid_resolutions_label.Name = "valid_resolutions_label";
            valid_resolutions_label.Size = new Size(333, 26);
            valid_resolutions_label.TabIndex = 6;
            valid_resolutions_label.Text = "Monitors Have Valid Resolutons";
            // 
            // timer1
            // 
            timer1.Enabled = true;
            timer1.Interval = 1000;
            timer1.Tick += timer1_Tick;
            // 
            // refresh_number
            // 
            refresh_number.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            refresh_number.Location = new Point(713, 38);
            refresh_number.Name = "refresh_number";
            refresh_number.Size = new Size(75, 16);
            refresh_number.TabIndex = 7;
            refresh_number.Text = "0";
            refresh_number.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(todoLabel);
            groupBox1.Font = new Font("Satoshi Black", 15.7499981F, FontStyle.Bold);
            groupBox1.Location = new Point(39, 196);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(706, 242);
            groupBox1.TabIndex = 8;
            groupBox1.TabStop = false;
            groupBox1.Text = "Solution Guide";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(groupBox1);
            Controls.Add(refresh_number);
            Controls.Add(valid_resolutions_label);
            Controls.Add(reload);
            Controls.Add(fix_multi_monitor_button);
            Controls.Add(monitor_duplication_mode_label);
            Controls.Add(monitors_detected_label);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "Form1";
            Text = "0";
            TopMost = true;
            groupBox1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label monitors_detected_label;
        private Button monitors_detected_fix;
        private Label todoLabel;
        private Button fix_multi_monitor_button;
        private Label monitor_duplication_mode_label;
        private Button reload;
        private Label valid_resolutions_label;
        private System.Windows.Forms.Timer timer1;
        private Label refresh_number;
        private GroupBox groupBox1;
    }
}
