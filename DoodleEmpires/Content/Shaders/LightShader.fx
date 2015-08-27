/// This Shader file is a compilation of sshaders creates by Shawn Matthews for use
/// in any project that uses deferred rendering. Copyright © 2014 to Shawn Matthews
/// Please don't steal code... 

// The texture to store the screen texture under
texture ScreenTexture;
texture LightTexture;
texture BackgroundTexture;

float BackgroundBleed;

// Our sampler for the texture, which is just going to be pretty simple
sampler TextureSampler = sampler_state
{
	Texture = <ScreenTexture>;
};
// Our sampler for the texture, which is just going to be pretty simple
sampler LightSampler = sampler_state
{
	Texture = <LightTexture>;
};
// Our sampler for the texture, which is just going to be pretty simple
sampler BackgroundSampler = sampler_state
{
	Texture = <BackgroundTexture>;
};

//------------------------ PIXEL SHADER ----------------------------------------
float4 LightingPS(float4 position : SV_Position, float4 color : COLOR0, float2 texCoord : TEXCOORD0) : COLOR
{
	float4 backColor = tex2D(BackgroundSampler, texCoord) * BackgroundBleed;
	float4 texColor = tex2D(TextureSampler, texCoord);

	if (texColor.a == 0)
	{
	    return backColor;
	}
	else
	{
		float4 lightColor = tex2D(LightSampler, texCoord);
		return saturate(lightColor * backColor);
	}
}

//-------------------------- TECHNIQUES ----------------------------------------
// This technique is pretty simple - only one pass, and only a pixel shader
technique Lighting
{
	pass Pass1
	{
		PixelShader = compile ps_3_0 LightingPS();
	}
}