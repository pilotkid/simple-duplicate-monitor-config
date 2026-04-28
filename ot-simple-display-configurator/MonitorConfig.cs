using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ot_simple_display_configurator
{
    public static class MonitorConfig
    {
        private const uint SDC_APPLY = 0x00000080;
        private const uint SDC_TOPOLOGY_CLONE = 0x00000002;
        private const uint SDC_TOPOLOGY_EXTEND = 0x00000004;

        private const uint QDC_ALL_PATHS = 0x00000001;
        private const uint QDC_ONLY_ACTIVE_PATHS = 0x00000002;

        private const int ERROR_SUCCESS = 0;
        private const int ERROR_INSUFFICIENT_BUFFER = 122;

        [DllImport("user32.dll")]
        private static extern int SetDisplayConfig(
            uint numPathArrayElements,
            IntPtr pathArray,
            uint numModeInfoArrayElements,
            IntPtr modeInfoArray,
            uint flags
        );

        [DllImport("user32.dll")]
        private static extern int GetDisplayConfigBufferSizes(
            uint flags,
            out uint numPathArrayElements,
            out uint numModeInfoArrayElements
        );

        [DllImport("user32.dll")]
        private static extern int QueryDisplayConfig(
            uint flags,
            ref uint numPathArrayElements,
            [Out] DISPLAYCONFIG_PATH_INFO[] pathInfoArray,
            ref uint numModeInfoArrayElements,
            [Out] DISPLAYCONFIG_MODE_INFO[] modeInfoArray,
            IntPtr currentTopologyId
        );

        public static void SetExtend()
        {
            int result = SetDisplayConfig(
                0,
                IntPtr.Zero,
                0,
                IntPtr.Zero,
                SDC_APPLY | SDC_TOPOLOGY_EXTEND
            );

            if (result != 0)
                throw new InvalidOperationException($"SetDisplayConfig failed: {result}");
        }

        public static void SetDuplicate()
        {
            int result = SetDisplayConfig(
                0,
                IntPtr.Zero,
                0,
                IntPtr.Zero,
                SDC_APPLY | SDC_TOPOLOGY_CLONE
            );

            if (result != 0)
                throw new InvalidOperationException($"SetDisplayConfig failed: {result}");
        }

        [DllImport("user32.dll")]
        private static extern int DisplayConfigGetDeviceInfo(
    ref DISPLAYCONFIG_TARGET_DEVICE_NAME requestPacket
);

        private const uint DISPLAYCONFIG_DEVICE_INFO_GET_TARGET_NAME = 2;

        private static string GetMonitorFriendlyName(LUID adapterId, uint targetId)
        {
            var deviceName = new DISPLAYCONFIG_TARGET_DEVICE_NAME
            {
                header = new DISPLAYCONFIG_DEVICE_INFO_HEADER
                {
                    type = DISPLAYCONFIG_DEVICE_INFO_GET_TARGET_NAME,
                    size = (uint)Marshal.SizeOf<DISPLAYCONFIG_TARGET_DEVICE_NAME>(),
                    adapterId = adapterId,
                    id = targetId
                }
            };

            int result = DisplayConfigGetDeviceInfo(ref deviceName);

            if (result != ERROR_SUCCESS)
                return string.Empty;

            return deviceName.monitorFriendlyDeviceName ?? string.Empty;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct DISPLAYCONFIG_DEVICE_INFO_HEADER
        {
            public uint type;
            public uint size;
            public LUID adapterId;
            public uint id;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct DISPLAYCONFIG_TARGET_DEVICE_NAME
        {
            public DISPLAYCONFIG_DEVICE_INFO_HEADER header;
            public DISPLAYCONFIG_TARGET_DEVICE_NAME_FLAGS flags;
            public DISPLAYCONFIG_VIDEO_OUTPUT_TECHNOLOGY outputTechnology;
            public ushort edidManufactureId;
            public ushort edidProductCodeId;
            public uint connectorInstance;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public string monitorFriendlyDeviceName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string monitorDevicePath;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct DISPLAYCONFIG_TARGET_DEVICE_NAME_FLAGS
        {
            public uint value;
        }

        /// <summary>
        /// Returns the number of connected/available display targets, even if not currently active
        /// (e.g. 2 monitors plugged in but Windows is set to PC screen only).
        /// </summary>
        public static int GetConnectedDisplayCount()
        {
            var paths = GetDisplayPaths(QDC_ALL_PATHS);

            var uniqueMonitors = new HashSet<string>();

            foreach (var path in paths)
            {
                if (!path.targetInfo.targetAvailable)
                    continue;

                string name = GetMonitorFriendlyName(
                    path.targetInfo.adapterId,
                    path.targetInfo.id
                );

                if (string.IsNullOrWhiteSpace(name))
                    continue;

                string key =
                    $"{path.targetInfo.adapterId.LowPart}:{path.targetInfo.adapterId.HighPart}:{path.targetInfo.id}:{name}";

                uniqueMonitors.Add(key);
            }

            return uniqueMonitors.Count;
        }

        /// <summary>
        /// Returns the number of active displays in the current desktop layout.
        /// </summary>
        public static int GetActiveDisplayCount()
        {
            var paths = GetDisplayPaths(QDC_ONLY_ACTIVE_PATHS);

            var uniqueTargets = new HashSet<string>();

            foreach (var path in paths)
            {
                string key = $"{path.targetInfo.adapterId.LowPart}:{path.targetInfo.adapterId.HighPart}:{path.targetInfo.id}";
                uniqueTargets.Add(key);
            }

            return uniqueTargets.Count;
        }

        private static DISPLAYCONFIG_PATH_INFO[] GetDisplayPaths(uint flags)
        {
            int result = GetDisplayConfigBufferSizes(flags, out uint pathCount, out uint modeCount);
            if (result != ERROR_SUCCESS)
                throw new InvalidOperationException($"GetDisplayConfigBufferSizes failed: {result}");

            var paths = new DISPLAYCONFIG_PATH_INFO[pathCount];
            var modes = new DISPLAYCONFIG_MODE_INFO[modeCount];

            uint pathCountLocal = pathCount;
            uint modeCountLocal = modeCount;

            result = QueryDisplayConfig(
                flags,
                ref pathCountLocal,
                paths,
                ref modeCountLocal,
                modes,
                IntPtr.Zero
            );

            // Retry once if the display config changed while querying
            if (result == ERROR_INSUFFICIENT_BUFFER)
            {
                result = GetDisplayConfigBufferSizes(flags, out pathCount, out modeCount);
                if (result != ERROR_SUCCESS)
                    throw new InvalidOperationException($"GetDisplayConfigBufferSizes retry failed: {result}");

                paths = new DISPLAYCONFIG_PATH_INFO[pathCount];
                modes = new DISPLAYCONFIG_MODE_INFO[modeCount];

                pathCountLocal = pathCount;
                modeCountLocal = modeCount;

                result = QueryDisplayConfig(
                    flags,
                    ref pathCountLocal,
                    paths,
                    ref modeCountLocal,
                    modes,
                    IntPtr.Zero
                );
            }

            if (result != ERROR_SUCCESS)
                throw new InvalidOperationException($"QueryDisplayConfig failed: {result}");

            // Trim array in case Windows returned fewer paths than buffer size
            if (pathCountLocal != paths.Length)
            {
                Array.Resize(ref paths, (int)pathCountLocal);
            }

            return paths;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct LUID
        {
            public uint LowPart;
            public int HighPart;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct DISPLAYCONFIG_PATH_INFO
        {
            public DISPLAYCONFIG_PATH_SOURCE_INFO sourceInfo;
            public DISPLAYCONFIG_PATH_TARGET_INFO targetInfo;
            public uint flags;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct DISPLAYCONFIG_PATH_SOURCE_INFO
        {
            public LUID adapterId;
            public uint id;
            public uint modeInfoIdx;
            public uint statusFlags;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct DISPLAYCONFIG_PATH_TARGET_INFO
        {
            public LUID adapterId;
            public uint id;
            public uint modeInfoIdx;
            public DISPLAYCONFIG_VIDEO_OUTPUT_TECHNOLOGY outputTechnology;
            public DISPLAYCONFIG_ROTATION rotation;
            public DISPLAYCONFIG_SCALING scaling;
            public DISPLAYCONFIG_RATIONAL refreshRate;
            public DISPLAYCONFIG_SCANLINE_ORDERING scanLineOrdering;
            [MarshalAs(UnmanagedType.Bool)]
            public bool targetAvailable;
            public uint statusFlags;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct DISPLAYCONFIG_MODE_INFO
        {
            public DISPLAYCONFIG_MODE_INFO_TYPE infoType;
            public uint id;
            public LUID adapterId;
            public DISPLAYCONFIG_MODE_INFO_UNION modeInfo;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct DISPLAYCONFIG_MODE_INFO_UNION
        {
            [FieldOffset(0)]
            public DISPLAYCONFIG_TARGET_MODE targetMode;

            [FieldOffset(0)]
            public DISPLAYCONFIG_SOURCE_MODE sourceMode;

            [FieldOffset(0)]
            public DISPLAYCONFIG_DESKTOP_IMAGE_INFO desktopImageInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct DISPLAYCONFIG_TARGET_MODE
        {
            public DISPLAYCONFIG_VIDEO_SIGNAL_INFO targetVideoSignalInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct DISPLAYCONFIG_SOURCE_MODE
        {
            public uint width;
            public uint height;
            public uint pixelFormat;
            public POINTL position;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct DISPLAYCONFIG_DESKTOP_IMAGE_INFO
        {
            public POINTL PathSourceSize;
            public RECT DesktopImageRegion;
            public RECT DesktopImageClip;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct DISPLAYCONFIG_VIDEO_SIGNAL_INFO
        {
            public ulong pixelRate;
            public DISPLAYCONFIG_RATIONAL hSyncFreq;
            public DISPLAYCONFIG_RATIONAL vSyncFreq;
            public DISPLAYCONFIG_2DREGION activeSize;
            public DISPLAYCONFIG_2DREGION totalSize;
            public uint videoStandard;
            public DISPLAYCONFIG_SCANLINE_ORDERING scanLineOrdering;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct DISPLAYCONFIG_2DREGION
        {
            public uint cx;
            public uint cy;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct DISPLAYCONFIG_RATIONAL
        {
            public uint Numerator;
            public uint Denominator;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct POINTL
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        private enum DISPLAYCONFIG_VIDEO_OUTPUT_TECHNOLOGY : uint
        {
            Other = 0xFFFFFFFF
        }

        private enum DISPLAYCONFIG_ROTATION : uint
        {
            Identity = 1
        }

        private enum DISPLAYCONFIG_SCALING : uint
        {
            Identity = 1
        }

        private enum DISPLAYCONFIG_SCANLINE_ORDERING : uint
        {
            Unspecified = 0
        }

        private enum DISPLAYCONFIG_MODE_INFO_TYPE : uint
        {
            Source = 1,
            Target = 2,
            DesktopImage = 3
        }

        public static bool DisplaysHaveValidResolutions()
        {
            var result = GetDisplayConfig(QDC_ONLY_ACTIVE_PATHS);

            foreach (var path in result.Paths)
            {
                if (!path.targetInfo.targetAvailable)
                    continue;

                if (path.targetInfo.modeInfoIdx == uint.MaxValue)
                    return false;

                var mode = result.Modes[path.targetInfo.modeInfoIdx];

                if (mode.infoType != DISPLAYCONFIG_MODE_INFO_TYPE.Target)
                    continue;

                var size = mode.modeInfo.targetMode.targetVideoSignalInfo.activeSize;

                if (size.cx == 0 || size.cy == 0)
                    return false;
            }

            return result.Paths.Length > 0;
        }

        private static DisplayConfigResult GetDisplayConfig(uint flags)
        {
            int result = GetDisplayConfigBufferSizes(flags, out uint pathCount, out uint modeCount);
            if (result != ERROR_SUCCESS)
                throw new InvalidOperationException($"GetDisplayConfigBufferSizes failed: {result}");

            var paths = new DISPLAYCONFIG_PATH_INFO[pathCount];
            var modes = new DISPLAYCONFIG_MODE_INFO[modeCount];

            uint pathCountLocal = pathCount;
            uint modeCountLocal = modeCount;

            result = QueryDisplayConfig(
                flags,
                ref pathCountLocal,
                paths,
                ref modeCountLocal,
                modes,
                IntPtr.Zero
            );

            if (result != ERROR_SUCCESS)
                throw new InvalidOperationException($"QueryDisplayConfig failed: {result}");

            Array.Resize(ref paths, (int)pathCountLocal);
            Array.Resize(ref modes, (int)modeCountLocal);

            return new DisplayConfigResult
            {
                Paths = paths,
                Modes = modes
            };
        }

        private struct DisplayConfigResult
        {
            public DISPLAYCONFIG_PATH_INFO[] Paths;
            public DISPLAYCONFIG_MODE_INFO[] Modes;
        }


    }
}