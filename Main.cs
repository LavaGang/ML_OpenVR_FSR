using System;
using System.Diagnostics;
using System.IO;
using MelonLoader;

namespace ML_OpenVR_FSR
{
    public static class BuildInfo
    {
        public const string Name = "ML_OpenVR_FSR";
        public const string Description = "Loads the OpenVR FSR Mod at Runtime.";
        public const string Author = "Herp Derpinstine";
        public const string Company = "Lava Gang";
        public const string Version = "1.0.1";
        public const string DownloadLink = "https://github.com/HerpDerpinstine/ML_OpenVR_FSR";
    }

    public class ML_OpenVR_FSR : MelonPlugin
    {
        private static string fsrFilePath = null;
        private static NativeLibrary openVRLib = null;
        private static NativeLibrary fsrLib = null;

        private static string[] ExportTbl =
        {
            "VRHeadsetView",
            "VR_GetGenericInterface",
            "VR_GetInitToken",
            "VR_GetRuntimePath",
            "VR_GetStringForHmdError",
            "VR_GetVRInitErrorAsEnglishDescription",
            "VR_GetVRInitErrorAsSymbol",
            "VR_InitInternal",
            "VR_InitInternal2",
            "VR_IsHmdPresent",
            "VR_IsInterfaceVersionValid",
            "VR_IsRuntimeInstalled",
            "VR_RuntimePath",
            "VR_ShutdownInternal"
        };

        unsafe public override void OnApplicationEarlyStart()
        {
            ExtractResources();

            MelonLogger.Msg("Searching for openvr_api.dll...");

            string plugins_path = Path.Combine(Path.Combine(MelonUtils.GameDirectory,
                $"{Process.GetCurrentProcess().ProcessName}_Data"),
                "Plugins");

            string orig_openvr = null;
            foreach (string file_path in Directory.GetFiles(plugins_path, "*.*", SearchOption.AllDirectories))
            {
                if (string.IsNullOrEmpty(file_path))
                    continue;
                if (Path.GetFileName(file_path).Equals("openvr_api.dll"))
                {
                    orig_openvr = file_path;
                    break;
                }
            }
            if (orig_openvr == null)
            {
                MelonLogger.Error("Unable to Find openvr_api.dll!");
                return;
            }

            MelonLogger.Msg("Loading OpenVR API...");
            openVRLib = NativeLibrary.Load(orig_openvr);
            if (openVRLib == null)
                return;

            MelonLogger.Msg("Loading FSR Mod...");
            fsrLib = NativeLibrary.Load(fsrFilePath);
            if (fsrLib == null)
            {
                MelonLogger.Error("Unable to Load FSR Mod Native Library!");
                return;
            }

            MelonLogger.Msg("Attaching Exports...");
            foreach (string export_name in ExportTbl)
            {
                IntPtr export_ptr_orig;
                try { export_ptr_orig = openVRLib.GetExport(export_name); }
                catch (Exception ex)
                {
                    MelonDebug.Error(ex.ToString());
                    MelonDebug.Error($"Unable to Get Export {export_name} from OpenVR API! Ignoring...");
                    continue;
                }
                if (export_ptr_orig == IntPtr.Zero)
                    continue;

                IntPtr export_ptr_new;
                try { export_ptr_new = fsrLib.GetExport(export_name); }
                catch (Exception ex)
                {
                    MelonDebug.Error(ex.ToString());
                    MelonDebug.Error($"Unable to Get Export {export_name} on FSR Mod! Ignoring...");
                    continue;
                }
                if (export_ptr_new == IntPtr.Zero)
                    continue;

                IntPtr* export_ptr_orig_ptr = &export_ptr_orig;
                MelonUtils.NativeHookAttach((IntPtr)export_ptr_orig_ptr, export_ptr_new);
            }

            MelonLogger.Msg("Initialized!");
        }

        private static void ExtractResources()
        {
            string fsr_folder = Path.Combine(MelonUtils.UserDataDirectory, BuildInfo.Name);
            if (!Directory.Exists(fsr_folder))
            {
                Directory.CreateDirectory(fsr_folder);
                MelonLogger.Msg("UserData Folder Created!");
            }

            string fsr_name = "openvr_mod";

            try
            {
                string fsr_log = Path.Combine(fsr_folder, $"{fsr_name}.log");
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

            string fsr_cfg = Path.Combine(fsr_folder, $"{fsr_name}.cfg");
            if (!File.Exists(fsr_cfg))
            {
                MelonLogger.Msg("Extracting Default Config File...");
                File.WriteAllBytes(fsr_cfg, Properties.Resources.openvr_mod_cfg);
            }

            fsrFilePath = Path.Combine(fsr_folder, $"{fsr_name}.dll");
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
            File.WriteAllBytes(fsrFilePath, Properties.Resources.openvr_mod_dll);
        }
    }
}