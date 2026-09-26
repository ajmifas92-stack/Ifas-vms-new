using System.IO;
using IFAS.VMS.Shared;
namespace IFAS.VMS.Client.Services;
public sealed class RecordingPlaybackService {
 public IReadOnlyList<RecordingSegment> Find(string root,Guid deviceId,string name,DateOnly date) {
   var day=Path.Combine(root,date.ToString("yyyy"),date.ToString("MM"),date.ToString("dd")); if(!Directory.Exists(day))return [];
   return Directory.EnumerateFiles(day,"*.mp4",SearchOption.AllDirectories).Select(f=>{var i=new FileInfo(f);return new RecordingSegment(Guid.NewGuid(),deviceId,name,i.CreationTimeUtc,i.LastWriteTimeUtc,f,i.Length);}).OrderBy(x=>x.StartUtc).ToList();
 }
}
