using System.Diagnostics;
using System.Numerics;
using System.Timers;

namespace ot_simple_display_configurator
{
    public partial class Form1 : Form
    {

        private readonly Dictionary<string, Form1> _formsByMonitor = new();

        private SharedDisplaySettings _settings;
        private bool _updatingCheckbox = false;
        private bool _isSubForm = false;

        public Form1()
        {
            InitializeComponent();
            this.Text = $"OT Display Configurator v{Application.ProductVersion}";
            _settings = new SharedDisplaySettings();
        }
        public Form1(SharedDisplaySettings settings, bool isSubForm)
        {
            InitializeComponent();

            _settings = settings;
            _isSubForm = isSubForm;
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
        int lastKnownActiveScreens = -1;
        int triggerReloadTick = -1;
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

            if (activeDisplays != lastKnownActiveScreens )
            {
                lastKnownActiveScreens = activeDisplays;
                Debug.WriteLine($"Active screen count changed: {lastKnownActiveScreens}|{activeDisplays}");

                if (show_on_all_monitors_box.Checked && show_on_all_monitors_box.Enabled && reloadNumber > 5)
                {
                    triggerReloadTick = reloadNumber + 3;                    
                }
            }

            if(triggerReloadTick != -1 && reloadNumber >= triggerReloadTick)
            {
                triggerReloadTick = -1;
                //After 5 seconds trigger this function

                timer1.Enabled = false;
                show_on_all_monitors_box.Enabled = false;
                CloseMonitorForms();
                DuplicateMainFormAcrossMonitors();
                timer1.Enabled = true;
                show_on_all_monitors_box.Enabled = true;
            }

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

            //todo iterate thru all forms that aren't this one and update all labels and buttons to match this one
            foreach (Form form in Application.OpenForms)
            {
                if (form != this && form is Form1 otherForm)
                {
                    otherForm.monitors_detected_label.Text = monitors_detected_label.Text;
                    otherForm.todoLabel.Text = todoLabel.Text;
                    otherForm.monitor_duplication_mode_label.Text = monitor_duplication_mode_label.Text;
                    otherForm.valid_resolutions_label.Text = valid_resolutions_label.Text;
                    otherForm.fix_multi_monitor_button.Visible = fix_multi_monitor_button.Visible;
                }
            }
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

        private void DuplicateMainFormAcrossMonitors()
        {
            var currentScreen = Screen.FromControl(this);
            _formsByMonitor[currentScreen.DeviceName] = this;
            this.TopMost = true;

            foreach (var screen in Screen.AllScreens)
            {
                if (_formsByMonitor.TryGetValue(screen.DeviceName, out var existing) &&
                    existing != null &&
                    !existing.IsDisposed)
                {
                    continue;
                }

                string deviceName = screen.DeviceName;

                var form = new Form1(_settings, true);

                form.StartPosition = FormStartPosition.Manual;
                form.Location = screen.Bounds.Location;
                form.WindowState = FormWindowState.Normal;
                form.ControlBox = false;
                form.Size = this.Size;
                form.TopMost = true;
                form.ShowInTaskbar = false;
                form.Text = screen.DeviceName;

                form.FormClosed += (_, _) =>
                {
                    if (_formsByMonitor.TryGetValue(deviceName, out var existingForm) &&
                        ReferenceEquals(existingForm, form))
                    {
                        _formsByMonitor.Remove(deviceName);
                        
                    }
                };

                _formsByMonitor[deviceName] = form;
                form.Show();
            }
        }
        private void CloseMonitorForms()
        {
            foreach (var kvp in _formsByMonitor.ToList())
            {
                var form = kvp.Value;

                if (ReferenceEquals(form, this))
                    continue;

                if (form != null && !form.IsDisposed)
                    form.Close();
            }

            _formsByMonitor.Clear();

            var currentScreen = Screen.FromControl(this);
            _formsByMonitor[currentScreen.DeviceName] = this;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _settings.ShowOnAllMonitorsChanged += Settings_ShowOnAllMonitorsChanged;

            _updatingCheckbox = true;
            show_on_all_monitors_box.Checked = _settings.ShowOnAllMonitors;
            _updatingCheckbox = false;

            if (!_isSubForm)
            {
                _settings.ShowOnAllMonitors = true;
                reload_Click(null, null);
            }
            else
            {                
                timer1.Enabled = false;
                reload.Visible = false;
                refresh_number.Text = "Secondary Window";
            }
        }

        private void Settings_ShowOnAllMonitorsChanged(bool enabled)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => Settings_ShowOnAllMonitorsChanged(enabled)));
                return;
            }

            _updatingCheckbox = true;
            show_on_all_monitors_box.Checked = enabled;
            _updatingCheckbox = false;

            if (_isSubForm && !enabled)
            {
                Close();
                return;
            }

            if (!_isSubForm)
            {
                show_on_all_monitors_box.Enabled = false;
                if (enabled)
                    DuplicateMainFormAcrossMonitors();
                else
                    CloseMonitorForms();
                show_on_all_monitors_box.Enabled = true;
            }
        }

        private void show_on_all_monitors_box_CheckedChanged(object sender, EventArgs e)
        {
            if (_updatingCheckbox)
                return;

            show_on_all_monitors_box.Enabled = false;

            _settings.ShowOnAllMonitors = show_on_all_monitors_box.Checked;

            show_on_all_monitors_box.Enabled = true;
        }
    }

}
