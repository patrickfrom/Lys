using OpenTK.Audio.OpenAL;
using OpenTK.Mathematics;

namespace Lys.Audio;

public static class AudioManager
{
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
        using var wavReader = new WavReader(file);
        var audioFormat = wavReader.AudioFormat;
        
        AL.BufferData(buffer, wavReader.GetOpenAlFormat() , wavReader.Data, audioFormat.SampleRate);

        return buffer;
    }
}