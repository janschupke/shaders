# 01 — Fundamentals

**Prerequisites:** None. Start here.

**Concepts:** Rasterization pipeline, vertex/fragment shaders, ShaderLab, HLSL, coordinate spaces.

---

## What you'll learn

The GPU renders 3D geometry by running **vertex shaders** (once per vertex), **rasterizing** triangles into pixels, then running **fragment shaders** (once per pixel). Unity wraps HLSL in **ShaderLab**, which defines properties, subshaders, and passes. Understanding **coordinate spaces** (object, world, clip) is essential for correct transforms and lighting.

---

## Implementation steps

### Step 1: Create a minimal URP unlit shader

1. In Unity, create `Assets/Shaders/01-Fundamentals/UnlitColor.shader`.
2. Paste the following:

```hlsl
Shader "Study/01-UnlitColor"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
            float4 _BaseColor;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionCS = TransformObjectToClipSpace(IN.positionOS.xyz);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                return half4(_BaseColor);
            }
            ENDHLSL
        }
    }
    FallBack "Universal Render Pipeline/Unlit"
}
```

3. Create a Material using this shader. Assign it to a cube in the scene.
4. **Verify:** The cube renders in the color set in the material.

---

### Step 2: Understand the pipeline

- **Attributes** — Per-vertex input from the mesh (`POSITION`, `NORMAL`, `TEXCOORD0`, etc.).
- **Vertex shader** — Transforms `positionOS` (object space) to `positionCS` (clip space) so the GPU can rasterize.
- **Varyings** — Data passed from vertex to fragment; interpolated across the triangle.
- **Fragment shader** — Returns the final color per pixel.

`TransformObjectToClipSpace` is a URP helper that does object → world → view → clip in one call.

---

### Step 3: Add world-space position to varyings

Extend the shader to pass world position and visualize it:

1. Add to `Varyings`:
   ```hlsl
   float3 positionWS : TEXCOORD0;
   ```

2. In the vertex shader, before setting `positionCS`:
   ```hlsl
   OUT.positionWS = TransformObjectToWorld(IN.positionOS.xyz);
   OUT.positionCS = TransformWorldToHClip(OUT.positionWS);
   ```

3. In the fragment shader, replace the return with:
   ```hlsl
   half3 col = IN.positionWS * 0.1;  // Scale down for visibility
   return half4(col, 1);
   ```

4. **Verify:** The cube shows a gradient based on world position (e.g. red/green/blue along X/Y/Z).

---

### Step 4: Add normal and view direction

1. Add `float3 normalOS : NORMAL;` to `Attributes`.
2. Add to `Varyings`:
   ```hlsl
   float3 normalWS : TEXCOORD1;
   float3 viewDirWS : TEXCOORD2;
   ```

3. In the vertex shader:
   ```hlsl
   OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
   OUT.viewDirWS = GetCameraPositionWS() - OUT.positionWS;
   ```

4. In the fragment shader, visualize the normal:
   ```hlsl
   half3 N = normalize(IN.normalWS);
   return half4(N * 0.5 + 0.5, 1);  // Remap [-1,1] to [0,1] for display
   ```

5. **Verify:** Each face shows a different color (normals facing different directions).

---

### Step 5: Restore base color and add a property

Revert the fragment shader to use `_BaseColor`. Add a `[Header]` in Properties and a `Range` slider for future use. Confirm you can change the color from the Material inspector.

---

## Unity setup checklist

- [ ] Project uses URP (Project Settings → Graphics).
- [ ] Scene has a Directional Light and a camera.
- [ ] Freefly camera or Scene view for inspection.
- [ ] Test mesh (Cube or built-in sphere) with your material.

---

## Key takeaways

- Vertex shader: object → clip space; pass data via varyings.
- Fragment shader: compute final color from interpolated data.
- `TransformObjectToClipSpace`, `TransformObjectToWorld`, `TransformWorldToHClip` — URP space helpers.
- `GetCameraPositionWS()` — camera position in world space.

---

## Next

[02 — Diffuse Lighting](02-diffuse-lighting.md) — Use the normal and light direction for Lambert shading.
