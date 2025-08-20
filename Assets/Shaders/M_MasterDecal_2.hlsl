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
uniform float4 SelectionColor;
uniform float Desaturation;
uniform float Brightness;
uniform float OpacityInt;
uniform float RefractionDepthBias;

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
	float4 PreshaderBuffer[2];
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
	Material.PreshaderBuffer[0] = float4(0.000000,0.000000,0.000000,0.000000);//(Unknown)
	Material.PreshaderBuffer[1] = float4(0.000000,1.000000,1.000000,0.000000);//(Unknown)

	Material.PreshaderBuffer[0].x = SelectionColor.w;
	Material.PreshaderBuffer[0].yzw = SelectionColor.xyz;
	Material.PreshaderBuffer[1].x = Desaturation;
	Material.PreshaderBuffer[1].y = Brightness;
	Material.PreshaderBuffer[1].z = OpacityInt;
	Material.PreshaderBuffer[1].w = RefractionDepthBias;
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

	// The Normal is a special case as it might have its own expressions and also be used to calculate other inputs, so perform the assignment here
	PixelMaterialInputs.Normal = MaterialFloat3(0.00000000,0.00000000,1.00000000);


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
	MaterialFloat3 Local0 = lerp(MaterialFloat3(0.00000000,0.00000000,0.00000000),Material.PreshaderBuffer[0].yzw,Material.PreshaderBuffer[0].x);
	MaterialFloat2 Local1 = Parameters.TexCoords[0].xy;
	MaterialFloat Local2 = MaterialStoreTexCoordScale(Parameters, DERIV_BASE_VALUE(Local1), 0);
	MaterialFloat4 Local3 = ProcessMaterialColorTextureLookup(Texture2DSampleBias(Material_Texture2D_0,samplerMaterial_Texture2D_0,DERIV_BASE_VALUE(Local1),View.MaterialTextureMipBias));
	MaterialFloat Local4 = MaterialStoreTexSample(Parameters, Local3, 0);
	MaterialFloat Local5 = dot(Local3.rgb,MaterialFloat3(0.30000001,0.58999997,0.11000000));
	MaterialFloat3 Local6 = lerp(Local3.rgb,((MaterialFloat3)Local5),Material.PreshaderBuffer[1].x);
	MaterialFloat3 Local7 = (Local6 * ((MaterialFloat3)Material.PreshaderBuffer[1].y));
	MaterialFloat4 Local8 = MaterialCollection0.Vectors[3];
	FWSVector3 Local9 = GetWorldPosition_NoMaterialOffsets(Parameters);
	FWSVector3 Local10 = MakeWSVector(WSGetX(DERIV_BASE_VALUE(Local9)), WSGetY(DERIV_BASE_VALUE(Local9)), WSGetZ(DERIV_BASE_VALUE(Local9)));
	MaterialFloat4 Local11 = MaterialCollection0.Vectors[0];
	MaterialFloat Local12 = abs(Local11.g);
	MaterialFloat Local13 = (DERIV_BASE_VALUE(Local12) * -1.00000000);
	FWSVector3 Local14 = WSDivide(DERIV_BASE_VALUE(Local10), ((MaterialFloat3)DERIV_BASE_VALUE(Local13)));
	FWSVector2 Local15 = MakeWSVector(WSGetX(DERIV_BASE_VALUE(Local14)), WSGetZ(DERIV_BASE_VALUE(Local14)));
	MaterialFloat2 Local16 = WSApplyAddressMode(DERIV_BASE_VALUE(Local15), LWCADDRESSMODE_WRAP, LWCADDRESSMODE_WRAP);
	MaterialFloat Local17 = MaterialStoreTexCoordScale(Parameters, Local16, 3);
	MaterialFloat4 Local18 = ProcessMaterialColorTextureLookup(Texture2DSample_Decal(Material_Texture2D_1,GetMaterialSharedSampler(samplerMaterial_Texture2D_1,View_MaterialTextureBilinearWrapedSampler),Local16));
	MaterialFloat Local19 = MaterialStoreTexSample(Parameters, Local18, 3);
	FWSVector2 Local20 = MakeWSVector(WSGetY(DERIV_BASE_VALUE(Local14)), WSGetZ(DERIV_BASE_VALUE(Local14)));
	MaterialFloat2 Local21 = WSApplyAddressMode(DERIV_BASE_VALUE(Local20), LWCADDRESSMODE_WRAP, LWCADDRESSMODE_WRAP);
	MaterialFloat Local22 = MaterialStoreTexCoordScale(Parameters, Local21, 3);
	MaterialFloat4 Local23 = ProcessMaterialColorTextureLookup(Texture2DSample_Decal(Material_Texture2D_1,GetMaterialSharedSampler(samplerMaterial_Texture2D_1,View_MaterialTextureBilinearWrapedSampler),Local21));
	MaterialFloat Local24 = MaterialStoreTexSample(Parameters, Local23, 3);
	MaterialFloat Local25 = abs(Parameters.TangentToWorld[2].r);
	MaterialFloat Local26 = lerp((0.00000000 - 1.00000000),(1.00000000 + 1.00000000),Local25);
	MaterialFloat Local27 = saturate(Local26);
	MaterialFloat3 Local28 = lerp(Local18.rgb,Local23.rgb,Local27.r.r);
	FWSVector2 Local29 = MakeWSVector(WSGetX(DERIV_BASE_VALUE(Local14)), WSGetY(DERIV_BASE_VALUE(Local14)));
	MaterialFloat2 Local30 = WSApplyAddressMode(DERIV_BASE_VALUE(Local29), LWCADDRESSMODE_WRAP, LWCADDRESSMODE_WRAP);
	MaterialFloat Local31 = MaterialStoreTexCoordScale(Parameters, Local30, 3);
	MaterialFloat4 Local32 = ProcessMaterialColorTextureLookup(Texture2DSample_Decal(Material_Texture2D_1,GetMaterialSharedSampler(samplerMaterial_Texture2D_1,View_MaterialTextureBilinearWrapedSampler),Local30));
	MaterialFloat Local33 = MaterialStoreTexSample(Parameters, Local32, 3);
	MaterialFloat Local34 = abs(Parameters.TangentToWorld[2].b);
	MaterialFloat Local35 = lerp((0.00000000 - 1.00000000),(1.00000000 + 1.00000000),Local34);
	MaterialFloat Local36 = saturate(Local35);
	MaterialFloat3 Local37 = lerp(Local28,Local32.rgb,Local36.r.r);
	MaterialFloat Local38 = (Local37.g * Local11.a);
	MaterialFloat4 Local39 = MaterialCollection0.Vectors[1];
	MaterialFloat Local40 = PositiveClampedPow(Local38,Local39.r);
	MaterialFloat3 Local41 = lerp(Local7,Local8.rgba.rgb,Local40);
	MaterialFloat4 Local42 = MaterialCollection0.Vectors[4];
	MaterialFloat Local43 = abs(Local11.r);
	MaterialFloat Local44 = (DERIV_BASE_VALUE(Local43) * -1.00000000);
	FWSVector3 Local45 = WSDivide(DERIV_BASE_VALUE(Local10), ((MaterialFloat3)DERIV_BASE_VALUE(Local44)));
	FWSVector2 Local46 = MakeWSVector(WSGetX(DERIV_BASE_VALUE(Local45)), WSGetZ(DERIV_BASE_VALUE(Local45)));
	MaterialFloat2 Local47 = WSApplyAddressMode(DERIV_BASE_VALUE(Local46), LWCADDRESSMODE_WRAP, LWCADDRESSMODE_WRAP);
	MaterialFloat Local48 = MaterialStoreTexCoordScale(Parameters, Local47, 3);
	MaterialFloat4 Local49 = ProcessMaterialColorTextureLookup(Texture2DSample_Decal(Material_Texture2D_1,GetMaterialSharedSampler(samplerMaterial_Texture2D_1,View_MaterialTextureBilinearWrapedSampler),Local47));
	MaterialFloat Local50 = MaterialStoreTexSample(Parameters, Local49, 3);
	FWSVector2 Local51 = MakeWSVector(WSGetY(DERIV_BASE_VALUE(Local45)), WSGetZ(DERIV_BASE_VALUE(Local45)));
	MaterialFloat2 Local52 = WSApplyAddressMode(DERIV_BASE_VALUE(Local51), LWCADDRESSMODE_WRAP, LWCADDRESSMODE_WRAP);
	MaterialFloat Local53 = MaterialStoreTexCoordScale(Parameters, Local52, 3);
	MaterialFloat4 Local54 = ProcessMaterialColorTextureLookup(Texture2DSample_Decal(Material_Texture2D_1,GetMaterialSharedSampler(samplerMaterial_Texture2D_1,View_MaterialTextureBilinearWrapedSampler),Local52));
	MaterialFloat Local55 = MaterialStoreTexSample(Parameters, Local54, 3);
	MaterialFloat3 Local56 = lerp(Local49.rgb,Local54.rgb,Local27.r.r);
	FWSVector2 Local57 = MakeWSVector(WSGetX(DERIV_BASE_VALUE(Local45)), WSGetY(DERIV_BASE_VALUE(Local45)));
	MaterialFloat2 Local58 = WSApplyAddressMode(DERIV_BASE_VALUE(Local57), LWCADDRESSMODE_WRAP, LWCADDRESSMODE_WRAP);
	MaterialFloat Local59 = MaterialStoreTexCoordScale(Parameters, Local58, 3);
	MaterialFloat4 Local60 = ProcessMaterialColorTextureLookup(Texture2DSample_Decal(Material_Texture2D_1,GetMaterialSharedSampler(samplerMaterial_Texture2D_1,View_MaterialTextureBilinearWrapedSampler),Local58));
	MaterialFloat Local61 = MaterialStoreTexSample(Parameters, Local60, 3);
	MaterialFloat3 Local62 = lerp(Local56,Local60.rgb,Local36.r.r);
	MaterialFloat Local63 = (Local62.b * Local39.g);
	MaterialFloat Local64 = PositiveClampedPow(Local63,Local39.b);
	MaterialFloat3 Local65 = lerp(Local41,Local42.rgba.rgb,Local64);
	MaterialFloat4 Local66 = MaterialCollection0.Vectors[5];
	MaterialFloat Local67 = abs(Local11.b);
	MaterialFloat Local68 = (DERIV_BASE_VALUE(Local67) * -1.00000000);
	FWSVector3 Local69 = WSDivide(DERIV_BASE_VALUE(Local10), ((MaterialFloat3)DERIV_BASE_VALUE(Local68)));
	FWSVector2 Local70 = MakeWSVector(WSGetX(DERIV_BASE_VALUE(Local69)), WSGetZ(DERIV_BASE_VALUE(Local69)));
	MaterialFloat2 Local71 = WSApplyAddressMode(DERIV_BASE_VALUE(Local70), LWCADDRESSMODE_WRAP, LWCADDRESSMODE_WRAP);
	MaterialFloat Local72 = MaterialStoreTexCoordScale(Parameters, Local71, 3);
	MaterialFloat4 Local73 = ProcessMaterialColorTextureLookup(Texture2DSample_Decal(Material_Texture2D_1,GetMaterialSharedSampler(samplerMaterial_Texture2D_1,View_MaterialTextureBilinearWrapedSampler),Local71));
	MaterialFloat Local74 = MaterialStoreTexSample(Parameters, Local73, 3);
	FWSVector2 Local75 = MakeWSVector(WSGetY(DERIV_BASE_VALUE(Local69)), WSGetZ(DERIV_BASE_VALUE(Local69)));
	MaterialFloat2 Local76 = WSApplyAddressMode(DERIV_BASE_VALUE(Local75), LWCADDRESSMODE_WRAP, LWCADDRESSMODE_WRAP);
	MaterialFloat Local77 = MaterialStoreTexCoordScale(Parameters, Local76, 3);
	MaterialFloat4 Local78 = ProcessMaterialColorTextureLookup(Texture2DSample_Decal(Material_Texture2D_1,GetMaterialSharedSampler(samplerMaterial_Texture2D_1,View_MaterialTextureBilinearWrapedSampler),Local76));
	MaterialFloat Local79 = MaterialStoreTexSample(Parameters, Local78, 3);
	MaterialFloat3 Local80 = lerp(Local73.rgb,Local78.rgb,Local27.r.r);
	FWSVector2 Local81 = MakeWSVector(WSGetX(DERIV_BASE_VALUE(Local69)), WSGetY(DERIV_BASE_VALUE(Local69)));
	MaterialFloat2 Local82 = WSApplyAddressMode(DERIV_BASE_VALUE(Local81), LWCADDRESSMODE_WRAP, LWCADDRESSMODE_WRAP);
	MaterialFloat Local83 = MaterialStoreTexCoordScale(Parameters, Local82, 3);
	MaterialFloat4 Local84 = ProcessMaterialColorTextureLookup(Texture2DSample_Decal(Material_Texture2D_1,GetMaterialSharedSampler(samplerMaterial_Texture2D_1,View_MaterialTextureBilinearWrapedSampler),Local82));
	MaterialFloat Local85 = MaterialStoreTexSample(Parameters, Local84, 3);
	MaterialFloat3 Local86 = lerp(Local80,Local84.rgb,Local36.r.r);
	MaterialFloat Local87 = (Local86.r * Local39.a);
	MaterialFloat4 Local88 = MaterialCollection0.Vectors[2];
	MaterialFloat Local89 = PositiveClampedPow(Local87,Local88.r);
	MaterialFloat3 Local90 = lerp(Local65,Local66.rgba.rgb,Local89);
	MaterialFloat Local91 = (Local3.a * Material.PreshaderBuffer[1].z);

	PixelMaterialInputs.EmissiveColor = Local0;
	PixelMaterialInputs.Opacity = Local91;
	PixelMaterialInputs.OpacityMask = 1.00000000;
	PixelMaterialInputs.BaseColor = Local90;
	PixelMaterialInputs.Metallic = 0.00000000;
	PixelMaterialInputs.Specular = 0.50000000;
	PixelMaterialInputs.Roughness = 0.50000000;
	PixelMaterialInputs.Anisotropy = 0.00000000;
	PixelMaterialInputs.Normal = MaterialFloat3(0.00000000,0.00000000,1.00000000);
	PixelMaterialInputs.Tangent = MaterialFloat3(1.00000000,0.00000000,0.00000000);
	PixelMaterialInputs.Subsurface = 0;
	PixelMaterialInputs.AmbientOcclusion = 1.00000000;
	PixelMaterialInputs.Refraction = MaterialFloat3(MaterialFloat3(1.00000000,0.00000000,0.00000000).xy,Material.PreshaderBuffer[1].w);
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
	//if( PixelMaterialInputs.OpacityMask < 0.333 ) discard;
	//o.Alpha = PixelMaterialInputs.OpacityMask;

	o.Metallic = PixelMaterialInputs.Metallic;
	o.Smoothness = 1.0 - PixelMaterialInputs.Roughness;
	o.Normal = normalize( PixelMaterialInputs.Normal );
	o.Emission = PixelMaterialInputs.EmissiveColor.rgb;
	o.Occlusion = PixelMaterialInputs.AmbientOcclusion;
}