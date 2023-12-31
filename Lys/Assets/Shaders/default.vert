#version 440

layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec3 aNormal;
layout (location = 2) in vec2 aTexCoords;

out vec3 Normal;
out vec2 TexCoords;
out vec3 FragPos;

layout (std140, binding = 0) uniform Matrices {
    mat4 projection;
    mat4 view;
};

uniform mat4 model;
uniform mat3 normalInverse;

void main() {
    
    gl_Position = vec4(aPosition, 1.0) * model * view * projection;
    
    FragPos = vec3(vec4(aPosition, 1.0) * model);
    Normal = normalInverse * aNormal;
    TexCoords = aTexCoords;
}
