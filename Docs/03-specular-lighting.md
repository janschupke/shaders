# 03 — Specular Lighting

**Prerequisites:** [02 — Diffuse Lighting](02-diffuse-lighting.md). You need Lambert diffuse and URP lighting includes.

**Concepts:** Specular highlights, Blinn-Phong, N·H, shininess/roughness.

---

## What you'll learn

**Specular** lighting creates highlights where the surface reflects the light toward the viewer. **Blinn-Phong** uses the **halfway vector** H = normalize(L + V). The specular term is (N·H)^shininess — higher shininess = tighter highlight. URP provides `LightingBlinnPhong` and supports multiple lights.

**Math:** H is the vector halfway between light and view. When H aligns with N, the surface reflects light toward the camera → highlight. `(N·H)^p` with large p gives a sharp peak; small p gives a broad highlight.

---

## Implementation steps

### Step 1: Start from diffuse shader

Use your diffuse shader from 02. Ensure you have:
- `Lighting.hlsl`, `ShaderVariablesFunctions.hlsl`
- `GetMainLight()`, `LightingLambert`
- `_BaseColor` (albedo)

---

### Step 2: Add specular with Blinn-Phong

1. Add to Properties:
   ```hlsl
   _Shininess ("Shininess", Range(1, 128)) = 32
   _SpecColor ("Specular Color", Color) = (1, 1, 1, 1)
   ```

2. Add to `CBUFFER_START`:
   ```hlsl
   float _Shininess;
   float4 _SpecColor;
   ```

3. In the fragment shader, after computing diffuse:
   ```hlsl
   half3 normalWS = normalize(IN.normalWS);
   half3 viewDirWS = normalize(IN.viewDirWS);
   half3 specular = LightingBlinnPhong(mainLight.color, mainLight.direction,
       normalWS, viewDirWS, _SpecColor, _Shininess);
   half3 color = albedo * diffuse + specular;
   ```

4. **Verify:** A highlight appears where the surface reflects the light toward the camera. Adjust `_Shininess` — higher = smaller, sharper highlight.

---

### Step 3: Add additional lights (URP forward)

URP forward rendering supports multiple lights. Add after the main light:

```hlsl
uint pixelLightCount = GetAdditionalLightsCount();
for (uint i = 0; i < pixelLightCount; i++)
{
    Light light = GetAdditionalLight(i, IN.positionWS);
    half3 addDiffuse = LightingLambert(light.color, light.direction, normalWS);
    half3 addSpecular = LightingBlinnPhong(light.color, light.direction,
        normalWS, viewDirWS, _SpecColor, _Shininess);
    color += albedo * addDiffuse * light.distanceAttenuation + addSpecular * light.distanceAttenuation;
}
```

5. Add a second light (Point or Spot) in the scene. **Verify:** Both lights contribute.

---

### Step 3b: Point and spot light attenuation (math)

**Point lights** fade with distance. URP's `GetAdditionalLight` returns `distanceAttenuation` (0–1) and `shadowAttenuation`. The attenuation curve is typically: `1 / (1 + k·d²)` or similar — inverse-square falloff with a constant to avoid singularity at d=0.

**Spot lights** add an angular falloff: light is full strength inside the inner cone, fades to 0 at the outer cone. `dot(lightDir, spotDirection)` gives the angle; a smoothstep remaps the cone angles to attenuation.

You don't implement this manually — `light.distanceAttenuation` and `light.direction` already encode it. Just multiply your diffuse and specular by `light.distanceAttenuation` so point/spot lights correctly fade with distance and angle.

---

### Step 4: Combine diffuse and ambient

Ensure your final color includes ambient and doesn't double-count:

```hlsl
half3 ambient = half3(0.1, 0.1, 0.15);
half3 color = albedo * (diffuse + ambient) + specular;
// Then add additional light contributions in the loop
```

---

### Step 5: Expose specular intensity

Add a `_SpecularStrength` (Range 0–1) and multiply `specular` by it. This lets you make materials more or less glossy from the inspector.

---

## Unity setup checklist

- [ ] Main directional light present.
- [ ] Optional: Point or Spot light for multi-light test.
- [ ] URP asset: Additional Lights count > 0 if using extra lights.

---

## Key takeaways

- Blinn-Phong: highlight ∝ (N·H)^shininess. H = halfway between L and V.
- `LightingBlinnPhong(lightColor, lightDir, normal, viewDir, specColor, shininess)` — URP helper.
- Specular is additive (not multiplied by albedo) — highlights stay bright on dark surfaces.
- `GetAdditionalLight(i, positionWS)` — fetches per-pixel lights with attenuation.

---

## Next

[04 — Texturing](04-texturing.md) — UV mapping and texture sampling.
