using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class TeamFormation
{
    public string teamName;
    public string formation;
    public List<PlayerPosition> playersByPosition;
}

[Serializable]
public class AllTeamFormations
{
    public List<TeamFormation> teamFormations;
}

[System.Serializable]
public class PlayerPosition
{
    public string position;
    public string player;
}