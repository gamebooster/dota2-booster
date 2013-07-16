
using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Dota2Booster;
using Xceed.Wpf.Toolkit;
using wpf_dota2booster.Properties;

namespace wpf_dota2booster
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window {
    private readonly IsVisibleOverlay _isVisibleOverlay;
    private readonly PlayersOverlay _playersOverlay;
    private readonly ProcessMemory _processMemory = new ProcessMemory();

    private readonly DispatcherTimer _searchTimer = new DispatcherTimer();
    private readonly DispatcherTimer _checkTimer  = new DispatcherTimer();

    private IntPtr _distanceAddress;
    private IntPtr _dotaPlayerAddress;
    private IntPtr _engineAddress;
    private IntPtr _fovAddress;
    private IntPtr _heroAddress = IntPtr.Zero;
    private IntPtr _rangeAddress;
    private IntPtr _playerResourceAddress;
    private IntPtr _playersListAddress;

    public MainWindow() {
      InitializeComponent();

      _searchTimer.Interval = new TimeSpan(0,0,0,0,1000);
      _searchTimer.Tick += _searchTimer_Tick;
      _searchTimer.Start();
      _checkTimer.Interval =  new TimeSpan(0,0,0,0,200);
      _checkTimer.Tick += _checkTimer_Tick;

      distanceUpDown.Visibility = Visibility.Hidden;
      fovUpDown.Visibility = Visibility.Hidden;
      rangeCheckbox.Visibility = Visibility.Hidden;

      _isVisibleOverlay = new IsVisibleOverlay { Topmost = true};
      _isVisibleOverlay.Show();

      _playersOverlay = new PlayersOverlay { Topmost = true };
      _playersOverlay.Show();

      positionXLayoutUpDown.Value = Settings.Default.positionXOverlay;
      positionYLayoutUpDown.Value = Settings.Default.positionYOverlay;

      _playersOverlay.Left = Settings.Default.positionXOverlay;
      _playersOverlay.Top = Settings.Default.positionYOverlay;

      _searchTimer.Start();
    }

    private void distanceUpDown_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
      if (_distanceAddress == IntPtr.Zero) return;

      _processMemory.WriteInt32(_distanceAddress, (int)distanceUpDown.Value);
    }

    private void _searchTimer_Tick(object sender, EventArgs e)
    {
      distanceUpDown.Visibility = Visibility.Hidden;
      fovUpDown.Visibility = Visibility.Hidden;
      rangeCheckbox.Visibility = Visibility.Hidden;


      Process[] processes = Process.GetProcessesByName("dota");
      if (processes.Length == 0) return;

      _processMemory.OpenProcess(processes[0]);

      _playerResourceAddress = _processMemory.FindPattern(new byte[] {
        0xC6, 0x40, 0x10, 0x02, 0x8B, 0x35, 0xFF, 0xFF, 0xFF, 0xFF, 0x85, 0xF6
      }, "xxxxxx????xx", _processMemory.GetModuleBaseAddress("client.dll"), 0xffffff);

      _playerResourceAddress = _processMemory.ReadInt32Ptr(_playerResourceAddress + 6);

      _dotaPlayerAddress = _processMemory.FindPattern(new byte[] {
        0xA1, 0xFF, 0xFF, 0xFF, 0xFF, 0x83, 0xEC, 0x10, 0x53, 0x57
      }, "x????xxxxx", _processMemory.GetModuleBaseAddress("client.dll"), 0xffffff);

      _dotaPlayerAddress = _processMemory.ReadInt32Ptr(_dotaPlayerAddress + 1);

      _playersListAddress = _processMemory.FindPattern(new byte[] { // dota_players_list
        0x7c, 0x71, 0xBB, 0xFF, 0xFF, 0xFF, 0xFF
      }, "xxx????", _processMemory.GetModuleBaseAddress("client.dll"), 0xffffff);

      _playersListAddress = _processMemory.ReadInt32Ptr(_playersListAddress + 3);

      _engineAddress = _processMemory.FindPattern(new byte[] { // hero_npc_list
        0xC1, 0xEB, 0x10, 0x81, 0xC2, 0xFF, 0xFF, 0xFF, 0xFF
      }, "xxxxx????", _processMemory.GetModuleBaseAddress("client.dll"), 0xffffff);

      _engineAddress = _processMemory.ReadInt32Ptr(_engineAddress + 5);

      IntPtr offsetAddress = _processMemory.FindPattern(new byte[] {
        0x8B, 0x0D, 0xFF, 0xFF, 0xFF, 0xFF, 0x8B, 0x40, 0x08, 0x66, 0x0F, 0xD6, 0x44, 0x24, 0xFF
      }, "xx????xxxxxxxx?", _processMemory.GetModuleBaseAddress("client.dll"), 0xffffff);

      _distanceAddress = _processMemory.ReadInt32Ptr(offsetAddress + 2) + 0x14;

      _fovAddress = _processMemory.FindPattern(new byte[] {
        0x74, 0x08, 0x8B, 0xCE, 0x5E, 0xE9, 0xFF, 0xFF, 0xFF, 0xFF, 0xD9, 0x05
      }, "xxxxxx????xx", _processMemory.GetModuleBaseAddress("client.dll"), 0xffffff);

      _fovAddress = _processMemory.ReadInt32Ptr(_fovAddress + 12);

      _rangeAddress = _processMemory.FindPattern(new byte[] {
        0xA3, 0xFF, 0xFF, 0xFF, 0xFF, 0x33, 0xC9
      }, "x????xx", _processMemory.GetModuleBaseAddress("client.dll"), 0xffffff);

      _rangeAddress = _processMemory.ReadInt32Ptr(_rangeAddress + 1);

      _searchTimer.Stop();
      _checkTimer.Start();

      distanceUpDown.Value = Settings.Default.distance;
      fovUpDown.Value = Settings.Default.fov;

      distanceUpDown.Visibility = Visibility.Visible;
      fovUpDown.Visibility = Visibility.Visible;
      rangeCheckbox.Visibility = Visibility.Visible;
    }

    private void _checkTimer_Tick(object sender, EventArgs e) {
      IntPtr playerResourceObject = IntPtr.Zero;
      try {
        if (_processMemory.ReadInt32Ptr(_dotaPlayerAddress) == IntPtr.Zero) return;
        playerResourceObject = _processMemory.ReadInt32Ptr(_playerResourceAddress);
        if (playerResourceObject == IntPtr.Zero) return;
      }
      catch (ApplicationException) {
        _checkTimer.Stop();
        _searchTimer.Start();
        return;
      }

      int numHeroes = 0;
      var container = new StackPanel { Margin = new Thickness(0)};
      for (int i = 0; i < 32; i++)
      {
        IntPtr dotaPlayer = _processMemory.ReadInt32Ptr(_playersListAddress + 16 * i);
        if (dotaPlayer != IntPtr.Zero)
        {
          int selectedUnits = _processMemory.ReadInt32(dotaPlayer + 0x1A78);
          int playerId = _processMemory.ReadInt32(dotaPlayer + 0x1A14);
          int index2 = _processMemory.ReadInt16(dotaPlayer + 0x19A4);
          IntPtr hero = _processMemory.ReadInt32Ptr(_engineAddress + index2 * 16);
          bool visible = _processMemory.ReadBytes(hero + 0x12E0, 1)[0] == 30;
          float mana = _processMemory.ReadFloat(hero + 0x1134);
          float maxMana = _processMemory.ReadFloat(hero + 0x1138);
          float health = _processMemory.ReadInt32(hero + 0xfc);
          float maxHealth = _processMemory.ReadInt32(hero + 0x110c);
          int level = _processMemory.ReadInt32(hero + 0x10fc);
          int damageMax = _processMemory.ReadInt32(hero + 0x12d8);
          int gold = _processMemory.ReadInt32(playerResourceObject + 0x4c80 + 4 * playerId) + _processMemory.ReadInt32(playerResourceObject + 0x4d00 + 4 * playerId);

          IntPtr playerName = _processMemory.ReadInt32Ptr(playerResourceObject + 0x5BC0 + 4 * playerId);
          if (playerName == IntPtr.Zero) continue;
          var semiTransparentControl = new Grid
          {
            Margin = new Thickness(0, 5, 0, 0),
            Height = 40,
            Width = 100,
            Background = new SolidColorBrush(Color.FromArgb(100, 0, 0, 0))
          };

          semiTransparentControl.Children.Add(new Label
          {
            Content = string.Format("{0}", level),
            Margin = new Thickness(0, 0, 0, 0),
            Foreground = Brushes.White,
          });
          semiTransparentControl.Children.Add(new Label
          {
            Content = string.Format("{0}", _processMemory.ReadCString(playerName)),
            Margin = new Thickness(25, 0, 0, 0),
            Foreground = Brushes.White,
          });
          semiTransparentControl.Children.Add(new Label
          {
            Content = string.Format("{0:0}", mana),
            Margin = new Thickness(30, 15, 0, 0),
            Foreground = Brushes.LightBlue
          });
          semiTransparentControl.Children.Add(new Label
          {
            Content = string.Format("{0}", health),
            Margin = new Thickness(0, 15, 0, 0),
            Foreground = health < 500 ? Brushes.OrangeRed : Brushes.GreenYellow

          });
          semiTransparentControl.Children.Add(new Label
          {
            Content = string.Format("{0}", gold),
            Margin = new Thickness(60, 15, 0, 0),
            Foreground = Brushes.Gold
          });
          container.Children.Add(semiTransparentControl);
          numHeroes++;
        }
      }
      _playersOverlay.Content = container;
      _playersOverlay.Height = 100 + numHeroes * 35;
      _playersOverlay.Width = 120;

      int index = _processMemory.ReadInt16(_processMemory.ReadInt32Ptr(_dotaPlayerAddress) + 0x19A4);
      _heroAddress = _processMemory.ReadInt32Ptr(_engineAddress + index * 16);
      _isVisibleOverlay.Background = _processMemory.ReadBytes(_heroAddress + 0x12E0, 1)[0] == 30 ? Brushes.Red : Brushes.Green;

      distanceUpDown.Value = _processMemory.ReadInt32(_distanceAddress);
      fovUpDown.Value = (int?) _processMemory.ReadFloat(_fovAddress);
      rangeCheckbox.IsChecked = _processMemory.ReadInt32(_rangeAddress) == 816;
    }

    private void rangeCheckbox_Click(object sender, RoutedEventArgs e)
    {
      _processMemory.WriteInt32(_rangeAddress, (bool) rangeCheckbox.IsChecked ? 816 : 570);
    }

    private void fovUpDown_ValueChanged_1(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
      if (_fovAddress == IntPtr.Zero) return;

      _processMemory.WriteFloat(_fovAddress, (int)fovUpDown.Value);
    }

    private void LinkLabelLinkClicked(object sender)
    {
      Process.Start("https://github.com/gamebooster/dota2-booster");
    }

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
      Settings.Default.positionYOverlay = (int) _playersOverlay.Top;
      Settings.Default.positionXOverlay = (int) _playersOverlay.Left;
      Settings.Default.distance = (int)distanceUpDown.Value;
      Settings.Default.fov = (int)fovUpDown.Value;
      Settings.Default.Save();
      _playersOverlay.Close();
      _isVisibleOverlay.Close();
    }

    private void positionXLayoutUpDown_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e) {
      _playersOverlay.Left = (int)positionXLayoutUpDown.Value;
    }

    private void positionYLayoutUpDown_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e) {
      _playersOverlay.Top = (int)positionYLayoutUpDown.Value;
    }
  }
}
