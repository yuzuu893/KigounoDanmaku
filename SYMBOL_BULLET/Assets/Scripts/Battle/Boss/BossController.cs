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
    [Header("段階の切り替え条件(残りHP割合)")]
    [Tooltip("この段階以下で第二形態移行")]
    [Range(0f, 1f)]
    [SerializeField] private float phase2Threshold = 0.7f;

    [Tooltip("この段階以下で第三形態移行 第二形態の値より小さく")]
    [Range(0f, 1f)]
    [SerializeField] private float phase3Threshold = 0.35f;

    [Header("待ち時間")]
    [Tooltip("戦闘開始から最初の発射までの待ち時間")]
    [SerializeField] private float initialDeley = 1f;

    [Tooltip("段階が切り替わってから最初の発射までの待ち時間")]
    [SerializeField] private float phaseChangeDeley = 1.5f;

    [Header("各段階の弾幕(要素0=第一段階 1=第二 2=第三")]
    [SerializeField] private BossPhase[] phases = new BossPhase[3];

    private BossHealth health;
    private BulletPatternEmitter emitter;

    ///<summary>現在の段階(1～3)</summary>
    public int PhaseNumber { get; private set; }

    ///<summary>段階が切り替わった時</summary>
    public event Action<int> OnPhaseChanged;

    private void Awake()
    {
        health = GetComponent<BossHealth>();
        emitter = GetComponent<BulletPatternEmitter>();

        if(phase3Threshold >= phase2Threshold)
        {
            Debug.LogWarning("[BossController] Phase3ThresholdはPhase2Thresholdより小さくしてください", this);
        }
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
        ApplyPhase(1, initialDeley);
    }

    private void HandleHealthChanged(int current, int max)
    {
        if (health.IsDefeated || max <= 0) return;

        float ratio = (float)current / max;
        int target = 1;
        if (ratio <= phase3Threshold) target = 3;
        else if (ratio <= phase2Threshold) target = 2;

        if (target > PhaseNumber)
        {
            ApplyPhase(target, phaseChangeDeley);
        }
    }

    private void ApplyPhase(int number, float deley)
    {
        PhaseNumber = number;

        int index = number - 1;
        BossPhase phase = (phases != null && index < phases.Length) ? phases[index] : null;

        if (phase == null || phase.patterns == null || phase.patterns.Count == 0)
        {
            Debug.LogWarning($"[BossController] 第{number}段階の弾幕が設定されていません。", this);
            emitter.SetPatterns(null, deley);
        }
        else
        {
            emitter.SetPatterns(phase.patterns, deley);
        }

        Debug.Log($"[BossController] {name} 第{number}段階へ移行");
        OnPhaseChanged?.Invoke(number);
    }
}
