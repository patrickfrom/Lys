using System.Text;

namespace Lys.Audio;

public class AudioFormat(int sampleRate, int channels, int bitsPerSample)
{
    public readonly int SampleRate = sampleRate;
    public readonly int BitsPerSample = bitsPerSample;
    public readonly int Channels = channels;
}

public class WavReader : IDisposable
{
    private readonly FileStream _fileStream;

    public AudioFormat AudioFormat { get; private set; }
    public byte[] Data { get; private set; }

    public WavReader(string filePath)
    {
        using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        Data = Array.Empty<byte>();
        _fileStream = fileStream;
        ReadWav();
    }

    private void ReadWav()
    {
        var buffer = new byte[44];

        try
        {
            _fileStream.Read(buffer, 0, buffer.Length);
        }
        catch (Exception exception)
        {
            Console.WriteLine($"Error reading file: {exception.Message}");
        }

        try
        {
            var riffHeader = Encoding.UTF8.GetString(buffer, 0, 4);
            if (riffHeader != "RIFF")
                throw new Exception("Not a RIFF");

            var format = Encoding.UTF8.GetString(buffer, 8, 4);
            if (format != "WAVE")
                throw new Exception("Not a WAVE File");

            var sampleRate = BitConverter.ToInt32(buffer, 24);
            var channels = BitConverter.ToInt16(buffer, 22);
            var bitsPerSample = BitConverter.ToInt16(buffer, 34);

            AudioFormat = new AudioFormat(sampleRate, channels, bitsPerSample);

            var dataSize = BitConverter.ToInt32(buffer, 40);
            Data = new byte[dataSize];
            _fileStream.Read(Data, 0, dataSize);
        }
        catch (Exception exception)
        {
            Console.WriteLine($"Error processing WAV header: {exception.Message}");
        }
    }

    public void Dispose()
    {
        _fileStream.Dispose();
    }
}