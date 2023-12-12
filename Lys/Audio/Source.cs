using OpenTK.Audio.OpenAL;

namespace Lys.Audio;

public class Source
{
    private int _sourceId;

    public Source()
    {
        _sourceId = AL.GenSource();

        AL.Source(_sourceId, ALSourcef.Gain, 1.0f);
        AL.Source(_sourceId, ALSourcef.Pitch, 1.0f);
    }

    public void Play(int buffer)
    {
        AL.Source(_sourceId, ALSourcei.Buffer, buffer);
        AL.SourcePlay(_sourceId);
    }

    public void SetLooping(bool enable)
    {
        AL.Source(_sourceId, ALSourceb.Looping, enable);
    }

    public void Delete()
    {
        AL.DeleteSource(_sourceId);
    }
}