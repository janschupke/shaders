# Shader Study Plan

A structured learning path for shader programming in **3D games and simulations**. Each document is self-contained with explanations, implementation steps, and Unity setup. Steps build on each other within and across documents.

**Approach:** Code-centric HLSL/ShaderLab (not Shader Graph).

---

## Numbered documents

| # | Document | Prerequisites | Topics |
|---|----------|---------------|--------|
| 01 | [01 — Fundamentals](01-fundamentals.md) | None | Pipeline, ShaderLab, HLSL, coordinate spaces |
| 02 | [02 — Diffuse Lighting](02-diffuse-lighting.md) | 01 | Lambert, N·L, URP lighting |
| 03 | [03 — Specular Lighting](03-specular-lighting.md) | 02 | Blinn-Phong, highlights, multi-light |
| 04 | [04 — Texturing](04-texturing.md) | 03 | UV mapping, sampling, tiling, offset |
| 05 | [05 — Normal Mapping](05-normal-mapping.md) | 04 | TBN, tangent-space normals |
| 06 | [06 — Transparency](06-transparency.md) | 05 | Alpha blend, alpha clip, depth, queue |
| 07 | [07 — Fresnel & Rim](07-fresnel-rim.md) | 06 | View-dependent edge lighting |
| 08 | [08 — Toon / Cel Shading](08-toon-cel-shading.md) | 07 | Banded lighting, outline |
| 09 | [09 — Dissolve](09-dissolve.md) | 08 | Clip, noise, burn edge |
| 10 | [10 — Vertex Displacement](10-vertex-displacement.md) | 01 | Procedural deformation, waves |
| 11 | [11 — UV Animation](11-uv-animation.md) | 04 | Scrolling, rotation, flow |
| 12 | [12 — PBR](12-pbr.md) | 05, 07 | Metallic-roughness, BRDF |
| 13 | [13 — Advanced](13-advanced.md) | 12 | Multi-pass, stencil, instancing, variants |

---

## Suggested order

**Core path (builds sequentially):**

1. **01** → **02** → **03** → **04** → **05** — Fundamentals through normal mapping  
2. **06** → **07** → **08** → **09** — Transparency and surface effects  
3. **12** — PBR (after 05, 07)  
4. **13** — Advanced (as needed)

**Parallel (can be done after 01 or 04):**

- **10** — Vertex displacement (needs 01)  
- **11** — UV animation (needs 04)

---

## Document structure

Each numbered document includes:

- **Prerequisites** — Prior documents to complete
- **Concepts** — What you'll learn
- **Implementation steps** — Code and Unity actions, ordered so each builds on the previous
- **Unity setup checklist** — Scene and project requirements
- **Key takeaways** — Summary
- **Next** — Link to the following document

---

## References

- [terminology.md](terminology.md) — Definitions of terms used across the plan
- [Unity Shader Reference](https://docs.unity3d.com/Manual/SL-Reference.html)
- [URP Shader Library](https://github.com/Unity-Technologies/Graphics/tree/master/Packages/com.unity.render-pipelines.universal/ShaderLibrary)
