using BepInEx.Configuration;

namespace LethalModelSwitcher.Managers
{
    public static class SaveManager
    {
        private static ConfigFile configFile = new ConfigFile("LethalModelSwitcher.cfg", true);

        public static void SaveModelState(string suitName, string modelName)
        {
            configFile.Bind("ModelState", suitName, modelName);
            configFile.Save();
        }

        public static string LoadModelState(string suitName)
        {
            return configFile.Bind("ModelState", suitName, "").Value;
        }
    }
}