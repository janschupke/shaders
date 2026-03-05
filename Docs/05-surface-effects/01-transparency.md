# 01 — Transparency

**Prerequisites:** [01 — Parallax & Triplanar Mapping](../04-advanced-texturing/01-parallax-triplanar.md) or any lit, textured shader.

**Concepts:** Alpha blend, alpha clip, depth write, render queue, blend modes.

---

## What you'll learn

**Alpha clip** discards fragments below a threshold — hard edges, no blending. **Alpha blend** combines the fragment color with the framebuffer using the alpha channel — soft transparency. Transparency requires correct **render queue** (draw after opaque) and often **depth write off** so transparent objects don't occlude each other incorrectly. Blend mode (e.g. `SrcAlpha OneMinusSrcAlpha`) controls how colors combine.

**Math:** `final = src * src.a + dst * (1 - src.a)`. The source (fragment) and destination (framebuffer) are blended; alpha weights the contribution.

---

## Implementation steps

### Step 1: Alpha clip (cutout)

1. Add to Properties:
   ```hlsl
   _Cutoff ("Alpha Cutoff", Range(0, 1)) = 0.5
   ```

2. In the fragment shader, after sampling the base map:
   ```hlsl
   clip(baseMap.a - _Cutoff);
   ```

3. Add tags for the cutout pass:
   ```hlsl
   Tags { "RenderType" = "TransparentCutout" "Queue" = "AlphaTest" }
   ```

4. **Verify:** Use a texture with alpha (e.g. foliage, grille). Fragments with alpha < _Cutoff disappear. Adjust _Cutoff to control the cutout.

---

### Step 2: Alpha blend — setup

For soft transparency, use blending instead of clip.

1. Add a second Pass (or replace the pass) with:
   ```hlsl
   Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
   Pass
   {
       ZWrite Off
       Blend SrcAlpha OneMinusSrcAlpha
       Cull Off
   ```

2. `ZWrite Off` — don't write to the depth buffer (transparent objects don't occlude).
3. `Blend SrcAlpha OneMinusSrcAlpha` — standard alpha blending: `final = src * src.a + dst * (1 - src.a)`.
4. `Cull Off` — render both sides (optional; use `Cull Back` if you want back-face culling).

---

### Step 3: Use alpha from texture or color

Ensure your fragment shader returns alpha:

```hlsl
half4 baseMap = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv);
half4 color = half4(baseMap.rgb * _BaseColor.rgb, baseMap.a * _BaseColor.a);
return color;
```

**Verify:** Semi-transparent areas blend with the scene. Opaque objects behind show through.

---

### Step 4: Additive blend (optional)

For glow or light accumulation:

```hlsl
Blend One One
```

Or `Blend SrcAlpha One` for additive with alpha. Use for particles, lens flares.

---

### Step 5: Render queue and ordering

- `Queue = "Geometry"` (2000) — opaque.
- `Queue = "AlphaTest"` (2450) — alpha clip; still depth-tested.
- `Queue = "Transparent"` (3000) — blended; drawn after opaque.

Transparent objects are drawn back-to-front by distance when possible. For complex overlapping transparency, consider dithering or multiple passes.

---

## Unity setup checklist

- [ ] Texture has alpha channel (RGBA) for blend; or use a gradient/cutout texture.
- [ ] Material Render Queue matches your pass (Transparent = 3000).
- [ ] Scene has opaque geometry behind the transparent object to verify blending.

---

## Key takeaways

- `clip(x)` — discards fragment if x ≤ 0.
- `ZWrite Off` — skip depth write for transparent objects.
- `Blend SrcAlpha OneMinusSrcAlpha` — standard alpha blend.
- Render queue controls draw order; Transparent is after Opaque.

---

## Next

[02 — Fresnel & Rim Lighting](02-fresnel-rim.md) — View-dependent edge lighting.
