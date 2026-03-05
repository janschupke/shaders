# 03 — Toon / Cel Shading

**Prerequisites:** [02 — Fresnel & Rim Lighting](02-fresnel-rim.md) or any lit shader.

**Concepts:** Banded lighting, step/floor functions, toon shading.

---

## What you'll learn

**Toon** (cel) shading replaces smooth lighting gradients with discrete bands. Instead of N·L varying continuously, use `floor(NdotL * _Bands) / _Bands` or `step(threshold, NdotL)` to create sharp transitions. This gives a cartoon or comic look.

**Math:** `floor(x * n) / n` quantizes x into n equal steps. `step(a, x)` returns 0 if x < a, else 1 — a binary threshold. `smoothstep(a, b, x)` gives a smooth S-curve between a and b.

---

## Practical applications

- **Anime/cartoon style** — Zelda: Breath of the Wild, Genshin Impact, Guilty Gear. Discrete bands instead of smooth gradients.
- **Comic/cel look** — 2–4 bands for strong stylization; more bands for a softer toon feel.
- **Outline (inverted hull)** — character outlines, UI highlights, emphasis. Scale mesh along normals, cull front, dark color.
- **Combine with rim** — full stylized character pipeline: banded diffuse + banded specular + rim.

---

## Implementation steps

### Step 1: Band the diffuse term

1. Add to Properties:
   ```hlsl
   _Bands ("Light Bands", Range(2, 8)) = 4
   ```

2. After computing NdotL (or the diffuse factor):
   ```hlsl
   half NdotL = saturate(dot(normalWS, mainLight.direction));
   half banded = floor(NdotL * _Bands) / _Bands;
   half3 diffuse = mainLight.color * banded;
   ```

3. **Verify:** Lighting shows discrete steps instead of smooth gradients. Increase _Bands for finer steps.

---

### Step 2: Add a dark fill for shadows

Ensure the darkest band isn't pure black — add a small ambient:

```hlsl
banded = max(banded, 0.1);
```

Or use a `_ShadowColor` property for the shadow band.

---

### Step 3: Band the specular

Apply the same idea to specular for a toon highlight:

```hlsl
half specFactor = pow(saturate(dot(N, H)), _Shininess);
half bandedSpec = step(0.5, specFactor);  // Binary: on or off
half3 specular = mainLight.color * bandedSpec * _SpecColor.rgb;
```

Or use multiple bands with `floor(specFactor * _SpecBands) / _SpecBands`.

---

### Step 4: Outline (inverted hull, optional)

A simple outline: render the mesh slightly scaled along normals, with dark color and front-face culling, then render the main mesh on top.

1. First pass — outline:
   ```hlsl
   Cull Front
   // In vertex: expand position along normal
   float3 outlinePos = positionOS.xyz + normalOS * _OutlineWidth;
   ```
   Use a dark color (e.g. black). Cull front so only back faces (now visible from the front) render.

2. Second pass — main mesh:
   ```hlsl
   Cull Back
   ```
   Renders normally on top.

3. **Verify:** A dark outline appears around the mesh. Adjust _OutlineWidth.

---

### Step 5: Refine bands with smoothstep (optional)

For softer band edges:

```hlsl
half edge = 0.02;
half banded = smoothstep(bandCenter - edge, bandCenter + edge, NdotL);
```

Or use multiple smoothsteps for each band transition.

---

## Unity setup checklist

- [ ] Directional light for clear light/shadow split.
- [ ] Test on a sphere or character for visible bands.

---

## Key takeaways

- `floor(x * n) / n` — quantizes to n bands.
- `step(threshold, x)` — binary: 0 if x < threshold, 1 otherwise.
- Inverted hull outline: Cull Front, expand along normal, dark color.
- Toon works on diffuse and specular; combine with rim for a full stylized look.

---

## Next

[04 — Dissolve](04-dissolve.md) — Clip-based dissolve effect.
