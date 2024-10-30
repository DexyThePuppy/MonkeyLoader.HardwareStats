namespace MonkeyLoader.HardwareStats
{
    public class HardwareInfo
    {
        public MotherboardInfo Motherboard { get; set; }
        public CPUInfo CPU { get; set; }
        public GPUInfo GPU { get; set; }
        public RAMInfo RAM { get; set; }
        public List<StorageInfo> Storage { get; set; }
        public List<FanInfo> Fans { get; set; }
        public NetworkInfo Network { get; set; }
    }

    public class MotherboardInfo
    {
        public string Name { get; set; }
        public string Manufacturer { get; set; }
    }

    public class CPUInfo
    {
        public string Name { get; set; }
        public float Usage { get; set; }
        public float Temperature { get; set; }
    }

    public class GPUInfo
    {
        public string Name { get; set; }
        public float Temperature { get; set; }
        public float Usage { get; set; }
    }

    public class RAMInfo
    {
        public float PercentageLoad { get; set; }
        public long AmountUsed { get; set; }
        public long AmountFree { get; set; }
        public long TotalAmount { get; set; }
    }

    public class StorageInfo
    {
        public string Name { get; set; }
        public long AmountUsed { get; set; }
        public long AmountFree { get; set; }
        public long TotalAmount { get; set; }
    }

    public class FanInfo
    {
        public string Name { get; set; }
        public float Speed { get; set; }
    }

    public class NetworkInfo
    {
        public long BytesSent { get; set; }
        public long BytesReceived { get; set; }
        public float UploadSpeed { get; set; }
        public float DownloadSpeed { get; set; }
    }
} 