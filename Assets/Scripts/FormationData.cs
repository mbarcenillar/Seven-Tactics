using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class FormationData
{
    public string formationName;
    public List<PositionPlayer> playersByPosition;
}

[Serializable]
public class PositionPlayer
{
    public string position;
    public string player;
}
