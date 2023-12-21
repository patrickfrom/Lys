using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Lys.Renderer;

public class Shader
{
    private readonly int _handle;
    private readonly Dictionary<string, int> _uniformLocations;

    public Shader(string vertexPath, string fragmentPath)
    {
        var vertexShader = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(vertexShader, File.ReadAllText(vertexPath));
        GL.CompileShader(vertexShader);

        var fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(fragmentShader, File.ReadAllText(fragmentPath));
        GL.CompileShader(fragmentShader);

        _handle = GL.CreateProgram();
        GL.AttachShader(_handle, vertexShader);
        GL.AttachShader(_handle, fragmentShader);
        GL.LinkProgram(_handle);

        GL.DetachShader(_handle, vertexShader);
        GL.DetachShader(_handle, fragmentShader);
        GL.DeleteShader(vertexShader);
        GL.DeleteShader(fragmentShader);

        GL.GetProgram(_handle, GetProgramParameterName.ActiveUniforms, out var numberOfUniforms);

        _uniformLocations = new Dictionary<string, int>();

        for (var i = 0; i < numberOfUniforms; i++)
        {
            var key = GL.GetActiveUniform(_handle, i, out _, out _);
            var location = GL.GetUniformLocation(_handle, key);

            _uniformLocations.Add(key, location);
        }
    }

    public void Use()
    {
        GL.UseProgram(_handle);
    }

    public void SetFloat(string location, float amount)
    {
        GL.UseProgram(_handle);
        GL.Uniform1(_uniformLocations[location], amount);
    }

    public void SetMatrix3(string location, Matrix3 data)
    {
        GL.UseProgram(_handle);
        GL.UniformMatrix3(_uniformLocations[location], true, ref data);
    }

    public void SetMatrix4(string location, Matrix4 data)
    {
        GL.UseProgram(_handle);
        GL.UniformMatrix4(_uniformLocations[location], true, ref data);
    }

    public void SetVector3(string location, Vector3 data)
    {
        GL.UseProgram(_handle);
        GL.Uniform3(_uniformLocations[location], ref data);
    }

    public void SetInt(string location, int amount)
    {
        GL.UseProgram(_handle);
        GL.Uniform1(_uniformLocations[location], amount);
    }

    public void Dispose()
    {
        GL.DeleteProgram(_handle);
    }
    
    public static int ShaderDataTypeSize(ShaderDataType type)
    {
        return type switch
        {
            ShaderDataType.Float => 4,
            ShaderDataType.Float2 => 4 * 2,
            ShaderDataType.Float3 => 4 * 3,
            ShaderDataType.Float4 => 4 * 4,
            _ => 0
        };
    }
}