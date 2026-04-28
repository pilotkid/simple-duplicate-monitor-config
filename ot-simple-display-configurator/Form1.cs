using System.Diagnostics;
using System.Numerics;

namespace ot_simple_display_configurator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.Text = $"OT Display Configurator v{Application.ProductVersion}";
            reload_Click(null, null);
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void applyLabelStyle(Label label, bool error)
        {
            if (error)
            {
                label.ForeColor = Color.White;
                label.BackColor = Color.Red;
            }
            else
            {
                label.ForeColor = todoLabel.ForeColor;
                label.BackColor = todoLabel.BackColor;
            }
        }

        int reloadNumber = 0;
        private void reload_Click(object? sender, EventArgs? e)
        {
            reloadNumber++;
            if (reloadNumber == 300)
            {
                ShowCloseWarningAsync();
            }
            else if (reloadNumber > 360)
            {
                Application.Exit();
            }
            refresh_number.Text = reloadNumber.ToString();

            int connectedDisplays = MonitorConfig.GetConnectedDisplayCount();
            int activeDisplays = MonitorConfig.GetActiveDisplayCount();
            monitors_detected_label.Text = "Monitors Detected = " + connectedDisplays + " (" + activeDisplays + " active)";
            todoLabel.Text = "Instructions will appear here";
            monitor_duplication_mode_label.Text = "";
            fix_multi_monitor_button.Visible = false;
            if (connectedDisplays < 2)
            {
                applyLabelStyle(monitors_detected_label, true);
                todoLabel.Text = "There is an issue seeing the second monitor.\n1. Check the HDMI cable is plugged into this computer (left most port should work)\n2. Restart the HDMI to Ethernet box under the desk (and light is on)\n3. Make sure studio TVs are on\n4. Try a new HDMI cable";
                return;
            }
            else
            {
                monitors_detected_label.Text += '✓';
                applyLabelStyle(monitors_detected_label, false);
            }


            bool isDuplicated = Screen.AllScreens
            .Select(s => s.Bounds.Location)
            .Distinct()
            .Count() == 1;

            if (isDuplicated || activeDisplays < connectedDisplays)
            {
                monitor_duplication_mode_label.Text = "Monitor Mode = Duplicate";
                todoLabel.Text = "The monitors are currently in Duplication mode not extend mode.\n1. Press the green fix button\n2.If that does not work press WindowsKey+P and select extend";
                applyLabelStyle(monitor_duplication_mode_label, true);
                fix_multi_monitor_button.Visible = true;
                return;
            }
            else
            {
                monitor_duplication_mode_label.Text = "Monitor Mode = Extend ✓";
                applyLabelStyle(monitor_duplication_mode_label, false);
            }

            if (MonitorConfig.DisplaysHaveValidResolutions())
            {
                valid_resolutions_label.Text = "Monitors have valid resolutions ✓";
                applyLabelStyle(valid_resolutions_label, false);
            }
            else
            {
                valid_resolutions_label.Text = "One or more monitors have invalid resolutions";
                applyLabelStyle(valid_resolutions_label, true);
                todoLabel.Text = "1. Check the adapter is plugged in and on\n2. Check that studio TVs are on\n3. Unplug the HDMI cable - reboot the HDMI adapter under the desk - Plug back in HDMI cable. 4. Check for updates\n5. Reboot\n6. Request help";
            }

            todoLabel.Text = "All systems appear to be operational. Monitors are configured correctly, and we are able to see the studio display output from this computer.\nIf issues still appear the issue could be:\n1. OT software needs to be restarted\n2. TVs are not on the right input\n3. Ethernet cable not connected for the HDMI adapter on both ends or broken";
        }

        private void fix_multi_monitor_button_Click(object sender, EventArgs e)
        {
            MonitorConfig.SetExtend();
            reload_Click(null, null);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            reload_Click(null, null);
        }

        private async Task ShowCloseWarningAsync()
        {
            using var form = new Form
            {
                Width = 420,
                Height = 170,
                Text = "Application Closing",
                StartPosition = FormStartPosition.CenterScreen,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                TopMost = true
            };

            var label = new Label
            {
                Text = "The application is about to close.\n\nClick OK to keep it open.",
                AutoSize = false,
                Dock = DockStyle.Top,
                Height = 80,
                TextAlign = ContentAlignment.MiddleCenter
            };

            var okButton = new Button
            {
                Text = "OK",
                DialogResult = DialogResult.OK,
                Width = 100,
                Height = 32,
                Left = (form.ClientSize.Width - 100) / 2,
                Top = 90,
                Anchor = AnchorStyles.Top
            };

            form.Controls.Add(label);
            form.Controls.Add(okButton);
            form.AcceptButton = okButton;



            okButton.Click += (_, _) =>
            {
                reloadNumber = 0;
                form.Close();
            };


            form.ShowDialog();
            
        }
    }
}
