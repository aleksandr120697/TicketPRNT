using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketPrint.ViewModel
{
    internal class MainVM: BaseInpc
    {
            private int _progress;

            public MainVM()
            {
                Start = new RelayCommand(async () =>
                {
                    for (Progress = 0; Progress < 200; Progress++)
                    {
                        await Task.Delay(1);
                    }
                });
            }

            public int Progress { get => _progress; private set => Set(ref _progress, value); }

            public RelayCommand Start { get; }


    }
}
