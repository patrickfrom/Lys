#version 440

struct Material {
    sampler2D diffuse;
    sampler2D specular;
    
    float shininess;
};

struct Light {
    vec3 position;

    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
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

out vec4 FragColor;

in vec3 normal;
in vec3 fragPos;
in vec2 texCoords;

uniform Material material;
//uniform Light light;
uniform DirectionalLight directionalLight;

#define NUM_POINT_LIGHTS 3
uniform PointLight pointLight[NUM_POINT_LIGHTS];
uniform vec3 viewPos;

/*vec3 CalculateLight() {
    vec3 ambient = light.ambient * texture(material.diffuse, texCoords).rgb;

    vec3 norm = normalize(normal);
    vec3 lightDir = normalize(light.position - fragPos);
    float diff = max(dot(norm, lightDir), 0.0);
    vec3 diffuse = light.diffuse * diff * texture(material.diffuse, texCoords).rgb;

    vec3 viewDir = normalize(viewPos - fragPos);
    vec3 reflectDir = reflect(-lightDir, norm);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
    vec3 specular = light.specular * spec * texture(material.specular, texCoords).rgb;

    return (ambient + diffuse + specular);
}*/

vec3 CalculateDirectionalLight(DirectionalLight light, vec3 norm, vec3 viewDir) {
    vec3 ambient = light.ambient * texture(material.diffuse, texCoords).rgb;
    
    vec3 lightDir = normalize(-light.direction);
    float diff = max(dot(norm, lightDir), 0.0);
    vec3 diffuse = light.diffuse * diff * texture(material.diffuse, texCoords).rgb;

    vec3 reflectDir = reflect(-lightDir, norm);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
    vec3 specular = light.specular * spec * texture(material.specular, texCoords).rgb;

    return (ambient + diffuse + specular);
}

vec3 CalculatePointLight(PointLight light, vec3 norm, vec3 fragP, vec3 viewDir) {
    vec3 ambient = light.ambient * texture(material.diffuse, texCoords).rgb;
    
    vec3 lightDir = normalize(light.position - fragP);
    float diff = max(dot(norm, lightDir), 0.0);
    vec3 diffuse = light.diffuse * diff * texture(material.diffuse, texCoords).rgb;
    
    vec3 reflectDir = reflect(-lightDir, norm);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
    vec3 specular = light.specular * spec * texture(material.specular, texCoords).rgb;
    
    float distance = length(light.position - fragP);
    float attenuation = 1.0 / (light.constant + light.linear * distance + light.quadratic * (distance * distance));
    
    ambient *= attenuation;
    diffuse *= attenuation;
    specular *= attenuation;

    return (ambient + diffuse + specular);
}

void main() {
    vec3 norm = normalize(normal);
    vec3 viewDir = normalize(viewPos - fragPos);
    
    vec3 result = CalculateDirectionalLight(directionalLight, norm, viewDir);
    for(int i = 0; i < NUM_POINT_LIGHTS; i++)
        result += CalculatePointLight(pointLight[i], norm, fragPos, viewDir);
    
    FragColor = vec4(result, 1.0);
}
