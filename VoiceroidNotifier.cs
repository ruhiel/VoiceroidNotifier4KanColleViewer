using System;
using System.ComponentModel.Composition;
using System.Reflection;
using Grabacr07.KanColleViewer.Composition;

using saga.voiceroid;

namespace saga.kancolle
{
    [Export(typeof(IPlugin))]
    [InheritedExport(typeof(INotifier))]
    [ExportMetadata("Guid", "648b55ff-92a2-4c4a-8bb9-7f9edaa999df")]
    [ExportMetadata("Title", "VoiceroidNotifier")]
    [ExportMetadata("Description", "Voiceroidを使用して通知します。")]
    [ExportMetadata("Version", "1.3.1")]
    [ExportMetadata("Author", "@saga_dash")]
    public class VoiceroidNotifier : IPlugin, INotifier, IDisposable
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

            thread = new WorkerThread();
        }

        public void Notify(INotification notification)
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
                Run(notification.Body);
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
                VoiceroidNotify voiceroid = (VoiceroidNotify)Activator.CreateInstance(m.GetType("saga.voiceroid.VoiceroidNotify4Win7"), new object[] { "dic/ipadic", viewmodel.SelectedVoiceroid });
                voiceroid.SetPlayText(message);
                voiceroid.Play();
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
        }

        public object GetSettingsView()
        {
            return new SettingsView { DataContext = this.viewmodel,};
        }

        public void Dispose() { }
    }

}
