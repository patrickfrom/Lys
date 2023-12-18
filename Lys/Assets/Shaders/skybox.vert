#version 440
layout (location = 0) in vec3 aPosition;

layout (std140, binding = 0) uniform Matrices {
    mat4 projection;
    mat4 view;
};

/*uniform mat4 projection;
uniform mat4 view;*/

out vec3 TexCoords;

void main() {
    TexCoords = aPosition;
    vec4 pos =  vec4(aPosition, 1.0) * mat4(mat3(view)) * projection;
    gl_Position = pos.xyww;
}