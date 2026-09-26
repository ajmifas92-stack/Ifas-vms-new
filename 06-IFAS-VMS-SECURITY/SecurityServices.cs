using System.Security.Cryptography;
using System.Text;
namespace IFAS.VMS.Security;
public static class PasswordProtector {
    public static string Protect(string value) {
        if (OperatingSystem.IsWindows()) {
            var bytes = Encoding.UTF8.GetBytes(value);
            return Convert.ToBase64String(System.Security.Cryptography.ProtectedData.Protect(bytes, null, DataProtectionScope.CurrentUser));
        }
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(value));
    }
    public static string ProtectForPortableStorage(string value) => Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(value)));
}
