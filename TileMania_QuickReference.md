# ⚡ TileMania — Quick Reference
## CS 4700: Game Design Studio | Unity 6 + C#

> Keep this open in a second window while you code!

---

## 🏃 Player Movement Snippets

### Basic Left/Right + Jump
```csharp
// In FixedUpdate
float h = Input.GetAxisRaw("Horizontal");
rb.linearVelocity = new Vector2(h * moveSpeed, rb.linearVelocity.y);

// In Update
if (Input.GetButtonDown("Jump") && isGrounded)
    rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
```

### Ground Check
```csharp
isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.15f, groundLayer);
```

### Better Jump Physics
```csharp
// In FixedUpdate — AFTER moving
if (rb.linearVelocity.y < 0)
    rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
else if (rb.linearVelocity.y > 0 && !Input.GetButton("Jump"))
    rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime;
```

### Sprite Flip
```csharp
if (horizontalInput > 0) spriteRenderer.flipX = false;
else if (horizontalInput < 0) spriteRenderer.flipX = true;
```

---

## 🎬 Animation Parameters

| Parameter | Type | Condition |
|---|---|---|
| `isRunning` | Bool | `Mathf.Abs(h) > 0.01f` |
| `isGrounded` | Bool | OverlapCircle result |
| `yVelocity` | Float | `rb.linearVelocity.y` |
| `die` | Trigger | Called on death |

### Set Animator Values
```csharp
animator.SetBool("isRunning", isRunning);
animator.SetFloat("yVelocity", rb.linearVelocity.y);
animator.SetTrigger("die");
```

---

## 🏹 Projectile System

### Instantiate Arrow
```csharp
GameObject arrow = Instantiate(arrowPrefab, firePoint.position, Quaternion.identity);
arrow.GetComponent<Arrow>().SetDirection(facingDirection); // 1f or -1f
```

### Arrow Movement
```csharp
// In Arrow.Start()
rb.linearVelocity = new Vector2(direction * speed, 0f);
Destroy(gameObject, maxLifetime);
```

### Arrow Hit Detection (OnTriggerEnter2D)
```csharp
void OnTriggerEnter2D(Collider2D other)
{
    if (other.TryGetComponent(out EnemyPatrol enemy)) { enemy.Die(); Destroy(gameObject); }
    else if (other.gameObject.layer == LayerMask.NameToLayer("Ground")) Destroy(gameObject);
}
```

---

## ❤️ Health System

### Take Damage (with invincibility frames)
```csharp
public void TakeDamage(int amount)
{
    if (isInvincible) return;
    currentHealth = Mathf.Max(0, currentHealth - amount);
    if (currentHealth <= 0) Die();
    else { isInvincible = true; invincibilityTimer = invincibilityDuration; }
}
```

### Update Heart UI
```csharp
for (int i = 0; i < heartImages.Length; i++)
    heartImages[i].sprite = (i < currentHealth) ? fullHeart : emptyHeart;
```

---

## 👾 Enemy AI

### Simple Patrol
```csharp
// In FixedUpdate
float dir = movingRight ? 1f : -1f;
rb.linearVelocity = new Vector2(dir * speed, rb.linearVelocity.y);

// Turn at waypoints
if (movingRight && transform.position.x >= rightEdge.position.x) movingRight = false;
if (!movingRight && transform.position.x <= leftEdge.position.x) movingRight = true;
```

### Ledge Detection (Turn before falling off)
```csharp
bool groundAhead = Physics2D.OverlapCircle(groundDetect.position, 0.2f, groundLayer);
if (!groundAhead) movingRight = !movingRight;
```

---

## 🏗️ GameSession Singleton

### Load Next Scene
```csharp
SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
```

### Reload Current Scene
```csharp
SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
```

### DontDestroyOnLoad Pattern
```csharp
void Awake()
{
    if (Instance != null) { Destroy(gameObject); return; }
    Instance = this;
    DontDestroyOnLoad(gameObject);
}
```

---

## 📷 Camera Shake

```csharp
// Trigger from any script
CameraShake.Instance.Shake(intensity: 1.5f, duration: 0.2f);

// Recommended values:
// Damage: Shake(2f, 0.3f)
// Death:  Shake(3f, 0.5f)
// Hit:    Shake(1f, 0.15f)
```

---

## 🔲 Layer Setup

```
Edit → Project Settings → Tags and Layers → Layers:
  6: Ground
  7: Player
  8: Enemy
  9: Hazard
 10: PlayerProjectile
 11: EnemyProjectile

Layer Mask in code:
  LayerMask.GetMask("Ground")
  LayerMask.NameToLayer("Ground")  // returns int
  1 << LayerMask.NameToLayer("Ground")  // returns bitmask
```

---

## ⏱️ Timing Utilities

```csharp
// Delay a method call
Invoke("MethodName", delaySeconds);

// Wait then execute (Coroutine)
IEnumerator WaitThenDo()
{
    yield return new WaitForSeconds(1f);
    DoSomething();
}
StartCoroutine(WaitThenDo());

// Destroy with delay
Destroy(gameObject, 0.8f);

// Fire rate limiter
float nextFireTime;
if (Time.time >= nextFireTime) { Shoot(); nextFireTime = Time.time + fireRate; }
```

---

## 🔊 Audio Calls

```csharp
// Play one-shot SFX (doesn't interrupt other sounds)
audioSource.PlayOneShot(clip);

// Play/Stop looping music
musicSource.Play();
musicSource.Stop();

// Via AudioManager singleton
AudioManager.Instance.PlayJump();
AudioManager.Instance.PlayShoot();
```

---

## 🧱 Tilemap Tips

| Action | How |
|---|---|
| Paint tiles | Tile Palette → Brush (B) → paint in Scene view |
| Erase tiles | Tile Palette → Erase (D) |
| Fill area | Tile Palette → Fill (G) |
| Select tilemap | MUST select the correct Tilemap in Hierarchy first |
| Fix Z-order | Tilemap Renderer → Order in Layer |
| Efficient collision | Tilemap Collider 2D + Composite Collider 2D + Used By Composite |

---

## 🐛 Common Bugs & Fixes

| Bug | Likely Cause | Fix |
|---|---|---|
| Player falls through floor | Composite Collider not set up | Add Composite Collider 2D, enable Used By Composite |
| Player jumps in air | Ground Check layer mask wrong | Check LayerMask in Inspector matches Ground layer |
| Arrow hits player | Layer matrix not configured | Physics 2D → Collision Matrix → uncheck PlayerProjectile × Player |
| Enemies collide with each other | Enemy layer collides with itself | Uncheck Enemy × Enemy in matrix |
| NullReferenceException | Unassigned serialized field | Check Inspector — drag missing reference |
| Animation stuck | Has Exit Time checked | Uncheck Has Exit Time on all transitions |
| Camera jitter | Multiple cameras active | Disable Main Camera or set Cinemachine priority |
| Sprite flicker (invincibility) | Correct — it's a feature! | Adjust flash speed: `Mathf.Sin(timer * 20f)` |
| Enemy falls off level | Missing ledge detection | Add GroundDetect child + OverlapCircle |
| DontDestroyOnLoad duplicate | Two GameSessions exist | Singleton Awake() destroys extra instances |

---

## ⌨️ Unity Keyboard Shortcuts

| Shortcut | Action |
|---|---|
| `W / E / R` | Move / Rotate / Scale tool |
| `Ctrl + P` | Play / Stop |
| `Ctrl + D` | Duplicate GameObject |
| `F` | Focus selected object in Scene view |
| `Alt + drag` | Pan Scene view |
| `Scroll wheel` | Zoom Scene view |
| `Ctrl + Z` | Undo |
| `Ctrl + Shift + Z` | Redo |
| `Ctrl + S` | Save scene |

---

## 📐 Physics Settings Cheat Sheet

| Setting | Recommended Value | Why |
|---|---|---|
| Rigidbody2D Gravity Scale | 3 | Snappier than default 1 |
| Fall Multiplier | 2.5 | Faster fall = better game feel |
| Low Jump Multiplier | 2.0 | Short-tap = small hop |
| Arrow Gravity Scale | 0 | Horizontal projectile |
| Collision Detection | Continuous | Prevents tunneling at speed |
| Interpolation | Interpolate | Smooth movement |

---

## 🎯 Design Patterns Used

```
Singleton:   GameSession, AudioManager, CameraShake
             One instance, accessible from anywhere

Component:   PlayerMovement, PlayerShooter, PlayerHealth
             Split responsibilities across focused scripts

Prefab:      Enemy, Arrow, Collectible
             Reusable object templates

Observer:    OnDeath callback
             Health script calls movement script to disable input
```

---

*TileMania Quick Reference | CS 4700: Game Design Studio | Unity 6 + C#*
