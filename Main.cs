using System;
using System.IO;
using MelonLoader;
using MelonLoader.Utils;


namespace ML_OpenVR_FSR
{
    public static class BuildInfo
    {
        public const string Name = "ML_OpenVR_FSR";
        public const string Description = "Loads the OpenVR FSR Mod at Runtime.";
        public const string Author = "Herp Derpinstine";
        public const string Company = "Lava Gang";
        public const string Version = "1.1.0";
        public const string DownloadLink = "https://github.com/LavaGang/ML_OpenVR_FSR";
    }

    public class ML_OpenVR_FSR : MelonPlugin
    {
        private static string fsrFilePath = null;
        private static NativeLibrary fsrLib = null;

        unsafe public override void OnApplicationEarlyStart()
        {
            ExtractResources();

            MelonLogger.Msg("Loading FSR Mod ahead of OpenVR...");
            fsrLib = NativeLibrary.Load(fsrFilePath);
            if (fsrLib == null)
                return;

            MelonLogger.Msg("Initialized!");
        }

        private static void ExtractResources()
        {
            string fsr_folder = Path.Combine(MelonEnvironment.UserDataDirectory, BuildInfo.Name);
            if (!Directory.Exists(fsr_folder))
            {
                Directory.CreateDirectory(fsr_folder);
                MelonLogger.Msg("UserData Folder Created!");
            }

            try
            {
                string fsr_log = Path.Combine(fsr_folder, "openvr_mod.log");
                if (File.Exists(fsr_log))
                {
                    MelonLogger.Msg("Removing Existing Log File...");
                    File.Delete(fsr_log);
                }
            }
            catch (Exception ex)
            {
                MelonDebug.Error(ex.ToString());
                MelonDebug.Error($"Failed to Remove Existing Log File! Ignoring...");
            }

            string fsr_cfg = Path.Combine(fsr_folder, "openvr_mod.cfg");
            if (!File.Exists(fsr_cfg))
            {
                MelonLogger.Msg("Extracting Default Config File...");
                File.WriteAllBytes(fsr_cfg, Properties.Resources.openvr_mod_cfg);
            }

            fsrFilePath = Path.Combine(fsr_folder, "openvr_api.dll");
            try
            {
                if (File.Exists(fsrFilePath))
                {
                    MelonLogger.Msg("Removing Existing FSR Mod DLL...");
                    File.Delete(fsrFilePath);
                }
            }
            catch (Exception ex)
            {
                MelonDebug.Error(ex.ToString());
                MelonDebug.Error($"Failed to Extract FSR Mod DLL! Ignoring...");
                return;
            }

            MelonLogger.Msg("Extracting FSR Mod DLL...");
            File.WriteAllBytes(fsrFilePath,
                //MelonUtils.IsGame32Bit()
                //? Properties.Resources.openvr_mod_x86_dll :
                Properties.Resources.openvr_mod_dll);
        }
    }
}