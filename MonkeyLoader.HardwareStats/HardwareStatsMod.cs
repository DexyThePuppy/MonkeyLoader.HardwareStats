using Elements.Core;
using FrooxEngine;
using HarmonyLib;
using MonkeyLoader.Patching;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MonkeyLoader.HardwareStats
{
    [HarmonyPatch]
    internal sealed class HardwareStatsMod : Monkey<HardwareStatsMod>
    {
        private HardwareMonitor _monitor;
        private Dictionary<string, DynamicValueVariable<string>> _stringVariables;
        private Dictionary<string, DynamicValueVariable<float>> _floatVariables;
        private Dictionary<string, DynamicValueVariable<long>> _longVariables;

        protected override void OnEngineInit()
        {
            _monitor = new HardwareMonitor();
            _stringVariables = new Dictionary<string, DynamicValueVariable<string>>();
            _floatVariables = new Dictionary<string, DynamicValueVariable<float>>();
            _longVariables = new Dictionary<string, DynamicValueVariable<long>>();

            InitializeDynamicVariables();
            StartUpdateLoop();
        }

        private void InitializeDynamicVariables()
        {
            // CPU Variables
            RegisterFloatVariable("Hardware/CPU/Temperature");
            RegisterFloatVariable("Hardware/CPU/Usage");
            RegisterStringVariable("Hardware/CPU/Name");

            // GPU Variables
            RegisterFloatVariable("Hardware/GPU/Temperature");
            RegisterFloatVariable("Hardware/GPU/Usage");
            RegisterStringVariable("Hardware/GPU/Name");

            // RAM Variables
            RegisterLongVariable("Hardware/RAM/Total");
            RegisterLongVariable("Hardware/RAM/Used");
            RegisterLongVariable("Hardware/RAM/Free");
            RegisterFloatVariable("Hardware/RAM/UsagePercent");

            // Storage Variables (will be populated dynamically)
            foreach (var drive in _monitor.GetDrives())
            {
                string prefix = $"Hardware/Storage/{drive.Name}";
                RegisterLongVariable($"{prefix}/Total");
                RegisterLongVariable($"{prefix}/Used");
                RegisterLongVariable($"{prefix}/Free");
            }

            // Network Variables
            RegisterFloatVariable("Hardware/Network/Upload");
            RegisterFloatVariable("Hardware/Network/Download");
        }

        private void RegisterStringVariable(string path)
        {
            _stringVariables[path] = Engine.Current.DynamicVariables.RegisterVariable<string>(path, string.Empty);
        }

        private void RegisterFloatVariable(string path)
        {
            _floatVariables[path] = Engine.Current.DynamicVariables.RegisterVariable<float>(path, 0f);
        }

        private void RegisterLongVariable(string path)
        {
            _longVariables[path] = Engine.Current.DynamicVariables.RegisterVariable<long>(path, 0L);
        }

        private async void StartUpdateLoop()
        {
            while (Engine.Current.IsRunning)
            {
                UpdateHardwareStats();
                await Task.Delay(1000); // Update every second
            }
        }

        private void UpdateHardwareStats()
        {
            try
            {
                var info = _monitor.GetHardwareInfo();

                // Update CPU info
                _floatVariables["Hardware/CPU/Temperature"].Value = info.CPU.Temperature;
                _floatVariables["Hardware/CPU/Usage"].Value = info.CPU.Usage;
                _stringVariables["Hardware/CPU/Name"].Value = info.CPU.Name;

                // Update GPU info
                _floatVariables["Hardware/GPU/Temperature"].Value = info.GPU.Temperature;
                _floatVariables["Hardware/GPU/Usage"].Value = info.GPU.Usage;
                _stringVariables["Hardware/GPU/Name"].Value = info.GPU.Name;

                // Update RAM info
                _longVariables["Hardware/RAM/Total"].Value = info.RAM.TotalAmount;
                _longVariables["Hardware/RAM/Used"].Value = info.RAM.AmountUsed;
                _longVariables["Hardware/RAM/Free"].Value = info.RAM.AmountFree;
                _floatVariables["Hardware/RAM/UsagePercent"].Value = info.RAM.PercentageLoad;

                // Update Storage info
                foreach (var drive in info.Storage)
                {
                    string prefix = $"Hardware/Storage/{drive.Name}";
                    _longVariables[$"{prefix}/Total"].Value = drive.TotalAmount;
                    _longVariables[$"{prefix}/Used"].Value = drive.AmountUsed;
                    _longVariables[$"{prefix}/Free"].Value = drive.AmountFree;
                }

                // Update Network info
                if (info.Network != null)
                {
                    _floatVariables["Hardware/Network/Upload"].Value = info.Network.UploadSpeed;
                    _floatVariables["Hardware/Network/Download"].Value = info.Network.DownloadSpeed;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(() => $"Error updating hardware stats: {ex}");
            }
        }

        protected override void OnDispose()
        {
            _monitor?.Dispose();
            base.OnDispose();
        }

        protected override IEnumerable<IFeaturePatch> GetFeaturePatches() => 
            Enumerable.Empty<IFeaturePatch>();
    }
} 