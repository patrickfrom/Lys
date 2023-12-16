using System.Drawing;
using ImGuiNET;
using Lys.Utils;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace Lys;

public class Window(int width, int height, string title) : GameWindow(GameWindowSettings.Default,
    new NativeWindowSettings
    {
        ClientSize = new Vector2i(width, height),
        Title = title
    })
{

    private ImGuiController _controller;
    
    protected override void OnLoad()
    {
        GlDebugger.Init();
        GL.ClearColor(Color.Navy);
        
        _controller = new ImGuiController(ClientSize.X, ClientSize.Y);
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
        GL.Clear(ClearBufferMask.ColorBufferBit);
        
        _controller.Update(this, (float)e.Time);

        ImGui.DockSpaceOverViewport();
        ImGui.ShowDemoWindow();
        
        _controller.Render();
        SwapBuffers();
    }

    protected override void OnUpdateFrame(FrameEventArgs e)
    {

    }

    protected override void OnResize(ResizeEventArgs e)
    {
        GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);
        _controller.WindowResized(ClientSize.X, ClientSize.Y);
    }
    
    protected override void OnTextInput(TextInputEventArgs e)
    {
        base.OnTextInput(e);
        _controller.PressChar((char)e.Unicode);
    }

    protected override void OnMouseWheel(MouseWheelEventArgs e)
    {
        base.OnMouseWheel(e);
        _controller.MouseScroll(e.Offset);
    }
}