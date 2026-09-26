namespace IFAS.VMS.Client.Services;
public record SchedulePeriod(DayOfWeek Day, TimeSpan Start, TimeSpan End, bool Enabled=true);
public sealed class RecordingScheduleService {
 public bool IsActive(IEnumerable<SchedulePeriod> schedule, DateTime localNow) =>
   schedule.Any(x=>x.Enabled && x.Day==localNow.DayOfWeek && localNow.TimeOfDay>=x.Start && localNow.TimeOfDay<=x.End);
}
