#version 440

struct Material {
    sampler2D texture_diffuse1;
    sampler2D texture_specular1;

    float shininess;
};

struct DirectionalLight {
    vec3 direction;

    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};

struct PointLight {
    vec3 position;

    vec3 ambient;
    vec3 diffuse;
    vec3 specular;

    float constant;
    float linear;
    float quadratic;
};

struct SpotLight {
    vec3 position;
    vec3 direction;

    float cutOff;
    float outerCutOff;

    vec3 ambient;
    vec3 diffuse;
    vec3 specular;

    float constant;
    float linear;
    float quadratic;
};

out vec4 FragColor;

in vec3 normal;
in vec3 fragPos;
in vec2 texCoords;

uniform Material material;
uniform DirectionalLight directionalLight;

#define NUM_POINT_LIGHTS 3
uniform PointLight pointLight[NUM_POINT_LIGHTS];
#define NUM_SPOT_LIGHTS 2
uniform SpotLight spotLight[NUM_SPOT_LIGHTS];
uniform vec3 viewPos;

vec3 CalculateDirectionalLight(DirectionalLight light, vec3 norm, vec3 viewDir) {
    vec3 ambient = light.ambient * texture(material.texture_diffuse1, texCoords).rgb;

    vec3 lightDir = normalize(-light.direction);
    float diff = max(dot(norm, lightDir), 0.0);
    vec3 diffuse = light.diffuse * diff * texture(material.texture_diffuse1, texCoords).rgb;
    
    vec3 halfwayDir = normalize(lightDir + viewDir);

    vec3 reflectDir = reflect(-lightDir, norm);
    float spec = pow(max(dot(norm, halfwayDir), 0.0), material.shininess);
    vec3 specular = light.specular * spec * texture(material.texture_specular1, texCoords).rgb;

    
    return (ambient + diffuse + specular);
}

vec3 CalculatePointLight(PointLight light, vec3 norm, vec3 fragP, vec3 viewDir) {
    vec3 ambient = light.ambient * texture(material.texture_diffuse1, texCoords).rgb;

    vec3 lightDir = normalize(light.position - fragP);
    float diff = max(dot(norm, lightDir), 0.0);
    vec3 diffuse = light.diffuse * diff * texture(material.texture_diffuse1, texCoords).rgb;

    vec3 halfwayDir = normalize(lightDir + viewDir);
    
    vec3 reflectDir = reflect(-lightDir, norm);
    float spec = pow(max(dot(norm, halfwayDir), 0.0), material.shininess);
    vec3 specular = light.specular * spec * texture(material.texture_specular1, texCoords).rgb;

    float distance = length(light.position - fragP);
    float attenuation = 1.0 / (light.constant + light.linear * distance + light.quadratic * (distance * distance));

    ambient *= attenuation;
    diffuse *= attenuation;
    specular *= attenuation;

    return (ambient + diffuse + specular);
}

vec3 CalculateSpotLight(SpotLight light, vec3 norm, vec3 viewDir) {
    vec3 ambient = light.ambient * texture(material.texture_diffuse1, texCoords).rgb;
    
    vec3 lightDir = normalize(light.position - fragPos);
    
    float diff = max(dot(norm, lightDir), 0.0);
    vec3 diffuse = light.diffuse * diff * texture(material.texture_diffuse1, texCoords).rgb;

    vec3 halfwayDir = normalize(lightDir + viewDir);
    
    vec3 reflectDir = reflect(-lightDir, norm);
    float spec = pow(max(dot(norm, halfwayDir), 0.0), material.shininess);
    vec3 specular = light.specular * spec * texture(material.texture_specular1, texCoords).rgb;

    float theta = dot(lightDir, normalize(-light.direction));
    float epsilon = (light.cutOff - light.outerCutOff);
    float intensity = clamp((theta - light.outerCutOff) / epsilon, 0.0, 1.0);
    diffuse  *= intensity;
    specular *= intensity;
    
    float distance    = length(light.position - fragPos);
    float attenuation = 1.0 / (light.constant + light.linear * distance + light.quadratic * (distance * distance));

    ambient  *= attenuation;
    diffuse   *= attenuation;
    specular *= attenuation;

    return (ambient + diffuse + specular);
}

void main() {
    vec3 norm = normalize(normal);
    vec3 viewDir = normalize(viewPos - fragPos);

    vec3 result = CalculateDirectionalLight(directionalLight, norm, viewDir);
    for (int i = 0; i < NUM_POINT_LIGHTS; i++) {
        result += CalculatePointLight(pointLight[i], norm, fragPos, viewDir);
    }

    for (int i = 0; i < NUM_SPOT_LIGHTS; i++) {
        result += CalculateSpotLight(spotLight[i], norm, viewDir);
    }

    FragColor = vec4(result, 1.0);
}
