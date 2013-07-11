using System;
using System.Diagnostics;
using System.Windows.Forms;
using Dota2Booster.Properties;

namespace Dota2Booster {
  public partial class Main : Form {
    private readonly ProcessMemory _processMemory = new ProcessMemory();

    private IntPtr _distanceAddress;
    private IntPtr _fovAddress;
    private IntPtr _rangeAddress;

    internal Main() {
      InitializeComponent();
      distanceUpDown.Hide();
      fovUpDown.Hide();
      rangeCheckbox.Hide();

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
        _processMemory.ReadInt32Ptr(_distanceAddress);
      }
      catch (ApplicationException) {
        checkTimer.Stop();
        searchTimer.Start();
        return;
      }

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