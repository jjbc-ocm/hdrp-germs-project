using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackThreadInfo : DecisionThreadInfo
{
    public override DecisionNodeInfo GetFinalDecision(GameEntityManager[] entities)
    {
        return null;
    }
}
