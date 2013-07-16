using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Drawing;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace wpf_dota2booster
{
  /// <summary>
  /// Interaction logic for PlayersOverlay.xaml
  /// </summary>
  public partial class PlayersOverlay : Window
  {
    public PlayersOverlay()
    {
      InitializeComponent();
    }
    protected override void OnSourceInitialized(EventArgs e)
    {
      base.OnSourceInitialized(e);
      var hwnd = new WindowInteropHelper(this).Handle;
      Helpers.WindowsServices.SetWindowExTransparent(hwnd);
    }
  }
}
