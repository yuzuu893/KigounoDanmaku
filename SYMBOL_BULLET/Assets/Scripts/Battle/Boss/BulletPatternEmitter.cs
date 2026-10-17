using UnityEngine;

/// <summary>
/// ボスに付けてBossPatternDataを一定間隔で実行する
/// </summary>
public class BulletPatternEmitter : MonoBehaviour
{
    [Header("弾幕設定")]
    [Tooltip("実行する弾幕パターン")]
    [SerializeField] private BulletPatternData pattern;

    [Tooltip("戦闘開始から最初の発射までの待ち時間(秒)")]
    [SerializeField] private float startDelay = 1f;

    private float timer;
    private Transform player;

    private void OnDisable()
    {
        timer = startDelay;
    }

    private void Start()
    {
        GameObject p = GameObject.FindWithTag("Player");
        if (p != null) player = p.transform;
        else Debug.LogWarning("[BulletpatternEmitter] PlayerタグのObjectが見つかりません");
    }

    private void Update()
    {
        if (pattern == null) return;

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            Vector2? target = player != null ? (Vector2)player.position : null;
            pattern.Fire(transform.position, target);
            timer = pattern.FireInterval;
        }
    }
}
