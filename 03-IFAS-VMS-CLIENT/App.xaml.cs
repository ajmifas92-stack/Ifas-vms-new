using System.Windows;
namespace IFAS.VMS.Client;
public partial class App : Application { protected override void OnStartup(StartupEventArgs e){ base.OnStartup(e); new MainWindow().Show(); } }
