using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Dota2Booster.Properties;

namespace Dota2Booster {
  public partial class Main : Form {
    private readonly Overlay _overlay;
    private readonly ProcessMemory _processMemory = new ProcessMemory();

    private IntPtr _distanceAddress;
    private IntPtr _dotaPlayerAddress;
    private IntPtr _engineAddress;
    private IntPtr _fovAddress;
    private IntPtr _heroAddress = IntPtr.Zero;
    private IntPtr _rangeAddress;

    internal Main() {
      InitializeComponent();
      distanceUpDown.Hide();
      fovUpDown.Hide();
      rangeCheckbox.Hide();

      _overlay = new Overlay {TopMost = true};
      _overlay.Show();

      searchTimer.Start();
    }

    private void DistanceValueChanged(object sender, EventArgs e) {
      if (_distanceAddress == IntPtr.Zero) return;

      _processMemory.WriteInt32(_distanceAddress, (int) distanceUpDown.Value);
    }

    private void SearchTimerTick(object sender, EventArgs e) {
      distanceUpDown.Hide();
      fovUpDown.Hide();
      rangeCheckbox.Hide();

      statusLabel.Show();
      statusLabel.Text = Resources.searching_Dota2;
      Process[] processes = Process.GetProcessesByName("dota");
      if (processes.Length == 0) return;

      statusLabel.Text = Resources.checking_player_status___;

      _processMemory.OpenProcess(processes[0]);

      _dotaPlayerAddress = _processMemory.FindPattern(new byte[] {
        0xA1, 0xFF, 0xFF, 0xFF, 0xFF, 0x83, 0xEC, 0x10, 0x53, 0x57
      }, "x????xxxxx", _processMemory.GetModuleBaseAddress("client.dll"), 0xffffff);

      _dotaPlayerAddress = _processMemory.ReadInt32Ptr(_dotaPlayerAddress + 1);

      _engineAddress = _processMemory.FindPattern(new byte[] {
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

      searchTimer.Stop();
      checkTimer.Start();

      distanceUpDown.Value = Settings.Default.distance;
      fovUpDown.Value = Settings.Default.fov;

      statusLabel.Hide();
      distanceUpDown.Show();
      fovUpDown.Show();
      rangeCheckbox.Show();
    }

    private void CheckTimerTick(object sender, EventArgs e) {
      try {
        if (_processMemory.ReadInt32Ptr(_dotaPlayerAddress) == IntPtr.Zero) {
          return;
        }
      }
      catch (ApplicationException) {
        checkTimer.Stop();
        searchTimer.Start();
        return;
      }


      int index = _processMemory.ReadInt16(_processMemory.ReadInt32Ptr(_dotaPlayerAddress) + 0x19A4);
      index = index*16;
      _heroAddress = _processMemory.ReadInt32Ptr(_engineAddress + index);
      _overlay.BackColor = _processMemory.ReadBytes(_heroAddress + 0x12E0, 1)[0] == 30 ? Color.Red : Color.Green;

      distanceUpDown.Value = _processMemory.ReadInt32(_distanceAddress);
      fovUpDown.Value = (decimal) _processMemory.ReadFloat(_fovAddress);
      rangeCheckbox.Checked = _processMemory.ReadInt32(_rangeAddress) == 816;
    }

    private void rangeCheckbox_CheckedChanged(object sender, EventArgs e) {
      _processMemory.WriteInt32(_rangeAddress, rangeCheckbox.Checked ? 816 : 570);
    }

    private void fovUpDown_ValueChanged(object sender, EventArgs e) {
      if (_fovAddress == IntPtr.Zero) return;

      _processMemory.WriteFloat(_fovAddress, (int) fovUpDown.Value);
    }

    private void LinkLabelLinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
      Process.Start("https://github.com/gamebooster/dota2-booster");
    }

    private void Main_FormClosing(object sender, FormClosingEventArgs e) {
      Settings.Default.distance = (int) distanceUpDown.Value;
      Settings.Default.fov = (int) fovUpDown.Value;
      Settings.Default.Save();
    }
  }
}