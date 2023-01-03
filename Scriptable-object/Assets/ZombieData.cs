using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Zombie Data", menuName = "Scriptable Object/Zombie Data", order = int.MaxValue)] // Scriptable object를 상송받는 클래스를 scriptable object asset으로 만들어줌
public class ZombieData : ScriptableObject
{
    [SerializeField]
    private string zombieName;
    public string ZombieName { get { return zombieName; } }
    
    [SerializeField]
    private int hp;
    public int Hp { get { return hp; } }
    
    [SerializeField]
    private int damage;
    public int Damage { get { return damage; } }
    
    [SerializeField]
    private float sightRange;
    public float SightRang {get { return sightRange; } }
    
    [SerializeField]
    private float moveSpeed;
    public float MoveSpeed {get {return moveSpeed; } }
}
