using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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
  /// Interaction logic for IsVisibleOverlay.xaml
  /// </summary>
  public partial class IsVisibleOverlay : Window
  {
    public IsVisibleOverlay()
    {
      InitializeComponent();
    }


    private void Window_MouseDown(object sender, MouseButtonEventArgs e)
    {
      this.DragMove();
    }
  }
}
