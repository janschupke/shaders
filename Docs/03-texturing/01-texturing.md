# 01 — Texturing

**Prerequisites:** [03 — Fog](../02-basic-lighting/03-fog.md) or any lit shader. You need diffuse, specular, and world position.

**Concepts:** UV coordinates, texture sampling, tiling, offset.

---

## What you'll learn

**UV coordinates** map a 2D texture onto a 3D surface. The vertex shader passes UVs via varyings; the fragment shader samples the texture with `SAMPLE_TEXTURE2D` in URP. **Tiling** repeats the texture; **offset** shifts the origin. The albedo (base color) comes from the texture instead of a solid color.

**Math:** UV transform: `uv' = uv * scale + offset`. `_BaseMap_ST.xy` = scale (tiling), `_BaseMap_ST.zw` = offset.

---

## Practical applications

- **Tiling** — floors, walls, terrain. Repeat a small texture to cover large areas without huge assets.
- **Detail maps** — second texture at higher frequency for close-up detail (e.g. scratches on metal).
- **Vertex colors** — foliage tinting, terrain painting, per-vertex variation. Multiply albedo by `COLOR` from the mesh.
- **Alpha cutout** — foliage, grilles, chain-link fences. Use `clip()` for hard edges when you don't need soft transparency.

---

## Implementation steps

### Step 1: Add UV to attributes and varyings

1. Add to `Attributes`:
   ```hlsl
   float2 uv : TEXCOORD0;
   ```

2. Add to `Varyings`:
   ```hlsl
   float2 uv : TEXCOORD3;  // Use a free slot if others exist
   ```

3. In the vertex shader:
   ```hlsl
   OUT.uv = IN.uv;
   ```

---

### Step 2: Declare and sample the texture

1. Add to Properties:
   ```hlsl
   _BaseMap ("Base Map", 2D) = "white" {}
   _BaseColor ("Tint", Color) = (1, 1, 1, 1)
   ```

2. Add to `CBUFFER_START`:
   ```hlsl
   float4 _BaseMap_ST;  // Unity sets this: xy = scale (tiling), zw = offset
   float4 _BaseColor;
   ```
   Declare the texture (after includes, outside CBUFFER):
   ```hlsl
   TEXTURE2D(_BaseMap);
   SAMPLER(sampler_BaseMap);
   ```
   `Core.hlsl` provides `TEXTURE2D`, `SAMPLER`, and `SAMPLE_TEXTURE2D`.

3. In the fragment shader:
   ```hlsl
   half4 baseMap = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv);
   half3 albedo = baseMap.rgb * _BaseColor.rgb;
   ```

4. **Verify:** Assign a texture to the material. The mesh displays the texture. Use the default `white` texture if none assigned.

---

### Step 3: Apply tiling and offset

Unity provides `_BaseMap_ST`: `xy` = tiling, `zw` = offset. In the vertex shader:

```hlsl
OUT.uv = IN.uv * _BaseMap_ST.xy + _BaseMap_ST.zw;
```

Or use the macro (if available from your includes):
```hlsl
OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);
```

5. **Verify:** Change Tiling and Offset in the material; the texture repeats or shifts.

---

### Step 4: Use alpha for cutout (optional)

If your texture has alpha, use it for alpha clip:

```hlsl
clip(baseMap.a - 0.5);  // Discard fragments where alpha < 0.5
```

Or multiply the final alpha by `baseMap.a` if you later add transparency.

---

### Step 5: Vertex colors (optional)

Meshes can store per-vertex color in `COLOR`. Use it to tint the albedo:

1. Add to `Attributes`:
   ```hlsl
   float4 color : COLOR;
   ```

2. Add to `Varyings`:
   ```hlsl
   float4 color : TEXCOORD4;
   ```

3. In the vertex shader: `OUT.color = IN.color;`

4. In the fragment shader:
   ```hlsl
   half3 albedo = baseMap.rgb * _BaseColor.rgb * IN.color.rgb;
   ```

**Verify:** Paint vertex colors in a 3D tool (Blender, Maya) or use a terrain painter. The mesh tints by vertex color.

---

### Step 6: Add a second texture (detail)

Add `_DetailMap` and `_DetailMap_ST`. Sample at scaled UVs (e.g. `IN.uv * 4`) and blend with albedo (e.g. multiply or overlay). This reinforces the tiling + sampling pattern.

---

## Unity setup checklist

- [ ] Mesh has UVs (built-in Cube, Sphere, Plane do). For vertex colors: mesh must have a Color channel.
- [ ] Texture import: sRGB for color maps, Repeat wrap mode for tiling.
- [ ] Material has a texture assigned to Base Map.

---

## Key takeaways

- `TEXCOORD0` — semantic for UV in Attributes; use `TEXCOORDn` in Varyings for interpolated data.
- `SAMPLE_TEXTURE2D(tex, sampler, uv)` — URP texture sampling.
- `_BaseMap_ST` — Unity auto-fills from material Tiling/Offset.
- `TRANSFORM_TEX(uv, _BaseMap)` — applies `uv * _BaseMap_ST.xy + _BaseMap_ST.zw`.

---

## Next

[02 — Normal Mapping](02-normal-mapping.md) — Add surface detail with normal maps.
