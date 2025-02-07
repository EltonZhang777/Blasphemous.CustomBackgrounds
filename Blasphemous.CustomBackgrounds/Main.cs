using BepInEx;

namespace Blasphemous.CustomBackgrounds
{
    [BepInPlugin(ModInfo.MOD_ID, ModInfo.MOD_NAME, ModInfo.MOD_VERSION)]
    [BepInDependency("Blasphemous.ModdingAPI", "0.1.0")]
    public class Main : BaseUnityPlugin
    {
        public static CustomBackgrounds CustomBackgrounds { get; private set; }

        private void Start()
        {
            CustomBackgrounds = new CustomBackgrounds();
        }
    }
}
