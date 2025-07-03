using System;
using System.Reflection;
using ML_OpenVR_FSR;

[assembly: AssemblyTitle(BuildInfo.Description)]
[assembly: AssemblyDescription(BuildInfo.Description)]
[assembly: AssemblyCompany(BuildInfo.Company)]
[assembly: AssemblyProduct(BuildInfo.Name)]
[assembly: AssemblyCopyright("Created by " + BuildInfo.Author)]
[assembly: AssemblyTrademark(BuildInfo.Company)]
[assembly: AssemblyVersion(BuildInfo.Version)]
[assembly: AssemblyFileVersion(BuildInfo.Version)]

[assembly: MelonLoader.MelonInfo(typeof(ML_OpenVR_FSR.ML_OpenVR_FSR), 
    ML_OpenVR_FSR.BuildInfo.Name,
    ML_OpenVR_FSR.BuildInfo.Version, 
    ML_OpenVR_FSR.BuildInfo.Author,
    ML_OpenVR_FSR.BuildInfo.DownloadLink)]
[assembly: MelonLoader.MelonGame(null, null)]
[assembly: MelonLoader.MelonPlatform(MelonLoader.MelonPlatformAttribute.CompatiblePlatforms.WINDOWS_X64)]
[assembly: MelonLoader.VerifyLoaderVersion("0.7.1", true)]