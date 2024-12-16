//////////////////////////////////////////////////////////////
/// Shadero Sprite: Sprite Shader Editor - by VETASOFT 2020 //
/// Shader generate with Shadero 1.9.9                      //
/// http://u3d.as/V7t #AssetStore                           //
/// http://www.shadero.com #Docs                            //
//////////////////////////////////////////////////////////////

Shader "Shadero Customs/Background"
{
Properties
{
ColorLimit("ColorLimit", Range(1, 128)) = 15
[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
PixelXYUV_SizeX_1("PixelXYUV_SizeX_1", Range(1, 128)) = 128
PixelXYUV_SizeY_1("PixelXYUV_SizeY_1", Range(1, 128)) = 128
DistortionUV_WaveX_1("DistortionUV_WaveX_1", Range(0, 128)) = 10
DistortionUV_WaveY_1("DistortionUV_WaveY_1", Range(0, 128)) = 10
DistortionUV_DistanceX_1("DistortionUV_DistanceX_1", Range(0, 1)) = 0.3
DistortionUV_DistanceY_1("DistortionUV_DistanceY_1", Range(0, 1)) = 0.3
DistortionUV_Speed_1("DistortionUV_Speed_1", Range(-2, 2)) = 1
RotationUV_Rotation_1("RotationUV_Rotation_1", Range(-360, 360)) = 90
RotationUV_Rotation_PosX_1("RotationUV_Rotation_PosX_1", Range(-1, 2)) = 0.5
RotationUV_Rotation_PosY_1("RotationUV_Rotation_PosY_1", Range(-1, 2)) =0.5
RotationUV_Rotation_Speed_1("RotationUV_Rotation_Speed_1", Range(-8, 8)) =0
_ColorGradients_Color1_1("_ColorGradients_Color1_1", COLOR) = (0.09803922,0.1176471,0.2352941,1)
_ColorGradients_Color2_1("_ColorGradients_Color2_1", COLOR) = (0.2156863,0.254902,0.3529412,1)
_ColorGradients_Color3_1("_ColorGradients_Color3_1", COLOR) = (0.2156863,0.254902,0.3529412,1)
_ColorGradients_Color4_1("_ColorGradients_Color4_1", COLOR) = (0.4705883,0.5686275,0.6470588,1)
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
float DistortionUV_WaveX_1;
float DistortionUV_WaveY_1;
float DistortionUV_DistanceX_1;
float DistortionUV_DistanceY_1;
float DistortionUV_Speed_1;
float RotationUV_Rotation_1;
float RotationUV_Rotation_PosX_1;
float RotationUV_Rotation_PosY_1;
float RotationUV_Rotation_Speed_1;
float4 _ColorGradients_Color1_1;
float4 _ColorGradients_Color2_1;
float4 _ColorGradients_Color3_1;
float4 _ColorGradients_Color4_1;
float ColorLimit;

v2f vert(appdata_t IN)
{
v2f OUT;
OUT.vertex = UnityObjectToClipPos(IN.vertex);
OUT.texcoord = IN.texcoord;
OUT.color = IN.color;
return OUT;
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
float2 pos = float2(x, y) * 2;
uv = floor(uv * pos+0.5) / pos;
return uv;
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
float2 PixelXYUV_1 = PixelXYUV(i.texcoord,PixelXYUV_SizeX_1,PixelXYUV_SizeY_1);
float2 DistortionUV_1 = DistortionUV(PixelXYUV_1,DistortionUV_WaveX_1,DistortionUV_WaveY_1,DistortionUV_DistanceX_1,DistortionUV_DistanceY_1,DistortionUV_Speed_1);
float2 RotationUV_1 = RotationUV(DistortionUV_1,RotationUV_Rotation_1,RotationUV_Rotation_PosX_1,RotationUV_Rotation_PosY_1,RotationUV_Rotation_Speed_1);
float4 _ColorGradients_1 = Color_Gradients(float4(0,0,0,1),RotationUV_1,_ColorGradients_Color1_1,_ColorGradients_Color2_1,_ColorGradients_Color3_1,_ColorGradients_Color4_1);
float4 FinalResult = _ColorGradients_1;
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
