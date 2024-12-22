using System;
using System.Reflection;

namespace BetterVanish.Patches
{
    public static class HarmonyExceptionHandler
    {
        public delegate void ReportCleanupRequested(Type type, Exception exception, MethodBase originalMethod);
        public static event ReportCleanupRequested OnReportCleanupRequested;

        public static void ReportCleanupException(Type type, Exception exception, MethodBase originalMethod)
        {
            if (exception != null)
            {
                OnReportCleanupRequested?.Invoke(type, exception, originalMethod);
            }
        }
    }
}