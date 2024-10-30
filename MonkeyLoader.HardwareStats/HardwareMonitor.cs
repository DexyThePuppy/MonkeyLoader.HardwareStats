using System;
using System.Collections.Generic;
using System.Linq;
using OpenHardwareMonitor.Hardware;
using System.IO;
using System.Net.NetworkInformation;

namespace MonkeyLoader.HardwareStats
{
    internal class HardwareMonitor : IDisposable
    {
        private readonly Computer _computer;
        private bool _disposed;
        private readonly NetworkInterface[] _networkInterfaces;
        private long _lastBytesReceived;
        private long _lastBytesSent;
        private DateTime _lastNetworkCheck;

        public HardwareMonitor()
        {
            _computer = new Computer
            {
                CPUEnabled = true,
                GPUEnabled = true,
                RAMEnabled = true,
                MainboardEnabled = true,
                FanControllerEnabled = true,
                HDDEnabled = true
            };

            _computer.Open();
            _networkInterfaces = NetworkInterface.GetAllNetworkInterfaces()
                .Where(ni => ni.OperationalStatus == OperationalStatus.Up &&
                            (ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 ||
                             ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet))
                .ToArray();

            _lastNetworkCheck = DateTime.Now;
            UpdateNetworkStats(); // Initialize network stats
        }

        private void UpdateNetworkStats()
        {
            _lastBytesReceived = _networkInterfaces.Sum(ni => ni.GetIPv4Statistics().BytesReceived);
            _lastBytesSent = _networkInterfaces.Sum(ni => ni.GetIPv4Statistics().BytesSent);
            _lastNetworkCheck = DateTime.Now;
        }

        private NetworkInfo GetNetworkInfo()
        {
            var currentReceived = _networkInterfaces.Sum(ni => ni.GetIPv4Statistics().BytesReceived);
            var currentSent = _networkInterfaces.Sum(ni => ni.GetIPv4Statistics().BytesSent);
            var timeSpan = (DateTime.Now - _lastNetworkCheck).TotalSeconds;

            var info = new NetworkInfo
            {
                BytesReceived = currentReceived,
                BytesSent = currentSent,
                DownloadSpeed = (float)((currentReceived - _lastBytesReceived) / timeSpan),
                UploadSpeed = (float)((currentSent - _lastBytesSent) / timeSpan)
            };

            UpdateNetworkStats();
            return info;
        }

        public IEnumerable<DriveInfo> GetDrives()
        {
            return DriveInfo.GetDrives().Where(d => d.IsReady);
        }

        public HardwareInfo GetHardwareInfo()
        {
            UpdateSensors();

            return new HardwareInfo
            {
                CPU = GetCPUInfo(),
                GPU = GetGPUInfo(),
                RAM = GetRAMInfo(),
                Storage = GetStorageInfo(),
                Network = GetNetworkInfo()
            };
        }

        private CPUInfo GetCPUInfo()
        {
            var cpu = _computer.Hardware.FirstOrDefault(h => h.HardwareType == HardwareType.CPU);
            if (cpu == null) return new CPUInfo();

            return new CPUInfo
            {
                Name = cpu.Name,
                Temperature = GetSensorValue(cpu, SensorType.Temperature),
                Usage = GetSensorValue(cpu, SensorType.Load)
            };
        }

        private GPUInfo GetGPUInfo()
        {
            var gpu = _computer.Hardware.FirstOrDefault(h => h.HardwareType == HardwareType.GpuNvidia || 
                                                           h.HardwareType == HardwareType.GpuAmd);
            if (gpu == null) return new GPUInfo();

            return new GPUInfo
            {
                Name = gpu.Name,
                Temperature = GetSensorValue(gpu, SensorType.Temperature),
                Usage = GetSensorValue(gpu, SensorType.Load)
            };
        }

        private RAMInfo GetRAMInfo()
        {
            var ram = _computer.Hardware.FirstOrDefault(h => h.HardwareType == HardwareType.RAM);
            if (ram == null) return new RAMInfo();

            var usedMemory = GetSensorValue(ram, SensorType.Data, "Used Memory");
            var availableMemory = GetSensorValue(ram, SensorType.Data, "Available Memory");
            var totalMemory = usedMemory + availableMemory;

            return new RAMInfo
            {
                TotalAmount = (long)totalMemory * 1024 * 1024, // Convert to bytes
                AmountUsed = (long)usedMemory * 1024 * 1024,
                AmountFree = (long)availableMemory * 1024 * 1024,
                PercentageLoad = GetSensorValue(ram, SensorType.Load)
            };
        }

        private List<StorageInfo> GetStorageInfo()
        {
            var drives = GetDrives();
            return drives.Select(drive => new StorageInfo
            {
                Name = drive.Name.TrimEnd('\\'),
                TotalAmount = drive.TotalSize,
                AmountFree = drive.AvailableFreeSpace,
                AmountUsed = drive.TotalSize - drive.AvailableFreeSpace
            }).ToList();
        }

        private float GetSensorValue(IHardware hardware, SensorType sensorType, string name = null)
        {
            hardware.Update();
            var sensor = hardware.Sensors.FirstOrDefault(s => 
                s.SensorType == sensorType && (name == null || s.Name == name));
            return sensor?.Value ?? 0;
        }

        private void UpdateSensors()
        {
            if (_disposed) return;
            foreach (var hardware in _computer.Hardware)
            {
                hardware.Update();
                foreach (var subHardware in hardware.SubHardware)
                    subHardware.Update();
            }
        }

        public void Dispose()
        {
            if (_disposed) return;
            _computer?.Close();
            _disposed = true;
        }
    }
} 