namespace Linq.GraphQL.Provider
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class LazyEnumerator<T> : IEnumerator<T>
    {
        private Task<IEnumerable<T>> deffered;
        IEnumerator<T> enumeratorFromDeffered;

        public LazyEnumerator(Task<IEnumerable<T>> deffered)
        {
            this.deffered = deffered;
        }

        public T Current
        {
            get
            {
                this.CheckInit();
                return enumeratorFromDeffered.Current;
            }
        }

        object IEnumerator.Current => this.Current;

        public bool MoveNext()
        {
            this.CheckInit();
            return enumeratorFromDeffered.MoveNext();
        }

        public void Reset()
        {
            this.CheckInit();
            enumeratorFromDeffered.Reset();
        }

        public void Dispose()
        {
            deffered.Dispose();
        }

        private void CheckInit()
        {
            if (enumeratorFromDeffered == null)
            {
                deffered.Wait();
                enumeratorFromDeffered = deffered.Result.GetEnumerator();
            }
        }
    }
}