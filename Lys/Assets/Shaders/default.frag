#version 440

out vec4 FragColor;

in vec3 Normal;
in vec2 TexCoords;
in vec3 FragPos;

uniform vec3 viewPos;

void main() {
    vec3 result = vec3(TexCoords, 0.0) * dot(normalize(Normal), viewPos - FragPos);
    FragColor = vec4(result, 1.0);
}