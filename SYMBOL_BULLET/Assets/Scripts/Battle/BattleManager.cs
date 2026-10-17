using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

///<summary>戦闘の状態</summary>
public enum BattleState
{
    Playing,
    Cleared,
    GameOver,
}


/// <summary>
/// 戦闘の進行と終了を担当
/// 勝敗は二重判定防止のため最初の一回のみ
/// </summary>

public class BattleManager : MonoBehaviour
{
    [Header("参照")]
    [SerializeField] private BossHealth boss;
    [SerializeField] private PlayerHealth player;

    public BattleState State { get; private set; } = BattleState.Playing;
    public float ElapsedTime { get; private set; }

    ///<summary>勝敗確定した時のみ一回だけ</summary>
    public event Action<BattleState> OnBattleEnded;

    private void Awake()
    {
        Time.timeScale = 1f;
        State = BattleState.Playing;
        ElapsedTime = 0f;

        if (boss == null) Debug.LogError("[BattleManager] Bossが設定されていません", this);
        if (player == null) Debug.LogError("[BattleManager] Playerが設定されていません", this);
    }

    private void OnEnable()
    {
        if (boss != null) boss.OnDefeated += HandleBossDefeated;
        if (player != null) player.OnDefeated += HandlePlayerDefeated;
    }
    private void OnDestroy()
    {
        // 破棄時にtimeScaleを戻しておく（他シーンへの持ち越し防止）
        Time.timeScale = 1f;
    }

    private void Update()
    {
        if (State == BattleState.Playing)
        {
            ElapsedTime += Time.deltaTime;
            return;
        }

        // 【テスト用】勝敗確定後、Rキーでリトライ。第5段階で結果画面に置き換える
        if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    private void HandleBossDefeated() => EndBattle(BattleState.Cleared);
    private void HandlePlayerDefeated() => EndBattle(BattleState.GameOver);

    private void EndBattle(BattleState result)
    {
        if (State != BattleState.Playing) return; // 二重判定防止

        State = result;
        Debug.Log($"[BattleManager] 戦闘終了: {result} / {ElapsedTime:F1}秒");

        if (result == BattleState.Cleared)
        {
            // 撃破：ボスを止め、残っている弾を消す
            if (boss != null) boss.gameObject.SetActive(false);
            foreach (Projectile p in FindObjectsByType<Projectile>(FindObjectsSortMode.None))
            {
                Destroy(p.gameObject);
            }
        }

        Time.timeScale = 0f; // 全体を停止（UIは影響を受けない）
        OnBattleEnded?.Invoke(result);
    }
}
