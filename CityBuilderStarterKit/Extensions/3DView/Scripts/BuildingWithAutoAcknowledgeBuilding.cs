using UnityEngine;
using System.Collections;

/**
 * A building implementation that automatically finishes all tasks.
 */
namespace CBSK
{
    public class BuildingWithAutoAcknowledgeBuilding : Building
    {
        /**
         * Finish building auto acknolwedge.
         */
        override public void CompleteBuild()
        {
            State = BuildingState.READY;
            BuildingManager.GetInstance().AcknowledgeBuilding(this);
            Acknowledge();
            // view.SendMessage ("UI_UpdateState");
        }
    } 
}

