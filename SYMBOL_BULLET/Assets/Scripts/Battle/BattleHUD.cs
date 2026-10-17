using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 戦闘画面のUI表示を担当
/// </summary>
public class BattleHUD : MonoBehaviour
{
    [Header("参照")]
    [SerializeField] private BattleManager battle;
    [SerializeField] private BossHealth boss;
    [SerializeField] private PlayerHealth player;

    [Header("UI部品")]
    [SerializeField] private Slider bossHpSlider;
    [SerializeField] private TMP_Text playerHpText;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text resultText;

    private void Start()
    {
        // 初期表示
        if (boss != null) UpdateBossHp(boss.CurrentHP, boss.MaxHP);
        if (player != null) UpdatePlayerHp(player.CurrentHP, player.MaxHP);
        if (resultText != null) resultText.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        if (boss != null) boss.OnHealthChanged += UpdateBossHp;
        if (player != null) player.OnHealthChanged += UpdatePlayerHp;
        if (battle != null) battle.OnBattleEnded += ShowResult;
    }

    private void OnDisable()
    {
        if (boss != null) boss.OnHealthChanged -= UpdateBossHp;
        if (player != null) player.OnHealthChanged -= UpdatePlayerHp;
        if (battle != null) battle.OnBattleEnded -= ShowResult;
    }

    private void Update()
    {
        if (battle != null && timerText != null)
        {
            float t = battle.ElapsedTime;
            timerText.text = $"{(int)t / 60:00}:{t % 60f:00.0}";
        }
    }

    private void UpdateBossHp(int current, int max)
    {
        if (bossHpSlider != null) bossHpSlider.value = max > 0 ? (float)current / max : 0f;
    }

    private void UpdatePlayerHp(int current, int max)
    {
        if (playerHpText != null) playerHpText.text = $"HP {current} / {max}";
    }

    private void ShowResult(BattleState state)
    {
        if (resultText == null) return;
        string title = state == BattleState.Cleared ? "CLEAR" : "GAME OVER";
        resultText.text = $"{title}\n<size=40%>Rキーでリトライ（仮）</size>";
        resultText.gameObject.SetActive(true);
    }
}
