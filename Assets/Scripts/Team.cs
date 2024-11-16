using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Team : MonoBehaviour
{
    public string teamName;
    public List<Player> players = new List<Player>();

    public Team(string name)
    {
        teamName = name;
    }
}
