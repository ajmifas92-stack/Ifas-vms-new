using IFAS.VMS.Shared;
using LibVLCSharp.Shared;
using VlcMediaPlayer = LibVLCSharp.Shared.MediaPlayer;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace IFAS.VMS.Client;
public partial class MainWindow : Window {
    readonly HttpClient http=new(){BaseAddress=new Uri("http://localhost:5180")};
    int rows=2, cols=2;
    readonly List<VlcMediaPlayer> players=new();
    readonly List<Camera> cameras=new();
    public MainWindow(){ InitializeComponent(); Core.Initialize(); Loaded += async(_,_)=>await LoadCameras(); BuildGrid(); }
    async Task LoadCameras(){ try { cameras.Clear(); cameras.AddRange(await http.GetFromJsonAsync<Camera[]>("/api/cameras")??[]); DeviceGrid.ItemsSource=cameras; } catch(Exception ex){ TitleText.Text="Server unavailable: "+ex.Message; } }
    void BuildGrid(){ LiveGrid.Children.Clear(); LiveGrid.RowDefinitions.Clear(); LiveGrid.ColumnDefinitions.Clear(); players.ForEach(p=>p.Dispose()); players.Clear();
        for(int r=0;r<rows;r++)LiveGrid.RowDefinitions.Add(new RowDefinition()); for(int c=0;c<cols;c++)LiveGrid.ColumnDefinitions.Add(new ColumnDefinition());
        for(int i=0;i<rows*cols;i++){ var b=new Border{BorderBrush=Brushes.DimGray,BorderThickness=new Thickness(1),Background=Brushes.Black,Tag=i};
            var sp=new StackPanel(); sp.Children.Add(new TextBlock{Text=$"Channel {i+1} — Drag camera here",Foreground=Brushes.White,Margin=new Thickness(5)});
            var vv=new LibVLCSharp.WPF.VideoView(); sp.Children.Add(vv); b.Child=sp; Grid.SetRow(b,i/cols);Grid.SetColumn(b,i%cols); LiveGrid.Children.Add(b);
            b.AllowDrop=true; b.Drop += LiveGrid_Drop;
        }}
    void SetGrid(int r,int c){rows=r;cols=c;BuildGrid();}
    void G1(object s,RoutedEventArgs e)=>SetGrid(1,1); void G4(object s,RoutedEventArgs e)=>SetGrid(2,2); void G8(object s,RoutedEventArgs e)=>SetGrid(2,4); void G16(object s,RoutedEventArgs e)=>SetGrid(4,4); void G32(object s,RoutedEventArgs e)=>SetGrid(4,8); void G64(object s,RoutedEventArgs e)=>SetGrid(8,8);
    void Dashboard_Click(object s,RoutedEventArgs e)=>TitleText.Text="Dashboard"; void Live_Click(object s,RoutedEventArgs e)=>TitleText.Text="Live View"; void Playback_Click(object s,RoutedEventArgs e)=>TitleText.Text="Playback — Calendar / Timeline";
    void Camera_Click(object s,RoutedEventArgs e){TitleText.Text="Camera Management — ONVIF / RTSP / Hikvision / Dahua / Generic";} void Nvr_Click(object s,RoutedEventArgs e){TitleText.Text="NVR Management — Discover Channels";} void Storage_Click(object s,RoutedEventArgs e){TitleText.Text="Recording Storage / Schedule / Retention";} 
    void Monitor_Click(object s,RoutedEventArgs e){ var w=new MonitorWindow(); w.Show(); }
    void LiveGrid_Drop(object? sender,DragEventArgs e){ if(e.Data.GetData(typeof(Camera)) is Camera c && sender is Border b){ b.Tag=c; if(b.Child is StackPanel sp && sp.Children[0] is TextBlock t)t.Text=c.Name+" — "+c.RtspUrl; TryPlay(c,b); } }
    void TryPlay(Camera c,Border b){ try { if(b.Child is not StackPanel sp || sp.Children.Count<2)return; var vv=(LibVLCSharp.WPF.VideoView)sp.Children[1]; var vlc=new LibVLC("--network-caching=800","--rtsp-tcp"); var mp=new VlcMediaPlayer(vlc); players.Add(mp); vv.MediaPlayer=mp; using var media=new Media(vlc,new Uri(c.RtspUrl)); mp.Play(media); } catch { } }
}
public class MonitorWindow:Window { public MonitorWindow(){Title="IFAS VMS — Extended Monitor";Width=1200;Height=800;Background=Brushes.Black; Content=new TextBlock{Text="Independent IFAS VMS monitor window — assign cameras and layout here.",Foreground=Brushes.White,FontSize=22,Margin=new Thickness(30)};} }
