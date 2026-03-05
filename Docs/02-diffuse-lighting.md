# 02 — Diffuse Lighting

**Prerequisites:** [01 — Fundamentals](01-fundamentals.md). You need vertex/fragment structure, normals, and world-space varyings.

**Concepts:** Lambert diffuse, N·L, directional lights, URP lighting API.

---

## What you'll learn

**Diffuse** lighting models how matte surfaces scatter light. The **Lambert** model: brightness ∝ max(0, **N**·**L**) — the dot product of the surface normal and light direction. Surfaces facing the light are bright; surfaces facing away are dark. URP provides `GetMainLight()` and `LightingLambert()` to implement this.

**Math:** `N·L = dot(N, L)` = cos(θ) when N and L are unit vectors. θ = 0° (surface faces light) → N·L = 1 (brightest). θ = 90° → N·L = 0. θ > 90° → N·L < 0, so we clamp with `max(0, N·L)` or `saturate(N·L)`.

---

## Implementation steps

### Step 1: Start from the fundamentals shader

Use your `UnlitColor` shader from 01, or create a copy. Ensure it has:
- `positionWS`, `normalWS` in `Varyings`
- `TransformObjectToWorld`, `TransformObjectToWorldNormal`, `GetCameraPositionWS`

---

### Step 2: Include URP lighting

1. Add after `Core.hlsl`:
   ```hlsl
   #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
   ```

2. Add to the fragment shader, before the return:
   ```hlsl
   Light mainLight = GetMainLight();
   half3 normalWS = normalize(IN.normalWS);
   half3 diffuse = LightingLambert(mainLight.color, mainLight.direction, normalWS);
   ```

3. Return `half4(diffuse, 1)` instead of `_BaseColor`.

4. **Verify:** The mesh is lit by the directional light. Rotate the light; shading updates.

---

### Step 3: Multiply by base color

Diffuse lighting gives light contribution. Multiply by the surface color (albedo):

1. Add `_BaseColor` back to Properties and `CBUFFER_START`.
2. In the fragment shader:
   ```hlsl
   half3 albedo = _BaseColor.rgb;
   half3 diffuse = LightingLambert(mainLight.color, mainLight.direction, normalWS);
   half3 color = albedo * diffuse;
   return half4(color, 1);
   ```

3. **Verify:** Changing `_BaseColor` tints the lit result.

---

### Step 4: Add ambient

Ambient light simulates indirect lighting. Add after computing `diffuse`:

```hlsl
half3 ambient = half3(0.1, 0.1, 0.15);  // Slight blue tint
half3 color = albedo * (diffuse + ambient);
```

**Verify:** Surfaces facing away from the light are no longer pure black.

---

### Step 5: Optional — Half-Lambert

Half-Lambert remaps N·L from [0,1] to [0.5,1] for a softer look:

```hlsl
half NdotL = saturate(dot(normalWS, mainLight.direction));
half halfLambert = NdotL * 0.5 + 0.5;
half3 diffuse = mainLight.color * halfLambert;
```

Compare with standard Lambert by toggling (e.g. via a `[Toggle]` property).

---

## Unity setup checklist

- [ ] Scene has a Directional Light (Rotation matters for direction).
- [ ] Light intensity > 0; color is white or tinted.
- [ ] Mesh has correct normals (built-in primitives are fine).

---

## Key takeaways

- Lambert: `max(0, N·L)` — surfaces perpendicular to light are brightest.
- `GetMainLight()` returns direction, color, distance, attenuation for the main directional light.
- `LightingLambert(lightColor, lightDir, normal)` — URP helper for diffuse.
- Ambient prevents completely black shadowed areas.

---

## Next

[03 — Specular Lighting](03-specular-lighting.md) — Add highlights with Blinn-Phong.
