# 02 — UV Animation

**Prerequisites:** [01 — Texturing](../03-texturing/01-texturing.md). You need UVs and texture sampling.

**Concepts:** UV scrolling, UV rotation, animated textures, flow maps.

---

## What you'll learn

**UV animation** changes texture coordinates over time. **Scrolling** adds a time-based offset so the texture moves (water, conveyor belts). **Rotation** spins the UVs around a pivot. Both are done in the vertex shader (or fragment) by modifying the UV before sampling.

**Math (rotation):** Rotate 2D point (x,y) by angle θ: `x' = x*cos(θ) - y*sin(θ)`, `y' = x*sin(θ) + y*cos(θ)`. Translate to pivot, rotate, translate back.

---

## Practical applications

- **Water, lava** — scroll two layers at different speeds for flowing liquid.
- **Conveyor belts, treadmills** — horizontal scroll; match speed to gameplay.
- **Rotating elements** — radar sweeps, loading spinners, magic circles.
- **Flow maps** — directional distortion for rivers, wind-blown grass, directional currents.

---

## Implementation steps

### Step 1: UV scroll

1. Add to Properties:
   ```hlsl
   _ScrollSpeed ("Scroll Speed (XY)", Vector) = (0.1, 0, 0, 0)
   ```

2. In the vertex shader (or fragment):
   ```hlsl
   float2 uv = IN.uv + _ScrollSpeed.xy * _Time.y;
   OUT.uv = uv;
   ```

3. **Verify:** The texture scrolls. Use (0.1, 0) for horizontal, (0, 0.1) for vertical.

---

### Step 2: Separate U and V speed

Use two Range properties for clarity:

```hlsl
_ScrollSpeedU ("Scroll U", Range(-2, 2)) = 0.5
_ScrollSpeedV ("Scroll V", Range(-2, 2)) = 0
// ...
float2 scroll = float2(_ScrollSpeedU, _ScrollSpeedV) * _Time.y;
OUT.uv = IN.uv + scroll;
```

---

### Step 3: UV rotation

Rotate UVs around a center (e.g. 0.5, 0.5):

```hlsl
float2 center = float2(0.5, 0.5);
float angle = _Time.y * _RotationSpeed;
float2 uv = IN.uv - center;
float s = sin(angle), c = cos(angle);
uv = float2(uv.x * c - uv.y * s, uv.x * s + uv.y * c) + center;
OUT.uv = uv;
```

**Verify:** The texture rotates around its center.

---

### Step 4: Two-layer scroll (e.g. water)

Use two UV sets or two scroll speeds for a layered effect:

```hlsl
float2 uv1 = IN.uv + _Scroll1 * _Time.y;
float2 uv2 = IN.uv * 2 + _Scroll2 * _Time.y;  // Different scale
half4 layer1 = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, uv1);
half4 layer2 = SAMPLE_TEXTURE2D(_DetailMap, sampler_DetailMap, uv2);
half4 combined = (layer1 + layer2) * 0.5;
```

---

### Step 5: Flow map (directional distortion)

A flow map stores 2D flow direction (RG). Use it to distort UVs over time:

```hlsl
half2 flow = SAMPLE_TEXTURE2D(_FlowMap, sampler_FlowMap, IN.uv).rg * 2 - 1;
float2 uvOffset = flow * _FlowStrength * frac(_Time.y);
OUT.uv = IN.uv + uvOffset;
```

`frac(_Time.y)` creates a repeating cycle; use for seamless looping.

---

## Unity setup checklist

- [ ] Texture wrap mode = Repeat for scrolling.
- [ ] Use a tiling texture (water, lava, stripes) to see the motion clearly.

---

## Key takeaways

- UV scroll: `uv + speed * _Time.y`
- UV rotation: translate to center, rotate, translate back.
- `frac(_Time.y)` — for looping animations.
- Flow maps: encode direction in RG, use to offset UVs.

---

## Next

[01 — PBR](../07-production-rendering/01-pbr.md) — Physically based rendering.
