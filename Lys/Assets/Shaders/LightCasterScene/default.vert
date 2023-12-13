#version 440
layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec3 aNormal;
layout (location = 2) in vec2 aTexCoords;

out vec3 normal;
out vec3 fragPos;
out vec2 texCoords;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

uniform mat3 normalInverse;

void main() {
    gl_Position = vec4(aPosition, 1.0) * model * view * projection;;

    fragPos = vec3(vec4(aPosition, 1.0) * model);
    normal = normalInverse * aNormal;
    texCoords = aTexCoords;
}
