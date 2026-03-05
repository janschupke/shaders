# 15 — Cubemaps & Environment

**Prerequisites:** [13 — PBR](13-pbr.md) or any shader with view direction and normals.

**Concepts:** Cubemaps, reflection vectors, environment sampling, skybox.

---

## What you'll learn

A **cubemap** is a texture with six faces (+X, -X, +Y, -Y, +Z, -Z) representing the environment. Reflection: the view ray reflects off the surface normal; the reflected direction samples the cubemap. Used for reflections, skybox, and ambient in PBR.

**Math:** Reflection vector: `R = V - 2 * (N·V) * N`. Or `reflect(V, N)` in HLSL. R points from the surface toward the reflected environment. Sample the cubemap with R to get the reflected color.

---

## Implementation steps

### Step 1: Add a reflection cubemap property

1. Add to Properties:
   ```hlsl
   _EnvMap ("Environment Map", Cube) = "grey" {}
   _ReflectionStrength ("Reflection Strength", Range(0, 1)) = 0.5
   ```

2. Declare:
   ```hlsl
   TEXTURECUBE(_EnvMap);
   SAMPLER(sampler_EnvMap);
   ```

3. In the fragment shader, compute the reflection vector:
   ```hlsl
   float3 N = normalize(IN.normalWS);
   float3 V = normalize(IN.viewDirWS);
   float3 R = reflect(-V, N);
   half4 envReflection = SAMPLE_TEXTURECUBE(_EnvMap, sampler_EnvMap, R);
   half3 reflection = envReflection.rgb * _ReflectionStrength;
   ```

4. Add to the final color (e.g. `color += reflection` or blend with specular).

5. **Verify:** Assign a cubemap (e.g. from Reflection Probe or skybox). Surfaces reflect the environment.

---

### Step 2: Use Unity's global reflection probe

Unity provides `unity_SpecCube0` — the scene's reflection probe. Sample it with the reflection vector:

```hlsl
half3 R = reflect(-V, N);
half4 envSample = SAMPLE_TEXTURECUBE_LOD(unity_SpecCube0, samplerunity_SpecCube0, R, 0);
half3 envReflection = DecodeHDREnvironment(envSample, unity_SpecCube0_HDR);
```

`DecodeHDREnvironment` decodes the HDR cubemap. Include `Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderVariablesFunctions.hlsl` or the equivalent that defines `unity_SpecCube0` and `DecodeHDREnvironment`.

---

### Step 3: Roughness-based mip level (PBR)

For rough surfaces, reflections are blurred. Sample at a higher mip level:

```hlsl
half roughness = 1 - _Smoothness;
half mipLevel = roughness * 6;  // 6 mips typical for cubemaps
half4 envSample = SAMPLE_TEXTURECUBE_LOD(unity_SpecCube0, samplerunity_SpecCube0, R, mipLevel);
```

---

### Step 4: Fresnel blend for reflections

Reflections are stronger at grazing angles. Blend with Fresnel:

```hlsl
float NdotV = saturate(dot(N, V));
float fresnel = pow(1.0 - NdotV, 5);
half3 reflection = envReflection * fresnel * _ReflectionStrength;
```

---

### Step 5: Skybox as fallback

If no reflection probe is set, Unity may use the skybox. Ensure your scene has a skybox or reflection probe for visible reflections.

---

## Unity setup checklist

- [ ] Reflection Probe in the scene (or use skybox).
- [ ] Cubemap texture: Import as Cube or use Reflection Probe's baked output.
- [ ] For `unity_SpecCube0`: use URP Lit shader includes or ensure the correct globals are available.

---

## Key takeaways

- `reflect(-V, N)` — reflection vector for environment sampling.
- `SAMPLE_TEXTURECUBE` / `SAMPLE_TEXTURECUBE_LOD` — sample cubemaps.
- Roughness → higher mip level for blurred reflections.
- Fresnel blend for physically plausible reflection strength.

---

## Next

[16 — Post-Processing](16-post-processing.md) — Full-screen effects.
