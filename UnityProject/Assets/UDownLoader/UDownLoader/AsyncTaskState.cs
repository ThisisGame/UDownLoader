using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThisisGame
{
    public enum AsyncTaskState
    {
        None,
        Prepare,
        DownLoading,
        Error,
        Complete
    }
}
