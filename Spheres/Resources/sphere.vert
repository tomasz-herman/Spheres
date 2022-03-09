#version 400

layout(location = 0) in vec3 cameraSpherePos;
layout(location = 1) in float sphereRadius;

uniform mat4 view;

out VertexData
{
    vec3 cameraSpherePos;
    float sphereRadius;
} outData;

void main()
{
    outData.cameraSpherePos = (view * vec4(cameraSpherePos, 1)).xyz;
    outData.sphereRadius = sphereRadius;
}