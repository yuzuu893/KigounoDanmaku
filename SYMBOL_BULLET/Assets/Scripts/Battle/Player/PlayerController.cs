using UnityEngine;
using UnityEngine.InputSystem:

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
}
