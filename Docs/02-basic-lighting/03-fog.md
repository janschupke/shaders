# 03 — Fog

**Prerequisites:** [02 — Specular Lighting](02-specular-lighting.md). You need a lit shader with world position.

**Concepts:** Distance fog, linear/exponential fog, URP fog integration.

---

## What you'll learn

**Fog** fades distant objects with a fog color for depth and atmosphere. **Linear fog** uses distance; **exponential fog** uses `exp(-density * distance)` for denser falloff. URP provides fog via `_FogColor`, `_FogParams`, and includes; you can also implement a simple custom fog.

**Math:** Linear: `fogFactor = saturate((distance - start) / (end - start))`. Exponential: `fogFactor = 1 - exp(-density * distance)`. Final color: `lerp(litColor, fogColor, fogFactor)`.

---

## Practical applications

- **Atmospheric depth** — distant objects fade into sky/horizon. Essential for outdoor scenes.
- **Mood** — fog color (gray, blue, orange) sets tone. Dense fog for horror.
- **URP integration** — enable fog in URP Asset; use `#pragma multi_compile_fog` and `MixFog` for built-in fog.

---

## Implementation steps

### Step 1: Add fog properties

1. Add to Properties:
   ```hlsl
   _FogColor ("Fog Color", Color) = (0.5, 0.6, 0.7, 1)
   _FogStart ("Fog Start", Range(0, 500)) = 10
   _FogEnd ("Fog End", Range(0, 1000)) = 200
   ```

2. Add to CBUFFER.

---

### Step 2: Compute fog factor

In the fragment shader, after computing the final lit color:

```hlsl
float3 positionWS = IN.positionWS;
float distance = length(GetCameraPositionWS() - positionWS);
float fogFactor = saturate((distance - _FogStart) / (_FogEnd - _FogStart));
half3 color = lerp(litColor, _FogColor.rgb, fogFactor);
return half4(color, 1);
```

**Verify:** Distant objects fade toward the fog color. Adjust start/end.

---

### Step 3: Exponential fog (optional)

For denser falloff:

```hlsl
_FogDensity ("Fog Density", Range(0, 0.1)) = 0.02
// ...
float fogFactor = 1.0 - exp(-_FogDensity * distance);
```

---

### Step 4: URP built-in fog

URP has built-in fog. Add to your pass:

```hlsl
#pragma multi_compile_fog
```

Include `Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderVariablesFunctions.hlsl` or the Lit shader's include chain. Then:

```hlsl
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderVariablesFunctions.hlsl"
// ...
half3 color = MixFog(litColor, IN.fogFactor);
```

Add `float fogFactor : TEXCOORDn` to Varyings and compute it in the vertex shader:

```hlsl
OUT.fogFactor = ComputeFogFactor(TransformWorldToHClip(positionWS).z);
```

Enable fog in URP Asset → Environment → Fog.

---

## Unity setup checklist

- [ ] URP Asset: Fog enabled if using built-in.
- [ ] Test with a long view (e.g. plane or terrain) to see fog falloff.

---

## Key takeaways

- Linear fog: `saturate((distance - start) / (end - start))`.
- Exponential: `1 - exp(-density * distance)`.
- `MixFog(color, fogFactor)` — URP helper for built-in fog.
- `#pragma multi_compile_fog` — enables fog variants.

---

## Next

[01 — Texturing](../03-texturing/01-texturing.md) — UV mapping and texture sampling.
