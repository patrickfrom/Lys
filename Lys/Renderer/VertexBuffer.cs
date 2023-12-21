using OpenTK.Graphics.OpenGL4;

namespace Lys.Renderer;

public class VertexBuffer<T> where T : struct
{
    private readonly int _rendererId;
    private BufferLayout _layout;
        
    public VertexBuffer(IEnumerable<T> vertices, int size)
    {
        _rendererId = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, _rendererId);
        GL.BufferData(BufferTarget.ArrayBuffer, size, vertices.ToArray(), BufferUsageHint.StaticDraw);
    }

    public void Bind()
    {
        GL.BindBuffer(BufferTarget.ArrayBuffer, _rendererId);
    }
    
    public void Unbind()
    {
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
    }

    public BufferLayout GetLayout()
    {
        return _layout;
    }

    public void SetLayout(BufferLayout layout)
    {
        _layout = layout;
    }
}