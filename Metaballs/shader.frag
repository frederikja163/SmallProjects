#version 450 core
#define BallCount 16
in vec2 fPosition;

out vec4 Color;

uniform vec2 uBalls[BallCount];

vec3 hueShift(vec3 color, float hueAdjust);

void main()
{
    float sum = 0;
    for (int i = 0; i < BallCount; i++)
    {
        vec2 distVec = fPosition - uBalls[i];
        float dist = distVec.x * distVec.x + distVec.y * distVec.y;
        sum += 0.1f / dist;
    }
    Color = vec4(hueShift(vec3(1, 0, 0), 6 - clamp(sum, 0, 6)), 1);
}

// Src: https://gist.github.com/mairod/a75e7b44f68110e1576d77419d608786
vec3 hueShift( vec3 color, float hueAdjust ){

    const vec3  kRGBToYPrime = vec3 (0.299, 0.587, 0.114);
    const vec3  kRGBToI      = vec3 (0.596, -0.275, -0.321);
    const vec3  kRGBToQ      = vec3 (0.212, -0.523, 0.311);

    const vec3  kYIQToR     = vec3 (1.0, 0.956, 0.621);
    const vec3  kYIQToG     = vec3 (1.0, -0.272, -0.647);
    const vec3  kYIQToB     = vec3 (1.0, -1.107, 1.704);

    float   YPrime  = dot (color, kRGBToYPrime);
    float   I       = dot (color, kRGBToI);
    float   Q       = dot (color, kRGBToQ);
    float   hue     = atan (Q, I);
    float   chroma  = sqrt (I * I + Q * Q);

    hue += hueAdjust;

    Q = chroma * sin (hue);
    I = chroma * cos (hue);

    vec3    yIQ   = vec3 (YPrime, I, Q);

    return vec3( dot (yIQ, kYIQToR), dot (yIQ, kYIQToG), dot (yIQ, kYIQToB) );

}