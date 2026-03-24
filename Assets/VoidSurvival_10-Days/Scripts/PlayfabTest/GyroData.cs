using Newtonsoft.Json;
using System;
using System.Collections.Generic;

[Serializable]
public class GameMasterDataRoot
{
    public List<GyroData> Gyro;
    public List<MagData> Mag1; // ★Mag1用
    public List<MagData> Mag2; // ★Mag2用
    public List<MagData> Mag3; // ★Mag3用
    public List<RadiationData> Radiation;
}

[Serializable]
public class SensorData
{
    [JsonProperty("Time (HH:mm:ss)")] public string TimeStr;
    public int TotalSeconds 
    {
        get 
        {
            if (string.IsNullOrEmpty(TimeStr)) return 0;
            if (DateTime.TryParse(TimeStr, out DateTime dt))
            {
                return dt.Hour * 3600 + dt.Minute * 60 + dt.Second;
            }
            return 0;
        }
    }
}

[Serializable]
public class GyroData : SensorData
{
    [JsonProperty("gx [deg/s]")] public float gx;
    [JsonProperty("gy [deg/s]")] public float gy;
    [JsonProperty("gz [deg/s]")] public float gz;
}

[Serializable]
public class MagData : SensorData
{
    [JsonProperty("mx [nT]")] public float mx;
    [JsonProperty("my [nT]")] public float my;
    [JsonProperty("mz [nT]")] public float mz;
}

[Serializable]
public class RadiationData : SensorData
{
    [JsonProperty("Signal Count")] public int SignalCount;
    [JsonProperty("Noise Count")] public int NoiseCount;
}