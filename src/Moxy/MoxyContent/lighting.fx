texture lightMask;
sampler mainSampler : register(s0);
sampler lightSampler = sampler_state{Texture = lightMask;};

struct PixelShaderInput
{
    float4 TextureCoords: TEXCOORD0;
};

float4 PixelShaderFunction(PixelShaderInput input) : COLOR0
{
        float2 texCoord = input.TextureCoords;

        float4 mainColor = tex2D(mainSampler, texCoord);
        float4 lightColor = tex2D(lightSampler, texCoord);

    return mainColor * lightColor;
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}