#nullable disable
using System;
using System.Threading.Tasks;

namespace ISocketAndSocketHandle
{
    class OwnSocket: SocketHandle
    {
        #region delegates
        public Func<int, bool> func { get; set; }
        #endregion

        #region methods
        public void Start(int timeOut)
        {
            func.Invoke(timeOut);
        }
        public async Task asyncStart(int timeOut)
        {
            await Task.Run(() => Start(timeOut));
        }
        #endregion
    }
}
