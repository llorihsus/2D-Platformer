# 🎮 TileMania — Complete Tutorial
## CS 4700: Game Design Studio | Unity 6 + C#

> **Build a challenging 2D platformer from scratch** — tile-based worlds, a running/jumping hero, patrolling enemies, projectile shooting, health systems, and polished game feel.

---

## 📋 Table of Contents

1. [Project Overview](#1-project-overview)
2. [Project Setup](#2-project-setup)
3. [Tilemap World Building](#3-tilemap-world-building)
4. [Player Movement & Physics](#4-player-movement--physics)
5. [Player Animation](#5-player-animation)
6. [Camera Follow with Cinemachine](#6-camera-follow-with-cinemachine)
7. [Collision Layers & Layer Matrix](#7-collision-layers--layer-matrix)
8. [Enemy Patrol AI](#8-enemy-patrol-ai)
9. [Projectile Shooting System](#9-projectile-shooting-system)
10. [Health & Damage System](#10-health--damage-system)
11. [Scene Management & Game States](#11-scene-management--game-states)
12. [Audio & Game Feel Polish](#12-audio--game-feel-polish)
13. [Creative Expansion Challenges](#13-creative-expansion-challenges)

---

## 1. Project Overview

### What You're Building
TileMania is a side-scrolling 2D platformer where a player navigates procedurally designed tile worlds, defeats enemies using projectiles, collects power-ups, and reaches the end goal. Think classic Metroidvania/Castlevania — tile art, crisp physics, and satisfying combat.

### Learning Objectives
| Skill | Concept |
|---|---|
| Tilemap System | Rule Tiles, Tile Palette, composite colliders |
| Player Physics | Rigidbody2D, ground detection, jump feel |
| Animation | Animator Controller, blend trees, transition conditions |
| Camera | Cinemachine Virtual Camera, confiner |
| AI | Patrol behavior, FlipX, trigger detection |
| Combat | Projectile pooling, raycasting, damage callbacks |
| Architecture | Singleton GameSession, SceneManager, event system |

### Final Game Features
- Tile-based world with Rule Tiles (auto-tiling)
- Smooth player run, jump, climb
- Patrolling enemies with flip behavior
- Player arrow shooting
- Health bar with lives system
- Hazard tiles (spikes, water)
- Win condition with scene transition
- Sound effects + background music

---

## 2. Project Setup

### Step 1 — Create the Unity Project

1. Open **Unity Hub** → click **New Project**
2. Select template: **2D (URP)**
3. Name: `TileMania`
4. Unity version: **6.x** (LTS recommended)
5. Click **Create Project**

### Step 2 — Install Required Packages

Open **Window → Package Manager** and install:

| Package | Purpose |
|---|---|
| **Cinemachine** | Smart camera follow |
| **2D Tilemap Editor** | Usually pre-installed in 2D template |
| **2D Tilemap Extras** | Rule Tiles (search "2D Tilemap Extras") |

> 💡 **2D Tilemap Extras** gives you Rule Tiles — the magic that makes tiles auto-connect.

### Step 3 — Folder Structure

In the **Project window**, create these folders inside `Assets/`:

```
Assets/
├── Art/
│   ├── Sprites/
│   ├── Tiles/
│   └── Animations/
├── Audio/
│   ├── Music/
│   └── SFX/
├── Prefabs/
├── Scripts/
│   ├── Player/
│   ├── Enemy/
│   ├── Combat/
│   └── Core/
└── Scenes/
```

### Step 4 — Import Art Assets

**Option A — Use Free Assets (Recommended for learning):**
- Download **Pixel Adventure** pack from itch.io (free): `https://pixelfrog-assets.itch.io/pixel-adventure-1`
- Import by dragging the folder into `Assets/Art/Sprites/`

**Option B — Use Unity's built-in sprites:**
- Right-click in Project → **Create → 2D → Sprites → Square** for placeholders

### Step 5 — Configure Sprites

https://hackernoon.com/creating-a-2d-platformer-in-unity-the-beginning 

For every character/enemy sprite sheet:
1. Select the sprite in Project window
2. In Inspector:
   - **Texture Type**: Sprite (2D and UI)
   - **Sprite Mode**: Multiple
   - **Pixels Per Unit**: 16 (for pixel art) or 100 (for HD art)
   - **Filter Mode**: Point (No Filter) — keeps pixel art crisp
3. Click **Apply**
4. Click **Sprite Editor** → **Slice → Automatic** → **Apply**
5. 

---

## 3. Tilemap World Building

### Understanding the Tilemap System

```
Scene Hierarchy Structure:
└── Grid (GameObject with Grid component)
    ├── Ground Tilemap     ← solid platforms
    ├── Hazards Tilemap    ← spikes, lava
    ├── Background Tilemap ← decorative, no collision
    └── Foreground Tilemap ← decorative, no collision
```

### Step 1 — Create the Grid

1. **Hierarchy → right-click → 2D Object → Tilemap → Rectangular**
2. Unity auto-creates a `Grid` parent with one `Tilemap` child
3. Rename the Tilemap child to **"Ground"**

### Step 2 — Create Additional Tilemaps

Right-click the `Grid` object → **2D Object → Tilemap → Rectangular** and create:
- **Hazards** (rename)
- **Background** (rename)

Arrange layer order so Background renders behind Ground:
- Select **Background Tilemap** → Tilemap Renderer → **Order in Layer: -1**
- Select **Ground Tilemap** → Order in Layer: 0

### Step 3 — Create a Rule Tile

Rule Tiles auto-select the correct tile variant based on neighboring tiles — no more manually placing corners!

1. Right-click in `Assets/Art/Tiles/` → **Create → 2D → Tiles → Rule Tile**
2. Name it `GroundRuleTile`
3. Open it in Inspector
4. Set **Default Sprite** to your main ground tile
5. Click **+** to add rules:

```
Rule Setup Example (for a 3x3 neighbor grid):
┌───┬───┬───┐
│ X │ ■ │ X │   ← neighbor IS same tile = ■
├───┼───┼───┤      neighbor is NOT = X
│ ■ │ ● │ X │      "this tile" = ●
├───┼───┼───┤
│ X │ X │ X │
└───┴───┴───┘
Sprite assigned: top-left corner variant
```

> 💡 **Pro tip**: Add 10–15 rules covering: top edge, bottom edge, left edge, right edge, all 4 corners, center (surrounded), and isolated.

### Step 4 — Open Tile Palette

1. **Window → 2D → Tile Palette**
2. **Create New Palette** → name `GroundPalette`
3. Drag your `GroundRuleTile` into the palette

### Step 5 — Paint Your Level

Select the **Brush tool (B)** in the Tile Palette window, then:
- Select the **Ground** Tilemap in Hierarchy
- Paint platforms in Scene view
- Use **Erase (D)** to remove tiles
- Use **Fill (G)** for large flat areas

**Level Design Tips:**
```
Good starter level layout:
─────────────────────────────────────── (sky)

   [==]          [====]      [==]       ← floating platforms
[=====]   gap  [=======]  gap [=====]  ← main ground
══════════════════════════════════════ ← floor
```

### Step 6 — Add Colliders to Tilemaps

1. Select the **Ground Tilemap** GameObject
2. **Add Component → Tilemap Collider 2D**
3. **Add Component → Composite Collider 2D**
   - This auto-adds a **Rigidbody2D** — set it to **Static**
4. On **Tilemap Collider 2D** → enable **Used By Composite**

> ✅ Composite Collider merges all individual tile colliders into one efficient shape — critical for performance!

**Repeat for Hazards Tilemap** (but we'll tag it differently later).

---

## 4. Player Movement & Physics

### Step 1 — Create the Player GameObject

1. **Hierarchy → Create Empty** → rename to `Player`
2. Set Transform Position to `(0, 2, 0)`
3. Add these components:
   - **Sprite Renderer** (assign your player sprite)
   - **Rigidbody2D**
   - **CapsuleCollider2D** (or BoxCollider2D)
   - **Animator**

**Rigidbody2D Settings:**
```
Body Type:        Dynamic
Collision Detection: Continuous   ← prevents tunneling at speed
Constraints:      Freeze Rotation Z ← stops player from tipping over
Gravity Scale:    3               ← snappier fall than default
```

**CapsuleCollider2D Settings:**
- Adjust `Size` to fit the character sprite
- `Direction`: Vertical

### Step 2 — Create PlayerMovement Script

Create `Assets/Scripts/Player/PlayerMovement.cs`:

```csharp
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // ─── Serialized Fields ──────────────────────────────────────────────
    [Header("Movement")]
    [SerializeField] float moveSpeed = 8f;
    [SerializeField] float jumpForce = 18f;

    [Header("Ground Check")]
    [SerializeField] Transform groundCheck;
    [SerializeField] float groundCheckRadius = 0.15f;
    [SerializeField] LayerMask groundLayer;

    [Header("Jump Feel")]
    [SerializeField] float fallMultiplier = 2.5f;   // faster fall
    [SerializeField] float lowJumpMultiplier = 2f;  // tap = small jump

    // ─── Private Variables ──────────────────────────────────────────────
    Rigidbody2D rb;
    Animator animator;
    SpriteRenderer spriteRenderer;

    bool isGrounded;
    bool isAlive = true;
    float horizontalInput;

    // ─── Lifecycle ───────────────────────────────────────────────────────
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (!isAlive) return;

        ReadInput();
        CheckGrounded();
        HandleJump();
        FlipSprite();
        UpdateAnimator();
    }

    void FixedUpdate()
    {
        if (!isAlive) return;
        Move();
        ApplyBetterJumpPhysics();
    }

    // ─── Input ───────────────────────────────────────────────────────────
    void ReadInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal"); // -1, 0, or 1
    }

    // ─── Movement ────────────────────────────────────────────────────────
    void Move()
    {
        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
    }

    // ─── Ground Check ────────────────────────────────────────────────────
    void CheckGrounded()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    // ─── Jump ────────────────────────────────────────────────────────────
    void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    // ─── Better Jump Feel (Apex trick) ───────────────────────────────────
    void ApplyBetterJumpPhysics()
    {
        if (rb.linearVelocity.y < 0)
        {
            // Falling — apply extra gravity
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
        else if (rb.linearVelocity.y > 0 && !Input.GetButton("Jump"))
        {
            // Released jump early — cut height
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime;
        }
    }

    // ─── Sprite Flip ────────────────────────────────────────────────────
    void FlipSprite()
    {
        if (horizontalInput > 0)
            spriteRenderer.flipX = false;
        else if (horizontalInput < 0)
            spriteRenderer.flipX = true;
    }

    // ─── Animator ────────────────────────────────────────────────────────
    void UpdateAnimator()
    {
        bool isRunning = Mathf.Abs(horizontalInput) > Mathf.Epsilon;
        animator.SetBool("isRunning", isRunning);
        animator.SetBool("isGrounded", isGrounded);
        animator.SetFloat("yVelocity", rb.linearVelocity.y);
    }

    // ─── Public API (called by other scripts) ───────────────────────────
    public void OnDeath()
    {
        isAlive = false;
        animator.SetTrigger("die");
        rb.linearVelocity = Vector2.zero;
        GetComponent<Collider2D>().enabled = false;
    }
}
```

### Step 3 — Set Up Ground Check

1. Right-click on Player in Hierarchy → **Create Empty** → rename `GroundCheck`
2. Set its local position to the **bottom** of the player: `(0, -0.5, 0)` (adjust to your sprite)
3. In `PlayerMovement` Inspector:
   - Drag `GroundCheck` into the **Ground Check** field
   - Set **Ground Layer** to your ground layer (we'll create this in Section 7)

> 💡 **Why OverlapCircle?** It detects ground reliably even on slopes, unlike a simple raycast downward.

### Step 4 — Ladder Climbing (Optional Extension)

```csharp
// Add to PlayerMovement.cs for ladder support
[Header("Climbing")]
[SerializeField] float climbSpeed = 5f;
[SerializeField] LayerMask ladderLayer;

bool isOnLadder;
float originalGravity;

void Start()
{
    // ... existing Start code ...
    originalGravity = rb.gravityScale;
}

void OnTriggerEnter2D(Collider2D other)
{
    if (((1 << other.gameObject.layer) & ladderLayer) != 0)
        isOnLadder = true;
}

void OnTriggerExit2D(Collider2D other)
{
    if (((1 << other.gameObject.layer) & ladderLayer) != 0)
    {
        isOnLadder = false;
        rb.gravityScale = originalGravity;
    }
}

void HandleClimb()
{
    if (!isOnLadder) return;

    float verticalInput = Input.GetAxisRaw("Vertical");
    rb.linearVelocity = new Vector2(rb.linearVelocity.x, verticalInput * climbSpeed);
    rb.gravityScale = (Mathf.Abs(verticalInput) > 0) ? 0f : originalGravity;
}
```

---

## 5. Player Animation

### Step 1 — Create Animation Clips

1. Select the **Player** GameObject
2. Open **Window → Animation → Animation**
3. Click **Create** → save as `Assets/Art/Animations/Player_Idle.anim`

**Create these clips:**

| Clip Name | Frames | Description |
|---|---|---|
| `Player_Idle` | 4–6 frames, 0.15s each | Standing still |
| `Player_Run` | 6–8 frames, 0.1s each | Running left/right |
| `Player_Jump` | 2–3 frames | Rising up |
| `Player_Fall` | 2–3 frames | Falling down |
| `Player_Death` | 5–8 frames | Death sequence |

**To add frames to a clip:**
1. Make sure the correct clip is selected in Animation window
2. Enable **Record mode** (red dot)
3. Drag sprite frames from Project into the Animation timeline
4. Press Play to preview

### Step 2 — Configure the Animator Controller

1. Double-click the **Animator** component → opens Animator window
2. You'll see the states auto-created from your clips

**Create Parameters** (+ button in Parameters tab):
- `isRunning` (Bool)
- `isGrounded` (Bool)
- `yVelocity` (Float)
- `die` (Trigger)

**Wire the Transitions:**

```
Idle ──[isRunning = true]──► Run
Run  ──[isRunning = false]──► Idle

Idle/Run ──[isGrounded = false, yVelocity > 0]──► Jump
Idle/Run ──[isGrounded = false, yVelocity < 0]──► Fall

Jump ──[yVelocity <= 0]──► Fall
Fall ──[isGrounded = true]──► Idle

Any State ──[die trigger]──► Death
```

**For every transition, uncheck "Has Exit Time"** unless you specifically want the animation to finish before transitioning.

### Step 3 — Transition Settings

Select each transition arrow and set:
```
Has Exit Time:   false (most transitions)
Transition Duration: 0    (instant snapping for platformers)
Conditions:      [your parameter condition]
```

---

## 6. Camera Follow with Cinemachine

### Step 1 — Add Cinemachine Virtual Camera

1. **GameObject → Cinemachine → 2D Camera** (or search "Cinemachine Virtual Camera")
2. In the Inspector:
   - **Follow**: Drag your `Player` GameObject
   - **Look At**: Drag your `Player` GameObject

### Step 2 — Configure 2D Settings

On the **CinemachineVirtualCamera** component:

```
Lens → Orthographic Size: 5  (adjust to taste)
Body → Transposer:
  Follow Offset: (0, 1, -10)   ← slight upward offset so player sees ahead
  X Damping:     0.5            ← smooth horizontal follow
  Y Damping:     0.2            ← snappier vertical
```

> Switch Body to **Framing Transposer** for more 2D-specific control:
> - Dead Zone Width/Height: 0.2 (small zone where camera doesn't move)
> - Soft Zone: 0.8

### Step 3 — Confine Camera to Level Bounds

1. Create an empty GameObject: `CameraConfiner`
2. Add a **Polygon Collider 2D** (or **Composite Collider 2D**)
3. Draw the bounds around your entire level
4. Set the collider **Is Trigger: true**
5. On the Virtual Camera:
   - Add Extension: **CinemachineConfiner2D**
   - Drag `CameraConfiner` into **Bounding Shape 2D**

### Step 4 — Camera Shake (Game Feel)

```csharp
// Add to a CameraShake.cs script on the Virtual Camera
using Cinemachine;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }

    CinemachineVirtualCamera vcam;
    CinemachineBasicMultiChannelPerlin noise;
    float shakeTimer;

    void Awake()
    {
        Instance = this;
        vcam = GetComponent<CinemachineVirtualCamera>();
        noise = vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    void Update()
    {
        if (shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;
            if (shakeTimer <= 0)
            {
                noise.m_AmplitudeGain = 0;
                noise.m_FrequencyGain = 0;
            }
        }
    }

    public void Shake(float intensity = 1f, float duration = 0.2f)
    {
        noise.m_AmplitudeGain = intensity;
        noise.m_FrequencyGain = 2f;
        shakeTimer = duration;
    }
}
```

**Usage:** Call `CameraShake.Instance.Shake(1.5f, 0.3f);` from any script when damage occurs.

---

## 7. Collision Layers & Layer Matrix

### Why Layers Matter
Without layers, enemies will collide with each other, projectiles will hit the player who fired them, and hazards will behave unpredictably.

### Step 1 — Create Layers

**Edit → Project Settings → Tags and Layers → Layers:**

| Index | Layer Name | Used By |
|---|---|---|
| 6 | Ground | Tilemap ground |
| 7 | Player | Player GameObject |
| 8 | Enemy | All enemy GameObjects |
| 9 | Hazard | Spike/lava tiles |
| 10 | PlayerProjectile | Arrows shot by player |
| 11 | EnemyProjectile | Projectiles from enemies |

### Step 2 — Assign Layers

- Select **Ground Tilemap** → Layer: **Ground**
- Select **Player** → Layer: **Player**
- For enemies: set Layer on the prefab

### Step 3 — Configure Layer Collision Matrix

**Edit → Project Settings → Physics 2D → Layer Collision Matrix**

Uncheck boxes to PREVENT collision:

```
                Ground | Player | Enemy | Hazard | PlayerProj | EnemyProj
Ground          ✓      | ✓      | ✓     | ✓      | ✓          | ✓
Player          ✓      | ✓      | ✓     | ✓      | ✗          | ✓
Enemy           ✓      | ✓      | ✗     | ✓      | ✓          | ✗
PlayerProj      ✓      | ✗      | ✓     | ✗      | ✗          | ✗
EnemyProj       ✓      | ✓      | ✗     | ✗      | ✗          | ✗
```

Key rules:
- **PlayerProjectile ✗ Player** — arrows don't hurt the shooter
- **Enemy ✗ Enemy** — enemies don't push each other
- **PlayerProjectile ✗ EnemyProjectile** — projectiles pass through each other

### Step 4 — Set Ground Check Layer

Back in `PlayerMovement`, set the `groundLayer` field in Inspector to **Ground**.

---

## 8. Enemy Patrol AI

### Step 1 — Create Enemy Prefab

1. **Hierarchy → Create Empty** → rename `Enemy`
2. Add components:
   - **Sprite Renderer** (assign enemy sprite)
   - **Rigidbody2D** (Gravity Scale: 3, Freeze Rotation Z)
   - **CapsuleCollider2D**
   - **Animator**
3. Set Layer to **Enemy**
4. Create prefab: drag into `Assets/Prefabs/`

### Step 2 — EnemyPatrol Script

Create `Assets/Scripts/Enemy/EnemyPatrol.cs`:

```csharp
using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    // ─── Settings ────────────────────────────────────────────────────────
    [Header("Patrol")]
    [SerializeField] float moveSpeed = 3f;
    [SerializeField] Transform leftEdge;
    [SerializeField] Transform rightEdge;

    [Header("Ground/Wall Detection")]
    [SerializeField] Transform groundDetect;
    [SerializeField] float detectRadius = 0.2f;
    [SerializeField] LayerMask groundLayer;

    // ─── State ───────────────────────────────────────────────────────────
    Rigidbody2D rb;
    SpriteRenderer spriteRenderer;
    Animator animator;

    bool movingRight = true;
    bool isAlive = true;

    // ─── Lifecycle ───────────────────────────────────────────────────────
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (!isAlive) return;
        CheckEdges();
    }

    void FixedUpdate()
    {
        if (!isAlive) return;
        Patrol();
    }

    // ─── Patrol Logic ────────────────────────────────────────────────────
    void Patrol()
    {
        float direction = movingRight ? 1f : -1f;
        rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);
        spriteRenderer.flipX = !movingRight;
    }

    void CheckEdges()
    {
        // Turn around at patrol waypoints
        if (movingRight && transform.position.x >= rightEdge.position.x)
            TurnAround();
        else if (!movingRight && transform.position.x <= leftEdge.position.x)
            TurnAround();

        // Turn around at ledge (no ground detected below)
        bool groundAhead = Physics2D.OverlapCircle(groundDetect.position, detectRadius, groundLayer);
        if (!groundAhead)
            TurnAround();
    }

    void TurnAround()
    {
        movingRight = !movingRight;
    }

    // ─── Death ───────────────────────────────────────────────────────────
    public void Die()
    {
        isAlive = false;
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 0f;
        GetComponent<Collider2D>().enabled = false;
        animator.SetTrigger("die");
        Destroy(gameObject, 0.8f);
    }
}
```

### Step 3 — Set Up Patrol Waypoints

1. Create two empty child objects on Enemy: `LeftEdge` and `RightEdge`
2. Position them at the patrol boundaries in Scene view
3. Drag them into the script fields in Inspector

Create a child `GroundDetect` at the front-bottom of the enemy for ledge detection.

### Step 4 — Enemy Hurt Player on Touch

Create `Assets/Scripts/Enemy/EnemyHurt.cs`:

```csharp
using UnityEngine;

public class EnemyHurt : MonoBehaviour
{
    [SerializeField] int damageAmount = 1;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
                playerHealth.TakeDamage(damageAmount);
        }
    }
}
```

---

## 9. Projectile Shooting System

### Step 1 — Create Arrow/Bullet Prefab

1. **Create Empty** → rename `Arrow`
2. Add:
   - **Sprite Renderer** (arrow sprite)
   - **Rigidbody2D** (Gravity Scale: 0 for horizontal arrow)
   - **BoxCollider2D** (Is Trigger: true)
   - **Arrow.cs** script (below)
3. Set Layer to **PlayerProjectile**
4. Save as prefab

### Step 2 — Arrow Script

Create `Assets/Scripts/Combat/Arrow.cs`:

```csharp
using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] float speed = 15f;
    [SerializeField] float maxLifetime = 3f;
    [SerializeField] int damage = 1;

    Rigidbody2D rb;
    float direction = 1f; // 1 = right, -1 = left

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        rb.linearVelocity = new Vector2(direction * speed, 0f);
        Destroy(gameObject, maxLifetime);
    }

    public void SetDirection(float dir)
    {
        direction = dir;
        // Flip sprite if going left
        if (dir < 0)
            GetComponent<SpriteRenderer>().flipX = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Hit enemy
        EnemyPatrol enemy = other.GetComponent<EnemyPatrol>();
        if (enemy != null)
        {
            enemy.Die();
            CameraShake.Instance?.Shake(1f, 0.15f);
            Destroy(gameObject);
            return;
        }

        // Hit ground/wall
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Destroy(gameObject);
        }
    }
}
```

### Step 3 — Player Shooter Script

Create `Assets/Scripts/Player/PlayerShooter.cs`:

```csharp
using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    [Header("Shooting")]
    [SerializeField] GameObject arrowPrefab;
    [SerializeField] Transform firePoint;
    [SerializeField] float fireRate = 0.3f;

    SpriteRenderer spriteRenderer;
    float nextFireTime;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1") && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    void Shoot()
    {
        // Determine direction based on which way the player is facing
        float direction = spriteRenderer.flipX ? -1f : 1f;

        GameObject arrow = Instantiate(arrowPrefab, firePoint.position, Quaternion.identity);
        arrow.GetComponent<Arrow>().SetDirection(direction);

        // Optional: play shoot animation
        GetComponent<Animator>()?.SetTrigger("shoot");
    }
}
```

### Step 4 — Wire Up Fire Point

1. Create an empty child object on Player: `FirePoint`
2. Position it at the tip of the player (where arrows appear): `(0.5f, 0.2f, 0)`
3. Drag it into `PlayerShooter`'s **Fire Point** field

---

## 10. Health & Damage System

### Step 1 — Player Health Script

Create `Assets/Scripts/Player/PlayerHealth.cs`:

```csharp
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    // ─── Settings ────────────────────────────────────────────────────────
    [Header("Health")]
    [SerializeField] int maxHealth = 3;
    [SerializeField] float invincibilityDuration = 1.5f;

    [Header("UI")]
    [SerializeField] Image[] heartImages;      // Array of heart UI images
    [SerializeField] Sprite fullHeart;
    [SerializeField] Sprite emptyHeart;

    [Header("Effects")]
    [SerializeField] GameObject deathParticles;

    // ─── State ───────────────────────────────────────────────────────────
    int currentHealth;
    bool isInvincible;
    float invincibilityTimer;

    PlayerMovement movement;
    SpriteRenderer spriteRenderer;

    // ─── Lifecycle ───────────────────────────────────────────────────────
    void Start()
    {
        currentHealth = maxHealth;
        movement = GetComponent<PlayerMovement>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateHeartUI();
    }

    void Update()
    {
        HandleInvincibility();
    }

    // ─── Invincibility Flash ─────────────────────────────────────────────
    void HandleInvincibility()
    {
        if (!isInvincible) return;

        invincibilityTimer -= Time.deltaTime;
        // Flash effect: toggle sprite visibility
        spriteRenderer.enabled = Mathf.Sin(invincibilityTimer * 20f) > 0;

        if (invincibilityTimer <= 0)
        {
            isInvincible = false;
            spriteRenderer.enabled = true;
        }
    }

    // ─── Public API ──────────────────────────────────────────────────────
    public void TakeDamage(int amount)
    {
        if (isInvincible) return;

        currentHealth -= amount;
        currentHealth = Mathf.Max(0, currentHealth);
        UpdateHeartUI();
        CameraShake.Instance?.Shake(2f, 0.3f);

        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        // Start invincibility window
        isInvincible = true;
        invincibilityTimer = invincibilityDuration;
    }

    void Die()
    {
        if (deathParticles != null)
            Instantiate(deathParticles, transform.position, Quaternion.identity);

        movement.OnDeath();
        Invoke(nameof(GameOver), 1f);  // Wait for death animation
    }

    void GameOver()
    {
        FindFirstObjectByType<GameSession>()?.ProcessPlayerDeath();
    }

    // ─── UI ──────────────────────────────────────────────────────────────
    void UpdateHeartUI()
    {
        for (int i = 0; i < heartImages.Length; i++)
        {
            heartImages[i].sprite = (i < currentHealth) ? fullHeart : emptyHeart;
        }
    }
}
```

### Step 2 — Game Session (Singleton)

Create `Assets/Scripts/Core/GameSession.cs`:

```csharp
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSession : MonoBehaviour
{
    // ─── Singleton ───────────────────────────────────────────────────────
    public static GameSession Instance { get; private set; }

    // ─── State ───────────────────────────────────────────────────────────
    [SerializeField] int lives = 3;
    int score = 0;

    void Awake()
    {
        // Singleton pattern — one GameSession persists across scenes
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // ─── Score ───────────────────────────────────────────────────────────
    public void AddScore(int points)
    {
        score += points;
        // TODO: Update score UI
    }

    public int GetScore() => score;

    // ─── Lives ───────────────────────────────────────────────────────────
    public void ProcessPlayerDeath()
    {
        lives--;
        if (lives > 0)
            ReloadCurrentScene();
        else
            LoadGameOver();
    }

    // ─── Scene Loading ───────────────────────────────────────────────────
    void ReloadCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void LoadGameOver()
    {
        Destroy(gameObject);  // Reset session
        SceneManager.LoadScene("GameOver");
    }

    public void LoadNextLevel()
    {
        int nextIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextIndex < SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadScene(nextIndex);
        else
            SceneManager.LoadScene("Win");
    }
}
```

### Step 3 — Hazard Damage

Create `Assets/Scripts/Combat/Hazard.cs` and attach to hazard tiles:

```csharp
using UnityEngine;

public class Hazard : MonoBehaviour
{
    [SerializeField] int damage = 999; // Instant kill by default

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerHealth>()?.TakeDamage(damage);
        }
    }
}
```

### Step 4 — Health Bar UI Setup

1. **Hierarchy → right-click → UI → Canvas**
   - Canvas Scaler: **Scale With Screen Size**, Reference 1920×1080
2. Inside Canvas, create a **Panel** for the HUD
3. Add **Image** components for each heart
4. Assign **Full Heart** and **Empty Heart** sprites
5. Drag heart Images into the `PlayerHealth` component's `heartImages` array

---

## 11. Scene Management & Game States

### Step 1 — Winning the Level

Create a goal object (flag, door, etc.) with `LevelExit.cs`:

```csharp
using UnityEngine;

public class LevelExit : MonoBehaviour
{
    [SerializeField] float exitDelay = 1f;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(LoadNextLevel());
        }
    }

    System.Collections.IEnumerator LoadNextLevel()
    {
        yield return new WaitForSeconds(exitDelay);
        GameSession.Instance?.LoadNextLevel();
    }
}
```

### Step 2 — Scene Build Settings

1. **File → Build Settings**
2. Add your scenes in order:
   ```
   Index 0: MainMenu
   Index 1: Level_01
   Index 2: Level_02
   Index 3: Level_03
   Index 4: GameOver
   Index 5: Win
   ```

### Step 3 — Main Menu

Create a simple Main Menu scene with a `MainMenu.cs`:

```csharp
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene(1); // Load Level_01
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
```

Wire `Button` onClick events to these methods in the Inspector.

### Step 4 — Pause Menu

```csharp
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject pausePanel;
    bool isPaused;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            TogglePause();
    }

    void TogglePause()
    {
        isPaused = !isPaused;
        pausePanel.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;
    }

    public void ResumeGame() => TogglePause();

    public void QuitToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
}
```

---

## 12. Audio & Game Feel Polish

### Step 1 — Audio Manager

Create `Assets/Scripts/Core/AudioManager.cs`:

```csharp
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Music")]
    [SerializeField] AudioSource musicSource;

    [Header("SFX")]
    [SerializeField] AudioSource sfxSource;

    [Header("Clips")]
    [SerializeField] AudioClip jumpSFX;
    [SerializeField] AudioClip shootSFX;
    [SerializeField] AudioClip hitSFX;
    [SerializeField] AudioClip deathSFX;
    [SerializeField] AudioClip collectSFX;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void PlayJump()  => sfxSource.PlayOneShot(jumpSFX);
    public void PlayShoot() => sfxSource.PlayOneShot(shootSFX);
    public void PlayHit()   => sfxSource.PlayOneShot(hitSFX);
    public void PlayDeath() => sfxSource.PlayOneShot(deathSFX);
    public void PlayCollect() => sfxSource.PlayOneShot(collectSFX);

    public void SetMusicVolume(float volume) => musicSource.volume = volume;
}
```

**Integrate sound calls:**
```csharp
// In PlayerMovement.HandleJump():
AudioManager.Instance?.PlayJump();

// In PlayerShooter.Shoot():
AudioManager.Instance?.PlayShoot();

// In PlayerHealth.TakeDamage():
AudioManager.Instance?.PlayHit();
```

### Step 2 — Collectibles (Coins / Gems)

Create `Assets/Scripts/Combat/Collectible.cs`:

```csharp
using UnityEngine;

public class Collectible : MonoBehaviour
{
    [SerializeField] int pointValue = 100;
    [SerializeField] GameObject collectEffect;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameSession.Instance?.AddScore(pointValue);
            AudioManager.Instance?.PlayCollect();
            if (collectEffect != null)
                Instantiate(collectEffect, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
```

### Step 3 — Particle Effects

**Death particles:**
1. **GameObject → Effects → Particle System**
2. Settings:
   - **Duration**: 0.5, **Looping**: false
   - **Start Lifetime**: 0.3–0.8
   - **Start Speed**: 3–8
   - **Start Color**: your blood/spark color
   - **Shape**: Sphere, Radius 0.1
   - **Emission**: Burst of 15 particles at time 0
3. Save as prefab, assign to `PlayerHealth.deathParticles`

### Step 4 — Screenshake Integration

Add calls wherever impact should be felt:
```csharp
CameraShake.Instance.Shake(intensity: 1.5f, duration: 0.2f);
```

| Event | Intensity | Duration |
|---|---|---|
| Player takes damage | 2.0 | 0.3s |
| Enemy dies | 1.0 | 0.15s |
| Player dies | 3.0 | 0.5s |
| Player lands from big fall | 0.5 | 0.1s |

---

## 13. Creative Expansion Challenges

You've built the core game. Now make it yours! Here are challenges ranked by difficulty:

### 🟢 Beginner
- **Double Jump** — add a second jump before landing, tracked with a bool
- **Animated Coins** — sprite animation on collectibles
- **Multiple Enemy Types** — reskin the enemy prefab with different speeds

### 🟡 Intermediate
- **Moving Platforms** — Lerp between two waypoints, player parented to platform on contact
- **Boss Enemy** — enemy with multiple hit points, unique attack pattern
- **Checkpoints** — save player spawn position, respawn there on death

```csharp
// Moving Platform snippet
public class MovingPlatform : MonoBehaviour
{
    [SerializeField] Transform[] waypoints;
    [SerializeField] float speed = 2f;
    int currentWaypoint;

    void Update()
    {
        Transform target = waypoints[currentWaypoint];
        transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, target.position) < 0.05f)
            currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
            col.transform.SetParent(transform);
    }

    void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
            col.transform.SetParent(null);
    }
}
```

### 🔴 Advanced
- **Procedural Level Generation** — randomize platform positions from a seed
- **Save System** — persist high score with `PlayerPrefs` or JSON serialization
- **Enemy Line of Sight** — enemy charges player when they enter view range

```csharp
// Line of sight detection snippet
void DetectPlayer()
{
    Vector2 dirToPlayer = (player.position - transform.position).normalized;
    float dist = Vector2.Distance(transform.position, player.position);

    if (dist < detectionRange)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dirToPlayer, dist, groundLayer | playerLayer);
        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            // Player is in line of sight — chase!
            isChasing = true;
        }
    }
}
```

---

## 🏁 Final Checklist Before Submission

```
Core Systems:
□ Tilemap level with at least 2 distinct platform areas
□ Player moves left/right and jumps
□ Jump feel is snappy (better jump physics applied)
□ Enemies patrol and turn at edges
□ Player can shoot projectiles
□ Health system with UI hearts
□ Death triggers respawn or game over
□ Level exit leads to next scene or win screen

Polish:
□ Animations for idle, run, jump, fall, death
□ Camera follows player with Cinemachine
□ Camera shakes on damage
□ Sound effects for at least 3 actions
□ Background music loops

Code Quality:
□ No magic numbers — use [SerializeField] for all tweakable values
□ Scripts organized in folders
□ GameObjects properly named in Hierarchy
□ Prefabs used for reusable objects
□ Layer matrix configured correctly
```

---

## 📚 Reference Concepts

### The Better Jump Physics Explained
```
Standard Unity jump:
  ↑ slow rise          ↓ slow fall = floaty = bad game feel

With fallMultiplier:
  ↑ quick rise         ↓ fast fall = snappy = great game feel

With lowJumpMultiplier:
  Hold Jump = full arc
  Tap Jump  = short hop  ← variable jump height = player control!
```

### Singleton Pattern
```csharp
// One instance exists across scenes. Others destroy themselves.
void Awake()
{
    if (Instance != null) { Destroy(gameObject); return; }
    Instance = this;
    DontDestroyOnLoad(gameObject);
}
```

### Object Lifetime Summary
```
Projectile:    Instantiate on shoot → Destroy on hit or timeout
Enemy:         Lives in scene → Die() → Destroy(gameObject, delay)
GameSession:   DontDestroyOnLoad → Destroyed only on game over reset
```

---

*TileMania Tutorial | CS 4700: Game Design Studio | Unity 6 + C#*
