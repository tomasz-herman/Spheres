#version 400

layout(std140) uniform;
layout(points) in;
layout(triangle_strip, max_vertices=4) out;

uniform mat4 perspective;

in VertexData
{
    vec3 cameraSpherePos;
    float sphereRadius;
} vert[];

out FragData
{
    flat vec3 cameraSpherePos;
    flat float sphereRadius;
    smooth vec2 mapping;
};

const float g_boxCorrection = 1.5;

void main()
{
    vec4 cameraCornerPos;
    //Bottom-left
    mapping = vec2(-1.0, -1.0) * g_boxCorrection;
    cameraSpherePos = vec3(vert[0].cameraSpherePos);
    sphereRadius = vert[0].sphereRadius;
    cameraCornerPos = vec4(vert[0].cameraSpherePos, 1.0);
    cameraCornerPos.xy += vec2(-vert[0].sphereRadius, -vert[0].sphereRadius) * g_boxCorrection;
    gl_Position = perspective * cameraCornerPos;
    gl_PrimitiveID = gl_PrimitiveIDIn;
    EmitVertex();

    //Top-left
    mapping = vec2(-1.0, 1.0) * g_boxCorrection;
    cameraSpherePos = vec3(vert[0].cameraSpherePos);
    sphereRadius = vert[0].sphereRadius;
    cameraCornerPos = vec4(vert[0].cameraSpherePos, 1.0);
    cameraCornerPos.xy += vec2(-vert[0].sphereRadius, vert[0].sphereRadius) * g_boxCorrection;
    gl_Position = perspective * cameraCornerPos;
    gl_PrimitiveID = gl_PrimitiveIDIn;
    EmitVertex();

    //Bottom-right
    mapping = vec2(1.0, -1.0) * g_boxCorrection;
    cameraSpherePos = vec3(vert[0].cameraSpherePos);
    sphereRadius = vert[0].sphereRadius;
    cameraCornerPos = vec4(vert[0].cameraSpherePos, 1.0);
    cameraCornerPos.xy += vec2(vert[0].sphereRadius, -vert[0].sphereRadius) * g_boxCorrection;
    gl_Position = perspective * cameraCornerPos;
    gl_PrimitiveID = gl_PrimitiveIDIn;
    EmitVertex();

    //Top-right
    mapping = vec2(1.0, 1.0) * g_boxCorrection;
    cameraSpherePos = vec3(vert[0].cameraSpherePos);
    sphereRadius = vert[0].sphereRadius;
    cameraCornerPos = vec4(vert[0].cameraSpherePos, 1.0);
    cameraCornerPos.xy += vec2(vert[0].sphereRadius, vert[0].sphereRadius) * g_boxCorrection;
    gl_Position = perspective * cameraCornerPos;
    gl_PrimitiveID = gl_PrimitiveIDIn;
    EmitVertex();
}