using UnityEngine;

/// <summary>
/// 弾の所属陣営
/// </summary>
public enum Faction
{
    Player,
    Enemy
}

/// <summary>
/// 弾の移動と衝突処理担当
/// 生成側がInitialize() を呼び、方向や速度、ダメージや設定を判定
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    [Header("基本設定")]
    [Tooltip("弾の最大生存時間(秒) 画面の中に残った時用の保険")]
    [SerializeField] private float lifeTime = 6f;

    [Tooltip("画面外判定の余白 比率はビューボード比 0.1で画面の外側10%許容")]
    [SerializeField] private float offscreenMargin = 0.1f;

    private Rigidbody2D rb;
    private Camera cam;
    private Faction faction;
    private int damage;
    private bool initialized;
    private bool consumed; //二重ヒット防止用

    public Faction Faction => faction;
    public int Damage => damage;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        cam = Camera.main;
        if (cam == null)
        {
            Debug.LogWarning("[Projectile] Main Camera が見つかりません。MainCamera タグを確認してください。", this);
        }
        Destroy(gameObject, lifeTime);
    }

    /// <summary>生成直後に呼んで、弾の挙動を設定する。</summary>
    public void Initialize(Vector2 direction, float speed, int damage, Faction fanction)
    {
        this.damage = damage;
        this.faction = fanction;

        Vector2 dir = direction.sqrMagnitude > 0f ? direction.normalized : Vector2.up;
        rb.linearVelocity = dir * speed;

        //弾の向きを合わせる(spriteの上方向=前)
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0f,0f,angle);

        initialized = true;
    }

    private void Update()
    {
        if (!initialized || cam == null) return;

        //画面外出たら削除
        Vector3 vp = cam.WorldToViewportPoint(transform.position);
        float min = -offscreenMargin;
        float max = 1f + offscreenMargin;
        if (vp.x < min || vp.x > max || vp.y < min || vp.y > max)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!initialized || consumed) return;

        //Playerの弾がBossに命中
        if (faction == Faction.Player && other.CompareTag("Boss"))
        {
            BossHealth boss = other.GetComponentInParent<BossHealth>();
            if ( boss != null)
            {
                consumed = true;
                boss.TakeDamage(damage);
                Destroy(gameObject);
            }
        }

        //Bossの弾がPlayerに命中の処理は別
    }
}
