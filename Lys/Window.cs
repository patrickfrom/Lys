using System.Drawing;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using ImGuiNET;
using Lys.Lights;
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
        Title = title,
        APIVersion = new Version(4, 4, 0),
        NumberOfSamples = 4
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
    private Shader _lightCubeShader;

    private int _vao;
    private int _vbo;
    private int _ebo;

    private Shader _skyboxShader;
    private Skybox _skybox;

    private int _skyboxVao;
    private int _skyboxVbo;

    private Camera _camera;

    private Vector3 _cubePos = new(0.0f, -4.0f, 0.0f);
    private float _cubeScale = 5.0f;
    private Vector3 _cubeRotation;
    private float _cubeShininess = 8.0f;

    private int _uboMatrices;

    private Texture2D _walnutDiffuse;
    private Texture2D _walnutSpecular;

    private DirectionalLight _directionalLight = new(
        new Vector3(-0.090f, -0.340f, 0.320f),
        ambient: new Vector3(0.5f)
    );

    private PointLight _pointLight =
        new(new Vector3(2, 4, 2), diffuse: new Vector3(1, 0, 0), ambient: new Vector3(0.5f));

    private SpotLight _spotLight = new(new Vector3(0, 1, 0), new Vector3(0, -1, 0), diffuse: new Vector3(0, 1, 0),
        specular: new Vector3(0, 1, 0), ambient: new Vector3(0.0f));

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
            Marshal.OffsetOf<Vertex>(nameof(Vertex.Normal)));

        GL.EnableVertexAttribArray(2);
        GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, Unsafe.SizeOf<Vertex>(),
            Marshal.OffsetOf<Vertex>(nameof(Vertex.TexCoords)));

        _shader = new Shader("Assets/Shaders/default.vert", "Assets/Shaders/default.frag");
        _lightCubeShader = new Shader("Assets/Shaders/default.vert", "Assets/Shaders/lightcube.frag");
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

        _walnutDiffuse = new Texture2D("Assets/Textures/WalnutBaseTexture_albedo.png");
        _walnutSpecular = new Texture2D("Assets/Textures/WalnutBaseTexture_specular.png");
        _shader.SetInt("material.diffuse", 0);
        _shader.SetInt("material.specular", 1);

        GL.Enable(EnableCap.DepthTest);
        GL.Enable(EnableCap.Multisample);
        GL.Enable(EnableCap.FramebufferSrgb);

        GL.CullFace(CullFaceMode.Back);

        _uboMatrices = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.UniformBuffer, _uboMatrices);
        GL.BufferData(BufferTarget.UniformBuffer, 2 * Unsafe.SizeOf<Matrix4>(), IntPtr.Zero,
            BufferUsageHint.StaticDraw);
        GL.BindBuffer(BufferTarget.UniformBuffer, 0);

        GL.BindBufferRange(BufferRangeTarget.UniformBuffer, 0, _uboMatrices, 0, 2 * Unsafe.SizeOf<Matrix4>());
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        var view = Matrix4.Transpose(_camera.GetViewMatrix());
        var projection = Matrix4.Transpose(_camera.GetProjectionMatrix());

        GL.BindBuffer(BufferTarget.UniformBuffer, _uboMatrices);
        GL.BufferSubData(BufferTarget.UniformBuffer, 0, Unsafe.SizeOf<Matrix4>(), ref projection);
        GL.BufferSubData(BufferTarget.UniformBuffer, Unsafe.SizeOf<Matrix4>(), Unsafe.SizeOf<Matrix4>(), ref view);
        GL.BindBuffer(BufferTarget.UniformBuffer, 0);

        _shader.SetVector3("viewPos", _camera.Position);

        // Directional Light
        _shader.SetVector3("directionalLight.direction", _directionalLight.Direction);
        _shader.SetVector3("directionalLight.ambient", _directionalLight.Ambient);
        _shader.SetVector3("directionalLight.diffuse", _directionalLight.Diffuse);
        _shader.SetVector3("directionalLight.specular", _directionalLight.Specular);

        // Point Light
        _shader.SetVector3("pointLight.position", _pointLight.Position);
        _shader.SetFloat("pointLight.constant", _pointLight.Constant);
        _shader.SetFloat("pointLight.linear", _pointLight.Linear);
        _shader.SetFloat("pointLight.quadratic", _pointLight.Quadratic);
        _shader.SetVector3("pointLight.ambient", _pointLight.Ambient);
        _shader.SetVector3("pointLight.diffuse", _pointLight.Diffuse);
        _shader.SetVector3("pointLight.specular", _pointLight.Specular);

        GL.BindVertexArray(_vao);

        var model = Matrix4.Identity;
        model *= Matrix4.CreateScale(0.5f);
        model *= Matrix4.CreateTranslation(_pointLight.Position);

        _lightCubeShader.SetVector3("lightColor", _pointLight.Diffuse);
        _lightCubeShader.SetMatrix4("model", model);
        GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);

        // Spot Light
        _shader.SetVector3("spotLight.position", _spotLight.Position);
        _shader.SetVector3("spotLight.direction", _spotLight.Direction);
        _shader.SetFloat("spotLight.constant", _spotLight.Constant);
        _shader.SetFloat("spotLight.linear", _spotLight.Linear);
        _shader.SetFloat("spotLight.quadratic", _spotLight.Quadratic);
        _shader.SetFloat("spotLight.cutOff", (float)Math.Cos(MathHelper.DegreesToRadians(_spotLight.CutOff)));
        _shader.SetFloat("spotLight.outerCutOff", (float)Math.Cos(MathHelper.DegreesToRadians(_spotLight.OuterCutOff)));
        _shader.SetVector3("spotLight.ambient", _spotLight.Ambient);
        _shader.SetVector3("spotLight.diffuse", _spotLight.Diffuse);
        _shader.SetVector3("spotLight.specular", _spotLight.Specular);

        GL.BindVertexArray(_vao);
        model = Matrix4.Identity;
        model *= Matrix4.CreateScale(0.5f);
        model *= Matrix4.CreateTranslation(_spotLight.Position);

        _lightCubeShader.SetVector3("lightColor", _spotLight.Diffuse);
        _lightCubeShader.SetMatrix4("model", model);
        GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);

        GL.BindVertexArray(_vao);

        model = Matrix4.Identity;
        model *= Matrix4.CreateScale(_cubeScale);
        model *= Matrix4.CreateRotationX(MathHelper.DegreesToRadians(_cubeRotation.X));
        model *= Matrix4.CreateRotationY(MathHelper.DegreesToRadians(_cubeRotation.Y));
        model *= Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(_cubeRotation.Z));
        model *= Matrix4.CreateTranslation(_cubePos);
        _shader.SetFloat("material.shininess", _cubeShininess);
        _walnutDiffuse.Use();
        _walnutSpecular.Use(1);
        _shader.SetMatrix4("model", model);
        GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
        GL.BindTexture(TextureTarget.Texture2D, 0);

        GL.BindVertexArray(_vao);

        model = Matrix4.Identity;
        model *= Matrix4.CreateScale(1.0f);
        model *= Matrix4.CreateTranslation(new Vector3(1.0f, 2.0f, 1.0f));
        _shader.SetFloat("material.shininess", 32.0f);
        _walnutDiffuse.Use();
        _walnutSpecular.Use(1);
        _shader.SetMatrix4("model", model);
        GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
        GL.BindTexture(TextureTarget.Texture2D, 0);

        GL.BindVertexArray(_vao);

        model = Matrix4.Identity;
        model *= Matrix4.CreateScale(0.25f);
        model *= Matrix4.CreateTranslation(new Vector3(1.0f, 0.5f, 1.0f));
        _shader.SetFloat("material.shininess", 32.0f);
        _walnutDiffuse.Use();
        _walnutSpecular.Use(1);
        _shader.SetMatrix4("model", model);
        GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
        GL.BindTexture(TextureTarget.Texture2D, 0);

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


        if (CursorState == CursorState.Grabbed)
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
        _skyboxShader.Use();
        GL.BindVertexArray(_skyboxVao);
        _skybox.Use();
        GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
        GL.DepthFunc(DepthFunction.Less);
        GL.Enable(EnableCap.CullFace);
    }

    private void RenderUi(float time)
    {
        _controller.Update(this, time);


        ImGui.Begin("Editor");
        if (ImGui.TreeNode("Cubes"))
        {
            if (ImGui.TreeNode("Cube 1"))
            {
                ImGuiExtensions.DragFloat3("Position", ref _cubePos, 0.01f);
                ImGuiExtensions.DragFloat3("Rotation", ref _cubeRotation, 0.1f);
                ImGui.DragFloat("Scale", ref _cubeScale, 0.01f);
                ImGui.DragFloat("Shininess", ref _cubeShininess, 0.01f);
                ImGui.TreePop();
            }

            ImGui.TreePop();
        }

        if (ImGui.TreeNode("Lights"))
        {
            if (ImGui.TreeNode("Directional Light"))
            {
                ImGuiExtensions.DragFloat3("Direction", ref _directionalLight.Direction, 0.01f);
                ImGui.TreePop();
            }

            if (ImGui.TreeNode("Point Light"))
            {
                ImGuiExtensions.DragFloat3("Position", ref _pointLight.Position, 0.01f);
                ImGuiExtensions.ColorEdit3("Diffuse", ref _pointLight.Diffuse);
                ImGuiExtensions.ColorEdit3("Specular", ref _pointLight.Specular);
                ImGui.DragFloat("Constant", ref _pointLight.Constant, 0.01f);
                ImGui.DragFloat("Linear", ref _pointLight.Linear, 0.01f);
                ImGui.DragFloat("Quadratic", ref _pointLight.Quadratic, 0.01f);
                ImGui.TreePop();
            }

            if (ImGui.TreeNode("Spotlight"))
            {
                ImGuiExtensions.DragFloat3("Position", ref _spotLight.Position, 0.01f);
                ImGuiExtensions.DragFloat3("Direction", ref _spotLight.Direction, 0.01f);
                ImGuiExtensions.ColorEdit3("Diffuse", ref _spotLight.Diffuse);
                ImGuiExtensions.ColorEdit3("Specular", ref _spotLight.Specular);
                ImGui.DragFloat("Constant", ref _spotLight.Constant, 0.01f);
                ImGui.DragFloat("Linear", ref _spotLight.Linear, 0.01f);
                ImGui.DragFloat("Quadratic", ref _spotLight.Quadratic, 0.01f);
                ImGui.DragFloat("cutOff", ref _spotLight.CutOff, 0.01f);
                ImGui.DragFloat("outerCutOff", ref _spotLight.OuterCutOff, 0.01f);
                ImGui.TreePop();
            }
            ImGui.TreePop();
        }

        ImGui.End();

        Title = $"Lys - FPS {ImGui.GetIO().Framerate}";

        _controller.Render();
    }

    protected override void OnUnload()
    {
        base.OnUnload();

        _shader.Dispose();
    }
}