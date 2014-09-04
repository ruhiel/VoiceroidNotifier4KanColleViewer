using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using Grabacr07.KanColleViewer.Composition;


namespace saga.voiceroid
{
    [Export(typeof(INotifier))]
    //[Export(typeof(IToolPlugin))]
    [ExportMetadata("Title", "VoiceroidNotifier")]
    [ExportMetadata("Description", "Voiceroidを使用して通知します。")]
    [ExportMetadata("Version", "1.1")]
    [ExportMetadata("Author", "@saga_dash")]
    public class VoiceroidNotifier : INotifier, IToolPlugin
    {
        //private static List<VoiceroidNotify> voiceroid = new List<VoiceroidNotify>();
        private static WorkerThread thread;
        private Boolean firstFlag = true;

        private SettingsViewModel viewmodel;


        public void Initialize()
        {
            // インスタンス化
            Assembly m = Assembly.LoadFrom("Plugins/VoiceroidNotifyCore.dll");
            Type VoiceroidFactory4Win7 = m.GetType("saga.voiceroid.VoiceroidFactory4Win7");
            VoiceroidInfo[] info = (VoiceroidInfo[])(VoiceroidFactory4Win7.GetMethod("CreateAll").Invoke(null, null));
            viewmodel = new SettingsViewModel(info);
            //voiceroid = new VoiceroidNotify4Win7("Plugins/dic/ipadic", info);
            thread = new WorkerThread();
        }

        public void Show(NotifyType type, string header, string body, Action activated, Action<Exception> failed)
        {
            if (firstFlag)
            {
                var dispatcher = System.Windows.Application.Current.Dispatcher;
                if (dispatcher.CheckAccess())
                {
                    ShowDialog();
                }
                else
                {
                    dispatcher.Invoke(() => ShowDialog());
                } 
                firstFlag = false;
            }

            thread.Add(delegate()
            {
                Run(body);
//                System.Threading.Thread.Sleep(300);
            });
        }
        private void ShowDialog()
        {
                System.Windows.Window window = new System.Windows.Window
                {
                    Content = this.GetSettingsView(),
                    Width = 300,
                    Height = 200
                };
                window.ShowDialog();
        }

        private void Run(string message)
        {
            try
            {
                Assembly m = Assembly.LoadFrom("Plugins/VoiceroidNotifyCore.dll");
                VoiceroidNotify voiceroid = (VoiceroidNotify)Activator.CreateInstance(m.GetType("saga.voiceroid.VoiceroidNotify4Win7"), new object[] { "Plugins/dic/ipadic", viewmodel.SelectedVoiceroid });
                voiceroid.SetPlayText(message);
                voiceroid.Play();
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
        }

        public string ToolName
        {
            get { return "Voiceroid"; }
        }

        public object GetToolView()
        {
            return null;
        }

        public object GetSettingsView()
        {
            return new SettingsView { DataContext = this.viewmodel,};
        }

        public void Dispose()
        {
        }
    }

}
