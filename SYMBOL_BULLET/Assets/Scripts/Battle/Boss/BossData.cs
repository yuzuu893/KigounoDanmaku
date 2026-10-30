using UnityEngine;

/// <summary>
/// ボス一体分の設定データ
/// 実行中に値を書き換えない
/// </summary>
[CreateAssetMenu(fileName = "Boss_New" , menuName = "SymbolBullet/Boss Data")]
public class BossData : ScriptableObject
{
    [Header("基本情報")]
    [Tooltip("ボスID ハイスコア保存のキーに仕様 重複禁止")]
    [SerializeField] private string bossId = "BOSS_01";

    [Tooltip("表示名")]
    [SerializeField] private string displayName = "Boss";

    [Tooltip("記号(Boss名表示で使用")]
    [SerializeField] private string symbol = "〇";

    [Tooltip("ボスの色")]
    [SerializeField] private Color bossColor = Color.red;

    [Header("戦闘設定")]
    [Min(1)]
    [SerializeField] private int maxHP = 300;

    [Tooltip("この割合以下で第二段階へ")]
    [Range(0f, 1f)]
    [SerializeField] private float phase2Threshold = 0.7f;

    [Tooltip("この割合以下で第三段階へ 第二段階の値より小さく")]
    [Range (0f, 1f)]
    [SerializeField] private float phase3Threshold = 0.35f;

    [Tooltip("戦闘開始から最初の発射までの待ち時間(秒)")]
    [SerializeField] private float initialDeley = 1f;

    [Tooltip("段階が切り替わってから最初の発射までの待ち時間(秒)")]
    [SerializeField] private float phaseChangeDeley = 1.5f;

    [Header("各段階の弾幕(要素0=第一形態 要素1=第二形態 要素2=第二段階")]
    [SerializeField] private BossPhase[] phases = new BossPhase[3];

    public string BossId => bossId;
    public string DisplayName => displayName;
    public string Symbol => symbol;
    public Color BossColor => bossColor;
    public int MaxHP => maxHP;
    public float Phase2Threshold => phase2Threshold;
    public float Phase3Threshold => phase3Threshold;
    public float InitialDeley => initialDeley;
    public float PhaseChangeDeley => phaseChangeDeley;
    public BossPhase[] Phases => phases;

}
