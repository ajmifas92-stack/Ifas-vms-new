namespace IFAS.VMS.Shared;
public interface ICameraProvider {
    Task<DeviceTestResult> VerifyAsync(Camera camera, CancellationToken ct = default);
    Task<IReadOnlyList<Camera>> DiscoverAsync(CancellationToken ct = default);
}
public interface IRecordingService {
    Task StartAsync(Camera camera, CancellationToken ct = default);
    Task StopAsync(Guid cameraId, CancellationToken ct = default);
    Task<IReadOnlyList<RecordingSegment>> SearchAsync(Guid deviceId, DateOnly date, CancellationToken ct = default);
}
public interface IRetentionService {
    Task RunCleanupAsync(CancellationToken ct = default);
}
