using System;

public class HitsInfo
{
    public HitsInfo(string ip, DateTime startTime)
    {
        this.IPAddress = ip;
        this.StartTime = startTime;
    }

    public string IPAddress { get; private set; }

    public int Hits { get; set; }

    public DateTime StartTime { get; set; }
}