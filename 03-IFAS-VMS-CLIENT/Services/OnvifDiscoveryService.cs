using System.Net;
using System.Net.Sockets;
using System.Text;
using IFAS.VMS.Shared;
namespace IFAS.VMS.Client.Services;
public sealed class OnvifDiscoveryService : ICameraProvider {
 public async Task<IReadOnlyList<Camera>> DiscoverAsync(CancellationToken ct=default){
   var result=new List<Camera>(); using var udp=new UdpClient(AddressFamily.InterNetwork); udp.EnableBroadcast=true;
   var xml=$"""<s:Envelope xmlns:s="http://www.w3.org/2003/05/soap-envelope" xmlns:d="http://docs.oasis-open.org/wsn/b-2" xmlns:dn="http://www.onvif.org/ver10/network/wsdl"><s:Header/><s:Body><d:Probe><d:Types>dn:NetworkVideoTransmitter</d:Types></d:Probe></s:Body></s:Envelope>""";
   var b=Encoding.UTF8.GetBytes(xml); await udp.SendAsync(b,b.Length,new IPEndPoint(IPAddress.Parse("239.255.255.250"),3702));
   var end=DateTime.UtcNow.AddSeconds(3); while(DateTime.UtcNow<end && !ct.IsCancellationRequested){ if(udp.Available==0){await Task.Delay(100,ct);continue;} var r=await udp.ReceiveAsync(ct); result.Add(new Camera(Guid.NewGuid(),"ONVIF "+r.RemoteEndPoint.Address,r.RemoteEndPoint.Address.ToString(),80,"","",DeviceProtocol.ONVIF,TransportMode.TCP,$"rtsp://{r.RemoteEndPoint.Address}:554/stream1",@"C:\IFAS-VMS\Recordings")); } return result;
 }
 public Task<DeviceTestResult> VerifyAsync(Camera c,CancellationToken ct=default)=>Task.FromResult(new DeviceTestResult(!string.IsNullOrWhiteSpace(c.Host),"Discovery/provider verification requires device credentials and capabilities.",c.RtspUrl));
}
