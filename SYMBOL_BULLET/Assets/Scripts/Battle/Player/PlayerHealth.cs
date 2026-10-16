using System;
using UnityEngine;

/// <summary>
///PlayerのHP、被弾、無敵時間担当
///点滅は見た目だけ 無敵判定は別で独立して判定
/// </summary>
public class PlayerHealth : MonoBehaviour
{
    [Header("HP設定")]
    [Tooltip("プレイヤーの最大HP")]
    [SerializeField] private int maxHP = 5;

    [Header("無敵設定")]
    [Tooltip("被弾後の無敵時間設定")]
    [SerializeField] private float invincibleDuration = 1.5f;

    [Header("点滅表示（見た目のみ）")]
    [Tooltip("点滅させるSpriteRenderer。未設定なら自動取得。")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Tooltip("点滅の切り替え時間(秒)")]
    [SerializeField] private float blinkInterval = 0.1f;

    [SerializeField] private int currentHP;

    private float invincibleUntil;

    ///<summary>HPが変化したとき</summary>
    public event Action<int, int> OnHealthChanged;

    ///<summary>HPが0になったときのみ</summary>
    public event Action OnDefeated;

    public int MaxHP => maxHP;
    public int CurrentHP => currentHP;
    public float HpRatio => MaxHP > 0 ? (float)currentHP / maxHP : 0f;
    public bool IsDefeated {  get; private set; }
    public bool IsInvincible => Time.time < invincibleUntil;

    private void Awake()
    {
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        ResetHealth();
    }

    public void ResetHealth()
    {
        currentHP = maxHP;
        IsDefeated = false;
        invincibleUntil = 0f;
        SetVisible(true);
        OnHealthChanged?.Invoke(currentHP, maxHP);
    }

    public void TakeDamage(int amount)
    {
        if (IsDefeated || IsInvincible || amount < 0) return;

        currentHP = Mathf.Max(0, currentHP - amount);
        OnHealthChanged?.Invoke(currentHP, maxHP);

        if (currentHP <= 0)
        {
            IsDefeated = true;
            SetVisible(true);
            Debug.Log("[PlayerHealth] プレイヤーのHPが0になりました");
            OnDefeated?.Invoke();
            return;
        }

        invincibleUntil = Time.time + invincibleDuration;
    }

    private void Update()
    {
        if (IsDefeated) return;

        if(IsInvincible)
        {
            //点滅(見た目)
            bool visible = Mathf.FloorToInt(Time.time / blinkInterval) % 2 == 0;
            SetVisible(visible);
        }
        else
        {
            SetVisible(true);
        }
    }

    private void SetVisible(bool visible)
    {
        if (spriteRenderer != null) spriteRenderer.enabled = visible;
    }
}
