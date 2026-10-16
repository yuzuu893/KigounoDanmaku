using UnityEngine;

/// <summary>
/// テスト用スクリプト のち削除
/// 一定間隔でプレイヤーに散弾を打つ
/// </summary>
public class EnemyBulletTester : MonoBehaviour
{
    [SerializeField] private Projectile enemyBulletPrefab;
    [SerializeField] private float fireInterval = 0.5f;
    [SerializeField] private float bulletSpeed = 5f;
    [SerializeField] private int bulletDamage = 1;

    private float timer;
    private Transform player;

    private void Start()
    {
        GameObject p = GameObject.FindWithTag("Player");
        if (p != null) player = p.transform;
        else Debug.LogWarning("[EnemyBullettester] Playerタグのオブジェクトが見つかりません");
    }

    private void Update()
    {
        if (enemyBulletPrefab == null || player == null) return;

        timer -= Time.deltaTime;
        if (timer < 0)
        {
            Vector2 dir = (Vector2)(player.position - transform.position);
            Projectile b = Instantiate(enemyBulletPrefab, transform.position, Quaternion.identity);
            b.Initialize(dir, bulletSpeed, bulletDamage, Faction.Enemy);
            timer = fireInterval;
        }
    }
}
