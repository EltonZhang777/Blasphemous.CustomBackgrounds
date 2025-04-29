using Blasphemous.ModdingAPI.Persistence;
using System.Collections.Generic;

namespace Blasphemous.CustomBackgrounds.Persistence;

/// <summary>
/// Contains save data that are specific to in-game save slots.
/// </summary>
public class SlotSaveData : SaveData
{
    public List<ModMapMarkSaveData> modMapMarks = new();

    public SlotSaveData() : base(ModInfo.MOD_ID) { }
}
