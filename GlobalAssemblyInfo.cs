using System.Reflection;

[assembly: AssemblyVersion(TgsAssemblyVer.Version + ".0")]
[assembly: AssemblyFileVersion(TgsAssemblyVer.Version + ".0")]
[assembly: AssemblyInformationalVersion(TgsAssemblyVer.Version)] // for nuget

internal static class TgsAssemblyVer
{
    public const string Version = "0.1.0";
}