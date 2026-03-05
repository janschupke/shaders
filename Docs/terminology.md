# Shader Terminology

Definitions of common terms used in the [study plan](study-plan.md). Math is explained in-place in the module steps; this file provides quick reference.

---

## Pipeline & stages

**Rasterization** — Process of converting geometry (triangles) into pixels. The GPU runs vertex shaders first, then rasterizes, then runs fragment shaders per pixel.

**Vertex shader** — Runs once per vertex. Transforms positions (e.g. object → clip space), computes per-vertex data passed to the fragment stage via **varyings**.

**Fragment shader** (pixel shader) — Runs once per pixel (or sample). Computes the final color written to the framebuffer.

**Varyings** — Data interpolated from the vertex shader to the fragment shader across the triangle (e.g. UVs, normals, world position).

---

## Coordinate spaces

**Object space** — Local to the mesh. Vertices as stored in the model.

**World space** — Global scene coordinates. Used for lighting and physics.

**View space** (camera space) — Relative to the camera. X right, Y up, Z toward camera.

**Clip space** — After projection. Used for rasterization. Coordinates outside [-1,1] are clipped.

**Screen space** — Pixel coordinates (0 to width/height).

**Tangent space** — Per-vertex space defined by tangent, bitangent, normal. Used for normal maps so they work when the surface is rotated.

---

## Lighting

**Diffuse** — Light scattered equally in all directions. Depends on angle between surface normal and light direction (Lambert: N·L).

**Specular** — Mirror-like reflection. Creates highlights. Phong uses reflection vector; Blinn-Phong uses halfway vector (N·H).

**Lambert** — Diffuse model: `max(0, N·L)`. No view dependence.

**Blinn-Phong** — Specular model using halfway vector H between view and light. Often paired with a power term for shininess.

**PBR** (Physically Based Rendering) — Lighting model that conserves energy and uses measured material properties (metallic, roughness) for realistic appearance.

**BRDF** (Bidirectional Reflectance Distribution Function) — Function describing how light reflects from a surface given incoming and outgoing directions.

**Fresnel** — View-dependent effect: surfaces reflect more at grazing angles. Used in PBR and rim lighting.

**Emissive** — Self-illumination. Surfaces emit light regardless of scene lights. Used for screens, magic glow, HDR bloom sources.

**Fog** — Distance-based atmospheric effect. Objects fade toward a fog color with distance. Linear or exponential falloff.

---

## Texturing

**UV** — 2D coordinates (u, v) mapping a texture onto a 3D surface. Typically 0–1 per face or chart.

**Normal map** — Texture storing surface normals (often in tangent space). Used to fake fine surface detail without extra geometry.

**TBN matrix** — Tangent–Bitangent–Normal matrix. Converts from tangent space to world space (or vice versa) for normal mapping.

**Parallax** — Technique that offsets UVs using a height map to simulate depth. Creates stronger depth illusion than normal mapping alone.

**Triplanar mapping** — Projects a texture from three axes (X, Y, Z) and blends. Avoids UV seams on procedural or complex geometry.

**Vertex color** — Per-vertex color stored in the mesh (COLOR attribute). Used in shaders to tint albedo (e.g. foliage, terrain painting).

**Detail map** — Second texture at higher UV scale for close-up detail. Blend with base albedo (multiply/overlay). **Detail normal** blends a second normal map for fine surface detail.

---

## Blending & transparency

**Alpha** — Fourth channel (A) in RGBA. Often used for transparency or cutout.

**Alpha blend** — Combines fragment color with existing framebuffer using alpha. Requires correct render order.

**Alpha clip** (cutout) — Discards fragments below a threshold. Binary transparency, no blending.

**Depth buffer** (Z-buffer) — Stores per-pixel depth. Used for occlusion. Transparent objects often skip depth write.

**Render queue** — Order in which objects are drawn. Opaque first, then transparent, to handle blending correctly.

**Alpha-to-coverage** — Converts alpha to MSAA coverage mask. Reduces jagged edges on alpha-clip foliage.

---

## Effects

**Rim lighting** — Brightening at edges where the surface meets the background. Often uses a Fresnel-like term.

**Toon / cel shading** — Stylized lighting with discrete bands instead of smooth gradients. Uses step or floor functions.

**Dissolve** — Effect where a surface appears to burn or dissolve away, often driven by a noise texture and clip.

**Stencil buffer** — Per-pixel mask. Can mark pixels (e.g. for outlines) or restrict rendering to certain areas.

**Bloom** — Post-processing effect that extracts bright pixels, blurs them, and adds back. Creates glow around emissive or bright areas. Requires HDR.

---

## Performance & build

**Shader variant** — A compiled version of a shader for a specific set of keywords (e.g. a lighting mode, platform).

**Multi-compile** — Compiles multiple variants of a shader. Increases build size and load time.

**Shader feature** — Like multi-compile but strips unused variants from builds. Good for optional features.

**GPU instancing** — Draws many copies of the same mesh in one draw call. Requires shader support (`UNITY_VERTEX_INPUT_INSTANCE_ID`).

**SRP Batcher** — Unity SRP optimization that batches draw calls with compatible materials without merging meshes.

---

## Unity / URP specific

**ShaderLab** — Unity’s shader wrapper language. Declares properties, subshaders, passes, and embeds HLSL.

**HLSL** — High-Level Shading Language. Used for the actual shader code in Unity (replacing Cg).

**URP** (Universal Render Pipeline) — Unity’s scalable forward renderer. Replaces the Built-in pipeline.

**Forward rendering** — Renders each object with all lights in one or few passes. Contrast with deferred, which separates geometry and lighting.

---

## Shadows & environment

**Shadow map** — Depth texture from the light's perspective. Fragments compare their depth in light space to the map; if behind, they're in shadow.

**Cubemap** — Six-faced texture (+X,-X,+Y,-Y,+Z,-Z) for environment sampling. Used for reflections and skybox.

**Reflection probe** — Captures the scene into a cubemap for reflections. Unity's `unity_SpecCube0` holds the active probe.
