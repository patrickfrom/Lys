using OpenTK.Graphics.OpenGL4;

namespace Lys.Renderer;

public class VertexArray
{
    public static VertexAttribPointerType ShaderDataTypeToOpenGl(ShaderDataType type)
    {
        return type switch
        {
            ShaderDataType.Float => VertexAttribPointerType.Float,
            ShaderDataType.Float2 => VertexAttribPointerType.Float,
            ShaderDataType.Float3 => VertexAttribPointerType.Float,
            ShaderDataType.Float4 => VertexAttribPointerType.Float,
            _ => 0
        };
    }
    
    private readonly int _rendererId = GL.GenVertexArray();

    public void Bind()
    {
        GL.BindVertexArray(_rendererId);
    }

    public void Unbind()
    {
        GL.BindVertexArray(0);
    }
}