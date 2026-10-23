using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ボスに付けてBossPatternDataを一定間隔で実行する
/// </summary>
public class BulletPatternEmitter : MonoBehaviour
{
    private readonly List<BulletPatternData> patterns = new List<BulletPatternData>();
    private readonly List<float> timers = new List<float>();
    private Transform player;

    private void Start()
    {
        GameObject p = GameObject.FindWithTag("Player");
        if (p != null) player = p.transform;
        else Debug.LogWarning("[BulletPattenEmitter] Playerタグのオブジェクトが見つかりません");
    }

    ///<summary>
    ///実行するパターンを入れ替える、以前のパターンとタイマーは廃棄
    /// firstDerey秒ごとにパターンの入れ替え
    /// </summary>
    public void SetPatterns(IReadOnlyList<BulletPatternData> newPatterns, float firstDerey)
    {
        patterns.Clear();
        timers.Clear();

        if (newPatterns == null) return;
        
        foreach (BulletPatternData p in newPatterns)
        {
            if (p == null) continue;
            patterns.Add(p);
            timers.Add(firstDerey);
        }
    }

    private void Update()
    {
        Vector2? target = player !=null ? (Vector2?)player.position : null;

        for (int i = 0; i < patterns.Count; i++) 
        {
            timers[i] -= Time.deltaTime;
            if (timers[i] <= 0f)
            {
                patterns[i].Fire(transform.position, target);
                timers[i] = patterns[i].FireInterval;
            }
        }
    }
}
