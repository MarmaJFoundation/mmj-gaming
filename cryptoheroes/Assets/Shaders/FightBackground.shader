//////////////////////////////////////////////////////////////
/// Shadero Sprite: Sprite Shader Editor - by VETASOFT 2020 //
/// Shader generate with Shadero 1.9.9                      //
/// http://u3d.as/V7t #AssetStore                           //
/// http://www.shadero.com #Docs                            //
//////////////////////////////////////////////////////////////

Shader "Shadero Customs/FightBackground"
{
Properties
{
ColorLimit("ColorLimit", Range(1, 128)) = 15
[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
PixelXYUV_SizeX_1("PixelXYUV_SizeX_1", Range(1, 128)) = 32
PixelXYUV_SizeY_1("PixelXYUV_SizeY_1", Range(1, 128)) = 32
DistortionUV_WaveX_2("DistortionUV_WaveX_2", Range(0, 128)) = 10
DistortionUV_WaveY_2("DistortionUV_WaveY_2", Range(0, 128)) = 10
DistortionUV_DistanceX_2("DistortionUV_DistanceX_2", Range(0, 1)) = 0.3
DistortionUV_DistanceY_2("DistortionUV_DistanceY_2", Range(0, 1)) = 0.3
DistortionUV_Speed_2("DistortionUV_Speed_2", Range(-2, 2)) = 1
LiquidUV_WaveX_2("LiquidUV_WaveX_2", Range(0, 2)) = 2
LiquidUV_WaveY_2("LiquidUV_WaveY_2", Range(0, 2)) = 2
LiquidUV_DistanceX_2("LiquidUV_DistanceX_2", Range(0, 1)) = 0.3
LiquidUV_DistanceY_2("LiquidUV_DistanceY_2", Range(0, 1)) = 0.3
LiquidUV_Speed_2("LiquidUV_Speed_2", Range(-2, 2)) = 1
RotationUV_Rotation_2("RotationUV_Rotation_2", Range(-360, 360)) = 0
RotationUV_Rotation_PosX_2("RotationUV_Rotation_PosX_2", Range(-1, 2)) = 0.5
RotationUV_Rotation_PosY_2("RotationUV_Rotation_PosY_2", Range(-1, 2)) =0.5
RotationUV_Rotation_Speed_2("RotationUV_Rotation_Speed_2", Range(-8, 8)) =0
_ColorGradients_Color1_2("_ColorGradients_Color1_2", COLOR) = (0.254902,0,0.254902,1)
_ColorGradients_Color2_2("_ColorGradients_Color2_2", COLOR) = (0.254902,0,0.254902,1)
_ColorGradients_Color3_2("_ColorGradients_Color3_2", COLOR) = (0.6666667,0.07843138,0.2352941,1)
_ColorGradients_Color4_2("_ColorGradients_Color4_2", COLOR) = (0.6666667,0.07843138,0.2352941,1)
AnimatedOffsetUV_X_1("AnimatedOffsetUV_X_1", Range(-1, 1)) = 1
AnimatedOffsetUV_Y_1("AnimatedOffsetUV_Y_1", Range(-1, 1)) = 0
AnimatedOffsetUV_ZoomX_1("AnimatedOffsetUV_ZoomX_1", Range(1, 10)) = 1
AnimatedOffsetUV_ZoomY_1("AnimatedOffsetUV_ZoomY_1", Range(1, 10)) = 1
AnimatedOffsetUV_Speed_1("AnimatedOffsetUV_Speed_1", Range(-1, 1)) = 1
DistortionUV_WaveX_1("DistortionUV_WaveX_1", Range(0, 128)) = 34.677
DistortionUV_WaveY_1("DistortionUV_WaveY_1", Range(0, 128)) = 30.946
DistortionUV_DistanceX_1("DistortionUV_DistanceX_1", Range(0, 1)) = 0.284
DistortionUV_DistanceY_1("DistortionUV_DistanceY_1", Range(0, 1)) = 0.317
DistortionUV_Speed_1("DistortionUV_Speed_1", Range(-2, 2)) = -0.091
LiquidUV_WaveX_1("LiquidUV_WaveX_1", Range(0, 2)) = 1.325
LiquidUV_WaveY_1("LiquidUV_WaveY_1", Range(0, 2)) = 1.273
LiquidUV_DistanceX_1("LiquidUV_DistanceX_1", Range(0, 1)) = 0.227
LiquidUV_DistanceY_1("LiquidUV_DistanceY_1", Range(0, 1)) = 0.245
LiquidUV_Speed_1("LiquidUV_Speed_1", Range(-2, 2)) = -2
RotationUV_Rotation_1("RotationUV_Rotation_1", Range(-360, 360)) = 90
RotationUV_Rotation_PosX_1("RotationUV_Rotation_PosX_1", Range(-1, 2)) = 0.5
RotationUV_Rotation_PosY_1("RotationUV_Rotation_PosY_1", Range(-1, 2)) =0.5
RotationUV_Rotation_Speed_1("RotationUV_Rotation_Speed_1", Range(-8, 8)) =0
_Generate_Fire_PosX_1("_Generate_Fire_PosX_1", Range(-1, 2)) = 1
_Generate_Fire_PosY_1("_Generate_Fire_PosY_1", Range(-1, 2)) = 2
_Generate_Fire_Precision_1("_Generate_Fire_Precision_1", Range(0, 1)) = 0
_Generate_Fire_Smooth_1("_Generate_Fire_Smooth_1", Range(0, 1)) = 0
_Generate_Fire_Speed_1("_Generate_Fire_Speed_1", Range(-2, 2)) = 1
_ColorGradients_Color1_1("_ColorGradients_Color1_1", COLOR) = (0.254902,0,0.254902,1)
_ColorGradients_Color2_1("_ColorGradients_Color2_1", COLOR) = (0.4901961,0,0.254902,1)
_ColorGradients_Color3_1("_ColorGradients_Color3_1", COLOR) = (0.6666667,0.07843138,0.2352941,1)
_ColorGradients_Color4_1("_ColorGradients_Color4_1", COLOR) = (0.9411765,0.4117647,0.1372549,1)
_OperationBlend_Fade_1("_OperationBlend_Fade_1", Range(0, 1)) = 1
_ColorHSV_Hue_1("_ColorHSV_Hue_1", Range(0, 360)) = 180
_ColorHSV_Saturation_1("_ColorHSV_Saturation_1", Range(0, 2)) = 1
_ColorHSV_Brightness_1("_ColorHSV_Brightness_1", Range(0, 2)) = 1
_SpriteFade("SpriteFade", Range(0, 1)) = 1.0

// required for UI.Mask
[HideInInspector]_StencilComp("Stencil Comparison", Float) = 8
[HideInInspector]_Stencil("Stencil ID", Float) = 0
[HideInInspector]_StencilOp("Stencil Operation", Float) = 0
[HideInInspector]_StencilWriteMask("Stencil Write Mask", Float) = 255
[HideInInspector]_StencilReadMask("Stencil Read Mask", Float) = 255
[HideInInspector]_ColorMask("Color Mask", Float) = 15

}

SubShader
{

Tags {"Queue" = "Transparent" "IgnoreProjector" = "true" "RenderType" = "Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="True" }
ZWrite Off Blend SrcAlpha OneMinusSrcAlpha Cull Off 

// required for UI.Mask
Stencil
{
Ref [_Stencil]
Comp [_StencilComp]
Pass [_StencilOp]
ReadMask [_StencilReadMask]
WriteMask [_StencilWriteMask]
}

Pass
{

CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma fragmentoption ARB_precision_hint_fastest
#include "UnityCG.cginc"

struct appdata_t{
float4 vertex   : POSITION;
float4 color    : COLOR;
float2 texcoord : TEXCOORD0;
};

struct v2f
{
float2 texcoord  : TEXCOORD0;
float4 vertex   : SV_POSITION;
float4 color    : COLOR;
};

sampler2D _MainTex;
float _SpriteFade;
float PixelXYUV_SizeX_1;
float PixelXYUV_SizeY_1;
float DistortionUV_WaveX_2;
float DistortionUV_WaveY_2;
float DistortionUV_DistanceX_2;
float DistortionUV_DistanceY_2;
float DistortionUV_Speed_2;
float LiquidUV_WaveX_2;
float LiquidUV_WaveY_2;
float LiquidUV_DistanceX_2;
float LiquidUV_DistanceY_2;
float LiquidUV_Speed_2;
float RotationUV_Rotation_2;
float RotationUV_Rotation_PosX_2;
float RotationUV_Rotation_PosY_2;
float RotationUV_Rotation_Speed_2;
float4 _ColorGradients_Color1_2;
float4 _ColorGradients_Color2_2;
float4 _ColorGradients_Color3_2;
float4 _ColorGradients_Color4_2;
float AnimatedOffsetUV_X_1;
float AnimatedOffsetUV_Y_1;
float AnimatedOffsetUV_ZoomX_1;
float AnimatedOffsetUV_ZoomY_1;
float AnimatedOffsetUV_Speed_1;
float DistortionUV_WaveX_1;
float DistortionUV_WaveY_1;
float DistortionUV_DistanceX_1;
float DistortionUV_DistanceY_1;
float DistortionUV_Speed_1;
float LiquidUV_WaveX_1;
float LiquidUV_WaveY_1;
float LiquidUV_DistanceX_1;
float LiquidUV_DistanceY_1;
float LiquidUV_Speed_1;
float RotationUV_Rotation_1;
float RotationUV_Rotation_PosX_1;
float RotationUV_Rotation_PosY_1;
float RotationUV_Rotation_Speed_1;
float _Generate_Fire_PosX_1;
float _Generate_Fire_PosY_1;
float _Generate_Fire_Precision_1;
float _Generate_Fire_Smooth_1;
float _Generate_Fire_Speed_1;
float4 _ColorGradients_Color1_1;
float4 _ColorGradients_Color2_1;
float4 _ColorGradients_Color3_1;
float4 _ColorGradients_Color4_1;
float _OperationBlend_Fade_1;
float _ColorHSV_Hue_1;
float _ColorHSV_Saturation_1;
float _ColorHSV_Brightness_1;
float ColorLimit;

v2f vert(appdata_t IN)
{
v2f OUT;
OUT.vertex = UnityObjectToClipPos(IN.vertex);
OUT.texcoord = IN.texcoord;
OUT.color = IN.color;
return OUT;
}


float2 AnimatedOffsetUV(float2 uv, float offsetx, float offsety, float zoomx, float zoomy, float speed)
{
speed *=_Time*25;
uv += float2(offsetx*speed, offsety*speed);
uv = fmod(uv * float2(zoomx, zoomy), 1);
return uv;
}
float2 DistortionUV(float2 p, float WaveX, float WaveY, float DistanceX, float DistanceY, float Speed)
{
Speed *=_Time*100;
p.x= p.x+sin(p.y*WaveX + Speed)*DistanceX*0.05;
p.y= p.y+cos(p.x*WaveY + Speed)*DistanceY*0.05;
return p;
}
float2 RotationUV(float2 uv, float rot, float posx, float posy, float speed)
{
rot=rot+(_Time*speed*360);
uv = uv - float2(posx, posy);
float angle = rot * 0.01744444;
float sinX = sin(angle);
float cosX = cos(angle);
float2x2 rotationMatrix = float2x2(cosX, -sinX, sinX, cosX);
uv = mul(uv, rotationMatrix) + float2(posx, posy);
return uv;
}
float2 PixelXYUV(float2 uv, float x, float y)
{
float2 pos = float2(x, y);
uv = floor(uv * pos+0.5) / pos;
return uv;
}
float4 ColorHSV(float4 RGBA, float HueShift, float Sat, float Val)
{

float4 RESULT = float4(RGBA);
float a1 = Val*Sat;
float a2 = HueShift*3.14159265 / 180;
float VSU = a1*cos(a2);
float VSW = a1*sin(a2);

RESULT.x = (.299*Val + .701*VSU + .168*VSW)*RGBA.x
+ (.587*Val - .587*VSU + .330*VSW)*RGBA.y
+ (.114*Val - .114*VSU - .497*VSW)*RGBA.z;

RESULT.y = (.299*Val - .299*VSU - .328*VSW)*RGBA.x
+ (.587*Val + .413*VSU + .035*VSW)*RGBA.y
+ (.114*Val - .114*VSU + .292*VSW)*RGBA.z;

RESULT.z = (.299*Val - .3*VSU + 1.25*VSW)*RGBA.x
+ (.587*Val - .588*VSU - 1.05*VSW)*RGBA.y
+ (.114*Val + .886*VSU - .203*VSW)*RGBA.z;

return RESULT;
}
float4 OperationBlend(float4 origin, float4 overlay, float blend)
{
float4 o = origin; 
o.a = overlay.a + origin.a * (1 - overlay.a);
o.rgb = (overlay.rgb * overlay.a + origin.rgb * origin.a * (1 - overlay.a)) * (o.a+0.0000001);
o.a = saturate(o.a);
o = lerp(origin, o, blend);
return o;
}
float Generate_Fire_hash2D(float2 x)
{
return frac(sin(dot(x, float2(13.454, 7.405)))*12.3043);
}

float Generate_Fire_voronoi2D(float2 uv, float precision)
{
float2 fl = floor(uv);
float2 fr = frac(uv);
float res = 1.0;
for (int j = -1; j <= 1; j++)
{
for (int i = -1; i <= 1; i++)
{
float2 p = float2(i, j);
float h = Generate_Fire_hash2D(fl + p);
float2 vp = p - fr + h;
float d = dot(vp, vp);
res += 1.0 / pow(d, 8.0);
}
}
return pow(1.0 / res, precision);
}

float4 Generate_Fire(float2 uv, float posX, float posY, float precision, float smooth, float speed, float black)
{
uv += float2(posX, posY);
float t = _Time*60*speed;
float up0 = Generate_Fire_voronoi2D(uv * float2(6.0, 4.0) + float2(0, -t), precision);
float up1 = 0.5 + Generate_Fire_voronoi2D(uv * float2(6.0, 4.0) + float2(42, -t ) + 30.0, precision);
float finalMask = up0 * up1  + (1.0 - uv.y);
finalMask += (1.0 - uv.y)* 0.5;
finalMask *= 0.7 - abs(uv.x - 0.5);
float4 result = smoothstep(smooth, 0.95, finalMask);
result.a = saturate(result.a + black);
return result;
}
float4 Color_Gradients(float4 txt, float2 uv, float4 col1, float4 col2, float4 col3, float4 col4)
{
float4 c1 = lerp(col1, col2, smoothstep(0., 0.33, uv.x));
c1 = lerp(c1, col3, smoothstep(0.33, 0.66, uv.x));
c1 = lerp(c1, col4, smoothstep(0.66, 1, uv.x));
c1.a = txt.a;
return c1;
}
float4 TurnC64(float4 txt, float value)
{
float3 a = float3(0, 0, 0);
#define Turn(n) a = lerp(n, a, step (length (a-txt.rgb), length (n-txt.rgb)));	
Turn(float3(0, 0, 0));
Turn(float3(1, 1, 1));
Turn(float3(116, 67, 53) / 256);
Turn(float3(124, 172, 186) / 256);
Turn(float3(123, 72, 144) / 256);
Turn(float3(100, 151, 79) / 256);
Turn(float3(64, 50, 133) / 256);
Turn(float3(191, 205, 122) / 256);
Turn(float3(123, 91, 47) / 256);
Turn(float3(79, 69, 0) / 256);
Turn(float3(163, 114, 101) / 256);
Turn(float3(80, 80, 80) / 256);
Turn(float3(120, 120, 120) / 256);
Turn(float3(164, 215, 142) / 256);
Turn(float3(120, 106, 189) / 256);
Turn(float3(159, 159, 150) / 256);
a = lerp(txt.rgb,a,value);
return float4(a, txt.a);
}
float2 LiquidUV(float2 p, float WaveX, float WaveY, float DistanceX, float DistanceY, float Speed)
{ Speed *= _Time * 100;
float x = sin(p.y * 4 * WaveX + Speed);
float y = cos(p.x * 4 * WaveY + Speed);
x += sin(p.x)*0.1;
y += cos(p.y)*0.1;
x *= y;
y *= x;
x *= y + WaveY*8;
y *= x + WaveX*8;
p.x = p.x + x * DistanceX * 0.015;
p.y = p.y + y * DistanceY * 0.015;

return p;
}
float4 frag (v2f i) : COLOR
{
float2 PixelXYUV_1 = PixelXYUV(i.texcoord,PixelXYUV_SizeX_1,PixelXYUV_SizeY_1);
float2 DistortionUV_2 = DistortionUV(PixelXYUV_1,DistortionUV_WaveX_2,DistortionUV_WaveY_2,DistortionUV_DistanceX_2,DistortionUV_DistanceY_2,DistortionUV_Speed_2);
float2 LiquidUV_2 = LiquidUV(DistortionUV_2,LiquidUV_WaveX_2,LiquidUV_WaveY_2,LiquidUV_DistanceX_2,LiquidUV_DistanceY_2,LiquidUV_Speed_2);
float2 RotationUV_2 = RotationUV(LiquidUV_2,RotationUV_Rotation_2,RotationUV_Rotation_PosX_2,RotationUV_Rotation_PosY_2,RotationUV_Rotation_Speed_2);
float4 _ColorGradients_2 = Color_Gradients(float4(0,0,0,1),RotationUV_2,_ColorGradients_Color1_2,_ColorGradients_Color2_2,_ColorGradients_Color3_2,_ColorGradients_Color4_2);
float2 AnimatedOffsetUV_1 = AnimatedOffsetUV(PixelXYUV_1,AnimatedOffsetUV_X_1,AnimatedOffsetUV_Y_1,AnimatedOffsetUV_ZoomX_1,AnimatedOffsetUV_ZoomY_1,AnimatedOffsetUV_Speed_1);
float2 DistortionUV_1 = DistortionUV(AnimatedOffsetUV_1,DistortionUV_WaveX_1,DistortionUV_WaveY_1,DistortionUV_DistanceX_1,DistortionUV_DistanceY_1,DistortionUV_Speed_1);
float2 LiquidUV_1 = LiquidUV(DistortionUV_1,LiquidUV_WaveX_1,LiquidUV_WaveY_1,LiquidUV_DistanceX_1,LiquidUV_DistanceY_1,LiquidUV_Speed_1);
float2 RotationUV_1 = RotationUV(LiquidUV_1,RotationUV_Rotation_1,RotationUV_Rotation_PosX_1,RotationUV_Rotation_PosY_1,RotationUV_Rotation_Speed_1);
float4 _Generate_Fire_1 = Generate_Fire(LiquidUV_1,_Generate_Fire_PosX_1,_Generate_Fire_PosY_1,_Generate_Fire_Precision_1,_Generate_Fire_Smooth_1,_Generate_Fire_Speed_1,0);
float4 _ColorGradients_1 = Color_Gradients(_Generate_Fire_1,RotationUV_1,_ColorGradients_Color1_1,_ColorGradients_Color2_1,_ColorGradients_Color3_1,_ColorGradients_Color4_1);
float4 OperationBlend_1 = OperationBlend(_ColorGradients_2, _ColorGradients_1, _OperationBlend_Fade_1); 
float4 _ColorHSV_1 = ColorHSV(OperationBlend_1,_ColorHSV_Hue_1,_ColorHSV_Saturation_1,_ColorHSV_Brightness_1);
float4 FinalResult = _ColorHSV_1;
FinalResult.rgb *= i.color.rgb;
FinalResult.a = FinalResult.a * _SpriteFade * i.color.a;
FinalResult.rgb = round(FinalResult.rgb * ColorLimit) / ColorLimit;
return FinalResult;
}

ENDCG
}
}
Fallback "Sprites/Default"
}
