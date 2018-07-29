namespace Linq.GraphQL.Provider
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Linq.GraphQL.DTO;

    public class GrapQLResponseEnumerator<T> : IEnumerator<T>
    {
        private Task<GraphQLResponse<T>> deffered;
        GraphQLResponse<T> response;
        IEnumerator<T> enumeratorFromDeffered;

        public GrapQLResponseEnumerator(Task<GraphQLResponse<T>> deffered)
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
                this.response = deffered.Result;
                if (this.response == null)
                    throw new NullReferenceException($"the response from server is null");

                var err = this.response.Meta.Error;
                if (err != null)
                    throw new Exception(err.Message);

                this.enumeratorFromDeffered = (this.response.Data as IEnumerable<T>).GetEnumerator();
            }
        }
    }
}