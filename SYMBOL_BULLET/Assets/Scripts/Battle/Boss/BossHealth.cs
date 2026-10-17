using System;
using UnityEngine;

/// <summary>
/// ボスのHPと撃破判定担当
/// 攻撃段階の切り替え(第4段階まで)や勝利判定はここのイベントを使用して行う想定
/// </summary>
public class BossHealth : MonoBehaviour
{
    [Header("HP設定")]
    [Tooltip("ボスの最大HP")]
    [SerializeField] private int maxHP = 100;

    [SerializeField] private int currentHP;

    ///<summary>HPが変化したとき(現在HP,最大HP)</summary>
    public event Action<int, int> OnHealthChanged;

    ///<summary>撃破されたときのみ呼び出す</summary>
    public event Action OnDefeated;

    public int MaxHP => maxHP;
    public int CurrentHP => currentHP;
    public float HpRatio => maxHP > 0 ? (float)currentHP / maxHP : 0f;
    public bool IsDefeated {  get; private set; }

    private void Awake()
    {
        ResetHealth();
    }

    ///<summary>HPと撃破状態の初期化</summary>
    private void ResetHealth()
    {
        currentHP = maxHP;
        IsDefeated = false;
        OnHealthChanged?.Invoke(currentHP, maxHP);
    }

    public void TakeDamage(int amount)
    {
        if (IsDefeated || amount <= 0) return;

        currentHP = Mathf.Max(0,currentHP - amount);
        OnHealthChanged?.Invoke(currentHP, maxHP);

        if (currentHP <= 0)
        {
            IsDefeated = true;
            Debug.Log($"[BossHealth]{name}を撃破しました");
            OnDefeated?.Invoke();
        }
    }
}
