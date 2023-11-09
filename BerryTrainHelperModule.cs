using System;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.BerryTrainHelper;
public class BerryTrainHelperModule : EverestModule
{
    public static BerryTrainHelperModule Instance { get; private set; }

    public override Type SettingsType => typeof(BerryTrainHelperModuleSettings);
    public static BerryTrainHelperModuleSettings Settings => (BerryTrainHelperModuleSettings) Instance._Settings;

    public BerryTrainHelperModule()
    {
        Instance = this;
#if DEBUG
        // debug builds use verbose logging
        Logger.SetLogLevel(nameof(BerryTrainHelperModule), LogLevel.Verbose);
#else
        // release builds use info logging to reduce spam in log files
        Logger.SetLogLevel(nameof(BerryTrainHelperModule), LogLevel.Info);
#endif
    }

    public override void Load()
    {
        // TODO: apply any hooks that should always be active
        KeepBerryTrainOnGround.Load();
    }

    public override void Unload()
    {
        // TODO: unapply any hooks applied in Load()
        KeepBerryTrainOnGround.Unload();
    }
}