using UnityEngine;

/// <summary>
/// プレイヤーの自動射撃担当
/// 一定間隔で弾のprefubを生成、上方向へ発射
/// 弾の移動処理はProjectileで処理
/// </summary>
public class PlayerAutoShooter : MonoBehaviour
{
    [Header("弾の設定")]
    [Tooltip("発射する弾のprefub (Projecttileが付いたもの)")]
    [SerializeField] private Projectile bulletPrefab;

    [Tooltip("発射感覚(秒) 小さいと連射が速い")]
    [SerializeField] private float fireInterval = 0.15f;

    [Tooltip("弾速(ユニット/秒)")]
    [SerializeField] private float bulletspeed = 14f;

    [Tooltip("弾一発の攻撃力")]
    [SerializeField] private int bulletdamage = 1;

    [Header("発射位置")]
    [Tooltip("プレイヤーの中心からの発射のずれ")]
    [SerializeField] private Vector2 muzzleOffset = new Vector2(0f, 0.4f);

    private float timer;

    private void OnEnable()
    {
        //有効化直後にすぐ打てるように
        timer = 0f;
    }

    private void Update()
    {
        if (bulletPrefab == null) return;

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            Fire();
            timer = fireInterval;
        }
    }

    private void Fire()
    {
        Vector3 pos = transform.position + (Vector3)muzzleOffset;
        Projectile bullet = Instantiate(bulletPrefab, pos, Quaternion.identity);
        bullet.Initialize(Vector2.up, bulletspeed, bulletdamage, Faction.Player);
    }
}
