using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

///<summary>弾幕の種類</summary>
public enum PatternType
{
    Radial,//全方向
    Fan,//扇
    Aimed,//狙い撃ち
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

    [Header("全方向弾幕の設定")]
    [Tooltip("一回の発射で出す球数")]
    [Min(1)]
    [SerializeField] private int bulletCount = 12;

    [Header("全方向の設定")]
    [Tooltip("最初の弾の角度")]
    [SerializeField] private float startAngle = 90f;

    [Header("扇形の設定")]
    [Tooltip("扇の広がり 弾の端から端までの角度")]
    [Range(0f, 360f)]
    [SerializeField] private float fanSpread = 60f;

    [Tooltip("ONにすると扇の中心をプレイヤーに、OFFなら固定角度")]
    [SerializeField] private bool fanAimAtPlayern = false;

    [Tooltip("扇の中心角度")]
    [SerializeField] private float fanCenterAngle = 270f;

    [Header("狙い撃ちの設定")]
    [Tooltip("弾の数の補正は不要、bulletCount発を同方向へ撃つ")]
    [Min(0f)]
    [SerializeField] private float aimedSpreadPerBullet = 0f;

    public float FireInterval => fireInterval;

    ///<summary>弾幕を1回発射する targetPosition は狙う対象（Player）の位置</summary>
    public void Fire(Vector2 origin, Vector2? targetPosition)
    {
        if (bulletPrefab == null)
        {
            Debug.LogWarning($"[BulletPatternData] {name}にBullet Prefabが追加されてません");
            return;
        }

        switch (type)
        {
            case PatternType.Radial:
                FireRadial(origin);
                break;
            case PatternType.Fan:
                FireFan(origin, targetPosition);
                break;
            case PatternType.Aimed:
                FireAimed(origin, targetPosition);
                break;
        }
    }
    
    //全方向発射(360度を弾数で等分する)
    private void FireRadial(Vector2 origin)
    {
        float step = 360f / bulletCount;
        for (int i = 0; i < bulletCount; i++)
        {
            SpawnBallet(origin, startAngle + step * i);
        }
    }

    //扇状発射(中心角度の座標spread/2ずつ広げて等間隔に並べる
    private void FireFan(Vector2 origin,Vector2? target)
    {
        float center = fanCenterAngle;
        if (fanAimAtPlayern && target.HasValue)
        {
            center = AngleTo(origin, target.Value);
        }

        if (bulletCount == 1)
        {
            SpawnBallet(origin, center);
            return;
        }

        float step = fanSpread / (bulletCount - 1);
        float first = center - fanSpread * 0.5f;
        for (int i = 0; i < bulletCount;i++)
        {
            SpawnBallet(origin, first + step * i);
        }
    }

    //狙い撃ち(発射時点のPlayerの方向に向けて撃つ
    private void FireAimed(Vector2 origin, Vector2? target)
    {
        if (!target.HasValue) return;

        float center = AngleTo(origin, target.Value);

        if (bulletCount == 1 || aimedSpreadPerBullet <= 0f)
        {
            SpawnBallet(origin, center);
            return;
        }

        float first = center - aimedSpreadPerBullet * (bulletCount - 1) * 0.5f;
        for (int i = 0;i < bulletCount;i++)
        {
            SpawnBallet(origin, first + aimedSpreadPerBullet * i);
        }
    }

    //originからtargetへの角度
    private static float AngleTo(Vector2 origin, Vector2 target)
    {
        Vector2 d = target - origin;
        return Mathf.Atan2(d.y, d.x) * Mathf.Rad2Deg;
    }

    //角度から方向を作成後、弾を1発発射する
    private void SpawnBallet(Vector2 origin, float angleDeg)
    {
        float rad = angleDeg * Mathf.Deg2Rad;
        Vector2 dir = new Vector2(Mathf.Cos(rad),Mathf.Sin(rad));

        Projectile b = Instantiate(bulletPrefab, origin, Quaternion.identity);
        b.Initialize(dir, bulletSpeed, damage, Faction.Enemy);
    }
}
