using System;
using UnityEngine;

/// <summary>ÉXÉRÉAÇÃì‡ñÛ </summary>
public struct ScoreResult
{
    public int basePoints;
    public int timeBones;
    public int hpBonue;
    public int Total => basePoints + timeBones + hpBonue;
}

public class ScoreCalculator : MonoBehaviour
{

}
