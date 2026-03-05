# 02 — Advanced Topics

**Prerequisites:** [02 — Shadows](../07-production-rendering/02-shadows.md), [03 — Cubemaps & Environment](../07-production-rendering/03-cubemaps-environment.md), [01 — Post-Processing](01-post-processing.md) or any prior shader. Each subsection can be tackled independently.

**Concepts:** Multi-pass, stencil buffer, GPU instancing, shader variants.

---

## 1. Multi-pass & stencil

### What you'll learn

Multiple **Pass** blocks run in sequence. The **stencil buffer** stores a per-pixel integer used to mask or mark regions. Common use: outline (first pass writes stencil, second pass draws outline where stencil differs).

### Steps

1. **Outline via stencil:**
   - Pass 1: Render object, write 1 to stencil where it draws. `Stencil { Ref 1 Comp Always Pass Replace }`
   - Pass 2: Render scaled object with dark color, only where stencil is 0. `Stencil { Ref 0 Comp Equal }` — draws only in background pixels.

2. **Stencil block syntax:**
   ```hlsl
   Stencil
   {
       Ref 1
       Comp Always
       Pass Replace
   }
   ```

3. **Verify:** Object has an outline. Adjust scale and color in Pass 2.

---

## 2. GPU instancing

### What you'll learn

**GPU instancing** draws many copies of a mesh in one draw call. The shader must support `UNITY_VERTEX_INPUT_INSTANCE_ID` and `UNITY_VERTEX_OUTPUT_STEREO` (for VR). Unity batches automatically when materials use the same shader and support instancing.

### Steps

1. Add to the `Attributes` struct:
   ```hlsl
   UNITY_VERTEX_INPUT_INSTANCE_ID
   ```

2. Add to the `Varyings` struct:
   ```hlsl
   UNITY_VERTEX_OUTPUT_STEREO
   ```

3. At the start of the vertex shader:
   ```hlsl
   UNITY_SETUP_INSTANCE_ID(IN);
   UNITY_TRANSFER_INSTANCE_ID(IN, OUT);
   ```

4. At the start of the fragment shader:
   ```hlsl
   UNITY_SETUP_INSTANCE_ID(IN);
   ```

5. Use `unity_InstanceID` if you need per-instance data (e.g. from a material property array or GPU buffer).

6. **Verify:** Place many objects with the same material; check Stats for reduced draw calls.

---

## 3. Shader variants & keywords

### What you'll learn

`#pragma multi_compile` and `#pragma shader_feature` create **shader variants** for different keyword combinations. Variants increase build size; `shader_feature` strips unused ones.

### Steps

1. Add a toggle property:
   ```hlsl
   [Toggle(_USE_RIM)] _UseRim ("Use Rim", Float) = 0
   ```

2. Add pragma:
   ```hlsl
   #pragma shader_feature _USE_RIM
   ```

3. In the fragment shader:
   ```hlsl
   #if defined(_USE_RIM)
       half3 rim = ...;
       color += rim;
   #endif
   ```

4. **Verify:** Toggling the property enables/disables rim. Build and check shader variant count.

5. **Multi_compile** for things that must always be present (e.g. fog, light count):
   ```hlsl
   #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE
   ```

---

## 4. SRP Batcher compatibility

### What you'll learn

The **SRP Batcher** batches draw calls when materials use the same shader and store per-material data in a `CBUFFER`. Declare all material properties in `CBUFFER_START(UnityPerMaterial)` / `CBUFFER_END`.

### Steps

1. Ensure all `_Property` variables used by the shader are inside:
   ```hlsl
   CBUFFER_START(UnityPerMaterial)
   float4 _BaseMap_ST;
   float4 _BaseColor;
   float _Metallic;
   // ... all material properties
   CBUFFER_END
   ```

2. Use `TEXTURE2D` and `SAMPLER` for textures; keep them outside the CBUFFER.

3. **Verify:** Window → Analysis → Frame Debugger — SRP Batcher shows as active when drawing with compatible materials.

---

## Key takeaways

- **Stencil:** Ref, Comp, Pass — control when and where to draw.
- **Instancing:** UNITY_VERTEX_INPUT_INSTANCE_ID, UNITY_SETUP_INSTANCE_ID.
- **Variants:** shader_feature (strip unused), multi_compile (keep all).
- **SRP Batcher:** CBUFFER for material data; avoid per-draw constant buffer changes.

---

## References

- [terminology.md](../terminology.md)
- [Unity Shader Reference](https://docs.unity3d.com/Manual/SL-Reference.html)
- [URP Lit Shader](https://github.com/Unity-Technologies/Graphics/blob/master/Packages/com.unity.render-pipelines.universal/Shaders/Lit.shader)
