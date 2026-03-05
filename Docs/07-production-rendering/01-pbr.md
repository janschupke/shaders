# 01 — Physically Based Rendering (PBR)

**Prerequisites:** [02 — Normal Mapping](../03-texturing/02-normal-mapping.md), [02 — Fresnel & Rim Lighting](../05-surface-effects/02-fresnel-rim.md). You need normals, view direction, and lighting basics.

**Concepts:** Metallic-roughness workflow, energy conservation, BRDF, URP PBR functions.

---

## What you'll learn

**PBR** uses measured material properties for realistic lighting. The **metallic-roughness** workflow: **albedo** (base color), **metallic** (0 = dielectric, 1 = metal), **roughness** (0 = mirror, 1 = diffuse). The BRDF combines diffuse and specular with energy conservation. URP provides `LightingPhysicallyBased` and related functions.

**Math:** Energy conservation: diffuse + specular ≤ 1. Fresnel-Schlick: `F = F0 + (1-F0)(1-cos θ)^5`. Cook-Torrance specular uses microfacet distribution (GGX) and geometry term. Metals: F0 = albedo, no diffuse. Dielectrics: F0 ≈ 0.04.

---

## Implementation steps

### Step 1: Add PBR properties

1. Add to Properties:
   ```hlsl
   _Metallic ("Metallic", Range(0, 1)) = 0
   _Smoothness ("Smoothness", Range(0, 1)) = 0.5
   ```

2. URP uses smoothness (1 - roughness). Add to CBUFFER.

---

### Step 2: Set up SurfaceData and InputData

URP's PBR lighting expects `SurfaceData` and `InputData`:

```hlsl
SurfaceData surfaceData;
surfaceData.albedo = albedo;
surfaceData.metallic = _Metallic;
surfaceData.smoothness = _Smoothness;
surfaceData.normalTS = normalTS;  // Or 0 if no normal map
surfaceData.emission = 0;

InputData inputData;
inputData.normalWS = normalWS;
inputData.viewDirectionWS = normalize(IN.viewDirWS);
inputData.positionWS = IN.positionWS;
inputData.bakedGI = 0;  // Or from light probes
inputData.normalizedScreenSpaceUV = 0;
inputData.shadowMask = half4(1,1,1,1);
inputData.vertexLighting = 0;
```

---

### Step 3: Call LightingPhysicallyBased

```hlsl
half4 color = UniversalFragmentPBR(inputData, surfaceData);
return color;
```

**Simplest approach:** Duplicate the URP Lit shader (`Packages/com.unity.render-pipelines.universal/Shaders/Lit.shader`) and modify it. Lit already includes the correct chain and `UniversalFragmentPBR`. Copy the ForwardLit pass and adjust `SurfaceData` / `InputData` as needed.

---

### Step 4: Simplified PBR (manual)

If you prefer to avoid the full Lit stack, implement a simplified version:

- **Diffuse:** Lambert, but reduce for metals (metals have no diffuse).
- **Specular:** Cook-Torrance or Blinn-Phong with roughness.
- **Fresnel:** Schlick approximation for view-dependent specular.

```hlsl
half3 kD = (1 - _Metallic) * albedo;  // Diffuse only for non-metals
half3 diffuse = kD * LightingLambert(...);
half3 F0 = lerp(0.04, albedo, _Metallic);  // Base reflectivity
// Simplified specular with roughness
half roughness = 1 - _Smoothness;
// ... (full Cook-Torrance is more involved; use URP's function for production)
```

For a full implementation, study URP's Lit shader or use `UniversalFragmentPBR`.

---

### Step 5: Test metallic and smoothness

- Metallic 0, Smoothness 0.5: plastic/dielectric.
- Metallic 1, Smoothness 1: mirror.
- Metallic 1, Smoothness 0: rough metal.

**Verify:** Materials respond realistically to light and view angle.

---

## Unity setup checklist

- [ ] URP Lit shader as reference for includes and structure.
- [ ] HDR color for albedo if using bright values.
- [ ] Environment (skybox, reflections) for full PBR look.

---

## Key takeaways

- PBR: albedo, metallic, smoothness (or roughness).
- Metals: no diffuse, tinted specular.
- Dielectrics: diffuse + white/gray specular.
- URP `UniversalFragmentPBR` handles the full BRDF; use Lit as a base for custom PBR.

---

## Next

[02 — Shadows](02-shadows.md) — Receive shadows from lights.
