# 01 — Parallax & Triplanar Mapping

**Prerequisites:** [02 — Normal Mapping](../03-texturing/02-normal-mapping.md). You need height/normal sampling and TBN.

**Concepts:** Parallax mapping, steep parallax, triplanar projection, UV-less texturing.

---

## What you'll learn

**Parallax mapping** offsets UVs using a height map so surfaces appear to have depth when the view angle changes. **Steep parallax** uses multiple samples for better quality. **Triplanar mapping** projects a texture from three world axes (X, Y, Z) and blends — no UVs needed, ideal for terrain and procedural geometry.

**Math (parallax):** At a grazing view angle, the "real" surface point is offset along the view ray. Approximate: `uvOffset = viewDirTS.xy / viewDirTS.z * height * scale`. The view direction in tangent space (`viewDirTS`) gives the offset direction; dividing by `.z` scales by slope.

---

## Practical applications

- **Parallax** — brick walls, floor tiles, rocky terrain. Adds apparent depth when viewed at an angle; use steep parallax for stronger effect.
- **Triplanar** — voxel worlds, procedural meshes, terrain without UVs. Projects texture from X/Y/Z axes; no seams. Ideal for Minecraft-style blocks, sculpted terrain, or any mesh where UV unwrapping is impractical.
- **Combined** — triplanar base + parallax on one axis for terrain with height variation.

---

## Implementation steps

### Step 1: Parallax — add height map and view in tangent space

1. Add to Properties:
   ```hlsl
   _HeightMap ("Height Map", 2D) = "gray" {}
   _HeightScale ("Height Scale", Range(0, 0.2)) = 0.05
   ```

2. Add `viewDirTS` to Varyings. In the vertex shader, transform view direction to tangent space:
   ```hlsl
   float3 viewDirWS = normalize(GetCameraPositionWS() - positionWS);
   float3x3 TBN = float3x3(tangentWS, bitangentWS, normalWS);
   OUT.viewDirTS = mul(TBN, viewDirWS);
   ```

3. In the fragment shader, sample height and offset UVs:
   ```hlsl
   half height = SAMPLE_TEXTURE2D(_HeightMap, sampler_HeightMap, IN.uv).r;
   float2 uvOffset = IN.viewDirTS.xy / (IN.viewDirTS.z + 0.001) * height * _HeightScale;
   float2 parallaxUV = IN.uv - uvOffset;
   ```
   Use `parallaxUV` for base map and normal map sampling.

4. **Verify:** Slanted surfaces show parallax shift when moving the camera. Adjust `_HeightScale`.

---

### Step 2: Steep parallax (optional)

Sample the height map in steps along the view ray for better accuracy:

```hlsl
float numLayers = 8;
float layerDepth = 1.0 / numLayers;
float currentLayerDepth = 0;
float2 deltaUV = (IN.viewDirTS.xy / IN.viewDirTS.z) * _HeightScale / numLayers;
float2 currentUV = IN.uv;
half currentDepth = SAMPLE_TEXTURE2D(_HeightMap, sampler_HeightMap, currentUV).r;

for (int i = 0; i < numLayers; i++)
{
    currentUV -= deltaUV;
    currentDepth = SAMPLE_TEXTURE2D(_HeightMap, sampler_HeightMap, currentUV).r;
    currentLayerDepth += layerDepth;
    if (currentDepth < currentLayerDepth) break;
}
float2 parallaxUV = currentUV;
```

---

### Step 3: Triplanar mapping — project from three axes

Triplanar samples the texture three times (along X, Y, Z) and blends by the surface normal:

1. Add to Varyings: `float3 positionWS` (you likely have this).

2. In the fragment shader:
   ```hlsl
   float3 pos = IN.positionWS * _TriplanarScale;  // Scale for texture density
   float3 blend = abs(normalize(IN.normalWS));
   blend = blend / (blend.x + blend.y + blend.z);

   half4 colX = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, pos.zy);
   half4 colY = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, pos.xz);
   half4 colZ = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, pos.xy);

   half4 triplanar = colX * blend.x + colY * blend.y + colZ * blend.z;
   half3 albedo = triplanar.rgb * _BaseColor.rgb;
   ```

3. **Math:** `blend = |N|` normalized. A face pointing along X has high blend.x → use YZ plane (pos.zy). The sum ensures smooth blending at edges.

4. **Verify:** Use on a sphere or complex mesh without UVs. Texture projects from world axes.

---

### Step 4: Triplanar with normal mapping (optional)

For triplanar normals, sample three normal maps (or the same map at pos.zy, pos.xz, pos.xy), unpack each, transform to world space (each projection has a different tangent frame), then blend by the same weights. This is more involved; start with triplanar albedo only.

---

## Unity setup checklist

- [ ] Height map: grayscale, same resolution as base/normal if possible.
- [ ] Triplanar: `_TriplanarScale` controls texture density (e.g. 0.1–1).

---

## Key takeaways

- Parallax: offset UVs by `viewDirTS.xy / viewDirTS.z * height * scale`.
- Steep parallax: multi-step ray march for better quality.
- Triplanar: sample from X, Y, Z planes; blend by `abs(N)` normalized.
- Triplanar avoids UV seams; good for terrain, voxels, procedural meshes.

---

## Next

[01 — Transparency](../05-surface-effects/01-transparency.md) — Alpha blending and alpha clip.
