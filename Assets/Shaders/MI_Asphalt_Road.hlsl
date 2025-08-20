#define NUM_TEX_COORD_INTERPOLATORS 2
#define NUM_MATERIAL_TEXCOORDS_VERTEX 2
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
TEXTURE2D(       Material_Texture2D_4 );
SAMPLER(  samplerMaterial_Texture2D_4 );
float4 Material_Texture2D_4_TexelSize;
float4 Material_Texture2D_4_ST;
TEXTURE2D(       Material_Texture2D_5 );
SAMPLER(  samplerMaterial_Texture2D_5 );
float4 Material_Texture2D_5_TexelSize;
float4 Material_Texture2D_5_ST;
TEXTURE2D(       Material_Texture2D_6 );
SAMPLER(  samplerMaterial_Texture2D_6 );
float4 Material_Texture2D_6_TexelSize;
float4 Material_Texture2D_6_ST;
TEXTURE2D(       Material_Texture2D_7 );
SAMPLER(  samplerMaterial_Texture2D_7 );
float4 Material_Texture2D_7_TexelSize;
float4 Material_Texture2D_7_ST;
TEXTURE2D(       Material_Texture2D_8 );
SAMPLER(  samplerMaterial_Texture2D_8 );
float4 Material_Texture2D_8_TexelSize;
float4 Material_Texture2D_8_ST;
uniform float4 SelectionColor;
uniform float Tiling_Soil_01;
uniform float Soil_01_NormalStrength;
uniform float Mask_Size;
uniform float AsphaltPatch_NormalFlatness;
uniform float AsphaltPatch_Detail_NormalFlatness;
uniform float NormalFlatten;
uniform float Mask_Intensity;
uniform float Mask_Power;
uniform float MaskPower;
uniform float MaskInt;
uniform float AsphaltPatchColor;
uniform float Roughness;
uniform float SpecularInt;
uniform float AsphaltPatch_Roughness;
uniform float PDO;

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
	float4 PreshaderBuffer[5];
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
	Material.PreshaderBuffer[0] = float4(0.002500,4.000000,0.000661,2.579844);//(Unknown)
	Material.PreshaderBuffer[1] = float4(-0.471057,-2.683228,1.000000,-0.043153);//(Unknown)
	Material.PreshaderBuffer[2] = float4(2.005331,1.000000,0.000000,0.000000);//(Unknown)
	Material.PreshaderBuffer[3] = float4(0.000000,0.000000,0.000000,0.651632);//(Unknown)
	Material.PreshaderBuffer[4] = float4(1.000000,0.123810,0.668736,-0.390150);//(Unknown)

	Material.PreshaderBuffer[0].x = rcp( Tiling_Soil_01 );
	Material.PreshaderBuffer[0].y = Soil_01_NormalStrength;
	Material.PreshaderBuffer[0].z = rcp( Mask_Size );
	Material.PreshaderBuffer[0].w = AsphaltPatch_NormalFlatness;
	Material.PreshaderBuffer[1].x = AsphaltPatch_Detail_NormalFlatness;
	Material.PreshaderBuffer[1].y = NormalFlatten;
	Material.PreshaderBuffer[1].z = Mask_Intensity;
	Material.PreshaderBuffer[1].w = Mask_Power;
	Material.PreshaderBuffer[2].x = MaskPower;
	Material.PreshaderBuffer[2].y = MaskInt;
	Material.PreshaderBuffer[2].z = SelectionColor.w;
	Material.PreshaderBuffer[3].xyz = SelectionColor.xyz;
	Material.PreshaderBuffer[3].w = AsphaltPatchColor;
	Material.PreshaderBuffer[4].x = Roughness;
	Material.PreshaderBuffer[4].y = SpecularInt;
	Material.PreshaderBuffer[4].z = AsphaltPatch_Roughness;
	Material.PreshaderBuffer[4].w = PDO;
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
	FWSVector3 Local0 = GetWorldPosition(Parameters);
	FWSVector3 Local1 = MakeWSVector(WSGetX(DERIV_BASE_VALUE(Local0)), WSGetY(DERIV_BASE_VALUE(Local0)), WSGetZ(DERIV_BASE_VALUE(Local0)));
	FWSVector2 Local2 = MakeWSVector(WSGetX(DERIV_BASE_VALUE(Local1)), WSGetY(DERIV_BASE_VALUE(Local1)));
	FWSVector2 Local3 = WSMultiply(DERIV_BASE_VALUE(Local2), ((MaterialFloat2)Material.PreshaderBuffer[0].x));
	MaterialFloat2 Local4 = WSApplyAddressMode(DERIV_BASE_VALUE(Local3), LWCADDRESSMODE_WRAP, LWCADDRESSMODE_WRAP);
	MaterialFloat Local5 = MaterialStoreTexCoordScale(Parameters, Local4, 7);
	MaterialFloat4 Local6 = UnpackNormalMap(Texture2DSample(Material_Texture2D_0,GetMaterialSharedSampler(samplerMaterial_Texture2D_0,View_MaterialTextureBilinearWrapedSampler),Local4));
	MaterialFloat Local7 = MaterialStoreTexSample(Parameters, Local6, 7);
	MaterialFloat2 Local8 = (Local6.rgb.rg * ((MaterialFloat2)Material.PreshaderBuffer[0].y));
	FWSVector3 Local9 = WSMultiply(DERIV_BASE_VALUE(Local1), ((MaterialFloat3)Material.PreshaderBuffer[0].z));
	FWSVector2 Local10 = MakeWSVector(WSGetX(DERIV_BASE_VALUE(Local9)), WSGetY(DERIV_BASE_VALUE(Local9)));
	MaterialFloat2 Local11 = WSApplyAddressMode(DERIV_BASE_VALUE(Local10), LWCADDRESSMODE_WRAP, LWCADDRESSMODE_WRAP);
	MaterialFloat Local12 = MaterialStoreTexCoordScale(Parameters, Local11, 4);
	MaterialFloat4 Local13 = UnpackNormalMap(Texture2DSampleBias(Material_Texture2D_1,samplerMaterial_Texture2D_1,Local11,View.MaterialTextureMipBias));
	MaterialFloat Local14 = MaterialStoreTexSample(Parameters, Local13, 4);
	MaterialFloat3 Local15 = lerp(Local13.rgb,MaterialFloat3(0.00000000,0.00000000,1.00000000).rgb,Material.PreshaderBuffer[0].w);
	MaterialFloat Local16 = (Local15.b + 1.00000000);
	MaterialFloat2 Local17 = Parameters.TexCoords[0].xy;
	MaterialFloat Local18 = MaterialStoreTexCoordScale(Parameters, DERIV_BASE_VALUE(Local17), 2);
	MaterialFloat4 Local19 = UnpackNormalMap(Texture2DSampleBias(Material_Texture2D_2,samplerMaterial_Texture2D_2,DERIV_BASE_VALUE(Local17),View.MaterialTextureMipBias));
	MaterialFloat Local20 = MaterialStoreTexSample(Parameters, Local19, 2);
	MaterialFloat3 Local21 = lerp(Local19.rgb,MaterialFloat3(0.00000000,0.00000000,1.00000000).rgb,Material.PreshaderBuffer[1].x);
	MaterialFloat2 Local22 = (Local21.rg * ((MaterialFloat2)-1.00000000));
	MaterialFloat Local23 = dot(MaterialFloat3(Local15.rg,Local16),MaterialFloat3(Local22,Local21.b));
	MaterialFloat3 Local24 = (MaterialFloat3(Local15.rg,Local16) * ((MaterialFloat3)Local23));
	MaterialFloat3 Local25 = (((MaterialFloat3)Local16) * MaterialFloat3(Local22,Local21.b));
	MaterialFloat3 Local26 = (Local24 - Local25);
	MaterialFloat3 Local27 = lerp(Local19.rgb,MaterialFloat3(0.00000000,0.00000000,1.00000000).rgb,Material.PreshaderBuffer[1].y);
	MaterialFloat Local28 = MaterialStoreTexCoordScale(Parameters, Local11, 0);
	MaterialFloat4 Local29 = Texture2DSampleBias(Material_Texture2D_3,samplerMaterial_Texture2D_3,Local11,View.MaterialTextureMipBias);
	MaterialFloat Local30 = MaterialStoreTexSample(Parameters, Local29, 0);
	MaterialFloat Local31 = (Local29.rgb.r * Material.PreshaderBuffer[1].z);
	MaterialFloat Local32 = PositiveClampedPow(Local31,Material.PreshaderBuffer[1].w);
	MaterialFloat3 Local33 = lerp(Local26,Local27,Local32);
	MaterialFloat2 Local34 = Parameters.TexCoords[1].xy;
	MaterialFloat Local35 = MaterialStoreTexCoordScale(Parameters, DERIV_BASE_VALUE(Local34), 5);
	MaterialFloat4 Local36 = Texture2DSampleBias(Material_Texture2D_4,samplerMaterial_Texture2D_4,DERIV_BASE_VALUE(Local34),View.MaterialTextureMipBias);
	MaterialFloat Local37 = MaterialStoreTexSample(Parameters, Local36, 5);
	MaterialFloat Local38 = PositiveClampedPow(Local36.rgb.r,Material.PreshaderBuffer[2].x);
	MaterialFloat Local39 = (Local38 * Material.PreshaderBuffer[2].y);
	MaterialFloat Local40 = max(Local39,-5.00000000);
	MaterialFloat Local41 = min(Local40,5.00000000);
	MaterialFloat3 Local42 = lerp(MaterialFloat3(Local8,Local6.rgb.b),Local33,Local41);
	MaterialFloat Local43 = MaterialStoreTexCoordScale(Parameters, Local4, 6);
	MaterialFloat4 Local44 = ProcessMaterialColorTextureLookup(Texture2DSample(Material_Texture2D_5,GetMaterialSharedSampler(samplerMaterial_Texture2D_5,View_MaterialTextureBilinearWrapedSampler),Local4));
	MaterialFloat Local45 = MaterialStoreTexSample(Parameters, Local44, 6);
	MaterialFloat Local46 = (Local44.a - 1.00000000);
	MaterialFloat4 Local47 = Parameters.VertexColor;
	MaterialFloat Local48 = DERIV_BASE_VALUE(Local47).r;
	MaterialFloat Local49 = (DERIV_BASE_VALUE(Local48) * 2.00000000);
	MaterialFloat Local50 = (Local46 + DERIV_BASE_VALUE(Local49));
	MaterialFloat Local51 = saturate(Local50);
	MaterialFloat Local52 = lerp((0.00000000 - 0.00000000),(0.00000000 + 1.00000000),Local51);
	MaterialFloat Local53 = saturate(Local52);
	MaterialFloat3 Local54 = lerp(MaterialFloat3(Local8,Local6.rgb.b),Local42.rgb,Local53.r);

	// The Normal is a special case as it might have its own expressions and also be used to calculate other inputs, so perform the assignment here
	PixelMaterialInputs.Normal = Local54;


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
	MaterialFloat3 Local55 = lerp(MaterialFloat3(0.00000000,0.00000000,0.00000000),Material.PreshaderBuffer[3].xyz,Material.PreshaderBuffer[2].z);
	MaterialFloat Local56 = MaterialStoreTexCoordScale(Parameters, DERIV_BASE_VALUE(Local17), 1);
	MaterialFloat4 Local57 = ProcessMaterialColorTextureLookup(Texture2DSampleBias(Material_Texture2D_6,samplerMaterial_Texture2D_6,DERIV_BASE_VALUE(Local17),View.MaterialTextureMipBias));
	MaterialFloat Local58 = MaterialStoreTexSample(Parameters, Local57, 1);
	MaterialFloat3 Local59 = (Local57.rgb * ((MaterialFloat3)Material.PreshaderBuffer[3].w));
	MaterialFloat3 Local60 = lerp(Local59,Local57.rgb,Local32);
	MaterialFloat3 Local61 = lerp(Local44.rgb,Local60,Local41);
	MaterialFloat3 Local62 = lerp(Local44.rgb,Local61.rgb,Local53.r);
	MaterialFloat Local63 = MaterialStoreTexCoordScale(Parameters, Local4, 8);
	MaterialFloat4 Local64 = Texture2DSample(Material_Texture2D_7,GetMaterialSharedSampler(samplerMaterial_Texture2D_7,View_MaterialTextureBilinearWrapedSampler),Local4);
	MaterialFloat Local65 = MaterialStoreTexSample(Parameters, Local64, 8);
	MaterialFloat Local66 = lerp(Local64.g,0.00000000,Local41);
	MaterialFloat Local67 = MaterialStoreTexCoordScale(Parameters, DERIV_BASE_VALUE(Local17), 3);
	MaterialFloat4 Local68 = Texture2DSampleBias(Material_Texture2D_8,samplerMaterial_Texture2D_8,DERIV_BASE_VALUE(Local17),View.MaterialTextureMipBias);
	MaterialFloat Local69 = MaterialStoreTexSample(Parameters, Local68, 3);
	MaterialFloat Local70 = (Local68.g * Material.PreshaderBuffer[4].x);
	MaterialFloat Local71 = (1.00000000 - Local70);
	MaterialFloat Local72 = (Local71 * Material.PreshaderBuffer[4].y);
	MaterialFloat Local73 = lerp(Local64.r,Local72,Local41);
	MaterialFloat Local74 = (Local68.g * Material.PreshaderBuffer[4].z);
	MaterialFloat Local75 = lerp(Local74,Local70,Local32);
	MaterialFloat Local76 = lerp(Local64.b,Local75,Local41);
	MaterialFloat Local77 = lerp(1.00000000,Local57.a,Local41);
	MaterialFloat Local78 = PositiveClampedPow(Local44.a,Material.PreshaderBuffer[4].w);

	PixelMaterialInputs.EmissiveColor = Local55;
	PixelMaterialInputs.Opacity = 1.00000000;
	PixelMaterialInputs.OpacityMask = 1.00000000;
	PixelMaterialInputs.BaseColor = Local62;
	PixelMaterialInputs.Metallic = Local66.r;
	PixelMaterialInputs.Specular = Local73.r;
	PixelMaterialInputs.Roughness = Local76.r;
	PixelMaterialInputs.Anisotropy = 0.00000000;
	PixelMaterialInputs.Normal = Local54;
	PixelMaterialInputs.Tangent = MaterialFloat3(1.00000000,0.00000000,0.00000000);
	PixelMaterialInputs.Subsurface = 0;
	PixelMaterialInputs.AmbientOcclusion = Local77.r;
	PixelMaterialInputs.Refraction = 0;
	PixelMaterialInputs.PixelDepthOffset = Local78;
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