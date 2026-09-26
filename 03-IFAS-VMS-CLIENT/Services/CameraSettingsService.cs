namespace IFAS.VMS.Client.Services;
public sealed class CameraSettingsService {
 public string BuildRtsp(string host,int port,string path,string user,string password) =>
   $"rtsp://{Uri.EscapeDataString(user)}:{Uri.EscapeDataString(password)}@{host}:{port}/{path.TrimStart('/')}";
}
