namespace Lys.Renderer;

public struct BufferElement
{
    public string Name;
    public ShaderDataType Type;
    public int Size;
    public int Offset;
    public bool Normalized;

    public BufferElement(ShaderDataType type, string name, bool normalized = false)
    {
        Name = name;
        Type = type;
        Size = Shader.ShaderDataTypeSize(Type);
        Offset = 0;
        Normalized = normalized;
    }

    public int GetComponentCount() 
    {
        return Type switch
        {
            ShaderDataType.Float => 1,
            ShaderDataType.Float2 => 2,
            ShaderDataType.Float3 => 3,
            ShaderDataType.Float4 => 4,
            _ => 0
        };
    }
}