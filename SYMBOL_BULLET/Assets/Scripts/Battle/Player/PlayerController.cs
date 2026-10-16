using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// プレイヤーの移動方法と範囲制限を担当
/// WASD or 矢印で移動可能 斜め移動を速度が変わらないように
/// </summary> 

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("移動設定")]
    [Tooltip("移動速度(ユニット/秒) 調整可")]
    [SerializeField] private float moveSpeed = 6f;

    [Header("移動範囲(ワールド座標)")]
    [Tooltip("移動可能範囲 左下")]
    [SerializeField] private Vector2 areaMin = new Vector2(-8f, -4.5f);

    [Tooltip("移動可能範囲 右上")]
    [SerializeField] private Vector2 areaMax = new Vector2(8f, 4.5f);

    private Rigidbody2D rb;
    private Vector2 moveInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        moveInput = ReadMoveInput();
    }

    private void FixedUpdate()
    {
        Vector2 next = rb.position + moveInput * moveSpeed * Time.deltaTime;
        next.x = Mathf.Clamp(next.x, areaMin.x, areaMax.x);
        next.y = Mathf.Clamp(next.y, areaMin.y, areaMax.y);
        rb.MovePosition(next);
    }

    /// <summary>
    /// WASD or 矢印キーから移動ベクトル作成
    /// </summary> 
    private Vector2 ReadMoveInput()
    {
        Keyboard kb = Keyboard.current;
        if (rb == null) return Vector2.zero;

        float x = 0f;
        float y = 0f;

        if (kb.aKey.isPressed || kb.leftArrowKey.isPressed) x -= 1f;
        if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) x += 1f;
        if (kb.sKey.isPressed || kb.downArrowKey.isPressed) y -= 1f;
        if (kb.wKey.isPressed || kb.upArrowKey.isPressed) y += 1f;

        Vector2 v = new Vector2(x, y);
        //斜め入力でも速度を統一
        return v.sqrMagnitude > 0f ? v.normalized : Vector2.zero;
    }

    //シーンビューで選択中に移動を緑の枠で
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Vector3 center = (areaMin + areaMax) * 0.5f;
        Vector3 size = areaMax - areaMin;
        Gizmos.DrawWireCube(center, size);
    }
}
