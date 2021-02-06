using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamTracker : MonoBehaviour
{
    public int teamID = -1;

    // Returns true if friendly, false if enemy
    public bool CompareTeam(TeamTracker tt)
    {
        return CompareTeam(tt.teamID);
    }
    public bool CompareTeam(int otherID)
    {
        if(teamID > -1 && teamID == otherID)
        {
            return true;
        }

        return false;
    }
}
