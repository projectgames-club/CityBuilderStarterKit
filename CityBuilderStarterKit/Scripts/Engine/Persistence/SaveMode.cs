namespace CBSK
{
    public enum SaveMode
    {
        SAVE_ALWAYS = 1,    // Save after all changes
        SAVE_MOSTLY = 2,    // Save after starting building, acknowledge building, starting task, acknowledge task.
        SAVE_NEVER = 4,     // Never automatically save.
    }
}