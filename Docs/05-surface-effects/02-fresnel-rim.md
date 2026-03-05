# 02 — Fresnel & Rim Lighting

**Prerequisites:** [01 — Transparency](01-transparency.md) or any lit shader with view direction.

**Concepts:** Fresnel effect, view-dependent falloff, rim lighting.

---

## What you'll learn

The **Fresnel** effect describes how surfaces reflect more at grazing angles (edges) than when viewed head-on. **Rim lighting** uses this to brighten edges, separating objects from the background. The core term is `1 - max(0, N·V)` — stronger when the view direction V is perpendicular to the normal N (grazing angle).

**Math:** N·V = cos(θ) where θ is the angle between normal and view. At the center of a sphere, N ‖ V → N·V = 1 → fresnel = 0. At the rim, N ⊥ V → N·V = 0 → fresnel = 1. `pow(1 - NdotV, p)` sharpens the falloff; higher p = narrower rim.

---

## Implementation steps

### Step 1: Compute N·V in the fragment shader

You already have `normalWS` and `viewDirWS` from the lighting setup. Ensure they're normalized:

```hlsl
float3 N = normalize(IN.normalWS);
float3 V = normalize(IN.viewDirWS);
float NdotV = saturate(dot(N, V));
```

---

### Step 2: Fresnel term

The fresnel factor is stronger at edges:

```hlsl
float fresnel = 1.0 - NdotV;
```

At the center of a sphere (N parallel to V): NdotV ≈ 1, fresnel ≈ 0. At the rim (N perpendicular to V): NdotV ≈ 0, fresnel ≈ 1.

---

### Step 3: Add power for falloff control

A power curve sharpens the effect:

```hlsl
_RimPower ("Rim Power", Range(0.5, 8)) = 3
// ...
float fresnel = pow(1.0 - NdotV, _RimPower);
```

Higher power = narrower rim; lower = broader glow.

---

### Step 4: Add rim color and intensity

1. Add to Properties:
   ```hlsl
   _RimColor ("Rim Color", Color) = (0.5, 0.7, 1, 1)
   _RimStrength ("Rim Strength", Range(0, 2)) = 1
   ```

2. In the fragment shader:
   ```hlsl
   half3 rim = _RimColor.rgb * fresnel * _RimStrength;
   half3 color = albedo * diffuse + specular + rim;
   ```

3. **Verify:** Edges of the mesh glow with the rim color. Adjust power and strength.

---

### Step 5: Fresnel-Schlick (optional)

For a more physically inspired curve:

```hlsl
float f0 = 0.04;  // Base reflectivity
float fresnel = f0 + (1 - f0) * pow(1.0 - NdotV, 5);
```

Use this for PBR-style fresnel or more control.

---

### Step 6: Combine with transparency

For transparent materials, add the rim to the final color before applying alpha. The rim can use a different alpha (e.g. stronger at edges) for a glass-like effect.

---

## Unity setup checklist

- [ ] View direction is correctly passed from vertex to fragment.
- [ ] Normals are in world space (or consistent space with view direction).

---

## Key takeaways

- Fresnel: `1 - N·V` — stronger at grazing angles.
- `pow(1 - NdotV, power)` — power controls rim sharpness.
- Rim is additive — add to the final lit color.
- Used for edges, glass, holograms, and sci-fi effects.

---

## Next

[03 — Toon / Cel Shading](03-toon-cel-shading.md) — Stylized banded lighting.
