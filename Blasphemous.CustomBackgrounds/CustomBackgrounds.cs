using Blasphemous.ModdingAPI;

namespace Blasphemous.CustomBackgrounds
{
    public class CustomBackgrounds : BlasMod
    {
        public CustomBackgrounds() : base(ModInfo.MOD_ID, ModInfo.MOD_NAME, ModInfo.MOD_AUTHOR, ModInfo.MOD_VERSION) { }

        protected override void OnInitialize()
        {
            LogError($"{ModInfo.MOD_NAME} has been initialized");
        }
    }
}
