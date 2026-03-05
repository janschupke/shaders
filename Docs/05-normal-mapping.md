# 05 — Normal Mapping

**Prerequisites:** [04 — Texturing](04-texturing.md). You need UV sampling and a lit shader.

**Concepts:** Tangent-space normals, TBN matrix, normal map unpacking.

---

## What you'll learn

A **normal map** stores perturbed surface normals in a texture. Lighting uses these instead of the geometric normal to fake detail without extra geometry. Normals are usually in **tangent space**; you convert them to world space using the **TBN matrix** (Tangent, Bitangent, Normal). URP provides `TransformTangentToWorld` and `UnpackNormalScale` for this.

**Math:** Bitangent B = cross(N, T) * sign. The TBN matrix has rows [T, B, N]; transforming a tangent-space vector v to world space: `vWorld = v.x*T + v.y*B + v.z*N` = `mul(v, TBN)`.

---

## Implementation steps

### Step 1: Add tangent and normal to attributes

1. Add to `Attributes`:
   ```hlsl
   float4 tangentOS : TANGENT;
   float3 normalOS : NORMAL;
   float2 uv : TEXCOORD0;
   ```

2. Ensure your vertex shader already passes `normalWS` (from 01/02). You'll need `tangentOS` to build TBN.

---

### Step 2: Build the TBN matrix in the vertex shader

The TBN matrix converts from tangent space to world space. Add to `Varyings`:

```hlsl
float3 tangentWS : TEXCOORD4;
float3 bitangentWS : TEXCOORD5;
```

In the vertex shader:

```hlsl
float3 normalWS = TransformObjectToWorldNormal(IN.normalOS);
float3 tangentWS = TransformObjectToWorldDir(IN.tangentOS.xyz);
float sign = IN.tangentOS.w * unity_WorldTransformParams.w;  // Handedness
float3 bitangentWS = cross(normalWS, tangentWS) * sign;

OUT.normalWS = normalWS;
OUT.tangentWS = tangentWS;
OUT.bitangentWS = bitangentWS;
```

---

### Step 3: Add normal map property and sample

1. Add to Properties:
   ```hlsl
   _BumpMap ("Normal Map", 2D) = "bump" {}
   _BumpScale ("Normal Scale", Range(0, 2)) = 1
   ```

2. Declare `TEXTURE2D(_BumpMap)`, `SAMPLER(sampler_BumpMap)`, `float _BumpScale`, and `float4 _BumpMap_ST`.

3. In the fragment shader:
   ```hlsl
   half4 n = SAMPLE_TEXTURE2D(_BumpMap, sampler_BumpMap, IN.uv);
   half3 normalTS = UnpackNormalScale(n, _BumpScale);
   ```

`UnpackNormalScale` decodes the stored normal (DXT5nm or similar) and scales the XY by `_BumpScale`.

---

### Step 4: Transform normal to world space

Build the TBN matrix from the interpolated varyings and transform the tangent-space normal:

```hlsl
float3x3 TBN = float3x3(
    normalize(IN.tangentWS),
    normalize(IN.bitangentWS),
    normalize(IN.normalWS)
);
float3 normalWS = normalize(mul(normalTS, TBN));
```

Use `normalWS` (instead of `IN.normalWS`) in your lighting calculations.

---

### Step 5: Use the perturbed normal for lighting

Replace `inputData.normalWS` with your computed `normalWS`:

```hlsl
inputData.normalWS = normalWS;
```

**Verify:** With a normal map assigned, surface detail (bricks, scratches, etc.) affects lighting. Set `_BumpScale` to 0 to fall back to geometric normal.

---

### Step 6: Handle missing tangent (optional)

Some meshes lack tangents. Use `#if defined(_NORMALMAP)` and fall back to `IN.normalWS` when the normal map is not used. Add a `[Toggle(_NORMALMAP)]` property to toggle.

---

## Unity setup checklist

- [ ] Normal map texture: Import Settings → Texture Type = Normal map (or leave Default and ensure correct format).
- [ ] Mesh has tangents (built-in primitives do; custom meshes need "Generate" tangents in import).
- [ ] Use a test normal map (e.g. brick, tile) from Unity's Standard Assets or create a simple one.

---

## Key takeaways

- Normal maps store normals in tangent space; TBN converts to world space for lighting.
- `UnpackNormalScale(sample, scale)` — decodes and scales the normal.
- TBN = [T, B, N] in world space; `mul(normalTS, TBN)` transforms the normal.
- `tangentOS.w` and `unity_WorldTransformParams.w` handle handedness for mirrored UVs.

---

## Next

[06 — Parallax & Triplanar](06-parallax-triplanar.md) — Height-based UV offset and UV-less projection.
