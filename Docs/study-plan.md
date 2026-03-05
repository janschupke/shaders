# Shader Study Plan

A modular learning path for shader programming in **3D games and simulations**. Modules build on each other; complexity increases across the path. Each document is self-contained with explanations, implementation steps, math in-place, and Unity setup.

**Approach:** Code-centric HLSL/ShaderLab (not Shader Graph).

---

## Module overview

| # | Topic | Prerequisites | Complexity | Documents |
|---|-------|--------------|------------|-----------|
| 01 | [Fundamentals](01-fundamentals/README.md) | None | ★☆☆☆☆ | 01 |
| 02 | [Basic Lighting](02-basic-lighting/README.md) | 01 | ★★☆☆☆ | 01, 02 |
| 03 | [Texturing](03-texturing/README.md) | 02 | ★★☆☆☆ | 01, 02 |
| 04 | [Advanced Texturing](04-advanced-texturing/README.md) | 03 | ★★★☆☆ | 01 |
| 05 | [Surface Effects](05-surface-effects/README.md) | 04 | ★★★☆☆ | 01, 02, 03, 04 |
| 06 | [Animation](06-animation/README.md) | 01, 03 | ★★★☆☆ | 01, 02 |
| 07 | [Production Rendering](07-production-rendering/README.md) | 03, 05 | ★★★★☆ | 01, 02, 03 |
| 08 | [Post-Processing & Advanced](08-post-processing-advanced/README.md) | 01, 07 | ★★★★★ | 01, 02 |

---

## Module dependency graph

```
01 (Fundamentals)
    │
    ├──► 02 (Basic Lighting)
    │         │
    │         └──► 03 (Texturing)
    │                   │
    │                   ├──► 04 (Advanced Texturing)
    │                   │         │
    │                   │         └──► 05 (Surface Effects)
    │                   │                   │
    │                   ├──► 06 (Animation)  ◄── 01
    │                   │
    │                   └──► 07 (Production Rendering)  ◄── 05
    │                             │
    └──► 08 (Post-Processing & Advanced)  ◄── 07
```

---

## Learning paths

**Core path (sequential):** 01 → 02 → 03 → 04 → 05 → 07 → 08

**With animation:** After 03, add 06 (can run in parallel with 04–05).

**Post-processing early:** 08 step 01 can be tried after 01 for a simple full-screen effect; full 08 assumes 07.

---

## Document index

| # | Document | Module |
|---|----------|--------|
| 01 | [01 — Fundamentals](01-fundamentals/01-fundamentals.md) | 01 |
| 02 | [01 — Diffuse Lighting](02-basic-lighting/01-diffuse-lighting.md) | 02 |
| 03 | [02 — Specular Lighting](02-basic-lighting/02-specular-lighting.md) | 02 |
| 04 | [01 — Texturing](03-texturing/01-texturing.md) | 03 |
| 05 | [02 — Normal Mapping](03-texturing/02-normal-mapping.md) | 03 |
| 06 | [01 — Parallax & Triplanar](04-advanced-texturing/01-parallax-triplanar.md) | 04 |
| 07 | [01 — Transparency](05-surface-effects/01-transparency.md) | 05 |
| 08 | [02 — Fresnel & Rim](05-surface-effects/02-fresnel-rim.md) | 05 |
| 09 | [03 — Toon / Cel Shading](05-surface-effects/03-toon-cel-shading.md) | 05 |
| 10 | [04 — Dissolve](05-surface-effects/04-dissolve.md) | 05 |
| 11 | [01 — Vertex Displacement](06-animation/01-vertex-displacement.md) | 06 |
| 12 | [02 — UV Animation](06-animation/02-uv-animation.md) | 06 |
| 13 | [01 — PBR](07-production-rendering/01-pbr.md) | 07 |
| 14 | [02 — Shadows](07-production-rendering/02-shadows.md) | 07 |
| 15 | [03 — Cubemaps & Environment](07-production-rendering/03-cubemaps-environment.md) | 07 |
| 16 | [01 — Post-Processing](08-post-processing-advanced/01-post-processing.md) | 08 |
| 17 | [02 — Advanced](08-post-processing-advanced/02-advanced.md) | 08 |

---

## References

- [terminology.md](terminology.md) — Definitions of terms used across the plan
- [Unity Shader Reference](https://docs.unity3d.com/Manual/SL-Reference.html)
- [URP Shader Library](https://github.com/Unity-Technologies/Graphics/tree/master/Packages/com.unity.render-pipelines.universal/ShaderLibrary)
