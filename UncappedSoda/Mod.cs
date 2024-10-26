using GDWeave;

namespace UncappedSoda;

public class Mod : IMod
{
    public Mod(IModInterface modInterface)
    {
        modInterface.RegisterScriptMod(new PlayerPatch());
    }

    public void Dispose()
    {
    }
}
