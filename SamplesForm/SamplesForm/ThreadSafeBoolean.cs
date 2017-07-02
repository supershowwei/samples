using System.Threading;

namespace SamplesForm
{
    public class ThreadSafeBoolean
    {
        private int interlockedInteger;

        public ThreadSafeBoolean(bool value)
        {
            this.Value = value;
        }

        private bool Value
        {
            get
            {
                return this.interlockedInteger == 1;
            }

            set
            {
                Interlocked.Exchange(ref this.interlockedInteger, value ? 1 : 0);
            }
        }

        public static implicit operator bool(ThreadSafeBoolean b)
        {
            return b.Value;
        }

        public static implicit operator ThreadSafeBoolean(bool b)
        {
            return new ThreadSafeBoolean(b);
        }

        public override string ToString()
        {
            return this.Value.ToString();
        }
    }
}