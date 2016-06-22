using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Quartz
{
    public interface IMyJob : IQJob
    {
       
        void Excute();

        void QRemove();

        void QPause();

        void QResume();
    }
}
