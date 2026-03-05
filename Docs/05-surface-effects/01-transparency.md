# 01 — Transparency

**Prerequisites:** [01 — Texturing](../03-texturing/01-texturing.md) or any lit shader with texture sampling.

**Concepts:** Alpha blend, alpha clip, depth write, render queue, blend modes.

---

## What you'll learn

**Alpha clip** discards fragments below a threshold — hard edges, no blending. **Alpha blend** combines the fragment color with the framebuffer using the alpha channel — soft transparency. Transparency requires correct **render queue** (draw after opaque) and often **depth write off** so transparent objects don't occlude each other incorrectly. Blend mode (e.g. `SrcAlpha OneMinusSrcAlpha`) controls how colors combine.

**Math:** `final = src * src.a + dst * (1 - src.a)`. The source (fragment) and destination (framebuffer) are blended; alpha weights the contribution.

---

## Practical applications

- **Alpha clip** — foliage, grilles, chain-link, cutout characters. Hard edges, no sorting issues. Use when you need crisp silhouettes.
- **Alpha blend** — glass, water surface, holograms, smoke. Soft transparency; watch draw order and overlapping.
- **Additive blend** — particles, lens flares, magic effects. Accumulates light; use for glow that brightens the scene.

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

### Step 6: Two-sided / double-sided

For foliage, leaves, or thin surfaces, render both sides:

```hlsl
Cull Off
```

Use `Cull Off` in the alpha blend pass so back faces are visible. For alpha clip, `Cull Off` avoids disappearing when viewed from behind. For performance, prefer `Cull Back` when one-sided is enough.

---

### Step 7: Alpha-to-coverage (optional)

For alpha-clip foliage, **alpha-to-coverage** converts alpha to coverage for multisample anti-aliasing (MSAA), reducing jagged edges:

```hlsl
AlphaToMask On
```

Requires MSAA enabled in URP Asset. The fragment alpha is used as a coverage mask; works best with soft alpha gradients near the cutoff.

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
- `Cull Off` — render both sides (foliage, thin leaves).
- `AlphaToMask On` — alpha-to-coverage for softer alpha-clip edges with MSAA.
- Render queue controls draw order; Transparent is after Opaque.

---

## Next

[02 — Fresnel & Rim Lighting](02-fresnel-rim.md) — View-dependent edge lighting.
