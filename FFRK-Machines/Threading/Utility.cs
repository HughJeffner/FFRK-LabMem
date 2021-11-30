using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FFRK_Machines.Threading
{
    public class Utility
    {

        public static Task StartSTATask(Action func)
        {
            var tcs = new TaskCompletionSource<object>();
            var thread = new Thread(() =>
            {
                try
                {
                    func();
                    tcs.SetResult(null);
                }
                catch (Exception e)
                {
                    tcs.SetException(e);
                }
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            return tcs.Task;
        }

    }
}
