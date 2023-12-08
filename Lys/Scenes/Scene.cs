using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace Lys.Scenes;

public abstract class Scene(NativeWindow window, string title = "Default Scene")
{
    public virtual void OnLoad()
    {
        window.Title = title;
    }

    public virtual void OnRender(FrameEventArgs e)
    {
    }

    public virtual void OnUpdate(FrameEventArgs e)
    {
    }

    public virtual void OnUnload()
    {
    }
}