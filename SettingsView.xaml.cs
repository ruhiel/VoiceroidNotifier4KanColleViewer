using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace saga.voiceroid
{
	/// <summary>
    /// SettingView.xaml の相互作用ロジック
	/// </summary>
	public partial class SettingsView : UserControl
	{
        public SettingsView()
		{
			InitializeComponent();
		}

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Close();
        }
	}
}
