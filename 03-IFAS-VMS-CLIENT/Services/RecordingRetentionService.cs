namespace IFAS.VMS.Client.Services;
public sealed class RecordingRetentionService {
 public int DeleteExpired(string root,int retentionDays) {
   if(!Directory.Exists(root))return 0; var cutoff=DateTime.Now.AddDays(-retentionDays); int count=0;
   foreach(var f in Directory.EnumerateFiles(root,"*.mp4",SearchOption.AllDirectories))try{if(File.GetCreationTime(f)<cutoff){File.Delete(f);count++;}}catch{}
   return count;
 }
}
