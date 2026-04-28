# OT Simple Display Configurator

A lightweight Windows utility that monitors your display configuration and guides you through fixing common dual-monitor issues in broadcast/studio environments.

## What it does

The application runs on Windows and automatically checks whether your monitors are set up correctly for studio use:

- **Detects connected monitors** — verifies that both the local screen and the studio display (e.g. a TV connected via HDMI-over-Ethernet) are visible to Windows.
- **Checks display mode** — confirms monitors are in **Extend** mode rather than **Duplicate** (clone) mode, which is required for independent studio output.
- **Validates resolutions** — ensures each active display is reporting a valid resolution, catching situations where an adapter is connected but not fully initialised.
- **One-click fix** — if duplicate mode is detected a green **Fix** button appears. Clicking it calls the Windows `SetDisplayConfig` API to switch to Extend mode immediately, without needing to open the Windows display settings.
- **Auto-refresh** — the status checks run every second so the UI always reflects the current state of the connected displays.
- **Solution guide** — a plain-English instructions panel tells you exactly what to do for each failure condition (e.g. cable checks, adapter reboot steps, Windows shortcut hints).

After roughly 5 minutes of inactivity the app warns the user it is about to close, giving them the option to keep it open.

## Requirements

- Windows 10 or Windows 11 (x64)
- .NET 8 runtime (included in the self-contained build — no separate install needed)

## Download

Pre-built releases are available on the [Releases](../../releases) page. Download the latest `ot-simple-display-configurator.exe` and run it directly — no installer required.

## Building from source

```
dotnet publish ot-simple-display-configurator/ot-simple-display-configurator.csproj \
  -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

## CI / Release pipeline

A GitHub Actions workflow (`.github/workflows/build.yml`) runs on every push to `main`:

1. Reads the latest `vX.Y.Z` git tag and increments the patch number.
2. Builds a self-contained single-file executable with the new version embedded.
3. Tags the commit and publishes a GitHub Release with the `.exe` attached.

The version number is embedded in the executable and shown in the application title bar at runtime.
