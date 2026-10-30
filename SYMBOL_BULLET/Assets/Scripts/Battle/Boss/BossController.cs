using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>一つの攻撃段階の設定 </summary>
[Serializable]
public class BossPhase
{
    [Tooltip("管理用メモ")]
    public string label = "Phase";

    [Tooltip("この段階で実行する弾幕パターン")]
    public List<BulletPatternData> patterns = new List<BulletPatternData>();
}

/// <summary>
/// ボスの攻撃段階の管理
/// 段階は残りHPで決まる、一度進んだら戻らない
/// </summary>
[RequireComponent(typeof(BossHealth))]
[RequireComponent(typeof(BulletPatternEmitter))]
public class BossController : MonoBehaviour
{
    [Header("ボス設定")]
    [Tooltip("このボスの設定データ。")]
    [SerializeField] private BossData data;

    private BossHealth health;
    private BulletPatternEmitter emitter;
    private bool started;

    ///<summary>現在の段階(1～3)</summary>
    public int PhaseNumber { get; private set; }

    /// <summary>適用中のBossData</summary>
    public BossData Data => data;

    ///<summary>段階が切り替わった時</summary>
    public event Action<int> OnPhaseChanged;

    private void Awake()
    {
        health = GetComponent<BossHealth>();
        emitter = GetComponent<BulletPatternEmitter>();
    }

    private void OnEnable()
    {
        health.OnHealthChanged += HandleHealthChanged;
    }

    private void OnDisable()
    {
        health.OnHealthChanged -= HandleHealthChanged;
    }

    private void Start()
    {
        if (data == null)
        {
            Debug.LogError("[BossController] Dataが設定されてません", this);
            return;
        }

        if (data.Phase3Threshold >= data.Phase2Threshold)
        {
            Debug.LogError("[BossController] Phase3ThresholdはPhase2Thresholdより小さくしてください", this);
        }

        //HPはAwake の実行順に邪魔されないよう,Startで適用
        health.SetMaxHP(data.MaxHP);

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.color = data.BossColor;

        started = true;
        ApplyPhase(1, data.InitialDeley);
    }

    private void HandleHealthChanged(int current, int max)
    {
        if (!started || health.IsDefeated ||max <= 0) return;

        float ratio = (float)current / max;
        int target = 1;
        if (ratio <= data.Phase3Threshold) target = 3;
        else if (ratio <= data.Phase2Threshold) target = 2;

        //後退しない：現在だけ進む場合のみ
        if (target > PhaseNumber)
        {
            ApplyPhase(target, data.PhaseChangeDeley);
        }
    }

    private void ApplyPhase(int number, float deley)
    {
        PhaseNumber = number;

        BossPhase[] phases = data.Phases;
        int index = number - 1;
        BossPhase phase = (phases != null && index < phases.Length) ? phases[index] : null;

        if (phase == null || phase.patterns == null || phase.patterns.Count == 0)
        {
            Debug.LogError($"[BossController] {data.BossId}の第{number}段階の弾幕が設定されてません", this);
            emitter.SetPatterns(null, deley);
        }
        else
        {
            emitter.SetPatterns(phase.patterns, deley);
        }

        Debug.Log($"[BossController] {data.BossId} 第{number}形態へ移行");
        OnPhaseChanged?.Invoke(number);
    }
}
