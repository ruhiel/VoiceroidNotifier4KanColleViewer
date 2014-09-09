using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Windows.Input;
using Livet;

using saga.voiceroid;

namespace saga.kancolle
{
	public class SettingsViewModel : ViewModel
	{
        private ObservableCollection<VoiceroidInfo> _Info;
        public ObservableCollection<VoiceroidInfo> Info
        {
            get { return this._Info; }
            set
            {
                if (this._Info == value){
                    return;
                }
                this._Info = value;

                RaisePropertyChanged();
            }
        }
        private VoiceroidInfo _SelectedVoiceroid;
        public VoiceroidInfo SelectedVoiceroid
        {
            get { return this._SelectedVoiceroid; }
            set
            { 
                this._SelectedVoiceroid = value;
                RaisePropertyChanged();
            }
        }

        public SettingsViewModel(VoiceroidInfo[] info)
		{
            this.Info = new ObservableCollection<VoiceroidInfo>(info);
            this.SelectedVoiceroid = info[0];
		}
	}
}
