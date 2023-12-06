using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class ProgressService
    {
        public event Action<int>? ProgressChanged;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public void UpdateProgress(int progress)
        {
            ProgressChanged?.Invoke(progress);
        }

        public CancellationToken GetCancellationToken()
        {
            return _cancellationTokenSource.Token;
        }

        public void ResetCancellationToken()
        {
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public void Cancel()
        {
            _cancellationTokenSource.Cancel();
        }
    }
}
