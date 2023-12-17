using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using ImGuiNET;
using Lys.Utils;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Lys;

public class Window(int width, int height, string title) : GameWindow(GameWindowSettings.Default,
    new NativeWindowSettings
    {
        ClientSize = new Vector2i(width, height),
        Title = title
    })
{
    private ImGuiController _controller;

    private readonly Vertex[] _vertices =
    {
        new(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(0.0f, 0.0f, -1.0f), new Vector2(0.0f, 0.0f)),
        new(new Vector3(0.5f, -0.5f, -0.5f), new Vector3(0.0f, 0.0f, -1.0f), new Vector2(1.0f, 0.0f)),
        new(new Vector3(0.5f, 0.5f, -0.5f), new Vector3(0.0f, 0.0f, -1.0f), new Vector2(1.0f, 1.0f)),
        new(new Vector3(-0.5f, 0.5f, -0.5f), new Vector3(0.0f, 0.0f, -1.0f), new Vector2(0.0f, 1.0f)),

        new(new Vector3(-0.5f, -0.5f, 0.5f), new Vector3(0.0f, 0.0f, 1.0f), new Vector2(0.0f, 0.0f)),
        new(new Vector3(0.5f, -0.5f, 0.5f), new Vector3(0.0f, 0.0f, 1.0f), new Vector2(1.0f, 0.0f)),
        new(new Vector3(0.5f, 0.5f, 0.5f), new Vector3(0.0f, 0.0f, 1.0f), new Vector2(1.0f, 1.0f)),
        new(new Vector3(-0.5f, 0.5f, 0.5f), new Vector3(0.0f, 0.0f, 1.0f), new Vector2(0.0f, 1.0f)),

        new(new Vector3(-0.5f, 0.5f, -0.5f), new Vector3(-1.0f, 0.0f, 0.0f), new Vector2(0.0f, 0.0f)),
        new(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(-1.0f, 0.0f, 0.0f), new Vector2(1.0f, 0.0f)),
        new(new Vector3(-0.5f, -0.5f, 0.5f), new Vector3(-1.0f, 0.0f, 0.0f), new Vector2(1.0f, 1.0f)),
        new(new Vector3(-0.5f, 0.5f, 0.5f), new Vector3(-1.0f, 0.0f, 0.0f), new Vector2(0.0f, 1.0f)),

        new(new Vector3(0.5f, 0.5f, 0.5f), new Vector3(1.0f, 0.0f, 0.0f), new Vector2(0.0f, 0.0f)),
        new(new Vector3(0.5f, 0.5f, -0.5f), new Vector3(1.0f, 0.0f, 0.0f), new Vector2(1.0f, 0.0f)),
        new(new Vector3(0.5f, -0.5f, -0.5f), new Vector3(1.0f, 0.0f, 0.0f), new Vector2(1.0f, 1.0f)),
        new(new Vector3(0.5f, -0.5f, 0.5f), new Vector3(1.0f, 0.0f, 0.0f), new Vector2(0.0f, 1.0f)),

        new(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(0.0f, -1.0f, 0.0f), new Vector2(0.0f, 0.0f)),
        new(new Vector3(0.5f, -0.5f, -0.5f), new Vector3(0.0f, -1.0f, 0.0f), new Vector2(1.0f, 0.0f)),
        new(new Vector3(0.5f, -0.5f, 0.5f), new Vector3(0.0f, -1.0f, 0.0f), new Vector2(1.0f, 1.0f)),
        new(new Vector3(-0.5f, -0.5f, 0.5f), new Vector3(0.0f, -1.0f, 0.0f), new Vector2(0.0f, 1.0f)),

        new(new Vector3(0.5f, 0.5f, -0.5f), new Vector3(0.0f, 1.0f, 0.0f), new Vector2(0.0f, 0.0f)),
        new(new Vector3(0.5f, 0.5f, 0.5f), new Vector3(0.0f, 1.0f, 0.0f), new Vector2(1.0f, 0.0f)),
        new(new Vector3(-0.5f, 0.5f, 0.5f), new Vector3(0.0f, 1.0f, 0.0f), new Vector2(1.0f, 1.0f)),
        new(new Vector3(-0.5f, 0.5f, -0.5f), new Vector3(0.0f, 1.0f, 0.0f), new Vector2(0.0f, 1.0f)),
    };

    private readonly int[] _indices =
    {
        0, 3, 1,
        3, 2, 1,

        4, 5, 7,
        7, 5, 6,

        8, 9, 11,
        11, 9, 10,

        12, 15, 13,
        15, 14, 13,

        16, 17, 19,
        19, 17, 18,

        20, 23, 21,
        23, 22, 21,
    };

    private Shader _shader;

    private int _vao;
    private int _vbo;
    private int _ebo;
    
    private Shader _skyboxShader;
    private Skybox _skybox;
    
    private int _skyboxVao;
    private int _skyboxVbo;

    private Camera _camera;
    
    private Vector3 _cubePos;

    protected override void OnLoad()
    {
        GlDebugger.Init();
        GL.ClearColor(Color.Navy);

        _controller = new ImGuiController(ClientSize.X, ClientSize.Y);

        _vao = GL.GenVertexArray();
        GL.BindVertexArray(_vao);

        _vbo = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * Unsafe.SizeOf<Vertex>(), _vertices,
            BufferUsageHint.StaticDraw);

        _ebo = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(int), _indices,
            BufferUsageHint.StaticDraw);

        GL.EnableVertexAttribArray(0);
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Unsafe.SizeOf<Vertex>(), IntPtr.Zero);

        GL.EnableVertexAttribArray(1);
        GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, Unsafe.SizeOf<Vertex>(),
            Marshal.OffsetOf<Vertex>("Normal"));

        GL.EnableVertexAttribArray(2);
        GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, Unsafe.SizeOf<Vertex>(),
            Marshal.OffsetOf<Vertex>("TexCoords"));

        _shader = new Shader("Assets/Shaders/default.vert", "Assets/Shaders/default.frag");
        _skyboxShader = new Shader("Assets/Shaders/skybox.vert", "Assets/Shaders/skybox.frag");
        
        _camera = new Camera(Vector3.UnitZ * 3, ClientSize.X / (float)ClientSize.Y, KeyboardState,
            MouseState);

        CursorState = CursorState.Grabbed;
        
        _skyboxVao = GL.GenVertexArray();
        GL.BindVertexArray(_skyboxVao);

        _skyboxVbo = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, _skyboxVbo);
        GL.BufferData(BufferTarget.ArrayBuffer, Skybox.SkyboxVertices.Length * sizeof(float), Skybox.SkyboxVertices,
            BufferUsageHint.StaticDraw);

        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
        
        GL.EnableVertexAttribArray(0);
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
        
        var skyboxPaths = new[]
        {
            "Assets/Skybox/Calm/px.png",
            "Assets/Skybox/Calm/nx.png",
            "Assets/Skybox/Calm/py.png",
            "Assets/Skybox/Calm/ny.png",
            "Assets/Skybox/Calm/pz.png",
            "Assets/Skybox/Calm/nz.png",
        };

        _skybox = new Skybox(skyboxPaths);
        _skyboxShader.SetInt("skybox", 0);

        GL.Enable(EnableCap.DepthTest);
        
        GL.CullFace(CullFaceMode.Back);
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
        GL.Clear(ClearBufferMask.ColorBufferBit |ClearBufferMask.DepthBufferBit);

        _shader.SetMatrix4("view", _camera.GetViewMatrix());
        _shader.SetMatrix4("projection", _camera.GetProjectionMatrix());
        _shader.SetVector3("viewPos", _camera.Position);
        
        
        GL.BindVertexArray(_vao);

        var model = Matrix4.Identity;
        model *= Matrix4.CreateScale(1.0f);
        model *= Matrix4.CreateTranslation(_cubePos);

        _shader.SetMatrix4("model", model);
        GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);

        ShowSkybox();

        RenderUi((float)e.Time);
        
        SwapBuffers();
    }

    protected override void OnUpdateFrame(FrameEventArgs e)
    {
        if (KeyboardState.IsKeyPressed(Keys.F))
        {
            CursorState = CursorState == CursorState.Grabbed ? CursorState.Normal : CursorState.Grabbed;
        }
        
        if (KeyboardState.IsKeyDown(Keys.Escape))
            Close();
        
        if (KeyboardState.IsKeyPressed(Keys.F11))
        {
            WindowState = WindowState != WindowState.Fullscreen
                ? WindowState.Fullscreen
                : WindowState.Normal;
        }
        

        if(CursorState == CursorState.Grabbed)
            _camera.Update(e.Time);
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

    private void ShowSkybox()
    {
        GL.Disable(EnableCap.CullFace);
        GL.DepthFunc(DepthFunction.Lequal);
        _skyboxShader.SetMatrix4("view", new Matrix4(new Matrix3(_camera.GetViewMatrix())));
        _skyboxShader.SetMatrix4("projection", _camera.GetProjectionMatrix());
        GL.BindVertexArray(_skyboxVao);
        _skybox.Use();
        GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
        GL.DepthFunc(DepthFunction.Less);
        GL.Enable(EnableCap.CullFace);
    }
    
    private void RenderUi(float time)
    {
        _controller.Update(this, time);

        ImGui.Begin("Cube Pos");
        ImGuiExtensions.DragFloat3("Position", ref _cubePos, 0.01f);
        ImGui.End();

        _controller.Render();
    }

    protected override void OnUnload()
    {
        base.OnUnload();
        
        _shader.Dispose();
    }
}