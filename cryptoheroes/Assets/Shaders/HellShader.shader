//////////////////////////////////////////////////////////////
/// Shadero Sprite: Sprite Shader Editor - by VETASOFT 2020 //
/// Shader generate with Shadero 1.9.9                      //
/// http://u3d.as/V7t #AssetStore                           //
/// http://www.shadero.com #Docs                            //
//////////////////////////////////////////////////////////////

Shader "Shadero Customs/HellShader"
{
Properties
{
[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
AnimatedOffsetUV_X_1("AnimatedOffsetUV_X_1", Range(-1, 1)) = 0.1
AnimatedOffsetUV_Y_1("AnimatedOffsetUV_Y_1", Range(-1, 1)) = 0
AnimatedOffsetUV_ZoomX_1("AnimatedOffsetUV_ZoomX_1", Range(1, 10)) = 1
AnimatedOffsetUV_ZoomY_1("AnimatedOffsetUV_ZoomY_1", Range(1, 10)) = 1
AnimatedOffsetUV_Speed_1("AnimatedOffsetUV_Speed_1", Range(-1, 1)) = 0.2
OffsetUV_X_1("OffsetUV_X_1", Range(-1, 1)) = 0
OffsetUV_Y_1("OffsetUV_Y_1", Range(-1, 1)) = 0.5
OffsetUV_ZoomX_1("OffsetUV_ZoomX_1", Range(0.1, 10)) = 10
OffsetUV_ZoomY_1("OffsetUV_ZoomY_1", Range(0.1, 10)) = 2
PixelXYUV_SizeX_1("PixelXYUV_SizeX_1", Range(1, 128)) = 128
PixelXYUV_SizeY_1("PixelXYUV_SizeY_1", Range(1, 128)) = 128
_Generate_Fire_PosX_1("_Generate_Fire_PosX_1", Range(-1, 2)) = 0
_Generate_Fire_PosY_1("_Generate_Fire_PosY_1", Range(-1, 2)) = -0.727
_Generate_Fire_Precision_1("_Generate_Fire_Precision_1", Range(0, 1)) = 0.1
_Generate_Fire_Smooth_1("_Generate_Fire_Smooth_1", Range(0, 1)) = 0.923
_Generate_Fire_Speed_1("_Generate_Fire_Speed_1", Range(-2, 2)) = 1
RotationUV_Rotation_1("RotationUV_Rotation_1", Range(-360, 360)) = 90
RotationUV_Rotation_PosX_1("RotationUV_Rotation_PosX_1", Range(-1, 2)) = 0
RotationUV_Rotation_PosY_1("RotationUV_Rotation_PosY_1", Range(-1, 2)) =0.26
RotationUV_Rotation_Speed_1("RotationUV_Rotation_Speed_1", Range(-8, 8)) =0
_ColorGradients_Color1_1("_ColorGradients_Color1_1", COLOR) = (0,0,0.01960784,1)
_ColorGradients_Color2_1("_ColorGradients_Color2_1", COLOR) = (0,0,0.01960784,1)
_ColorGradients_Color3_1("_ColorGradients_Color3_1", COLOR) = (1,1,1,1)
_ColorGradients_Color4_1("_ColorGradients_Color4_1", COLOR) = (1,1,1,1)
_MaskRGBA_Fade_1("_MaskRGBA_Fade_1", Range(0, 1)) = 0
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
float AnimatedOffsetUV_X_1;
float AnimatedOffsetUV_Y_1;
float AnimatedOffsetUV_ZoomX_1;
float AnimatedOffsetUV_ZoomY_1;
float AnimatedOffsetUV_Speed_1;
float OffsetUV_X_1;
float OffsetUV_Y_1;
float OffsetUV_ZoomX_1;
float OffsetUV_ZoomY_1;
float PixelXYUV_SizeX_1;
float PixelXYUV_SizeY_1;
float _Generate_Fire_PosX_1;
float _Generate_Fire_PosY_1;
float _Generate_Fire_Precision_1;
float _Generate_Fire_Smooth_1;
float _Generate_Fire_Speed_1;
float RotationUV_Rotation_1;
float RotationUV_Rotation_PosX_1;
float RotationUV_Rotation_PosY_1;
float RotationUV_Rotation_Speed_1;
float4 _ColorGradients_Color1_1;
float4 _ColorGradients_Color2_1;
float4 _ColorGradients_Color3_1;
float4 _ColorGradients_Color4_1;
float _MaskRGBA_Fade_1;

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
float2 OffsetUV(float2 uv, float offsetx, float offsety, float zoomx, float zoomy)
{
uv += float2(offsetx, offsety);
uv = fmod(uv * float2(zoomx, zoomy), 1);
return uv;
}

float2 OffsetUVClamp(float2 uv, float offsetx, float offsety, float zoomx, float zoomy)
{
uv += float2(offsetx, offsety);
uv = fmod(clamp(uv * float2(zoomx, zoomy), 0.0001, 0.9999), 1);
return uv;
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
float4 frag (v2f i) : COLOR
{
float2 AnimatedOffsetUV_1 = AnimatedOffsetUV(i.texcoord,AnimatedOffsetUV_X_1,AnimatedOffsetUV_Y_1,AnimatedOffsetUV_ZoomX_1,AnimatedOffsetUV_ZoomY_1,AnimatedOffsetUV_Speed_1);
float2 OffsetUV_1 = OffsetUV(AnimatedOffsetUV_1,OffsetUV_X_1,OffsetUV_Y_1,OffsetUV_ZoomX_1,OffsetUV_ZoomY_1);
float2 PixelXYUV_1 = PixelXYUV(OffsetUV_1,PixelXYUV_SizeX_1,PixelXYUV_SizeY_1);
float4 _Generate_Fire_1 = Generate_Fire(PixelXYUV_1,_Generate_Fire_PosX_1,_Generate_Fire_PosY_1,_Generate_Fire_Precision_1,_Generate_Fire_Smooth_1,_Generate_Fire_Speed_1,0);
float2 RotationUV_1 = RotationUV(i.texcoord,RotationUV_Rotation_1,RotationUV_Rotation_PosX_1,RotationUV_Rotation_PosY_1,RotationUV_Rotation_Speed_1);
float4 _ColorGradients_1 = Color_Gradients(float4(0,0,0,1),RotationUV_1,_ColorGradients_Color1_1,_ColorGradients_Color2_1,_ColorGradients_Color3_1,_ColorGradients_Color4_1);
float4 MaskRGBA_1=_Generate_Fire_1;
MaskRGBA_1.a = lerp(_ColorGradients_1.r, 1 - _ColorGradients_1.r ,_MaskRGBA_Fade_1);
float4 FinalResult = MaskRGBA_1;
FinalResult.rgb *= i.color.rgb;
FinalResult.a = FinalResult.a * _SpriteFade * i.color.a;
return FinalResult;
}

ENDCG
}
}
Fallback "Sprites/Default"
}
