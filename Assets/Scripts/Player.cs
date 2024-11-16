using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public string playerName;
    public int attack;
    public int defense;
    public int stamina;

    public Player (string playerName, int attack, int defense, int stamina)
    {
        this.playerName = playerName;
        this.attack = attack;
        this.defense = defense;
        this.stamina = stamina;
    }
}
