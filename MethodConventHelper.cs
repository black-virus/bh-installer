using System;
using System.Linq;

namespace blackhouse.Installer
{
    public static class MethodConventHelper
    {

        public static bool FindAndInvokeInstallVersionMethod(this InstallBase install, Version version) {
            var methodName = String.Format("InstallVersion{0}{1}{2}{3}", version.Major, version.Minor, version.Build,
                version.Revision);
            var type = install.GetType();
            var method = type.GetMethod(methodName);
            if (method == null) return false;
            if (method.GetParameters().Any()) return false;
            if (method.ReturnType != typeof(bool)) return false;
            return (bool)method.Invoke(install, null);
        }

    }
}
