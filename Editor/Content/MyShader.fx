#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

matrix WorldViewProjection;

matrix World; // each matrix can be passed to one parameter
matrix View;
matrix Projection;

texture Texture;
sampler BasicTextureSampler = sampler_state
{
    texture = <Texture>;
};

struct VertexShaderInput //the types we will send to the shader
{
    float4 Position : POSITION0;
    float2 UV : TEXCOORD0;
	//float4 Color : COLOR0;
};

struct VertexShaderOutput //the types we will GET from the shader
{
    float4 Position : SV_POSITION;
	//float4 Color : COLOR0;
    float2 UV : TEXCOORD0;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
    VertexShaderOutput output = (VertexShaderOutput) 0;

    output.Position = mul(input.Position, WorldViewProjection);
    output.UV = input.UV;
	//output.Color = input.Color;

    return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
    float3 output = tex2D(BasicTextureSampler, input.UV);
    return float4(output, 1);
	//return input.Color;
}

technique BasicColorDrawing
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
};