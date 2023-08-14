using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class ProgressService
    {
        public event Action<int> ProgressChanged;

        public void UpdateProgress(int progress)
        {
            ProgressChanged?.Invoke(progress);
        }
    }
}
