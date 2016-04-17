using UnityEngine;

namespace CBSK
{
    /**
     * Various predefined colors and methods for getting colors.
     */
    public class UIColor
    {
        public static Color BUILD = new Color(1.0f, 0.0f, 0.0f, 1.0f);
        public static Color DESATURATE = new Color(0.0f, 1.0f, 1.0f, 1.0f);
        public static Color PLACING_COLOR = new Color(0.0f, 1.0f, 0.0f, 0.5f);
        public static Color PLACING_COLOR_BLOCKED = new Color(1.0f, 0.0f, 0.0f, 0.5f);
        public static Color RECRUIT = new Color(1.0f, 0.5f, 0.0f, 1.0f);

        public static Color RESOURCE = new Color(0.5f, 1.0f, 0.0f);
        public static Color CUSTOM_RESOURCE = new Color(0.5f, 1.0f, 0.0f);
        public static Color GOLD = new Color(0.5f, 1.0f, 0.0f);

        public static Color GATHER = RESOURCE;

        public static Color GetColourForActivityType(string type)
        {
            switch (type)
            {
                case ActivityType.GATHER: return GATHER;
                case ActivityType.CLEAR: return GATHER;
                case ActivityType.BUILD: return BUILD;
                case ActivityType.RECRUIT: return RECRUIT;
            }
            return Color.white;
        }

        public static Color GetColourForRewardType(RewardType type)
        {
            switch (type)
            {
                case RewardType.GOLD: return GOLD;
                case RewardType.RESOURCE: return RESOURCE;
                case RewardType.CUSTOM_RESOURCE: return CUSTOM_RESOURCE;

            }
            return Color.white;
        }
    }
}


