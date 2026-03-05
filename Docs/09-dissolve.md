# 09 — Dissolve

**Prerequisites:** [08 — Toon / Cel Shading](08-toon-cel-shading.md) or any shader with texture sampling.

**Concepts:** Clip/discard, noise textures, animated cutout, burn edge.

---

## What you'll learn

A **dissolve** effect makes a surface appear to burn or disintegrate. A **noise texture** (or procedural noise) drives a threshold: fragments below the threshold are discarded with `clip()`. Animating the threshold over time creates the dissolve. A **burn edge** — a bright line at the cutoff — adds a glowing border.

---

## Implementation steps

### Step 1: Sample a noise texture

1. Add to Properties:
   ```hlsl
   _NoiseMap ("Noise Map", 2D) = "gray" {}
   _Dissolve ("Dissolve Amount", Range(0, 1)) = 0
   ```

2. Sample the noise in the fragment shader:
   ```hlsl
   half noise = SAMPLE_TEXTURE2D(_NoiseMap, sampler_NoiseMap, IN.uv).r;
   clip(noise - _Dissolve);
   ```

3. **Verify:** As you increase _Dissolve from 0 to 1, the mesh disappears in a noise-driven pattern. Use a grayscale noise texture (e.g. Perlin, Voronoi).

---

### Step 2: Animate from script

Create a small script that drives _Dissolve over time:

```csharp
material.SetFloat("_Dissolve", Mathf.PingPong(Time.time * 0.5f, 1f));
```

Or use a one-shot dissolve (0 → 1) for a death effect.

---

### Step 3: Add burn edge

1. Add Properties:
   ```hlsl
   _BurnWidth ("Burn Width", Range(0, 0.2)) = 0.05
   _BurnColor ("Burn Color", Color) = (1, 0.3, 0, 1)
   ```

2. After the clip, detect pixels near the threshold:
   ```hlsl
   clip(noise - _Dissolve);
   half edge = (noise - _Dissolve) / _BurnWidth;
   edge = saturate(edge);
   half3 burnColor = lerp(_BurnColor.rgb, albedo, edge);
   return half4(burnColor, 1);
   ```

3. **Verify:** A bright band appears at the dissolve edge. Adjust _BurnWidth for thickness.

---

### Step 4: Emissive burn (optional)

For a stronger glow, add the burn color additively:

```hlsl
half3 burn = _BurnColor.rgb * (1 - edge);
color += burn;
```

---

### Step 5: UV scale for noise

Scale the UVs for the noise texture so the pattern is finer or coarser:

```hlsl
float2 noiseUV = IN.uv * _NoiseScale;
half noise = SAMPLE_TEXTURE2D(_NoiseMap, sampler_NoiseMap, noiseUV).r;
```

---

## Unity setup checklist

- [ ] Noise texture: grayscale, Repeat wrap. Use a procedural or imported noise.
- [ ] Optional: Script to animate _Dissolve for testing.

---

## Key takeaways

- `clip(noise - threshold)` — discards fragments where noise < threshold.
- Dissolve = animated threshold over a noise texture.
- Burn edge = lerp or additive glow near the clip boundary.
- Works with any base shader; add after computing the base color.

---

## Next

[10 — Vertex Displacement](10-vertex-displacement.md) — Animate vertices in the vertex shader.
