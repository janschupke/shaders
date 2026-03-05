# Shader Study Plan

A structured learning path for shader programming in **3D games and simulations**. Each document is self-contained with explanations, implementation steps, math in-place, and Unity setup. Steps build on each other within and across documents.

**Approach:** Code-centric HLSL/ShaderLab (not Shader Graph).

---

## Numbered documents

| # | Document | Prerequisites | Topics |
|---|----------|---------------|--------|
| 01 | [01 — Fundamentals](01-fundamentals.md) | None | Pipeline, ShaderLab, HLSL, coordinate spaces, 3D math, debugging |
| 02 | [02 — Diffuse Lighting](02-diffuse-lighting.md) | 01 | Lambert, N·L, URP lighting |
| 03 | [03 — Specular Lighting](03-specular-lighting.md) | 02 | Blinn-Phong, point/spot attenuation |
| 04 | [04 — Texturing](04-texturing.md) | 03 | UV mapping, sampling, tiling, offset |
| 05 | [05 — Normal Mapping](05-normal-mapping.md) | 04 | TBN, tangent-space normals |
| 06 | [06 — Parallax & Triplanar](06-parallax-triplanar.md) | 05 | Height-based UV offset, UV-less projection |
| 07 | [07 — Transparency](07-transparency.md) | 06 | Alpha blend, alpha clip, depth, queue |
| 08 | [08 — Fresnel & Rim](08-fresnel-rim.md) | 07 | View-dependent edge lighting |
| 09 | [09 — Toon / Cel Shading](09-toon-cel-shading.md) | 08 | Banded lighting, outline |
| 10 | [10 — Dissolve](10-dissolve.md) | 09 | Clip, noise, burn edge |
| 11 | [11 — Vertex Displacement](11-vertex-displacement.md) | 01 | Procedural deformation, waves |
| 12 | [12 — UV Animation](12-uv-animation.md) | 04 | Scrolling, rotation, flow |
| 13 | [13 — PBR](13-pbr.md) | 05, 08 | Metallic-roughness, BRDF |
| 14 | [14 — Shadows](14-shadows.md) | 13 | Shadow mapping, shadow attenuation |
| 15 | [15 — Cubemaps & Environment](15-cubemaps-environment.md) | 13 | Reflections, environment sampling, skybox |
| 16 | [16 — Post-Processing](16-post-processing.md) | 01 | Blit, full-screen effects |
| 17 | [17 — Advanced](17-advanced.md) | 14–16 | Multi-pass, stencil, instancing, variants |

---

## Suggested order

**Core path (builds sequentially):**

1. **01** → **02** → **03** → **04** → **05** — Fundamentals through normal mapping  
2. **06** → **07** → **08** → **09** → **10** — Parallax, transparency, surface effects  
3. **13** — PBR (after 05, 08)  
4. **14** → **15** — Shadows, cubemaps  
5. **17** — Advanced (as needed)

**Parallel (can be done earlier):**

- **11** — Vertex displacement (after 01)  
- **12** — UV animation (after 04)  
- **16** — Post-processing (after 01)

---

## Document structure

Each numbered document includes:

- **Prerequisites** — Prior documents to complete
- **Concepts** — What you'll learn
- **Math** — In-place explanations (dot, cross, lerp, formulas) where relevant
- **Implementation steps** — Code and Unity actions, ordered so each builds on the previous
- **Unity setup checklist** — Scene and project requirements
- **Key takeaways** — Summary
- **Next** — Link to the following document

---

## References

- [terminology.md](terminology.md) — Definitions of terms used across the plan
- [Unity Shader Reference](https://docs.unity3d.com/Manual/SL-Reference.html)
- [URP Shader Library](https://github.com/Unity-Technologies/Graphics/tree/master/Packages/com.unity.render-pipelines.universal/ShaderLibrary)
