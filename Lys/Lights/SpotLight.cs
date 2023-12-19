using OpenTK.Mathematics;

namespace Lys.Lights;

public struct SpotLight(
    Vector3 position,
    Vector3 direction,
    float constant = 1.0f,
    float linear = 0.09f,
    float quadratic = 0.032f,
    float cutOff = 12.5f,
    float outerCutOff = 17.5f,
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

    public float CutOff = cutOff;
    public float OuterCutOff = outerCutOff;

    public Vector3 Ambient = ambient != default ? ambient : new Vector3(1.0f);
    public Vector3 Diffuse = diffuse != default ? diffuse : new Vector3(1.0f);
    public Vector3 Specular = specular != default ? specular : new Vector3(1.0f);
}