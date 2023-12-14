using OpenTK.Mathematics;

namespace Lys.Lights;

public struct PointLight(Vector3 position, Vector3 color, float constant = 1.0f)
{
    public Vector3 Position = position;
    public Vector3 Color = color;
    public float Constant = constant;
}