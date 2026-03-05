# 02 — Shadows

**Prerequisites:** [01 — PBR](01-pbr.md) or any lit shader. URP with shadows enabled.

**Concepts:** Shadow mapping, shadow sampling, shadow bias, main light shadows.

---

## What you'll learn

**Shadows** are cast when geometry blocks light. URP uses **shadow mapping**: the light renders depth from its view; at render time the shader compares fragment depth to the shadow map. `GetMainLight` returns `shadowAttenuation` (0 = in shadow, 1 = lit). You multiply diffuse and specular by this to darken shadowed areas.

**Math:** The fragment's position in light space is compared to the shadow map. `shadowAttenuation` is the result of that comparison (with PCF filtering for soft edges). Bias offsets the comparison to avoid shadow acne (self-shadowing artifacts).

---

## Practical applications

- **Directional light shadows** — sun/moon; primary source of depth and grounding in outdoor scenes.
- **Point/spot shadows** — flashlights, street lamps. More expensive; use sparingly for key lights.
- **Shadow acne** — adjust bias when you see speckled self-shadowing. Cascade resolution affects quality at distance.

---

## Implementation steps

### Step 1: Enable shadows in URP

1. Select your URP Asset (e.g. `PC_RPAsset`).
2. Main Light: Cast Shadows = On.
3. Shadow Resolution: 1024 or 2048.
4. **Verify:** The scene casts shadows in the viewport.

---

### Step 2: Add shadow coordinates to the shader

URP's `GetMainLight` can take a shadow coord. For the main directional light, you need the fragment position in shadow clip space:

1. Add to `Varyings`:
   ```hlsl
   float4 shadowCoord : TEXCOORD6;
   ```

2. In the vertex shader, after computing `positionWS` and `positionCS`:
   ```hlsl
   OUT.shadowCoord = TransformWorldToShadowCoord(OUT.positionWS);
   ```

3. Include `ShaderVariablesFunctions.hlsl` (or the URP Lit shader's include chain). `TransformWorldToShadowCoord` and `GetMainLight(shadowCoord)` come from the lighting/shadow helpers.

4. In the fragment shader, get the main light with shadows:
   ```hlsl
   Light mainLight = GetMainLight(IN.shadowCoord);
   ```

5. Multiply diffuse and specular by `mainLight.shadowAttenuation`:
   ```hlsl
   half3 diffuse = LightingLambert(mainLight.color, mainLight.direction, normalWS) * mainLight.shadowAttenuation;
   ```

6. **Verify:** Surfaces in shadow are darkened. Move objects to test.

---

### Step 3: Add shadow cascade keywords (URP)

For shadow cascades, URP uses `_MAIN_LIGHT_SHADOWS_CASCADE`. Add:

```hlsl
#pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE
#pragma multi_compile _ _SHADOWS_SOFT
```

These are typically already in URP Lit. If your shader doesn't receive shadows, check that these pragmas are present and that the main light has Cast Shadows enabled.

---

### Step 4: Additional light shadows (optional)

`GetAdditionalLight` also returns `shadowAttenuation` when the light casts shadows. Pass the shadow coord for each additional light (or use the built-in helpers). URP handles this in the Lit shader; for custom shaders, follow the Lit structure for shadow coords.

---

### Step 5: Shadow bias (optional)

If you see shadow acne (speckled self-shadowing), adjust the bias in the URP Asset: Main Light → Shadow Bias. Or use `ShadowCaster` pass parameters. Bias is a per-light setting, not a shader property.

---

## Unity setup checklist

- [ ] URP Asset: Main Light Cast Shadows = On.
- [ ] Directional Light: Cast Shadows = On.
- [ ] Scene has a plane or ground to receive shadows.
- [ ] Include `ShaderVariablesFunctions.hlsl` or equivalent for `TransformWorldToShadowCoord`.

---

## Key takeaways

- `GetMainLight(IN.shadowCoord)` returns light with `shadowAttenuation`.
- Multiply diffuse/specular by `shadowAttenuation` to darken shadowed areas.
- `TransformWorldToShadowCoord(positionWS)` — builds shadow coord for main light.
- `#pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE` — enables shadow variants.

---

## Next

[03 — Cubemaps & Environment](03-cubemaps-environment.md) — Reflections and skybox sampling.
