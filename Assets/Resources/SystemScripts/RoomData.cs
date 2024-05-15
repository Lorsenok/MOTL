using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomData : MonoBehaviour
{
    public static float HP = 100;
    public static float HPDefault = 100;

    public static float Speed = 10;
    public static float SpeedDefault = 10;

    public static float DamageMultiplier = 1;
    public static float DamageMultiplierDefault = 1;

    public static float Gravity = -9.81f;
    public static float GravityDefault = 9.81f;
    
    public static float PlayTime = 120;
    public static float PlayTimeDefault = 120;

    public static float SpawnTime = 5;
    public static float SpawnTimeDefault = 5;

    public static GameRuleType GameMode = GameRuleType.Deathmatch;
    public static GameRuleType GameModeDefault = GameRuleType.Deathmatch;

}
