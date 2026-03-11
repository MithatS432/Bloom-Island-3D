public class MissionRuntime
{
    public MissionDefinition definition;
    public int progress;

    public MissionRuntime(MissionDefinition def)
    {
        definition = def;
        progress = 0;
    }

    public bool IsComplete()
    {
        return progress >= definition.targetAmount;
    }

    public void AddProgress(int amount)
    {
        progress += amount;
    }
}