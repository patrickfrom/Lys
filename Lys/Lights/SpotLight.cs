using OpenTK.Mathematics;

namespace Lys.Lights;

public struct SpotLight(
    Vector3 position,
    Vector3 direction,
    float constant = 1.0f,
    float linear = 0.09f,
    float quadratic = 0.032f,
    float cutoffDegrees = 12.5f,
    float outerCutOffDegrees = 15.0f,
    Vector3 ambient = default,
    Vector3 diffuse = default,
    Vector3 specular = default
    )
{
    public Vector3 Position = position;
    public Vector3 Direction = direction;

    public float Constant = constant;
    public float Linear = linear;
    public float Quadratic = quadratic;

    public float CutOff
    {
        get => MathHelper.DegreesToRadians(cutoffDegrees);
        set => cutoffDegrees = value;
    }
    
    public float OuterCutOff
    {
        get => MathHelper.DegreesToRadians(outerCutOffDegrees);
        set => outerCutOffDegrees = value;
    }

    public Vector3 Ambient = ambient != default ? ambient : new Vector3(1.0f);
    public Vector3 Diffuse = diffuse != default ? diffuse : new Vector3(1.0f);
    public Vector3 Specular = specular != default ? specular : new Vector3(1.0f);
}