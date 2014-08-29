/// This Shader file is a compilation of sshaders creates by Shawn Matthews for use
/// in any project that uses deferred rendering. Copyright © 2014 to Shawn Matthews
/// Please don't steal code... 

// The texture to store the screen texture under
texture ScreenTexture;
 
// Our sampler for the texture, which is just going to be pretty simple
sampler TextureSampler = sampler_state
{
    Texture = <ScreenTexture>;
};

float blurDistance;
float edgeEpsilon;
extern float noiseEpsilon = 0.5;
extern float yeOldeEpsilon = 0.95;

float seed;

float rand_1_05(in float2 uv)
{
    float2 noise = (frac(sin(dot(uv ,float2(12.9898,78.233)*2.0)) * 43758.5453));
    return abs(noise.x + noise.y) * 0.5;
}
 
//------------------------ PIXEL SHADER ----------------------------------------
// This pixel shader will simply look up the color of the texture at the
// requested point, and turns it into a shade of gray
float4 GrayScalePS(float4 position : SV_Position, float4 color : COLOR0, float2 texCoord : TEXCOORD0) : COLOR
{
    float4 sColor = tex2D(TextureSampler, texCoord);
 
    float value = (sColor.r + sColor.g + sColor.b) / 3; 
    sColor.r = value;
    sColor.g = value;
    sColor.b = value;
 
    return sColor;
}

float4 BlackAndWhitePS(float4 position : SV_Position, float4 color : COLOR0, float2 texCoord : TEXCOORD0) : COLOR
{
    float4 sColor = tex2D(TextureSampler, texCoord);
 
    float value = (sColor.r + sColor.g + sColor.b) / 3;

    if (value > 0.5f)
    { 
        sColor.r = 1;
        sColor.g = 1;
        sColor.b = 1;
    }
    else
    {
        sColor.r = 0;
        sColor.g = 0;
        sColor.b = 0;
    }
 
    return sColor;
}

float4 SepiaPS(float4 position : SV_Position, float4 color : COLOR0, float2 texCoord : TEXCOORD0) : COLOR
{
    float4 sColor = tex2D(TextureSampler, texCoord);
 
    float4 outputColor = sColor;
    outputColor.r = (sColor.r * 0.393) + (sColor.g * 0.769) + (sColor.b * 0.189);
    outputColor.g = (sColor.r * 0.349) + (sColor.g * 0.686) + (sColor.b * 0.168);    
    outputColor.b = (sColor.r * 0.272) + (sColor.g * 0.534) + (sColor.b * 0.131);
 
    return outputColor;
}

float4 SimplePS(float4 position : SV_Position, float4 color : COLOR0, float2 texCoord : TEXCOORD0) : COLOR
{
    float4 sColor = tex2D(TextureSampler, texCoord);
    return sColor;
}

float4 SimpleBlurPS(float4 position : SV_Position, float4 color : COLOR0, float2 texCoord : TEXCOORD0) : COLOR
{
    float4 color1 = tex2D(TextureSampler, texCoord + float2(0, blurDistance));
    float4 color2 = tex2D(TextureSampler, texCoord + float2(-blurDistance,0));
    float4 color3 = tex2D(TextureSampler, texCoord + float2(blurDistance,0));
    float4 color4 = tex2D(TextureSampler, texCoord + float2(0, -blurDistance));

    return (color1 + color2 + color3 + color4) / 4;
}

float4 SimpleEdgePS(float4 position : SV_Position, float4 color : COLOR0, float2 texCoord : TEXCOORD0) : COLOR
{
    float4 color0 = tex2D(TextureSampler, texCoord);
    float4 color1 = tex2D(TextureSampler, texCoord + float2(0, 0.001));
    float4 color2 = tex2D(TextureSampler, texCoord + float2(-0.001,0));
    float4 color3 = tex2D(TextureSampler, texCoord + float2(0.001,0));
    float4 color4 = tex2D(TextureSampler, texCoord + float2(0, -0.001));

    float4 avg = (color1 + color2 + color3 + color4) / 4;

    if (length(avg - color0) > edgeEpsilon)
        return float4(0,0,0,color0.a);
    else
        return color0;
}

float4 SobelPS(float4 position : SV_Position, float4 color : COLOR0, float2 texCoord : TEXCOORD0) : COLOR
{
    float4 color0 = tex2D(TextureSampler, texCoord);
    float4 color1 = tex2D(TextureSampler, texCoord + float2(0, 0.001));
    float4 color2 = tex2D(TextureSampler, texCoord + float2(-0.001,0));
    float4 color3 = tex2D(TextureSampler, texCoord + float2(0.001,0));
    float4 color4 = tex2D(TextureSampler, texCoord + float2(0, -0.001));

    float4 avg = (color1 + color2 + color3 + color4) / 4;

    if (length(avg - color0) > edgeEpsilon)
        return float4(0,0,0,color0.a);
    else
        return float4(1,1,1,color0.a);
}

float4 YeOldePS(float4 position : SV_Position, float4 color : COLOR0, float2 texCoord : TEXCOORD0) : COLOR
{
    float rand = rand_1_05(texCoord * seed);

	if (rand > yeOldeEpsilon)
		return float4(0,0,0,1);
	else
	{	
		float4 sColor = tex2D(TextureSampler, texCoord);
 
		float4 outputColor = sColor;
		outputColor.r = (sColor.r * 0.393) + (sColor.g * 0.769) + (sColor.b * 0.189);
		outputColor.g = (sColor.r * 0.349) + (sColor.g * 0.686) + (sColor.b * 0.168);    
		outputColor.b = (sColor.r * 0.272) + (sColor.g * 0.534) + (sColor.b * 0.131);
 
		return outputColor;
	}
}

float4 NoisePS(float4 position : SV_Position, float4 color : COLOR0, float2 texCoord : TEXCOORD0) : COLOR
{
    float rand = rand_1_05(texCoord * seed);

	if (rand > noiseEpsilon)
		return float4(0,0,0,1);
	else
	{	
		float4 sColor = tex2D(TextureSampler, texCoord);
		return sColor;
	}
}

float4 SinCityPS(float4 position : SV_Position, float4 color : COLOR0, float2 texCoord : TEXCOORD0) : COLOR
{

        float3 colorin, colorout;
        colorin = tex2D( TextureSampler , texCoord.xy).rgb;
        //convert to linear space, probably not necessary since I totally destroy
        //the color space later anyway
        colorin = pow(colorin, .45f);
        
        //initialize the black&white color to the average color value,
        //not 100% correct but works decently
        float3 bwcolor = dot(colorin.rgb, 1.f.xxx) * 0.33333f;
              
        //calculate the weight of the red color. smoothstep is a function that
        //creates a quadratic interpolation between 0 and 1 using the first
        //arg as the min value, the second as max value and third as interpolation
        //value. Result is clamped to be [0, 1]
        //I just picked these constants because they looked decent in the image
        float weight = smoothstep(0.1f, 0.25f, colorin.r - bwcolor);

        //Mess with the color space of the b&w. Again, arbitrary constants
        bwcolor = pow(bwcolor * 1.1f, 2.f);
        //interpolate between black&white and red based on the weight
        colorout = lerp(bwcolor, colorin * float3(1.1f, 0.5f, 0.5f), weight);

         //pow: convert back to gamma space
        return pow(float4(colorout, 1.f), 2.2f);
}

float4 ToonPS(float4 position : SV_Position, float4 color : COLOR0, float2 texCoord : TEXCOORD0) : COLOR
{
	float4 colorIn = tex2D(TextureSampler, texCoord);

    return round(colorIn * 8) / 4;
}
 
//-------------------------- TECHNIQUES ----------------------------------------
// This technique is pretty simple - only one pass, and only a pixel shader
technique Simple
{
    pass Pass1
    {
        PixelShader = compile ps_4_0 SimplePS();
    }
}

technique YeOlde
{
	pass Pass1
	{
		PixelShader = compile ps_4_0 SepiaPS();
	}

	pass Pass2
	{
		PixelShader = compile ps_4_0 NoisePS();
	}
}

technique Noise
{
	pass Pass1
	{
		PixelShader = compile ps_4_0 NoisePS();
	}
}

technique BlackAndWhite
{
    pass Pass1
    {
        PixelShader = compile ps_4_0 BlackAndWhitePS();
    }
}

technique GrayScale
{
    pass Pass1
    {
        PixelShader = compile ps_4_0 GrayScalePS();
    }
}

technique Sepia
{
	pass Pass1
	{
		PixelShader = compile ps_4_0 SepiaPS();
	}
}

technique SimpleBlur
{
	pass Pass1
	{
		PixelShader = compile ps_4_0 SimpleBlurPS();
	}
}

technique SimpleEdge
{
	pass Pass1
	{
		PixelShader = compile ps_4_0 SimpleEdgePS();
	}
}

technique Sobel
{
    pass Pass1
    {
        PixelShader = compile ps_4_0 SobelPS();
    }
}

technique Toon
{
    pass Pass1
    {
        PixelShader = compile ps_4_0 ToonPS();
    }

	pass Pass2
	{
		PixelShader = compile ps_4_0 SimpleEdgePS();
	}
}

technique SinCity
{
    pass Pass1
    {
        PixelShader = compile ps_4_0 SinCityPS();
    }
}