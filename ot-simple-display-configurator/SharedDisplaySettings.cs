using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ot_simple_display_configurator
{
    public class SharedDisplaySettings
    {
        private bool _showOnAllMonitors;

        public event Action<bool>? ShowOnAllMonitorsChanged;

        public bool ShowOnAllMonitors
        {
            get => _showOnAllMonitors;
            set
            {
                if (_showOnAllMonitors == value)
                    return;

                _showOnAllMonitors = value;
                ShowOnAllMonitorsChanged?.Invoke(value);
            }
        }
    }
}
