using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : MonoBehaviour
{
    public ZombieData zombieData;

    pubilc void PrintZombieData(){
        Debug.Log("좀비이름 :: " + zombieData.ZombieName);
        Debug.Log("체력 :: " + zombieData.Hp);
        Debug.Log("공격력 :: " + zombieData.Damage);
        Debug.Log("시야 :: " + zombieData.SightRange);
        Debug.Log("이동속도:: " + zombieData.MoveSpeed);
        Debug.Log("----------------------------------");
    }
}
