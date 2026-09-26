using System.Text.Json.Serialization;

namespace IFAS.VMS.Shared;

public enum DeviceProtocol { ONVIF, RTSP, Hikvision, Dahua, GenericNvr }
public enum TransportMode { TCP, UDP }
public enum DeviceKind { Camera, Nvr }
public record Camera(
    Guid Id, string Name, string Host, int Port, string Username, string Password,
    DeviceProtocol Protocol, TransportMode Transport, string RtspUrl,
    string RecordingPath, int RetentionDays = 30, bool Enabled = true);

public record Nvr(
    Guid Id, string Name, string Host, int Port, string Username, string Password,
    DeviceProtocol Protocol, bool Enabled = true);

public record NvrChannel(
    Guid Id, Guid NvrId, int ChannelNumber, string Name, string RtspUrl,
    bool Selected = true, string RecordingPath = "", int RetentionDays = 30);

public record RecordingSegment(
    Guid Id, Guid DeviceId, string DeviceName, DateTime StartUtc, DateTime EndUtc,
    string FilePath, long SizeBytes);

public record RecordingDay(DateOnly Date, bool HasRecording);
public record DeviceTestResult(bool Success, string Message, string? StreamUrl = null);
public record LicenseInfo(string Customer, int MaxCameras, DateTime ExpiresUtc, string LicenseId);
public record AppSettings(string ServerUrl = "http://localhost:5180", string DefaultRecordingRoot = @"C:\IFAS-VMS\Recordings");
