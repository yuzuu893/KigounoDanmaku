using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

///<summary>弾幕の種類</summary>
public enum PatternType
{
    Radial,//全方向
    Fan,//扇
    Aimed,//狙い撃ち
    Rotating, // 回転
    Lines,    // 列状・格子状
    Chaotic   // 不確定（角度・弾速がランダム）
}

/// <summary>
/// 弾幕パターン1用データ
/// 実行中に値を書き換えないよう注意
/// </summary>
[CreateAssetMenu(fileName ="Pattern_New", menuName ="Symbolbullet/Bullet Pattern")]
public class BulletPatternData : ScriptableObject
{
    [Header("共通設定")]
    [Tooltip("弾幕の種類。")]
    [SerializeField] private PatternType type = PatternType.Radial;

    [Tooltip("発射する弾のPrefab(Projectile付き)")]
    [SerializeField] private Projectile bulletPrefab;

    [Tooltip("発射間隔(秒)")]
    [SerializeField] private float fireInterval = 1.0f;

    [Tooltip("弾速(ユニット/秒)")]
    [SerializeField] private float bulletSpeed = 4f;

    [Tooltip("弾一発のダメージ")]
    [SerializeField] private int damage = 1;

    [Tooltip("一回の発射で出す球数")]
    [Min(1)]
    [SerializeField] private int bulletCount = 12;

    [Header("全方向の設定")]
    [Tooltip("最初の弾の角度")]
    [SerializeField] private float startAngle = 90f;

    [Tooltip("発射1回ごとに角度をずらす量")]
    [SerializeField] private float rotationPerShot = 10f;

    [Header("扇形の設定")]
    [Tooltip("扇の広がり 弾の端から端までの角度")]
    [Range(0f, 360f)]
    [SerializeField] private float fanSpread = 60f;

    [Tooltip("ONにすると扇の中心をプレイヤーに、OFFなら固定角度")]
    [SerializeField] private bool fanAimAtPlayer = false;

    [Tooltip("扇の中心角度")]
    [SerializeField] private float fanCenterAngle = 270f;

    [Header("狙い撃ちの設定")]
    [Tooltip("弾の数の補正は不要、bulletCount発を同方向へ撃つ")]
    [Min(0f)]
    [SerializeField] private float aimedSpreadPerBullet = 0f;

    [Header("列状・格子状の設定")]
    [Tooltip("弾の進む方向（度）。270=下。")]
    [SerializeField] private float lineDirection = 270f;

    [Tooltip("列の弾同士の間隔（ユニット）。プレイヤーが通れる広さにすること。")]
    [Min(0.1f)]
    [SerializeField] private float lineSpacing = 1.3f;

    [Tooltip("ONにすると、発射のたびに列を半間隔ずらす（格子状になる）。")]
    [SerializeField] private bool lineStagger = true;

    [Header("不確定の設定")]
    [Tooltip("弾の角度の最小値（度）。")]
    [SerializeField] private float chaosAngleMin = 200f;

    [Tooltip("弾の角度の最大値（度）。")]
    [SerializeField] private float chaosAngleMax = 340f;

    [Tooltip("弾速の最小値。")]
    [SerializeField] private float chaosSpeedMin = 2f;

    [Tooltip("弾速の最大値。")]
    [SerializeField] private float chaosSpeedMax = 6f;

    public float FireInterval => fireInterval;

    ///<summary>弾幕を1回発射する targetPosition は狙う対象（Player）の位置</summary>
    public void Fire(Vector2 origin, Vector2? targetPosition, int shotIndex)
    {
        if (bulletPrefab == null)
        {
            Debug.LogWarning($"[BulletPatternData] {name}にBullet Prefabが追加されてません");
            return;
        }

        switch (type)
        {
            case PatternType.Radial: FireRadial(origin, startAngle); break;
            case PatternType.Fan: FireFan(origin, targetPosition); break;
            case PatternType.Aimed: FireAimed(origin, targetPosition); break;
            case PatternType.Rotating: FireRadial(origin, startAngle + rotationPerShot * shotIndex); break;
            case PatternType.Lines: FireLines(origin, shotIndex); break;
            case PatternType.Chaotic: FireChaotic(origin); break;
        }
    }

    // 全方向・回転：360度を弾数で等分（基準角度を指定）
    private void FireRadial(Vector2 origin, float baseAngle)
    {
        float step = 360f / bulletCount;
        for (int i = 0; i < bulletCount; i++)
        {
            SpawnBullet(origin, baseAngle + step * i, bulletSpeed);
        }
    }

    // 扇状
    private void FireFan(Vector2 origin, Vector2? target)
    {
        float center = fanCenterAngle;
        if (fanAimAtPlayer && target.HasValue)
        {
            center = AngleTo(origin, target.Value);
        }

        if (bulletCount == 1)
        {
            SpawnBullet(origin, center, bulletSpeed);
            return;
        }

        float step = fanSpread / (bulletCount - 1);
        float first = center - fanSpread * 0.5f;
        for (int i = 0; i < bulletCount; i++)
        {
            SpawnBullet(origin, first + step * i, bulletSpeed);
        }
    }

    // 狙い撃ち
    private void FireAimed(Vector2 origin, Vector2? target)
    {
        if (!target.HasValue) return;

        float center = AngleTo(origin, target.Value);

        if (bulletCount == 1 || aimedSpreadPerBullet <= 0f)
        {
            SpawnBullet(origin, center, bulletSpeed);
            return;
        }

        float first = center - aimedSpreadPerBullet * (bulletCount - 1) * 0.5f;
        for (int i = 0; i < bulletCount; i++)
        {
            SpawnBullet(origin, first + aimedSpreadPerBullet * i, bulletSpeed);
        }
    }

    // 列状：進行方向に垂直な向きへ弾を並べ、全弾を同じ方向へ撃つ
    private void FireLines(Vector2 origin, int shotIndex)
    {
        float rad = lineDirection * Mathf.Deg2Rad;
        Vector2 dir = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
        Vector2 side = new Vector2(-dir.y, dir.x); // 進行方向に垂直

        // 列の中心を origin に合わせる
        float firstOffset = -lineSpacing * (bulletCount - 1) * 0.5f;

        // 格子状：奇数回目は半間隔ずらす
        if (lineStagger && shotIndex % 2 == 1)
        {
            firstOffset += lineSpacing * 0.5f;
        }

        for (int i = 0; i < bulletCount; i++)
        {
            Vector2 pos = origin + side * (firstOffset + lineSpacing * i);
            SpawnBulletAt(pos, lineDirection, bulletSpeed);
        }
    }

    // 不確定：角度と弾速を範囲内でランダムにする
    private void FireChaotic(Vector2 origin)
    {
        for (int i = 0; i < bulletCount; i++)
        {
            float angle = Random.Range(chaosAngleMin, chaosAngleMax);
            float speed = Random.Range(chaosSpeedMin, chaosSpeedMax);
            SpawnBullet(origin, angle, speed);
        }
    }

    private static float AngleTo(Vector2 origin, Vector2 target)
    {
        Vector2 d = target - origin;
        return Mathf.Atan2(d.y, d.x) * Mathf.Rad2Deg;
    }

    private void SpawnBullet(Vector2 origin, float angleDeg, float speed)
    {
        SpawnBulletAt(origin, angleDeg, speed);
    }

    // 指定位置から、角度（度）と弾速で弾を1発生成
    private void SpawnBulletAt(Vector2 position, float angleDeg, float speed)
    {
        float rad = angleDeg * Mathf.Deg2Rad;
        Vector2 dir = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));

        Projectile b = Instantiate(bulletPrefab, position, Quaternion.identity);
        b.Initialize(dir, speed, damage, Faction.Enemy);
    }
}
