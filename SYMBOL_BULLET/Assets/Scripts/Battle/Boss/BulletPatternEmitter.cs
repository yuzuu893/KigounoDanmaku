using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ボスに付けて、渡された弾幕パターンを並行して実行する
/// どのパターンを使うかはBossControllerがSetPatternsで指示する
/// 各パターンの発射回数もここで数える
/// </summary>
public class BulletPatternEmitter : MonoBehaviour
{
    private readonly List<BulletPatternData> patterns = new List<BulletPatternData>();
    private readonly List<float> timers = new List<float>();
    private readonly List<int> shotCounts = new List<int>();
    private Transform player;

    private void Start()
    {
        GameObject p = GameObject.FindWithTag("Player");
        if (p != null) player = p.transform;
        else Debug.LogWarning("[BulletPatternEmitter] Player タグのオブジェクトが見つかりません。狙い撃ちは発射されません。", this);
    }

    /// <summary>
    /// 実行するパターンを入れ替える。以前のパターン・タイマー・発射回数は破棄される
    /// firstDelay 秒後に各パターンの最初の発射を行う
    /// </summary>
    public void SetPatterns(IReadOnlyList<BulletPatternData> newPatterns, float firstDelay)
    {
        patterns.Clear();
        timers.Clear();
        shotCounts.Clear();

        if (newPatterns == null) return;

        foreach (BulletPatternData p in newPatterns)
        {
            if (p == null) continue;
            patterns.Add(p);
            timers.Add(firstDelay);
            shotCounts.Add(0);
        }
    }

    private void Update()
    {
        Vector2? target = player != null ? (Vector2?)player.position : null;

        for (int i = 0; i < patterns.Count; i++)
        {
            timers[i] -= Time.deltaTime;
            if (timers[i] <= 0f)
            {
                patterns[i].Fire(transform.position, target, shotCounts[i]);
                shotCounts[i]++;
                timers[i] = patterns[i].FireInterval;
            }
        }
    }
}