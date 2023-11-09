using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using System;
using System.Reflection;

namespace Celeste.Mod.BerryTrainHelper;
public class KeepBerryTrainOnGround
{
    private static readonly MethodInfo m_Player_Update = typeof(Player).GetMethod("orig_Update");
    private static IDetour IL_Player_Update;

    private static readonly MethodInfo m_Strawberry_Update = typeof(Strawberry).GetMethod("orig_Update");
    private static IDetour IL_Strawberry_Update;

    public static void Load()
    {
        IL_Player_Update = new ILHook(m_Player_Update, Hook_Player_Update);
        IL_Strawberry_Update = new ILHook(m_Strawberry_Update, Hook_Strawberry_Update);
    }
    public static void Unload()
    {
        IL_Player_Update?.Dispose();
        IL_Strawberry_Update?.Dispose();
    }

    // modifies Player.Update to force StrawberriesBlocked to true if the mod is enabled
    private static void Hook_Player_Update(ILContext il)
    {
        ILCursor cursor = new(il);

        cursor.GotoNext(MoveType.Before, instr => instr.MatchStfld<Player>("StrawberriesBlocked"));

        // force StrawberriesBlocked to true if the mod is enabled
        cursor.EmitDelegate<Func<bool, bool>>(isInBlockfield =>
            isInBlockfield || BerryTrainHelperModule.Settings.KeepBerryTrainOnGround
        );
    }

    // modifies Strawberry.Update to collect one strawberry once the collect button is pressed 
    private static void Hook_Strawberry_Update(ILContext il)
    {
        ILCursor cursor = new(il);

        ILLabel gotoBaseUpdate = cursor.DefineLabel();

        cursor.GotoNext(MoveType.AfterLabel, instr => instr.MatchLdloc(4));

        // collect the berry train on button press (and on safe ground)
        cursor.Emit(OpCodes.Ldarg_0);  // this (berry)
        cursor.Emit(OpCodes.Ldloc_3);  // player

        cursor.EmitDelegate(CollectStrawberryOnButtonPress);

        // skip other stuff and go to base.Update() if our mod is enabled
        cursor.Emit(OpCodes.Brtrue, gotoBaseUpdate);

        cursor.GotoNext(MoveType.AfterLabel,
            instr => instr.MatchLdarg(0),
            instr => instr.MatchCall<Monocle.Entity>("Update"));
        cursor.MarkLabel(gotoBaseUpdate);
    }

    
    private static bool CollectStrawberryOnButtonPress(Strawberry berry, Player leader)
    {
        bool enabled = BerryTrainHelperModule.Settings.KeepBerryTrainOnGround;

        if (enabled 
                && !berry.Golden
                && leader.OnSafeGround
                && BerryTrainHelperModule.Settings.CollectButton.Pressed)
        {
            BerryTrainHelperModule.Settings.CollectButton.ConsumePress();
            berry.OnCollect();
        }
        return enabled;
    }
}
