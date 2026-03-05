# 01 — Post-Processing

**Prerequisites:** [01 — Fundamentals](../01-fundamentals/01-fundamentals.md). Understanding of render targets and full-screen passes.

**Concepts:** Blit, render targets, full-screen effects, URP volume framework.

---

## What you'll learn

**Post-processing** applies effects to the final image after rendering (bloom, color grading, vignette, etc.). A **full-screen pass** renders a quad covering the screen and samples the camera's color buffer. URP uses **Volumes** and **Render Features** for built-in effects; custom effects use a **Blit** or a **RenderFeature** that draws a full-screen pass.

**Math:** Bloom: threshold bright pixels, blur, add back. Tone mapping: `x / (x + 1)` or similar to compress HDR to LDR. Vignette: darken based on distance from center: `1 - smoothstep(inner, outer, dist)`.

---

## Practical applications

- **Grayscale** — flashbacks, death screen, stylized moments. Luminance weights: 0.299R + 0.587G + 0.114B.
- **Vignette** — focus attention, cinematic feel. Stronger in horror or dramatic scenes.
- **Color grading** — exposure, contrast, saturation for mood. Warm for sunset, desaturated for bleak.
- **Bloom** — bright emissive glow; common in fantasy, sci-fi, and stylized games.

---

## Implementation steps

### Step 1: Create a simple full-screen shader

1. Create `Assets/Shaders/PostProcess/Grayscale.shader`:
   ```hlsl
   Shader "Study/PostProcess/Grayscale"
   {
       SubShader
       {
           Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }
           Pass
           {
               HLSLPROGRAM
               #pragma vertex Vert
               #pragma fragment Frag

               #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

               struct Attributes { float4 positionOS : POSITION; float2 uv : TEXCOORD0; };
               struct Varyings { float4 positionCS : SV_POSITION; float2 uv : TEXCOORD0; };

               Varyings Vert(Attributes IN)
               {
                   Varyings OUT;
                   OUT.positionCS = TransformObjectToClipSpace(IN.positionOS.xyz);
                   OUT.uv = IN.uv;
                   return OUT;
               }

               TEXTURE2D(_BaseMap);
               SAMPLER(sampler_BaseMap);

               half4 Frag(Varyings IN) : SV_Target
               {
                   half4 col = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv);
                   half gray = dot(col.rgb, half3(0.299, 0.587, 0.114));
                   return half4(gray, gray, gray, col.a);
               }
               ENDHLSL
           }
       }
   }
   ```

2. **Math:** Grayscale luminance: `0.299*R + 0.587*G + 0.114*B` approximates perceived brightness.

---

### Step 2: Blit from a script

Create a simple script that blits the camera target through your shader:

```csharp
void OnRenderImage(RenderTexture src, RenderTexture dst)
{
    Graphics.Blit(src, dst, grayscaleMaterial);
}
```

Or use URP's `ScriptableRenderPass` and `RenderFeature` to inject a blit pass — see URP's built-in post-processing for the pattern.

---

### Step 3: Vignette

Add a vignette by darkening the edges:

```hlsl
float2 uv = IN.uv - 0.5;
float dist = length(uv);
float vignette = 1.0 - smoothstep(_VignetteInner, _VignetteOuter, dist);
return col * vignette;
```

`smoothstep(a, b, x)` returns 0 for x ≤ a, 1 for x ≥ b, smooth in between.

---

### Step 4: Simple color grading

Exposure and contrast:

```hlsl
half3 color = col.rgb * _Exposure;
color = (color - 0.5) * _Contrast + 0.5;
return half4(saturate(color), col.a);
```

---

### Step 5: Bloom (threshold + blur + add)

Bloom extracts bright pixels, blurs them, and adds back for glow. Use HDR color buffer (URP: HDR enabled on camera).

1. **Threshold pass** — extract pixels above a brightness threshold:
   ```hlsl
   half luminance = dot(col.rgb, half3(0.299, 0.587, 0.114));
   half3 bloom = max(0, col.rgb - _BloomThreshold) * _BloomStrength;
   ```

2. **Blur** — use a separate pass with a 4-tap or 9-tap Gaussian. Sample offsets from `_BlurOffset` (e.g. 1/width, 1/height). Or use two passes: horizontal blur, then vertical.

3. **Add** — combine original and bloom:
   ```hlsl
   return half4(col.rgb + bloom, col.a);
   ```

**Simplified single-pass:** Threshold in the fragment shader and add a soft glow by sampling neighbors with small offsets. For production, use URP's built-in Bloom or a multi-pass blur (downsample → blur → upsample).

**Verify:** Emissive or bright areas glow. Requires HDR; adjust threshold and strength.

---

### Step 6: URP Volume framework (conceptual)

URP's post-processing uses `Volume` components and `VolumeProfile` assets. Effects like Bloom, Color Adjustments, Vignette are implemented as `VolumeComponent` scripts and corresponding shaders. To add a custom effect, create a `VolumeComponent` and a `ScriptableRenderPass` that blits with your shader. Reference the URP Post-Processing package for the full pattern.

---

## Unity setup checklist

- [ ] For `OnRenderImage`: Built-in pipeline or URP with compatibility. URP uses `RenderFeature` instead.
- [ ] For URP: Create a `RenderFeature` that adds a `ScriptableRenderPass`; use `cmd.Blit` with your material and the camera color target.
- [ ] Test with a simple scene to verify the effect applies.

---

## Key takeaways

- Full-screen pass: sample camera color, apply effect, output.
- `smoothstep(a, b, x)` — smooth S-curve for vignette, transitions.
- Bloom: threshold bright pixels → blur → add back. Requires HDR.
- Tone mapping: HDR → LDR (e.g. `x / (x + 1)`).
- URP: use `ScriptableRenderPass` and `RenderFeature` for custom post-processing.

---

## Next

[02 — Advanced](02-advanced.md) — Multi-pass, stencil, instancing, variants.
