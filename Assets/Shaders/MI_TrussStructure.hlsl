#define NUM_TEX_COORD_INTERPOLATORS 1
#define NUM_MATERIAL_TEXCOORDS_VERTEX 1
#define NUM_CUSTOM_VERTEX_INTERPOLATORS 0

struct Input
{
	//float3 Normal;
	float2 uv_MainTex : TEXCOORD0;
	float2 uv2_Material_Texture2D_0 : TEXCOORD1;
	float4 color : COLOR;
	float4 tangent;
	//float4 normal;
	float3 viewDir;
	float4 screenPos;
	float3 worldPos;
	//float3 worldNormal;
	float3 normal2;
};
struct SurfaceOutputStandard
{
	float3 Albedo;		// base (diffuse or specular) color
	float3 Normal;		// tangent space normal, if written
	half3 Emission;
	half Metallic;		// 0=non-metal, 1=metal
	// Smoothness is the user facing name, it should be perceptual smoothness but user should not have to deal with it.
	// Everywhere in the code you meet smoothness it is perceptual smoothness
	half Smoothness;	// 0=rough, 1=smooth
	half Occlusion;		// occlusion (default 1)
	float Alpha;		// alpha for transparencies
};

//#define HDRP 1
#define URP 1
#define UE5
//#define HAS_CUSTOMIZED_UVS 1
#define MATERIAL_TANGENTSPACENORMAL 1
//struct Material
//{
	//samplers start
SAMPLER( SamplerState_Linear_Repeat );
SAMPLER( SamplerState_Linear_Clamp );
TEXTURE2D(       Material_Texture2D_0 );
SAMPLER(  samplerMaterial_Texture2D_0 );
float4 Material_Texture2D_0_TexelSize;
float4 Material_Texture2D_0_ST;
TEXTURE2D(       Material_Texture2D_1 );
SAMPLER(  samplerMaterial_Texture2D_1 );
float4 Material_Texture2D_1_TexelSize;
float4 Material_Texture2D_1_ST;
TEXTURE2D(       Material_Texture2D_2 );
SAMPLER(  samplerMaterial_Texture2D_2 );
float4 Material_Texture2D_2_TexelSize;
float4 Material_Texture2D_2_ST;
TEXTURE2D(       Material_Texture2D_3 );
SAMPLER(  samplerMaterial_Texture2D_3 );
float4 Material_Texture2D_3_TexelSize;
float4 Material_Texture2D_3_ST;
uniform float4 SelectionColor;
uniform float Tiling;
uniform float NormalStrength;
uniform float4 Tint;
uniform float MetalicAmount;
uniform float OpacityInt;

//};

#ifdef UE5
	#define UE_LWC_RENDER_TILE_SIZE			2097152.0
	#define UE_LWC_RENDER_TILE_SIZE_SQRT	1448.15466
	#define UE_LWC_RENDER_TILE_SIZE_RSQRT	0.000690533954
	#define UE_LWC_RENDER_TILE_SIZE_RCP		4.76837158e-07
	#define UE_LWC_RENDER_TILE_SIZE_FMOD_PI		0.673652053
	#define UE_LWC_RENDER_TILE_SIZE_FMOD_2PI	0.673652053
	#define INVARIANT(X) X
	#define PI 					(3.1415926535897932)

	#include "LargeWorldCoordinates.hlsl"
#endif
struct MaterialStruct
{
	float4 PreshaderBuffer[4];
	float4 ScalarExpressions[1];
	float VTPackedPageTableUniform[2];
	float VTPackedUniform[1];
};
#define SVTPackedUniform VTPackedUniform
static SamplerState View_MaterialTextureBilinearWrapedSampler;
static SamplerState View_MaterialTextureBilinearClampedSampler;
struct ViewStruct
{
	float GameTime;
	float RealTime;
	float DeltaTime;
	float PrevFrameGameTime;
	float PrevFrameRealTime;
	float MaterialTextureMipBias;	
	float4 PrimitiveSceneData[ 40 ];
	float4 TemporalAAParams;
	float2 ViewRectMin;
	float4 ViewSizeAndInvSize;
	float2 ResolutionFractionAndInv;
	float MaterialTextureDerivativeMultiply;
	uint StateFrameIndexMod8;
	uint StateFrameIndex;
	float FrameNumber;
	float2 FieldOfViewWideAngles;
	float4 RuntimeVirtualTextureMipLevel;
	float PreExposure;
	float4 BufferBilinearUVMinMax;
    float OneOverPreExposure;
};
struct ResolvedViewStruct
{
	#ifdef UE5
		FLWCVector3 WorldCameraOrigin;
		FLWCVector3 PrevWorldCameraOrigin;
		FLWCVector3 PreViewTranslation;
		FLWCVector3 WorldViewOrigin;
	#else
		float3 WorldCameraOrigin;
		float3 PrevWorldCameraOrigin;
		float3 PreViewTranslation;
		float3 WorldViewOrigin;
	#endif
	float4 ScreenPositionScaleBias;
	float4x4 TranslatedWorldToView;
	float4x4 TranslatedWorldToCameraView;
	float4x4 TranslatedWorldToClip;
	float4x4 ViewToTranslatedWorld;
	float4x4 PrevViewToTranslatedWorld;
	float4x4 CameraViewToTranslatedWorld;
	float4 BufferBilinearUVMinMax;
	float4 XRPassthroughCameraUVs[ 2 ];
};
struct PrimitiveStruct
{
	float4x4 WorldToLocal;
	float4x4 LocalToWorld;
};

static ViewStruct View;
static ResolvedViewStruct ResolvedView;
static PrimitiveStruct Primitive;
uniform float4 View_BufferSizeAndInvSize;
uniform float4 LocalObjectBoundsMin;
uniform float4 LocalObjectBoundsMax;
static SamplerState Material_Wrap_WorldGroupSettings;
static SamplerState Material_Clamp_WorldGroupSettings;

#include "UnrealCommon.cginc"

static MaterialStruct Material;
void InitializeExpressions()
{
	Material.PreshaderBuffer[0] = float4(1.000000,1.000000,0.000000,0.000000);//(Unknown)
	Material.PreshaderBuffer[1] = float4(0.000000,0.000000,0.000000,0.000000);//(Unknown)
	Material.PreshaderBuffer[2] = float4(1.000000,1.000000,1.000000,0.000000);//(Unknown)
	Material.PreshaderBuffer[3] = float4(3.000000,0.000000,0.000000,0.000000);//(Unknown)

	Material.PreshaderBuffer[0].x = Tiling;
	Material.PreshaderBuffer[0].y = NormalStrength;
	Material.PreshaderBuffer[0].z = SelectionColor.w;
	Material.PreshaderBuffer[1].xyz = SelectionColor.xyz;
	Material.PreshaderBuffer[2].xyz = Tint.xyz;
	Material.PreshaderBuffer[2].w = MetalicAmount;
	Material.PreshaderBuffer[3].x = OpacityInt;
}
struct MaterialCollection0Type
{
	float4 Vectors[6];
};
//MPC_WallMask
MaterialCollection0Type MaterialCollection0;
void Initialize_MaterialCollection0()
{
	MaterialCollection0.Vectors[0] = float4(1477.855469,7865.201172,1088.214966,1.260016);//DustSize,LeakSize,GrungeSize,LeakInt
	MaterialCollection0.Vectors[1] = float4(0.633929,2.123932,2.416789,2.881386);//LeakPov,DustInt,DustPov,GrungeInt
	MaterialCollection0.Vectors[2] = float4(1.036092,0.000000,0.000000,0.000000);//GrungePov,,,
	MaterialCollection0.Vectors[3] = float4(0.093750,0.067111,0.049805,0.000000);//LeakColor
	MaterialCollection0.Vectors[4] = float4(0.218750,0.210522,0.173520,0.000000);//DustColor
	MaterialCollection0.Vectors[5] = float4(0.234375,0.195613,0.180664,0.000000);//GrungeColor
}
float3 GetMaterialWorldPositionOffset(FMaterialVertexParameters Parameters)
{
	return MaterialFloat3(0.00000000,0.00000000,0.00000000);;
}
void CalcPixelMaterialInputs(in out FMaterialPixelParameters Parameters, in out FPixelMaterialInputs PixelMaterialInputs)
{
	//WorldAligned texturing & others use normals & stuff that think Z is up
	Parameters.TangentToWorld[0] = Parameters.TangentToWorld[0].xzy;
	Parameters.TangentToWorld[1] = Parameters.TangentToWorld[1].xzy;
	Parameters.TangentToWorld[2] = Parameters.TangentToWorld[2].xzy;

	float3 WorldNormalCopy = Parameters.WorldNormal;

	// Initial calculations (required for Normal)
	MaterialFloat2 Local0 = Parameters.TexCoords[0].xy;
	MaterialFloat2 Local1 = (DERIV_BASE_VALUE(Local0) * MaterialFloat2(1.00000000,2.00000000));
	MaterialFloat2 Local2 = (DERIV_BASE_VALUE(Local1) * ((MaterialFloat2)Material.PreshaderBuffer[0].x));
	MaterialFloat Local3 = MaterialStoreTexCoordScale(Parameters, DERIV_BASE_VALUE(Local2), 2);
	MaterialFloat4 Local4 = UnpackNormalMap(Texture2DSampleBias(Material_Texture2D_0,samplerMaterial_Texture2D_0,DERIV_BASE_VALUE(Local2),View.MaterialTextureMipBias));
	MaterialFloat Local5 = MaterialStoreTexSample(Parameters, Local4, 2);
	MaterialFloat2 Local6 = (Local4.rgb.rg * ((MaterialFloat2)Material.PreshaderBuffer[0].y));

	// The Normal is a special case as it might have its own expressions and also be used to calculate other inputs, so perform the assignment here
	PixelMaterialInputs.Normal = MaterialFloat3(Local6,Local4.rgb.b);


#if TEMPLATE_USES_SUBSTRATE
	Parameters.SubstratePixelFootprint = SubstrateGetPixelFootprint(Parameters.WorldPosition_CamRelative, GetRoughnessFromNormalCurvature(Parameters));
	Parameters.SharedLocalBases = SubstrateInitialiseSharedLocalBases();
	Parameters.SubstrateTree = GetInitialisedSubstrateTree();
#if SUBSTRATE_USE_FULLYSIMPLIFIED_MATERIAL == 1
	Parameters.SharedLocalBasesFullySimplified = SubstrateInitialiseSharedLocalBases();
	Parameters.SubstrateTreeFullySimplified = GetInitialisedSubstrateTree();
#endif
#endif

	// Note that here MaterialNormal can be in world space or tangent space
	float3 MaterialNormal = GetMaterialNormal(Parameters, PixelMaterialInputs);

#if MATERIAL_TANGENTSPACENORMAL

#if FEATURE_LEVEL >= FEATURE_LEVEL_SM4
	// Mobile will rely on only the final normalize for performance
	MaterialNormal = normalize(MaterialNormal);
#endif

	// normalizing after the tangent space to world space conversion improves quality with sheared bases (UV layout to WS causes shrearing)
	// use full precision normalize to avoid overflows
	Parameters.WorldNormal = TransformTangentNormalToWorld(Parameters.TangentToWorld, MaterialNormal);

#else //MATERIAL_TANGENTSPACENORMAL

	Parameters.WorldNormal = normalize(MaterialNormal);

#endif //MATERIAL_TANGENTSPACENORMAL

#if MATERIAL_TANGENTSPACENORMAL || TWO_SIDED_WORLD_SPACE_SINGLELAYERWATER_NORMAL
	// flip the normal for backfaces being rendered with a two-sided material
	Parameters.WorldNormal *= Parameters.TwoSidedSign;
#endif

	Parameters.ReflectionVector = ReflectionAboutCustomWorldNormal(Parameters, Parameters.WorldNormal, false);

#if !PARTICLE_SPRITE_FACTORY
	Parameters.Particle.MotionBlurFade = 1.0f;
#endif // !PARTICLE_SPRITE_FACTORY

	// Now the rest of the inputs
	MaterialFloat3 Local7 = lerp(MaterialFloat3(0.00000000,0.00000000,0.00000000),Material.PreshaderBuffer[1].xyz,Material.PreshaderBuffer[0].z);
	MaterialFloat Local8 = MaterialStoreTexCoordScale(Parameters, DERIV_BASE_VALUE(Local2), 1);
	MaterialFloat4 Local9 = ProcessMaterialColorTextureLookup(Texture2DSampleBias(Material_Texture2D_1,samplerMaterial_Texture2D_1,DERIV_BASE_VALUE(Local2),View.MaterialTextureMipBias));
	MaterialFloat Local10 = MaterialStoreTexSample(Parameters, Local9, 1);
	MaterialFloat3 Local11 = (Local9.rgb * Material.PreshaderBuffer[2].xyz);
	MaterialFloat4 Local12 = MaterialCollection0.Vectors[3];
	FWSVector3 Local13 = GetWorldPosition_NoMaterialOffsets(Parameters);
	FWSVector3 Local14 = MakeWSVector(WSGetX(DERIV_BASE_VALUE(Local13)), WSGetY(DERIV_BASE_VALUE(Local13)), WSGetZ(DERIV_BASE_VALUE(Local13)));
	MaterialFloat4 Local15 = MaterialCollection0.Vectors[0];
	MaterialFloat Local16 = abs(Local15.g);
	MaterialFloat Local17 = (DERIV_BASE_VALUE(Local16) * -1.00000000);
	FWSVector3 Local18 = WSDivide(DERIV_BASE_VALUE(Local14), ((MaterialFloat3)DERIV_BASE_VALUE(Local17)));
	FWSVector2 Local19 = MakeWSVector(WSGetX(DERIV_BASE_VALUE(Local18)), WSGetZ(DERIV_BASE_VALUE(Local18)));
	MaterialFloat2 Local20 = WSApplyAddressMode(DERIV_BASE_VALUE(Local19), LWCADDRESSMODE_WRAP, LWCADDRESSMODE_WRAP);
	MaterialFloat Local21 = MaterialStoreTexCoordScale(Parameters, Local20, 10);
	MaterialFloat4 Local22 = ProcessMaterialColorTextureLookup(Texture2DSample(Material_Texture2D_2,GetMaterialSharedSampler(samplerMaterial_Texture2D_2,View_MaterialTextureBilinearWrapedSampler),Local20));
	MaterialFloat Local23 = MaterialStoreTexSample(Parameters, Local22, 10);
	FWSVector2 Local24 = MakeWSVector(WSGetY(DERIV_BASE_VALUE(Local18)), WSGetZ(DERIV_BASE_VALUE(Local18)));
	MaterialFloat2 Local25 = WSApplyAddressMode(DERIV_BASE_VALUE(Local24), LWCADDRESSMODE_WRAP, LWCADDRESSMODE_WRAP);
	MaterialFloat Local26 = MaterialStoreTexCoordScale(Parameters, Local25, 10);
	MaterialFloat4 Local27 = ProcessMaterialColorTextureLookup(Texture2DSample(Material_Texture2D_2,GetMaterialSharedSampler(samplerMaterial_Texture2D_2,View_MaterialTextureBilinearWrapedSampler),Local25));
	MaterialFloat Local28 = MaterialStoreTexSample(Parameters, Local27, 10);
	MaterialFloat Local29 = abs(Parameters.TangentToWorld[2].r);
	MaterialFloat Local30 = lerp((0.00000000 - 1.00000000),(1.00000000 + 1.00000000),Local29);
	MaterialFloat Local31 = saturate(Local30);
	MaterialFloat3 Local32 = lerp(Local22.rgb,Local27.rgb,Local31.r.r);
	FWSVector2 Local33 = MakeWSVector(WSGetX(DERIV_BASE_VALUE(Local18)), WSGetY(DERIV_BASE_VALUE(Local18)));
	MaterialFloat2 Local34 = WSApplyAddressMode(DERIV_BASE_VALUE(Local33), LWCADDRESSMODE_WRAP, LWCADDRESSMODE_WRAP);
	MaterialFloat Local35 = MaterialStoreTexCoordScale(Parameters, Local34, 10);
	MaterialFloat4 Local36 = ProcessMaterialColorTextureLookup(Texture2DSample(Material_Texture2D_2,GetMaterialSharedSampler(samplerMaterial_Texture2D_2,View_MaterialTextureBilinearWrapedSampler),Local34));
	MaterialFloat Local37 = MaterialStoreTexSample(Parameters, Local36, 10);
	MaterialFloat Local38 = abs(Parameters.TangentToWorld[2].b);
	MaterialFloat Local39 = lerp((0.00000000 - 1.00000000),(1.00000000 + 1.00000000),Local38);
	MaterialFloat Local40 = saturate(Local39);
	MaterialFloat3 Local41 = lerp(Local32,Local36.rgb,Local40.r.r);
	MaterialFloat Local42 = (Local41.g * Local15.a);
	MaterialFloat4 Local43 = MaterialCollection0.Vectors[1];
	MaterialFloat Local44 = PositiveClampedPow(Local42,Local43.r);
	MaterialFloat3 Local45 = lerp(Local11,Local12.rgba.rgb,Local44);
	MaterialFloat4 Local46 = MaterialCollection0.Vectors[4];
	MaterialFloat Local47 = abs(Local15.r);
	MaterialFloat Local48 = (DERIV_BASE_VALUE(Local47) * -1.00000000);
	FWSVector3 Local49 = WSDivide(DERIV_BASE_VALUE(Local14), ((MaterialFloat3)DERIV_BASE_VALUE(Local48)));
	FWSVector2 Local50 = MakeWSVector(WSGetX(DERIV_BASE_VALUE(Local49)), WSGetZ(DERIV_BASE_VALUE(Local49)));
	MaterialFloat2 Local51 = WSApplyAddressMode(DERIV_BASE_VALUE(Local50), LWCADDRESSMODE_WRAP, LWCADDRESSMODE_WRAP);
	MaterialFloat Local52 = MaterialStoreTexCoordScale(Parameters, Local51, 10);
	MaterialFloat4 Local53 = ProcessMaterialColorTextureLookup(Texture2DSample(Material_Texture2D_2,GetMaterialSharedSampler(samplerMaterial_Texture2D_2,View_MaterialTextureBilinearWrapedSampler),Local51));
	MaterialFloat Local54 = MaterialStoreTexSample(Parameters, Local53, 10);
	FWSVector2 Local55 = MakeWSVector(WSGetY(DERIV_BASE_VALUE(Local49)), WSGetZ(DERIV_BASE_VALUE(Local49)));
	MaterialFloat2 Local56 = WSApplyAddressMode(DERIV_BASE_VALUE(Local55), LWCADDRESSMODE_WRAP, LWCADDRESSMODE_WRAP);
	MaterialFloat Local57 = MaterialStoreTexCoordScale(Parameters, Local56, 10);
	MaterialFloat4 Local58 = ProcessMaterialColorTextureLookup(Texture2DSample(Material_Texture2D_2,GetMaterialSharedSampler(samplerMaterial_Texture2D_2,View_MaterialTextureBilinearWrapedSampler),Local56));
	MaterialFloat Local59 = MaterialStoreTexSample(Parameters, Local58, 10);
	MaterialFloat3 Local60 = lerp(Local53.rgb,Local58.rgb,Local31.r.r);
	FWSVector2 Local61 = MakeWSVector(WSGetX(DERIV_BASE_VALUE(Local49)), WSGetY(DERIV_BASE_VALUE(Local49)));
	MaterialFloat2 Local62 = WSApplyAddressMode(DERIV_BASE_VALUE(Local61), LWCADDRESSMODE_WRAP, LWCADDRESSMODE_WRAP);
	MaterialFloat Local63 = MaterialStoreTexCoordScale(Parameters, Local62, 10);
	MaterialFloat4 Local64 = ProcessMaterialColorTextureLookup(Texture2DSample(Material_Texture2D_2,GetMaterialSharedSampler(samplerMaterial_Texture2D_2,View_MaterialTextureBilinearWrapedSampler),Local62));
	MaterialFloat Local65 = MaterialStoreTexSample(Parameters, Local64, 10);
	MaterialFloat3 Local66 = lerp(Local60,Local64.rgb,Local40.r.r);
	MaterialFloat Local67 = (Local66.b * Local43.g);
	MaterialFloat Local68 = PositiveClampedPow(Local67,Local43.b);
	MaterialFloat3 Local69 = lerp(Local45,Local46.rgba.rgb,Local68);
	MaterialFloat4 Local70 = MaterialCollection0.Vectors[5];
	MaterialFloat Local71 = abs(Local15.b);
	MaterialFloat Local72 = (DERIV_BASE_VALUE(Local71) * -1.00000000);
	FWSVector3 Local73 = WSDivide(DERIV_BASE_VALUE(Local14), ((MaterialFloat3)DERIV_BASE_VALUE(Local72)));
	FWSVector2 Local74 = MakeWSVector(WSGetX(DERIV_BASE_VALUE(Local73)), WSGetZ(DERIV_BASE_VALUE(Local73)));
	MaterialFloat2 Local75 = WSApplyAddressMode(DERIV_BASE_VALUE(Local74), LWCADDRESSMODE_WRAP, LWCADDRESSMODE_WRAP);
	MaterialFloat Local76 = MaterialStoreTexCoordScale(Parameters, Local75, 10);
	MaterialFloat4 Local77 = ProcessMaterialColorTextureLookup(Texture2DSample(Material_Texture2D_2,GetMaterialSharedSampler(samplerMaterial_Texture2D_2,View_MaterialTextureBilinearWrapedSampler),Local75));
	MaterialFloat Local78 = MaterialStoreTexSample(Parameters, Local77, 10);
	FWSVector2 Local79 = MakeWSVector(WSGetY(DERIV_BASE_VALUE(Local73)), WSGetZ(DERIV_BASE_VALUE(Local73)));
	MaterialFloat2 Local80 = WSApplyAddressMode(DERIV_BASE_VALUE(Local79), LWCADDRESSMODE_WRAP, LWCADDRESSMODE_WRAP);
	MaterialFloat Local81 = MaterialStoreTexCoordScale(Parameters, Local80, 10);
	MaterialFloat4 Local82 = ProcessMaterialColorTextureLookup(Texture2DSample(Material_Texture2D_2,GetMaterialSharedSampler(samplerMaterial_Texture2D_2,View_MaterialTextureBilinearWrapedSampler),Local80));
	MaterialFloat Local83 = MaterialStoreTexSample(Parameters, Local82, 10);
	MaterialFloat3 Local84 = lerp(Local77.rgb,Local82.rgb,Local31.r.r);
	FWSVector2 Local85 = MakeWSVector(WSGetX(DERIV_BASE_VALUE(Local73)), WSGetY(DERIV_BASE_VALUE(Local73)));
	MaterialFloat2 Local86 = WSApplyAddressMode(DERIV_BASE_VALUE(Local85), LWCADDRESSMODE_WRAP, LWCADDRESSMODE_WRAP);
	MaterialFloat Local87 = MaterialStoreTexCoordScale(Parameters, Local86, 10);
	MaterialFloat4 Local88 = ProcessMaterialColorTextureLookup(Texture2DSample(Material_Texture2D_2,GetMaterialSharedSampler(samplerMaterial_Texture2D_2,View_MaterialTextureBilinearWrapedSampler),Local86));
	MaterialFloat Local89 = MaterialStoreTexSample(Parameters, Local88, 10);
	MaterialFloat3 Local90 = lerp(Local84,Local88.rgb,Local40.r.r);
	MaterialFloat Local91 = (Local90.r * Local43.a);
	MaterialFloat4 Local92 = MaterialCollection0.Vectors[2];
	MaterialFloat Local93 = PositiveClampedPow(Local91,Local92.r);
	MaterialFloat3 Local94 = lerp(Local69,Local70.rgba.rgb,Local93);
	MaterialFloat Local95 = MaterialStoreTexCoordScale(Parameters, DERIV_BASE_VALUE(Local2), 0);
	MaterialFloat4 Local96 = Texture2DSampleBias(Material_Texture2D_3,samplerMaterial_Texture2D_3,DERIV_BASE_VALUE(Local2),View.MaterialTextureMipBias);
	MaterialFloat Local97 = MaterialStoreTexSample(Parameters, Local96, 0);
	MaterialFloat Local98 = (Local96.g * Material.PreshaderBuffer[2].w);
	MaterialFloat Local99 = (Local96.a * Material.PreshaderBuffer[3].x);

	PixelMaterialInputs.EmissiveColor = Local7;
	PixelMaterialInputs.Opacity = 1.00000000;
	PixelMaterialInputs.OpacityMask = Local99;
	PixelMaterialInputs.BaseColor = Local94;
	PixelMaterialInputs.Metallic = Local98;
	PixelMaterialInputs.Specular = Local96.r;
	PixelMaterialInputs.Roughness = Local96.b;
	PixelMaterialInputs.Anisotropy = 0.00000000;
	PixelMaterialInputs.Normal = MaterialFloat3(Local6,Local4.rgb.b);
	PixelMaterialInputs.Tangent = MaterialFloat3(1.00000000,0.00000000,0.00000000);
	PixelMaterialInputs.Subsurface = 0;
	PixelMaterialInputs.AmbientOcclusion = 1.00000000;
	PixelMaterialInputs.Refraction = 0;
	PixelMaterialInputs.PixelDepthOffset = 0.00000000;
	PixelMaterialInputs.ShadingModel = 1;
	PixelMaterialInputs.FrontMaterial = GetInitialisedSubstrateData();
	PixelMaterialInputs.SurfaceThickness = 0.01000000;
	PixelMaterialInputs.Displacement = 0.50000000;


#if MATERIAL_USES_ANISOTROPY
	Parameters.WorldTangent = CalculateAnisotropyTangent(Parameters, PixelMaterialInputs);
#else
	Parameters.WorldTangent = 0;
#endif
}

#define UnityObjectToWorldDir TransformObjectToWorld

void SetupCommonData( int Parameters_PrimitiveId )
{
	View_MaterialTextureBilinearWrapedSampler = SamplerState_Linear_Repeat;
	View_MaterialTextureBilinearClampedSampler = SamplerState_Linear_Clamp;

	Material_Wrap_WorldGroupSettings = SamplerState_Linear_Repeat;
	Material_Clamp_WorldGroupSettings = SamplerState_Linear_Clamp;

	#ifdef FULLSCREEN_SHADERGRAPH
		View.GameTime = View.RealTime = -_Time.y;// _Time is (t/20, t, t*2, t*3)
	#else
		View.GameTime = View.RealTime = _Time.y;// _Time is (t/20, t, t*2, t*3)
	#endif
	
	View.PrevFrameGameTime = View.GameTime - unity_DeltaTime.x;//(dt, 1/dt, smoothDt, 1/smoothDt)
	View.PrevFrameRealTime = View.RealTime;
	View.DeltaTime = unity_DeltaTime.x;
	View.MaterialTextureMipBias = 0.0;
	View.TemporalAAParams = float4( 0, 0, 0, 0 );
	View.ViewRectMin = float2( 0, 0 );
	View.ViewSizeAndInvSize = View_BufferSizeAndInvSize;
	View.ResolutionFractionAndInv = float2( View_BufferSizeAndInvSize.x / View_BufferSizeAndInvSize.y, 1.0 / ( View_BufferSizeAndInvSize.x / View_BufferSizeAndInvSize.y ));
	View.MaterialTextureDerivativeMultiply = 1.0f;
	View.StateFrameIndexMod8 = 0;
	View.FrameNumber = (int)_Time.y;
	View.FieldOfViewWideAngles = float2( PI * 0.42f, PI * 0.42f );//75degrees, default unity
	View.RuntimeVirtualTextureMipLevel = float4( 0, 0, 0, 0 );
	View.PreExposure = 1;
    View.OneOverPreExposure = 1;
	View.BufferBilinearUVMinMax = float4(
		View_BufferSizeAndInvSize.z * ( 0 + 0.5 ),//EffectiveViewRect.Min.X
		View_BufferSizeAndInvSize.w * ( 0 + 0.5 ),//EffectiveViewRect.Min.Y
		View_BufferSizeAndInvSize.z * ( View_BufferSizeAndInvSize.x - 0.5 ),//EffectiveViewRect.Max.X
		View_BufferSizeAndInvSize.w * ( View_BufferSizeAndInvSize.y - 0.5 ) );//EffectiveViewRect.Max.Y

	for( int i2 = 0; i2 < 40; i2++ )
		View.PrimitiveSceneData[ i2 ] = float4( 0, 0, 0, 0 );

	float4x4 LocalToWorld = transpose( UNITY_MATRIX_M );
    LocalToWorld[3] = float4(ToUnrealPos(LocalToWorld[3]), LocalToWorld[3].w);
	float4x4 WorldToLocal = transpose( UNITY_MATRIX_I_M );
	float4x4 ViewMatrix = transpose( UNITY_MATRIX_V );
	float4x4 InverseViewMatrix = transpose( UNITY_MATRIX_I_V );
	float4x4 ViewProjectionMatrix = transpose( UNITY_MATRIX_VP );
	uint PrimitiveBaseOffset = Parameters_PrimitiveId * PRIMITIVE_SCENE_DATA_STRIDE;
	View.PrimitiveSceneData[ PrimitiveBaseOffset + 0 ] = LocalToWorld[ 0 ];//LocalToWorld
	View.PrimitiveSceneData[ PrimitiveBaseOffset + 1 ] = LocalToWorld[ 1 ];//LocalToWorld
	View.PrimitiveSceneData[ PrimitiveBaseOffset + 2 ] = LocalToWorld[ 2 ];//LocalToWorld
	View.PrimitiveSceneData[ PrimitiveBaseOffset + 3 ] = LocalToWorld[ 3 ];//LocalToWorld
	View.PrimitiveSceneData[ PrimitiveBaseOffset + 5 ] = float4( ToUnrealPos( SHADERGRAPH_OBJECT_POSITION ), 100.0 );//ObjectWorldPosition
	View.PrimitiveSceneData[ PrimitiveBaseOffset + 6 ] = WorldToLocal[ 0 ];//WorldToLocal
	View.PrimitiveSceneData[ PrimitiveBaseOffset + 7 ] = WorldToLocal[ 1 ];//WorldToLocal
	View.PrimitiveSceneData[ PrimitiveBaseOffset + 8 ] = WorldToLocal[ 2 ];//WorldToLocal
	View.PrimitiveSceneData[ PrimitiveBaseOffset + 9 ] = WorldToLocal[ 3 ];//WorldToLocal
	View.PrimitiveSceneData[ PrimitiveBaseOffset + 10 ] = LocalToWorld[ 0 ];//PreviousLocalToWorld
	View.PrimitiveSceneData[ PrimitiveBaseOffset + 11 ] = LocalToWorld[ 1 ];//PreviousLocalToWorld
	View.PrimitiveSceneData[ PrimitiveBaseOffset + 12 ] = LocalToWorld[ 2 ];//PreviousLocalToWorld
	View.PrimitiveSceneData[ PrimitiveBaseOffset + 13 ] = LocalToWorld[ 3 ];//PreviousLocalToWorld
	View.PrimitiveSceneData[ PrimitiveBaseOffset + 18 ] = float4( ToUnrealPos( SHADERGRAPH_OBJECT_POSITION ), 0 );//ActorWorldPosition
	View.PrimitiveSceneData[ PrimitiveBaseOffset + 19 ] = LocalObjectBoundsMax - LocalObjectBoundsMin;//ObjectBounds
	View.PrimitiveSceneData[ PrimitiveBaseOffset + 21 ] = mul( LocalToWorld, float3( 1, 0, 0 ) );
	View.PrimitiveSceneData[ PrimitiveBaseOffset + 23 ] = LocalObjectBoundsMin;//LocalObjectBoundsMin 
	View.PrimitiveSceneData[ PrimitiveBaseOffset + 24 ] = LocalObjectBoundsMax;//LocalObjectBoundsMax

#ifdef UE5
	ResolvedView.WorldCameraOrigin = LWCPromote( ToUnrealPos( _WorldSpaceCameraPos.xyz ) );
	ResolvedView.PreViewTranslation = LWCPromote( float3( 0, 0, 0 ) );
	ResolvedView.WorldViewOrigin = LWCPromote( float3( 0, 0, 0 ) );
#else
	ResolvedView.WorldCameraOrigin = ToUnrealPos( _WorldSpaceCameraPos.xyz );
	ResolvedView.PreViewTranslation = float3( 0, 0, 0 );
	ResolvedView.WorldViewOrigin = float3( 0, 0, 0 );
#endif
	ResolvedView.PrevWorldCameraOrigin = ResolvedView.WorldCameraOrigin;
	ResolvedView.ScreenPositionScaleBias = float4( 1, 1, 0, 0 );
	ResolvedView.TranslatedWorldToView		 = ViewMatrix;
	ResolvedView.TranslatedWorldToCameraView = ViewMatrix;
	ResolvedView.TranslatedWorldToClip		 = ViewProjectionMatrix;
	ResolvedView.ViewToTranslatedWorld		 = InverseViewMatrix;
	ResolvedView.PrevViewToTranslatedWorld = ResolvedView.ViewToTranslatedWorld;
	ResolvedView.CameraViewToTranslatedWorld = InverseViewMatrix;
	ResolvedView.BufferBilinearUVMinMax = View.BufferBilinearUVMinMax;
	Primitive.WorldToLocal = WorldToLocal;
	Primitive.LocalToWorld = LocalToWorld;
}
#define VS_USES_UNREAL_SPACE 1
float3 PrepareAndGetWPO( float4 VertexColor, float3 UnrealWorldPos, float3 UnrealNormal, float4 InTangent,
						 float4 UV0, float4 UV1 )
{
	InitializeExpressions();
	Initialize_MaterialCollection0();

	FMaterialVertexParameters Parameters = (FMaterialVertexParameters)0;

	float3 InWorldNormal = UnrealNormal;
	float4 tangentWorld = InTangent;
	tangentWorld.xyz = normalize( tangentWorld.xyz );
	//float3x3 tangentToWorld = CreateTangentToWorldPerVertex( InWorldNormal, tangentWorld.xyz, tangentWorld.w );
	Parameters.TangentToWorld = float3x3( normalize( cross( InWorldNormal, tangentWorld.xyz ) * tangentWorld.w ), tangentWorld.xyz, InWorldNormal );

	
	#ifdef VS_USES_UNREAL_SPACE
		UnrealWorldPos = ToUnrealPos( UnrealWorldPos );
	#endif
	Parameters.WorldPosition = UnrealWorldPos;
	#ifdef VS_USES_UNREAL_SPACE
		Parameters.TangentToWorld[ 0 ] = Parameters.TangentToWorld[ 0 ].xzy;
		Parameters.TangentToWorld[ 1 ] = Parameters.TangentToWorld[ 1 ].xzy;
		Parameters.TangentToWorld[ 2 ] = Parameters.TangentToWorld[ 2 ].xzy;//WorldAligned texturing uses normals that think Z is up
	#endif

	Parameters.VertexColor = VertexColor;

#if NUM_MATERIAL_TEXCOORDS_VERTEX > 0			
	Parameters.TexCoords[ 0 ] = float2( UV0.x, UV0.y );
#endif
#if NUM_MATERIAL_TEXCOORDS_VERTEX > 1
	Parameters.TexCoords[ 1 ] = float2( UV1.x, UV1.y );
#endif
#if NUM_MATERIAL_TEXCOORDS_VERTEX > 2
	for( int i = 2; i < NUM_TEX_COORD_INTERPOLATORS; i++ )
	{
		Parameters.TexCoords[ i ] = float2( UV0.x, UV0.y );
	}
#endif

	Parameters.PrimitiveId = 0;

	SetupCommonData( Parameters.PrimitiveId );

#ifdef UE5
	Parameters.PrevFrameLocalToWorld = MakeLWCMatrix( float3( 0, 0, 0 ), Primitive.LocalToWorld );
#else
	Parameters.PrevFrameLocalToWorld = Primitive.LocalToWorld;
#endif
	
	float3 Offset = float3( 0, 0, 0 );
	Offset = GetMaterialWorldPositionOffset( Parameters );
	#ifdef VS_USES_UNREAL_SPACE
		//Convert from unreal units to unity
		Offset /= float3( 100, 100, 100 );
		Offset = Offset.xzy;
	#endif
	return Offset;
}

void SurfaceReplacement( Input In, out SurfaceOutputStandard o )
{
	InitializeExpressions();
	Initialize_MaterialCollection0();


	float3 Z3 = float3( 0, 0, 0 );
	float4 Z4 = float4( 0, 0, 0, 0 );

	float3 UnrealWorldPos = float3( In.worldPos.x, In.worldPos.y, In.worldPos.z );

	float3 UnrealNormal = In.normal2;	

	FMaterialPixelParameters Parameters = (FMaterialPixelParameters)0;
#if NUM_TEX_COORD_INTERPOLATORS > 0
    #ifdef FULLSCREEN_SHADERGRAPH
		Parameters.TexCoords[ 0 ] = float2( In.uv_MainTex.x, In.uv_MainTex.y );
	#else
		Parameters.TexCoords[ 0 ] = float2( In.uv_MainTex.x, 1.0 - In.uv_MainTex.y );
	#endif
#endif
#if NUM_TEX_COORD_INTERPOLATORS > 1
	Parameters.TexCoords[ 1 ] = float2( In.uv2_Material_Texture2D_0.x, 1.0 - In.uv2_Material_Texture2D_0.y );
#endif
#if NUM_TEX_COORD_INTERPOLATORS > 2
	for( int i = 2; i < NUM_TEX_COORD_INTERPOLATORS; i++ )
	{
		Parameters.TexCoords[ i ] = float2( In.uv_MainTex.x, 1.0 - In.uv_MainTex.y );
	}
#endif
	Parameters.PostProcessUV = In.uv_MainTex;
	Parameters.VertexColor = In.color;
	Parameters.WorldNormal = UnrealNormal;
	Parameters.ReflectionVector = half3( 0, 0, 1 );
	//Parameters.CameraVector = normalize( _WorldSpaceCameraPos.xyz - UnrealWorldPos.xyz );
	//Parameters.CameraVector = mul( ( float3x3 )unity_CameraToWorld, float3( 0, 0, 1 ) ) * -1;	
	float3 CameraDirection = (-1 * mul((float3x3)UNITY_MATRIX_M, transpose(mul(UNITY_MATRIX_I_M, UNITY_MATRIX_I_V)) [2].xyz));//From ShaderGraph
	Parameters.CameraVector = CameraDirection;
	Parameters.LightVector = half3( 0, 0, 0 );
	float4 screenpos = In.screenPos;
	screenpos /= screenpos.w;
	Parameters.SvPosition = screenpos;
	Parameters.ScreenPosition = Parameters.SvPosition;

	Parameters.UnMirrored = 1;

	Parameters.TwoSidedSign = 1;


	float3 InWorldNormal = UnrealNormal;	
	float4 tangentWorld = In.tangent;
	tangentWorld.xyz = normalize( tangentWorld.xyz );
	//float3x3 tangentToWorld = CreateTangentToWorldPerVertex( InWorldNormal, tangentWorld.xyz, tangentWorld.w );
	Parameters.TangentToWorld = float3x3( normalize( cross( InWorldNormal, tangentWorld.xyz ) * tangentWorld.w ), tangentWorld.xyz, InWorldNormal );

	//WorldAlignedTexturing in UE relies on the fact that coords there are 100x larger, prepare values for that
	//but watch out for any computation that might get skewed as a side effect
	UnrealWorldPos = ToUnrealPos( UnrealWorldPos );
	
	Parameters.AbsoluteWorldPosition = UnrealWorldPos;
	Parameters.WorldPosition_CamRelative = UnrealWorldPos;
	Parameters.WorldPosition_NoOffsets = UnrealWorldPos;

	Parameters.WorldPosition_NoOffsets_CamRelative = Parameters.WorldPosition_CamRelative;
	Parameters.LightingPositionOffset = float3( 0, 0, 0 );

	Parameters.AOMaterialMask = 0;

	Parameters.Particle.RelativeTime = 0;
	Parameters.Particle.MotionBlurFade;
	Parameters.Particle.Random = 0;
	Parameters.Particle.Velocity = half4( 1, 1, 1, 1 );
	Parameters.Particle.Color = half4( 1, 1, 1, 1 );
	Parameters.Particle.TranslatedWorldPositionAndSize = float4( UnrealWorldPos, 0 );
	Parameters.Particle.MacroUV = half4( 0, 0, 1, 1 );
	Parameters.Particle.DynamicParameter = half4( 0, 0, 0, 0 );
	Parameters.Particle.LocalToWorld = float4x4( Z4, Z4, Z4, Z4 );
	Parameters.Particle.Size = float2( 1, 1 );
	Parameters.Particle.SubUVCoords[ 0 ] = Parameters.Particle.SubUVCoords[ 1 ] = float2( 0, 0 );
	Parameters.Particle.SubUVLerp = 0.0;
	Parameters.TexCoordScalesParams = float2( 0, 0 );
	Parameters.PrimitiveId = 0;
	Parameters.VirtualTextureFeedback = 0;

	FPixelMaterialInputs PixelMaterialInputs = (FPixelMaterialInputs)0;
	PixelMaterialInputs.Normal = float3( 0, 0, 1 );
	PixelMaterialInputs.ShadingModel = 0;
	//PixelMaterialInputs.FrontMaterial = GetStrataUnlitBSDF( float3( 0, 0, 0 ), float3( 0, 0, 0 ) );

	SetupCommonData( Parameters.PrimitiveId );
	//CustomizedUVs
	#if NUM_TEX_COORD_INTERPOLATORS > 0 && HAS_CUSTOMIZED_UVS
		float2 OutTexCoords[ NUM_TEX_COORD_INTERPOLATORS ];
		//Prevent uninitialized reads
		for( int i = 0; i < NUM_TEX_COORD_INTERPOLATORS; i++ )
		{
			OutTexCoords[ i ] = float2( 0, 0 );
		}
		GetMaterialCustomizedUVs( Parameters, OutTexCoords );
		for( int i = 0; i < NUM_TEX_COORD_INTERPOLATORS; i++ )
		{
			Parameters.TexCoords[ i ] = OutTexCoords[ i ];
		}
	#endif
	//<-
	CalcPixelMaterialInputs( Parameters, PixelMaterialInputs );

	#define HAS_WORLDSPACE_NORMAL 0
	#if HAS_WORLDSPACE_NORMAL
		PixelMaterialInputs.Normal = mul( PixelMaterialInputs.Normal, (MaterialFloat3x3)( transpose( Parameters.TangentToWorld ) ) );
	#endif

	o.Albedo = PixelMaterialInputs.BaseColor.rgb;
	o.Alpha = PixelMaterialInputs.Opacity;
	if( PixelMaterialInputs.OpacityMask < 0.333 ) discard;
	//o.Alpha = PixelMaterialInputs.OpacityMask;

	o.Metallic = PixelMaterialInputs.Metallic;
	o.Smoothness = 1.0 - PixelMaterialInputs.Roughness;
	o.Normal = normalize( PixelMaterialInputs.Normal );
	o.Emission = PixelMaterialInputs.EmissiveColor.rgb;
	o.Occlusion = PixelMaterialInputs.AmbientOcclusion;
}