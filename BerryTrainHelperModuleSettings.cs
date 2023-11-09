using Microsoft.Xna.Framework.Input;

namespace Celeste.Mod.BerryTrainHelper;
public class BerryTrainHelperModuleSettings : EverestModuleSettings
{
    public bool KeepBerryTrainOnGround { get; set; }

    [DefaultButtonBinding(Buttons.RightStick, Keys.V)]
    public ButtonBinding CollectButton { get; set; }
}
