# Explain Concept in 3D Graphics / Unity / Shaders Context

## Overview

Explain the topic provided by the user in the context of **3D graphics programming**, **Unity**, and **shaders**. The user will type a topic after the command (e.g. `/explain normal mapping` or `/explain clip space`). Use that as the subject of your explanation.

## Instructions

1. **Identify the topic** from the user's message (whatever follows the command name).

2. **Explain in 3D graphics / Unity / shaders context**, covering:
   - **What it is**: Clear definition and purpose
   - **Why it matters**: Problem it solves, visual or performance impact
   - **How it works**: Math, pipeline stage, or algorithm (as relevant)
   - **Unity / URP specifics**: How Unity or URP implements or exposes it (APIs, shader functions, conventions)
   - **Shader implementation**: How to use it in hand-written HLSL (code-centric, not Shader Graph)
   - **Practical tips**: Common pitfalls, best practices, related concepts

3. **Adapt depth** to the topic:
   - For broad terms (e.g. "PBR", "lighting"): High-level overview with key components and Unity/URP usage
   - For specific techniques (e.g. "normal mapping", "fresnel"): Include math, shader snippets, and URP integration
   - For low-level concepts (e.g. "clip space", "tangent space"): Explain coordinate spaces and transforms with Unity conventions

4. **Assume code-centric study**: Focus on `.shader`, `.hlsl`, `.cginc`—not Shader Graph nodes or visual scripting.

## Example Usage

- `/explain normal mapping` → Explain tangent-space normals, TBN matrix, sampling in HLSL, URP normal handling
- `/explain clip space` → Explain NDC, projection matrix, Unity's `UnityObjectToClipPos`
- `/explain PBR` → Explain metallic-roughness, BRDF, URP's PBR lighting functions
- `/explain vertex displacement` → Explain vertex shader modification, object vs world space, URP compatibility
