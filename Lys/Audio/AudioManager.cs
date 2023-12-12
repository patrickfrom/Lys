using OpenTK.Audio.OpenAL;
using OpenTK.Mathematics;

namespace Lys.Audio;

public static class AudioManager
{
    private static readonly List<int> Buffers = [];
    
    public static void Init()
    {
        var device = ALC.OpenDevice(null);

        var context = ALC.CreateContext(device, new[]{0});
        ALC.MakeContextCurrent(context);
    }

    public static void SetListenerData(Vector3 position)
    {
        AL.Listener(ALListener3f.Position, ref position);
    }

    public static int LoadSound(string file)
    {
        var buffer = AL.GenBuffer();
        Buffers.Add(buffer);
        
        using var wavReader = new WavReader(file);
        var audioFormat = wavReader.AudioFormat;
        
        AL.BufferData(buffer, GetOpenAlFormat(audioFormat.Channels, audioFormat.Channels), wavReader.Data, audioFormat.SampleRate);

        return buffer;
    }

    public static void Cleanup()
    {
        foreach (var buffer in Buffers)
        {
            AL.DeleteBuffer(buffer);
        }
    }
    
    private static ALFormat GetOpenAlFormat(int channels, int bitsPerSample)
    {
        if (channels == 1)
        {
            return bitsPerSample == 8 ? ALFormat.Mono8 : ALFormat.Mono16;
        }

        return bitsPerSample == 8 ? ALFormat.Stereo8 : ALFormat.Stereo16;
    }
}