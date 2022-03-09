#version 400

in FragData
{
    flat vec3 cameraSpherePos;
    flat float sphereRadius;
    smooth vec2 mapping;
};

out vec4 outputColor;

layout(std140) uniform;

const vec4 diffuseColor = vec4(0.5f, 0.5f, 0.5f, 1.0f);
const vec4 specularColor = vec4(0.95f, 0.95f, 0.95f, 1.0f);
const float specularShininess = 0.3;

uniform vec4 cameraSpaceLightPos;
const vec4 lightIntensity = vec4(0.6f, 0.6f, 0.6f, 1.0f);

const vec4 ambientIntensity = vec4(0.01f, 0.01f, 0.01f, 1.0f);
const float lightAttenuation = 1.0f / (50.0f * 50.0f);

float CalcAttenuation(in vec3 cameraSpacePosition, in vec3 cameraSpaceLightPos, out vec3 lightDirection)
{
    vec3 lightDifference =  cameraSpaceLightPos - cameraSpacePosition;
    float lightDistanceSqr = dot(lightDifference, lightDifference);
    lightDirection = lightDifference * inversesqrt(lightDistanceSqr);

    return (1 / ( 1.0 + lightAttenuation * lightDistanceSqr));
}

uniform mat4 perspective;

vec4 ComputeLighting(in vec3 cameraSpacePosition, in vec3 cameraSpaceNormal)
{
    vec3 lightDir;
    vec4 light;
    if(cameraSpaceLightPos.w == 0.0)
    {
        lightDir = vec3(cameraSpaceLightPos);
        light = lightIntensity;
    }
    else
    {
        float atten = CalcAttenuation(cameraSpacePosition, cameraSpaceLightPos.xyz, lightDir);
        light = atten * lightIntensity;
    }

    vec3 surfaceNormal = normalize(cameraSpaceNormal);
    float cosAngIncidence = dot(surfaceNormal, lightDir);
    cosAngIncidence = cosAngIncidence < 0.0001 ? 0.0 : cosAngIncidence;

    vec3 viewDirection = normalize(-cameraSpacePosition);

    vec3 halfAngle = normalize(lightDir + viewDirection);
    float angleNormalHalf = acos(dot(halfAngle, surfaceNormal));
    float exponent = angleNormalHalf / specularShininess;
    exponent = -(exponent * exponent);
    float gaussianTerm = exp(exponent);

    gaussianTerm = cosAngIncidence != 0.0 ? gaussianTerm : 0.0;

    vec4 lighting = diffuseColor * light * cosAngIncidence;
    lighting += specularColor * light * gaussianTerm;

    return lighting;
}

void Impostor(out vec3 cameraPos, out vec3 cameraNormal)
{
    vec3 cameraPlanePos = vec3(mapping * sphereRadius, 0.0) + cameraSpherePos;
    vec3 rayDirection = normalize(cameraPlanePos);

    float B = 2.0 * dot(rayDirection, -cameraSpherePos);
    float C = dot(cameraSpherePos, cameraSpherePos) -
    (sphereRadius * sphereRadius);

    float det = (B * B) - (4 * C);
    if(det < 0.0)
    discard;

    float sqrtDet = sqrt(det);
    float posT = (-B + sqrtDet)/2;
    float negT = (-B - sqrtDet)/2;

    float intersectT = min(posT, negT);
    cameraPos = rayDirection * intersectT;
    cameraNormal = normalize(cameraPos - cameraSpherePos);
}

void main()
{
    vec3 cameraPos;
    vec3 cameraNormal;

    Impostor(cameraPos, cameraNormal);

    //Set the depth based on the new cameraPos.
    vec4 clipPos = perspective * vec4(cameraPos, 1.0);
    float ndcDepth = clipPos.z / clipPos.w;
    gl_FragDepth = ((gl_DepthRange.diff * ndcDepth) + gl_DepthRange.near + gl_DepthRange.far) / 2.0;

    vec4 accumLighting = diffuseColor * ambientIntensity;
    for(int light = 0; light < 1; light++)
    {
        accumLighting += ComputeLighting(cameraPos, cameraNormal);
    }

    outputColor = sqrt(accumLighting); //2.0 gamma correction
}