using System.Drawing;
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
    private Shader _lightingShader;
    private Shader _lightCubeShader;

    private Camera _camera;
    private bool _firstMove = true;
    private Vector2 _lastPos;

    private double _time;
    
    private int _vao;

    private float[] _vertices =
    {
        -0.5f, -0.5f, -0.5f, 0.2f, 1.0f, 1.0f, 1.0f,
        0.5f, -0.5f, -0.5f,  0.2f, 1.0f, 1.0f, 1.0f,
        0.5f,  0.5f, -0.5f,  0.2f, 1.0f, 1.0f, 1.0f,
        -0.5f,  0.5f, -0.5f, 0.2f, 1.0f, 1.0f, 1.0f,
        -0.5f, -0.5f,  0.5f, 0.2f, 1.0f, 1.0f, 1.0f,
        0.5f, -0.5f,  0.5f,  0.2f, 1.0f, 1.0f, 1.0f,
        0.5f,  0.5f,  0.5f,  0.2f, 1.0f, 1.0f, 1.0f,
        -0.5f,  0.5f,  0.5f, 0.2f, 1.0f, 1.0f, 1.0f,
        -0.5f,  0.5f, -0.5f, 0.2f, 1.0f, 1.0f, 1.0f,
        -0.5f, -0.5f, -0.5f, 0.2f, 1.0f, 1.0f, 1.0f,
        -0.5f, -0.5f,  0.5f, 0.2f, 1.0f, 1.0f, 1.0f,
        -0.5f,  0.5f,  0.5f, 0.2f, 1.0f, 1.0f, 1.0f,
        0.5f,  0.5f,  0.5f,  0.2f, 1.0f, 1.0f, 1.0f,
        0.5f,  0.5f, -0.5f,  0.2f, 1.0f, 1.0f, 1.0f,
        0.5f, -0.5f, -0.5f,  0.2f, 1.0f, 1.0f, 1.0f,
        0.5f, -0.5f,  0.5f,  0.2f, 1.0f, 1.0f, 1.0f,
        -0.5f, -0.5f, -0.5f, 0.2f, 1.0f, 1.0f, 1.0f,
        0.5f, -0.5f, -0.5f,  0.2f, 1.0f, 1.0f, 1.0f,
        0.5f, -0.5f,  0.5f,  0.2f, 1.0f, 1.0f, 1.0f,
        -0.5f, -0.5f,  0.5f, 0.2f, 1.0f, 1.0f, 1.0f,
        0.5f,  0.5f, -0.5f,  0.2f, 1.0f, 1.0f, 1.0f,
        0.5f,  0.5f,  0.5f,  0.2f, 1.0f, 1.0f, 1.0f,
        -0.5f,  0.5f,  0.5f, 0.2f, 1.0f, 1.0f, 1.0f,
        -0.5f,  0.5f, -0.5f, 0.2f, 1.0f, 1.0f, 1.0f,
    };

    private int[] _indices =
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

    protected override void OnLoad()
    {
        GlDebugger.Init();

        GL.ClearColor(Color.Navy);

        _vao = GL.GenVertexArray();
        GL.BindVertexArray(_vao);

        var vbo = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices,
            BufferUsageHint.StaticDraw);

        var ebo = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(int), _indices,
            BufferUsageHint.StaticDraw);

        GL.EnableVertexAttribArray(0);
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 7 * sizeof(float), 0);
        
        GL.EnableVertexAttribArray(1);
        GL.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, 7 * sizeof(float), 3 * sizeof(float));

        _lightingShader = new Shader("Assets/Shaders/colors.vert", "Assets/Shaders/colors.frag");
        _lightingShader.Use();

        _camera = new Camera(Vector3.UnitZ * 3, ClientSize.X / (float)ClientSize.Y);

        CursorState = CursorState.Grabbed;
        
        
        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        
        GL.Enable(EnableCap.DepthTest);
        GL.Enable(EnableCap.CullFace);
        GL.CullFace(CullFaceMode.Back); 
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
        _time += 4.0 * e.Time;

        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);  

        GL.BindVertexArray(_vao);

        var model = Matrix4.Identity;
        _lightingShader.SetMatrix4("model", model);
        _lightingShader.SetMatrix4("view", _camera.GetViewMatrix());
        _lightingShader.SetMatrix4("projection", _camera.GetProjectionMatrix());
        
        GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);

        SwapBuffers();
    }

    protected override void OnUpdateFrame(FrameEventArgs e)
    {
        if (!IsFocused)
        {
            return;
        }

        if (KeyboardState.IsKeyDown(Keys.Escape))
        {
            Close();
        }

        const float cameraSpeed = 1.5f;
        const float sensitivity = 0.2f;

        if (KeyboardState.IsKeyDown(Keys.W))
        {
            _camera.Position += _camera.Front * cameraSpeed * (float)e.Time;
        }

        if (KeyboardState.IsKeyDown(Keys.S))
        {
            _camera.Position -= _camera.Front * cameraSpeed * (float)e.Time;
        }

        if (KeyboardState.IsKeyDown(Keys.A))
        {
            _camera.Position -= _camera.Right * cameraSpeed * (float)e.Time;
        }

        if (KeyboardState.IsKeyDown(Keys.D))
        {
            _camera.Position += _camera.Right * cameraSpeed * (float)e.Time;
        }

        if (KeyboardState.IsKeyDown(Keys.Space))
        {
            _camera.Position += _camera.Up * cameraSpeed * (float)e.Time;
        }

        if (KeyboardState.IsKeyDown(Keys.LeftShift))
        {
            _camera.Position -= _camera.Up * cameraSpeed * (float)e.Time;
        }

        if (_firstMove)
        {
            _lastPos = new Vector2(MouseState.X, MouseState.Y);
            _firstMove = false;
        }
        else
        {
            var deltaX = MouseState.X - _lastPos.X;
            var deltaY = MouseState.Y - _lastPos.Y;
            _lastPos = new Vector2(MouseState.X, MouseState.Y);

            _camera.Yaw += deltaX * sensitivity;
            _camera.Pitch -= deltaY * sensitivity;
        }
    }

    protected override void OnResize(ResizeEventArgs e)
    {
        GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);
    }

    protected override void OnUnload()
    {
    }
}