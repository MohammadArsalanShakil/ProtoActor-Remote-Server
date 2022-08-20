using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProtoActor_Remote_Server.Common
{
    public class PeriodicTask
    {
        public async Task Run(Action action, TimeSpan period, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(period, cancellationToken);

                if (!cancellationToken.IsCancellationRequested)
                    action();
            }
        }

        public Task Run(Action action, TimeSpan period)
        {
            return Run(action, period, CancellationToken.None);
        }
    }
}
