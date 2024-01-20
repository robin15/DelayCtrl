using System;
using SharpPcap;
using PacketDotNet;
using System.Threading;

class DelayCtrl
{
    private static ICaptureDevice sourceInterface;
    private static IInjectionDevice destinationInterface;
    private static string delayTime;

    static void Main()
    {
        int cnt = 0;
        foreach (SharpPcap.ICaptureDevice device in CaptureDeviceList.Instance)
        {
            Console.Write(cnt + ": ");
            Console.WriteLine(device.Description.ToString());
            cnt += 1;
        }
        Console.Write("\nInput src device num: ");
        string srcInput = Console.ReadLine();
        Console.Write("Input dstOutput device num: ");
        string dstInput = Console.ReadLine();
        Console.Write("Input delay time(ms): ");
        delayTime = Console.ReadLine();

        sourceInterface = CaptureDeviceList.Instance[Int32.Parse(srcInput)];
        destinationInterface = CaptureDeviceList.Instance[Int32.Parse(dstInput)];

        Console.WriteLine(sourceInterface.ToString());
        Console.WriteLine(destinationInterface.ToString());

        sourceInterface.Open(DeviceModes.Promiscuous, 1000);
        destinationInterface.Open(DeviceModes.Promiscuous, 1000);

        sourceInterface.OnPacketArrival += SourceInterface_OnPacketArrival1;

        sourceInterface.StartCapture();

        Console.WriteLine("Press Enter to exit...");
        Console.ReadLine();

        sourceInterface.StopCapture();

        sourceInterface.Close();
        destinationInterface.Close();
    }

    private static void SourceInterface_OnPacketArrival1(object sender, SharpPcap.PacketCapture e)
    {
        try
        {
            var packet = Packet.ParsePacket(e.GetPacket().LinkLayerType, e.GetPacket().Data);
            Console.WriteLine(packet);
            Thread.Sleep(Int32.Parse(delayTime));
            SendPacket(packet);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERR: {ex.Message}");
        }
    }

    private static void SendPacket(Packet packet)
    {
        destinationInterface.SendPacket(packet);
    }
}
