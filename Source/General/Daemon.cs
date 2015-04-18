using System.Threading;

namespace Quark.Utilities
{
    public abstract class Daemon<T> : Daemon where T : Daemon
    {
        public static T GetInstance()
        {
            return QuarkMain.GetInstance().GetDaemon<T>();
        }
    }

    public abstract class Daemon
    {
        public virtual void Register()
        {
            Messenger.AddListener("Update", Update);
        }

        public virtual void Terminate()
        {
            Messenger.RemoveListener("Update", Update);
        }

        public abstract void Update();
    }

    public abstract class AsyncDaemon<T> : Daemon<T> where T : Daemon
    {
        private readonly Thread _thread;

        protected AsyncDaemon()
        {
            _thread = new Thread(Update);
        }

        public override void Register()
        {
            _thread.Start();
        }

        public override void Terminate()
        {
            _thread.Abort();
        }
    }
}
