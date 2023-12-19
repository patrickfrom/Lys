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

void main() {
    
    FragPos = vec3(vec4(aPosition, 1.0) * model);
    Normal = aNormal * mat3(transpose(inverse(model)));
    TexCoords = aTexCoords;
    
    gl_Position = vec4(aPosition, 1.0) * model * view * projection;
}