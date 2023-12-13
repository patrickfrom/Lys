using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

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
        if (window.KeyboardState.IsKeyPressed(Keys.F11))
        {
            window.WindowState = window.WindowState != WindowState.Fullscreen
                ? WindowState.Fullscreen
                : WindowState.Normal;
        }

        if (window.KeyboardState.IsKeyDown(Keys.Escape))
        {
            window.Close();
        }
    }

    public virtual void OnUnload()
    {
    }
}