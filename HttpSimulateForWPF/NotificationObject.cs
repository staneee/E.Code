using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HttpSimulateForWPF
{
    public class NotificationObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }


    public class DelegateCommand : ICommand
    {
        // 委托原型，无返回值
        public Action<object> ExecuteCommand = null;

        // 委托原型，返回一个bool值
        public Func<object, bool> CanExecuteCommand = null;

        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// 判断对象是否能执行
        /// </summary>
        /// <param name="parameter">要判断的对象</param>
        /// <returns>可执行返回ture，否则返回false</returns>
        public bool CanExecute(object parameter)
        {
            if (CanExecuteCommand != null)
            {
                return this.CanExecuteCommand(parameter);
            }
            else
            {
                return true;
            }
        }


        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object parameter)
        {
            if (this.ExecuteCommand != null)
                this.ExecuteCommand(parameter);
        }

        /// <summary>
        /// 执行后修改
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
            {
                CanExecuteChanged(this, EventArgs.Empty);
            }
        }
    }
}