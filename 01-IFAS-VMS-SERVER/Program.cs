using IFAS.VMS.Shared;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Xml.Linq;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<AppState>();
builder.Services.AddHostedService<RetentionWorker>();
builder.Logging.AddConsole();

var app = builder.Build();
app.MapGet("/", () => Results.Ok(new { Product="IFAS VMS", Version="1.0.0", Status="Running" }));

app.MapGet(ApiRoutes.Cameras, (AppState s) => s.Cameras.Values);
app.MapPost(ApiRoutes.Cameras, (Camera c, AppState s) => {
    s.Cameras[c.Id] = c;
    return Results.Ok(c);
});
app.MapPut(ApiRoutes.Cameras + "/{id:guid}", (Guid id, Camera c, AppState s) => {
    if (!s.Cameras.ContainsKey(id)) return Results.NotFound();
    s.Cameras[id] = c with { Id=id };
    return Results.Ok(s.Cameras[id]);
});
app.MapDelete(ApiRoutes.Cameras + "/{id:guid}", (Guid id, AppState s) =>
    s.Cameras.TryRemove(id, out _) ? Results.Ok() : Results.NotFound());

app.MapGet(ApiRoutes.Nvrs, (AppState s) => s.Nvrs.Values);
app.MapPost(ApiRoutes.Nvrs, (Nvr n, AppState s) => { s.Nvrs[n.Id]=n; return Results.Ok(n); });
app.MapDelete(ApiRoutes.Nvrs + "/{id:guid}", (Guid id, AppState s) => s.Nvrs.TryRemove(id, out _) ? Results.Ok() : Results.NotFound());

app.MapGet("/api/nvrs/{id:guid}/channels", (Guid id, AppState s) =>
    Results.Ok(s.Channels.Values.Where(x=>x.NvrId==id)));
app.MapPost("/api/nvrs/{id:guid}/channels/discover", async (Guid id, AppState s) => {
    var nvr = s.Nvrs.GetValueOrDefault(id);
    if (nvr is null) return Results.NotFound();
    var channels = Enumerable.Range(1, 16).Select(i => new NvrChannel(Guid.NewGuid(), id, i, $"Channel {i}",
        $"rtsp://{nvr.Host}:554/Streaming/Channels/{i}01")).ToList();
    foreach(var c in channels) s.Channels[c.Id]=c;
    return Results.Ok(channels);
});

app.MapPost(ApiRoutes.Verify, async (DeviceVerifyRequest req) => {
    if (string.IsNullOrWhiteSpace(req.Host)) return Results.BadRequest(new DeviceTestResult(false,"Host is required"));
    var reachable = await TcpProbe(req.Host, req.Port, 1500);
    return Results.Ok(new DeviceTestResult(reachable, reachable ? "TCP connection succeeded" : "Host/port is not reachable", req.RtspUrl));
});

app.MapPost(ApiRoutes.Discover, async () => {
    // ONVIF WS-Discovery probe. This discovers responding NetworkVideoTransmitters on the LAN.
    var discovered = new List<object>();
    using var udp = new UdpClient(AddressFamily.InterNetwork);
    udp.EnableBroadcast = true;
    var msgId = "uuid:" + Guid.NewGuid();
    var xml = $"""
<s:Envelope xmlns:s="http://www.w3.org/2003/05/soap-envelope" xmlns:a="http://www.w3.org/2005/08/addressing" xmlns:d="http://docs.oasis-open.org/wsn/b-2" xmlns:dn="http://www.onvif.org/ver10/network/wsdl">
<s:Header><a:Action s:mustUnderstand="1">http://schemas.xmlsoap.org/ws/2005/04/discovery/Probe</a:Action><a:MessageID>{msgId}</a:MessageID><a:To s:mustUnderstand="1">urn:schemas-xmlsoap-org:ws:2005:04:discovery</a:To></s:Header>
<s:Body><d:Probe><d:Types>dn:NetworkVideoTransmitter</d:Types></d:Probe></s:Body></s:Envelope>
""";
    var bytes=Encoding.UTF8.GetBytes(xml);
    await udp.SendAsync(bytes, bytes.Length, new IPEndPoint(IPAddress.Parse("239.255.255.250"),3702));
    var end=DateTime.UtcNow.AddSeconds(3);
    while(DateTime.UtcNow<end) {
        if (udp.Available==0) { await Task.Delay(100); continue; }
        var r=await udp.ReceiveAsync();
        var text=Encoding.UTF8.GetString(r.Buffer);
        string? xaddr=null;
        try { xaddr=XDocument.Parse(text).Descendants().FirstOrDefault(x=>x.Name.LocalName=="XAddrs")?.Value; } catch {}
        discovered.Add(new { Address=r.RemoteEndPoint.Address.ToString(), XAddr=xaddr });
    }
    return Results.Ok(discovered.DistinctBy(x=>x.Address));
});

app.MapGet("/api/recordings/{deviceId:guid}/{date}", (Guid deviceId, DateOnly date, AppState s) => {
    var root = s.Cameras.GetValueOrDefault(deviceId)?.RecordingPath;
    if (string.IsNullOrWhiteSpace(root)) return Results.Ok(Array.Empty<RecordingSegment>());
    var day = Path.Combine(root, date.ToString("yyyy"), date.ToString("MM"), date.ToString("dd"));
    if (!Directory.Exists(day)) return Results.Ok(Array.Empty<RecordingSegment>());
    var list = Directory.EnumerateFiles(day,"*.mp4",SearchOption.AllDirectories).Select(f=>{
        var fi=new FileInfo(f); return new RecordingSegment(Guid.NewGuid(),deviceId,
            s.Cameras[deviceId].Name,fi.CreationTimeUtc,fi.LastWriteTimeUtc,f,fi.Length);
    }).ToList();
    return Results.Ok(list);
});

app.Run("http://0.0.0.0:5180");

static async Task<bool> TcpProbe(string host,int port,int timeout) {
    try { using var c=new TcpClient(); using var t=new CancellationTokenSource(timeout);
        await c.ConnectAsync(host,port,t.Token); return true; } catch { return false; }
}
public record DeviceVerifyRequest(string Host,int Port,string? RtspUrl);
public sealed class AppState {
    public ConcurrentDictionary<Guid,Camera> Cameras {get;}=new();
    public ConcurrentDictionary<Guid,Nvr> Nvrs {get;}=new();
    public ConcurrentDictionary<Guid,NvrChannel> Channels {get;}=new();
}
public sealed class RetentionWorker(AppState state, ILogger<RetentionWorker> log) : BackgroundService {
    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
        while(!stoppingToken.IsCancellationRequested) {
            foreach(var c in state.Cameras.Values.Where(x=>x.Enabled && x.RetentionDays>0)) {
                try {
                    var cutoff=DateTime.Now.AddDays(-c.RetentionDays);
                    if(Directory.Exists(c.RecordingPath))
                        foreach(var f in Directory.EnumerateFiles(c.RecordingPath,"*.mp4",SearchOption.AllDirectories))
                            if(File.GetCreationTime(f)<cutoff) File.Delete(f);
                } catch(Exception ex) { log.LogWarning(ex,"Retention cleanup failed for {Camera}",c.Name); }
            }
            await Task.Delay(TimeSpan.FromMinutes(15),stoppingToken);
        }
    }
}
public partial class Program {}
