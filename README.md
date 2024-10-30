# Resonite HardwareStats

*Woof!* A Resonite mod that provides real-time hardware monitoring information through dynamic variables!

## Features 🐾

- Monitor system hardware in real-time:
  - CPU (usage, temperature)
  - GPU (usage, temperature)
  - RAM (usage, total, free)
  - Storage drives (usage, total, free)
  - Motherboard information
  - Fan speeds
  - Network statistics

- Access hardware information through dynamic variables in your Resonite worlds:
  - `Hardware/CPU/Temperature`
  - `Hardware/CPU/Usage`
  - `Hardware/GPU/Temperature`
  - `Hardware/RAM/UsagePercent`
  - And many more!

## Installation 🦴

1. Install [MonkeyLoader](https://github.com/MonkeyModdingTroop/MonkeyLoader)
2. Download the latest release from the [releases page](https://github.com/YourUsername/MonkeyLoader.HardwareStats/releases)
3. Place the mod in your Resonite's MonkeyLoader mods folder

## Usage 🐕

Once installed, the mod will automatically start monitoring your hardware. You can access the hardware information through dynamic variables in your Resonite worlds.

## Available Variables 📊

### CPU
- `Hardware/CPU/Name`
- `Hardware/CPU/Temperature`
- `Hardware/CPU/Usage`

### GPU
- `Hardware/GPU/Name`
- `Hardware/GPU/Temperature`
- `Hardware/GPU/Usage`

### RAM
- `Hardware/RAM/Total`
- `Hardware/RAM/Used`
- `Hardware/RAM/Free`
- `Hardware/RAM/UsagePercent`

### Storage (per drive)
- `Hardware/Storage/[DriveLetter]/Total`
- `Hardware/Storage/[DriveLetter]/Used`
- `Hardware/Storage/[DriveLetter]/Free`

### Network
- `Hardware/Network/Upload`
- `Hardware/Network/Download`

## Requirements 🔧

- Resonite
- [MonkeyLoader](https://github.com/MonkeyModdingTroop/MonkeyLoader)
- OpenHardwareMonitor library
- .NET Framework 4.6.2 or higher

## Contributing 🐾

Pull requests are welcome! For major changes, please open an issue first to discuss what you would like to change.

## License 📜

[LGPL-3.0](LICENSE.txt)
