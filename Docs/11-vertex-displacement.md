# 11 — Vertex Displacement

**Prerequisites:** [01 — Fundamentals](01-fundamentals.md). You need vertex shader access to positions and normals.

**Concepts:** Vertex shader position modification, procedural motion, sine waves, noise.

---

## What you'll learn

The **vertex shader** can modify vertex positions before rasterization. Moving vertices creates waves, wobble, or deformation. Use **object space** or **world space** for consistent behavior. Common patterns: sine waves, noise-based displacement, and time-based animation using `_Time`.

**Math:** `sin(x)` oscillates in [-1, 1]. `sin(freq * pos + time)` gives a traveling wave. Finite-difference normal: for height h(x,z), approximate `normal ≈ (-∂h/∂x, 1, -∂h/∂z)` using `(h(x+ε)-h(x-ε))/(2ε)`.

---

## Implementation steps

### Step 1: Displace in object space

1. Add to Properties:
   ```hlsl
   _Displacement ("Displacement", Range(0, 1)) = 0.1
   _Frequency ("Frequency", Range(0.1, 5)) = 1
   _Speed ("Speed", Range(0, 5)) = 1
   ```

2. In the vertex shader, before transforming to clip space:
   ```hlsl
   float3 posOS = IN.positionOS.xyz;
   float time = _Time.y * _Speed;
   float wave = sin(posOS.x * _Frequency + time) * sin(posOS.z * _Frequency + time);
   posOS.y += wave * _Displacement;
   float3 positionWS = TransformObjectToWorld(posOS);
   OUT.positionCS = TransformWorldToHClip(positionWS);
   ```

3. **Verify:** A plane or grid shows a wave. Adjust frequency and displacement.

---

### Step 2: Pass displaced position to fragment

Update `positionWS` in Varyings from the displaced position so lighting uses the new geometry:

```hlsl
OUT.positionWS = positionWS;
```

Normals will be wrong (they're still from the original mesh). For correct lighting, recompute the normal — see Step 4.

---

### Step 3: Use world space for consistent phase

If your mesh is built from chunks or tiles, use world position so waves align at seams:

```hlsl
float3 positionWS = TransformObjectToWorld(IN.positionOS.xyz);
float wave = sin(positionWS.x * _Frequency + time) * sin(positionWS.z * _Frequency + time);
positionWS.y += wave * _Displacement;
OUT.positionWS = positionWS;
OUT.positionCS = TransformWorldToHClip(positionWS);
```

---

### Step 4: Recompute normals (optional)

For correct lighting on deformed geometry, approximate the normal from neighboring vertices. A simple approach: use finite differences. For a height map `h(x,z)`:

```hlsl
float eps = 0.01;
float hL = sin((positionWS.x - eps) * _Frequency + time) * sin(positionWS.z * _Frequency + time);
float hR = sin((positionWS.x + eps) * _Frequency + time) * sin(positionWS.z * _Frequency + time);
float hD = sin(positionWS.x * _Frequency + time) * sin((positionWS.z - eps) * _Frequency + time);
float hU = sin(positionWS.x * _Frequency + time) * sin((positionWS.z + eps) * _Frequency + time);
float3 normalWS = normalize(float3(hL - hR, 2 * eps, hD - hU));
OUT.normalWS = normalWS;
```

Use this when the displacement is significant.

---

### Step 5: Displacement from texture (optional)

Add `float2 uv : TEXCOORD0` to `Attributes` and pass it through `Varyings` if not already present. Sample a height map in the vertex shader:

```hlsl
float2 uv = IN.uv * _HeightMap_ST.xy + _HeightMap_ST.zw;
float height = SAMPLE_TEXTURE2D_LOD(_HeightMap, sampler_HeightMap, uv, 0).r;
float3 posOS = IN.positionOS.xyz + IN.normalOS * height * _Displacement;
```

Add `_HeightMap` (2D) and `_HeightMap_ST` to Properties and CBUFFER. `SAMPLE_TEXTURE2D_LOD` with LOD 0 avoids mip artifacts. Vertex texture sampling is supported in URP but check platform compatibility.

---

## Unity setup checklist

- [ ] Mesh has enough vertices (Plane with high subdivision, or use a grid).
- [ ] `_Time` is built-in; no declaration needed in URP Core.hlsl.

---

## Key takeaways

- Modify `positionOS` or `positionWS` in the vertex shader before computing `positionCS`.
- `_Time.y` — seconds since load; use for animation.
- World-space displacement keeps waves consistent across tiled geometry.
- Recompute normals when displacement significantly changes the surface.

---

## Next

[12 — UV Animation](12-uv-animation.md) — Animate UVs for scrolling textures.
