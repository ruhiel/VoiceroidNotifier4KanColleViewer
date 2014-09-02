using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace saga.voiceroid
{
    //http://d.hatena.ne.jp/zarchis/20101015
    public class WorkerThread : IDisposable
    {
        private Queue<Action> jobs = new Queue<Action>();
        private volatile bool running = true;
        private ManualResetEvent ev = new ManualResetEvent(false);

        public WorkerThread()
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(Execute));
        }

        private void Execute(object o)
        {
            while (ev.WaitOne())
            {
                if (!running) { break; }

                Action actor = null;
                lock (jobs)
                {
                    if (jobs.Count > 0)
                    {
                        actor = jobs.Dequeue();
                    }
                    else
                    {
                        ev.Reset();
                    }
                }
                if (actor != null)
                {
                    actor();
                }
            }
        }

        public void Add(Action job)
        {
            lock (jobs)
            {
                jobs.Enqueue(job);
                ev.Set();
            }
        }

        #region IDisposable メンバ

        public void Dispose()
        {
            running = false;
            ev.Set();
        }

        #endregion
    }
}
