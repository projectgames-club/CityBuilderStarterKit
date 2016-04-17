using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/**
 * A building view built that can show simple unit animations.
 */
namespace CBSK
{
    public class BuildingViewWithUnits : BuildingView
    {

        /**
         * The unit animators
         */
        public List<SimpleOccupantAnimator> animators;

        /**
         * Sprites for units that aren't animated.
         */
        public List<SpriteRenderer> staticUnits;

        /**
         * Which units are in which spots.
         */
        protected Dictionary<OccupantData, int> allocatedSpots;

        /**
         * Initialise the building view.
         */
        override public void UI_Init(Building building)
        {
            base.UI_Init(building);

            // Position animators
            if (building.Type is BuildingDataWithUnitAnimations)
            {
                allocatedSpots = new Dictionary<OccupantData, int>();
                // Position animators
                for (int i = 0; i < animators.Count; i++)
                {
                    animators[i].gameObject.SetActive(false);
                    if (((BuildingDataWithUnitAnimations)building.Type).animationPositions.Count > i)
                    {
                        animators[i].gameObject.transform.localPosition = new Vector3(((BuildingDataWithUnitAnimations)building.Type).animationPositions[i].x / 100f,
                                                                                  ((BuildingDataWithUnitAnimations)building.Type).animationPositions[i].y / 100f, 0.0f);
                    }
                    animators[i].gameObject.SetActive(false);
                    animators[i].Hide();
                }
                // Position static sprites
                for (int i = 0; i < staticUnits.Count; i++)
                {
                    if (((BuildingDataWithUnitAnimations)building.Type).staticPositions.Count > i)
                    {
                        staticUnits[i].gameObject.transform.localPosition = new Vector3(((BuildingDataWithUnitAnimations)building.Type).staticPositions[i].x / 100f,
                                                                                    ((BuildingDataWithUnitAnimations)building.Type).staticPositions[i].y / 100f, 0.0f);
                    }
                    staticUnits[i].gameObject.SetActive(false);

                }
            }

            UpdateOccupants();

        }

        /**
         * Activity acknowledged.
         */
        override public void UI_AcknowledgeActivity()
        {
            if (!building.ActivityInProgress)
            {
                if (building.StoreFull) UI_StoreFull();
                else progressIndicator.gameObject.SetActive(false);
            }
            UpdateOccupants();
        }

        /**
         * OccupantData dismissed.
         */
        public void UI_DismissOccupant()
        {
            UpdateOccupants();
        }

        /**
         * Update occupant sprites
         **/
        virtual protected void UpdateOccupants()
        {
            if (building.Type is BuildingDataWithUnitAnimations)
            {
                // Remove any unused
                foreach (OccupantData o in allocatedSpots.Keys.ToList())
                {
                    if (!building.Occupants.Contains(o))
                    {
                        RemoveAllocatedSpot(allocatedSpots[o]);
                        allocatedSpots.Remove(o);
                    }
                }
                // Cycle through each occupant
                if (building.Occupants != null)
                {
                    for (int j = 0; j < building.Occupants.Count; j++)
                    {
                        if (allocatedSpots.ContainsKey(building.Occupants[j]))
                        {
                            // Occupant already done, do nothing
                        }
                        else
                        {
                            int spot = AllocateUnitToSpot(building.Occupants[j]);
                            if (spot != -1)
                                allocatedSpots.Add(building.Occupants[j], spot);
                        }
                    }
                }
            }

        }

        virtual protected void RemoveAllocatedSpot(int i)
        {
            if (i < ((BuildingDataWithUnitAnimations)building.Type).animationPositions.Count)
            {
                animators[i].Hide();
                animators[i].gameObject.SetActive(false);
            }
            else if (i < ((BuildingDataWithUnitAnimations)building.Type).staticPositions.Count + ((BuildingDataWithUnitAnimations)building.Type).animationPositions.Count)
            {
                staticUnits[i - ((BuildingDataWithUnitAnimations)building.Type).animationPositions.Count].gameObject.SetActive(false);
            }
        }

        virtual protected int AllocateUnitToSpot(OccupantData o)
        {

            for (int i = 0; i < ((BuildingDataWithUnitAnimations)building.Type).animationPositions.Count; i++)
            {
                if (animators[i].gameObject.activeInHierarchy == false && o.occupantTypeString == "SWORDSMAN")
                {
                    animators[i].gameObject.SetActive(true);
                    animators[i].Show();
                    return i;
                }
            }
            for (int i = 0; i < ((BuildingDataWithUnitAnimations)building.Type).staticPositions.Count; i++)
            {
                if (staticUnits[i].gameObject.activeInHierarchy == false)
                {
                    staticUnits[i].gameObject.SetActive(true);
                    staticUnits[i].sprite = SpriteManager.GetUnitSprite(o.Type.spriteName);
                    return i + ((BuildingDataWithUnitAnimations)building.Type).animationPositions.Count;
                }
            }
            return -1;
        }

    }
}