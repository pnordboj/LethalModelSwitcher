using BepInEx.Logging;

namespace LethalModelSwitcher
{
    public static class CustomLogging
    {
        private static ManualLogSource logger = BepInEx.Logging.Logger.CreateLogSource("LethalModelSwitcher");

        public static void Log(string message)
        {
            logger.LogInfo(message);
        }

        public static void LogWarning(string message)
        {
            logger.LogWarning(message);
        }

        public static void LogError(string message)
        {
            logger.LogError(message);
        }

        public static void LogDebug(string message)
        {
            logger.LogDebug(message);
        }
    }
}