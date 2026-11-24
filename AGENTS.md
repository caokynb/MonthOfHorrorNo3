# Unity Skills & Development Guidelines

## Unity Skills Rule
All official Unity skills from `https://github.com/Unity-Technologies/skills` are installed in `.agents/skills/`.

### Mandatory Workflow
1. **Search for relevant skills first**: Whenever assigned any game development task (physics, AI/navigation, rendering/URP, UI, audio, optimization, build, editor tooling, etc.), ALWAYS check `.agents/skills/` for a relevant skill matching the task.
2. **Read and apply on demand**: Read the skill's `SKILL.md` (and any related `references/` files) and follow its instructions, recipes, and verification steps.
3. **Use the Unity CLI**: The Unity CLI (`unity.exe`) is available. Use `unity command`, `unity status`, etc., to drive the open Unity Editor, run tests, or inspect scene objects when applicable instead of blind edits.

### Available Skills Index
- **Physics**:
  - `fix-3d-physics-collisions`: 3D PhysX collisions, triggers, raycasts, MeshCollider.
  - `create-2d-physics`: 2D Physics Core & Rigidbody2D.
- **AI & Navigation**:
  - `initialize-ai-navigation`: NavMesh surfaces, agents, obstacles, patrol routes, links.
- **Rendering & Graphics**:
  - `urp-postprocessing`: Volume framework, bloom, tonemapping, color grading, DoF.
  - `migrate-birp-to-urp`: Transitioning Built-in RP assets/materials to URP.
  - `shader-graph-create-custom-node`: HLSL custom nodes in Shader Graph via reflection.
  - `validate-urp-render-graph-renderer-feature`: Unity 6 Render Graph ScriptableRendererFeature review.
- **2D & Sprites**:
  - `create-tile-palette-or-rule-tiles`: Tile palettes, Rule Tiles, connected tiles.
  - `configure-2d-pixel-perfect`: Retro / pixel art camera alignment in URP/Built-in.
  - `manage-sprite-atlas`: SpriteAtlas master/variant configuration & packing.
  - `sprite-editor`: Slicing, pivots, borders via script.
  - `sprite-segment-3x3grid`: Analyzing sprite color/structure distribution.
- **UI & Input**:
  - `create-ui`: Menus, HUDs, editor windows across UI Toolkit, uGUI, or IMGUI.
- **Audio**:
  - `optimize-audio`: Memory, CPU, load types, sample rates, mono conversion.
  - `route-audio-to-mixer-groups`: AudioSource routing and mixer classification.
- **Game Loops & Polish**:
  - `craft-game-floors`: Core game loop, juice, sound, menus, polish, game feel.
- **Optimization & Diagnostics**:
  - `project-auditor-fixes`: Scan and fix project issues with Project Auditor.
  - `optimize-text-mesh-pro`: Font asset atlases, memory, fallbacks, SDF quality.
  - `optimize-web`: WebGL/WebGPU download and runtime performance.
- **Editor Tooling & Management**:
  - `unity-cli`: Command-line management, open editor control, scripting API lookup.
  - `unity-pipeline`: Driving and inspecting running Editor via com.unity.pipeline.
  - `unity-package-management`: Managing UPM packages from outside the Editor.
  - `generate-editor-search-query`: Unity Search queries inside the Editor.
  - `build-gtk`: Graph Toolkit editors and state machines (Unity 6.6+).
- **Multiplayer, LiveOps & Services**:
  - `build-multiplayer-game`: Netcode, matchmaking, sync, lag/desync troubleshooting.
  - `setup-vivox-voice-chat`: In-game voice and text chat.
  - `build-live-game`: Unity Gaming Services (Auth, Cloud Code, Leaderboards, Remote Config).
  - `implement-in-app-purchases`: In-App Purchases, subscriptions, receipt validation.
  - `levelplay-unity-integration`: LevelPlay ad mediation (interstitial, rewarded, banner).
  - `instrument-game-telemetry`: Insights Events game analytics and event instrumentation.
- **Localization & Assets**:
  - `localize-unity-project`: Unity Localization package, tables, CJK fonts.
  - `import-and-optimize-cad-models`: Pixyz Asset Transformer Toolkit for CAD/point clouds.
  - `manage-cloud-build`: Unity Build Automation targets and troubleshooting.
  - `create-xr`: OpenXR and XR Rig setup.
  - `new-unity-project`: Paved path for starting a new Unity game.
