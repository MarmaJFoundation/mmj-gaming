//////////////////////////////////////////////////////////////
/// Shadero Sprite: Sprite Shader Editor - by VETASOFT 2020 //
/// Shader generate with Shadero 1.9.9                      //
/// http://u3d.as/V7t #AssetStore                           //
/// http://www.shadero.com #Docs                            //
//////////////////////////////////////////////////////////////

Shader "Shadero Customs/HellGround"
{
Properties
{
[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
LiquidUV_WaveX_1("LiquidUV_WaveX_1", Range(0, 2)) = 1.455
LiquidUV_WaveY_1("LiquidUV_WaveY_1", Range(0, 2)) = 1.526
LiquidUV_DistanceX_1("LiquidUV_DistanceX_1", Range(0, 1)) = 0.063
LiquidUV_DistanceY_1("LiquidUV_DistanceY_1", Range(0, 1)) = 0.091
LiquidUV_Speed_1("LiquidUV_Speed_1", Range(-2, 2)) = 0.1
AnimatedOffsetUV_X_1("AnimatedOffsetUV_X_1", Range(-1, 1)) = 0.1
AnimatedOffsetUV_Y_1("AnimatedOffsetUV_Y_1", Range(-1, 1)) = 0
AnimatedOffsetUV_ZoomX_1("AnimatedOffsetUV_ZoomX_1", Range(1, 10)) = 1
AnimatedOffsetUV_ZoomY_1("AnimatedOffsetUV_ZoomY_1", Range(1, 10)) = 1
AnimatedOffsetUV_Speed_1("AnimatedOffsetUV_Speed_1", Range(-1, 1)) = 0.05
RotationUV_Rotation_1("RotationUV_Rotation_1", Range(-360, 360)) = 45
RotationUV_Rotation_PosX_1("RotationUV_Rotation_PosX_1", Range(-1, 2)) = 0.5
RotationUV_Rotation_PosY_1("RotationUV_Rotation_PosY_1", Range(-1, 2)) =0.5
RotationUV_Rotation_Speed_1("RotationUV_Rotation_Speed_1", Range(-8, 8)) =0
_Generate_Checker_PosX_1("_Generate_Checker_PosX_1", Range(0, 1)) = 0.5
_Generate_Checker_PosY_1("_Generate_Checker_PosY_1", Range(0, 1)) = 0.5
_Generate_Checker_Size_1("_Generate_Checker_Size_1", Range(1, 32)) = 32
RotationUV_Rotation_2("RotationUV_Rotation_2", Range(-360, 360)) = 90
RotationUV_Rotation_PosX_2("RotationUV_Rotation_PosX_2", Range(-1, 2)) = 0.5
RotationUV_Rotation_PosY_2("RotationUV_Rotation_PosY_2", Range(-1, 2)) =0.5
RotationUV_Rotation_Speed_2("RotationUV_Rotation_Speed_2", Range(-8, 8)) =0
_ColorGradients_Color1_1("_ColorGradients_Color1_1", COLOR) = (0,0,0,1)
_ColorGradients_Color2_1("_ColorGradients_Color2_1", COLOR) = (1,1,1,1)
_ColorGradients_Color3_1("_ColorGradients_Color3_1", COLOR) = (1,1,1,1)
_ColorGradients_Color4_1("_ColorGradients_Color4_1", COLOR) = (0,0,0,1)
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
float LiquidUV_WaveX_1;
float LiquidUV_WaveY_1;
float LiquidUV_DistanceX_1;
float LiquidUV_DistanceY_1;
float LiquidUV_Speed_1;
float AnimatedOffsetUV_X_1;
float AnimatedOffsetUV_Y_1;
float AnimatedOffsetUV_ZoomX_1;
float AnimatedOffsetUV_ZoomY_1;
float AnimatedOffsetUV_Speed_1;
float RotationUV_Rotation_1;
float RotationUV_Rotation_PosX_1;
float RotationUV_Rotation_PosY_1;
float RotationUV_Rotation_Speed_1;
float _Generate_Checker_PosX_1;
float _Generate_Checker_PosY_1;
float _Generate_Checker_Size_1;
float RotationUV_Rotation_2;
float RotationUV_Rotation_PosX_2;
float RotationUV_Rotation_PosY_2;
float RotationUV_Rotation_Speed_2;
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
float4 Generate_Checker(float2 uv, float posX, float posY, float Size, float black)
{
uv += float2(posX, posY);
uv = floor(Size * uv);
float r = fmod(uv.x + uv.y, 2.);
float4 result = float4(r, r, r, r);
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
float2 LiquidUV_1 = LiquidUV(i.texcoord,LiquidUV_WaveX_1,LiquidUV_WaveY_1,LiquidUV_DistanceX_1,LiquidUV_DistanceY_1,LiquidUV_Speed_1);
float2 AnimatedOffsetUV_1 = AnimatedOffsetUV(LiquidUV_1,AnimatedOffsetUV_X_1,AnimatedOffsetUV_Y_1,AnimatedOffsetUV_ZoomX_1,AnimatedOffsetUV_ZoomY_1,AnimatedOffsetUV_Speed_1);
float2 RotationUV_1 = RotationUV(AnimatedOffsetUV_1,RotationUV_Rotation_1,RotationUV_Rotation_PosX_1,RotationUV_Rotation_PosY_1,RotationUV_Rotation_Speed_1);
float4 _Generate_Checker_1 = Generate_Checker(RotationUV_1,_Generate_Checker_PosX_1,_Generate_Checker_PosY_1,_Generate_Checker_Size_1,0);
float2 RotationUV_2 = RotationUV(i.texcoord,RotationUV_Rotation_2,RotationUV_Rotation_PosX_2,RotationUV_Rotation_PosY_2,RotationUV_Rotation_Speed_2);
float4 _ColorGradients_1 = Color_Gradients(float4(0,0,0,1),RotationUV_2,_ColorGradients_Color1_1,_ColorGradients_Color2_1,_ColorGradients_Color3_1,_ColorGradients_Color4_1);
float4 MaskRGBA_1=_Generate_Checker_1;
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
