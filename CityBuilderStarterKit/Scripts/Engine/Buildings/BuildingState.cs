namespace CBSK
{

    /**
      * List of possible building states
      */
    public enum BuildingState
    {
        PLACING,            // Building being initially placed on the map
        PLACING_INVALID,    // Building being initially placed on the map, but currently in a place where it cannot be built
        IN_PROGRESS,        // Building being built
        READY,              // Building is built and awaiting the user to finish
        BUILT,              // Building is built
        MOVING              // Building is built but being moved
    }
}