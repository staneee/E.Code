using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpSimulateForWPF.ViewModels
{
    public class MainWindowViewModel
    {
        public  MainWindowModel Model { get; set; }


        public MainWindowViewModel()
        {
            Model = new MainWindowModel();
        }
    }
}
