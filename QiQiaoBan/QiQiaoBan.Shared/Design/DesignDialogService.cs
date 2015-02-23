using GalaSoft.MvvmLight.Views;
using System;
using System.Collections.Generic;
using System.Text;

namespace QiQiaoBan.Design
{
    public class DesignDialogService : IDialogService
    {
        public System.Threading.Tasks.Task ShowError(Exception error, string title, string buttonText, Action afterHideCallback)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task ShowError(string message, string title, string buttonText, Action afterHideCallback)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task<bool> ShowMessage(string message, string title, string buttonConfirmText, string buttonCancelText, Action<bool> afterHideCallback)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task ShowMessage(string message, string title, string buttonText, Action afterHideCallback)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task ShowMessage(string message, string title)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task ShowMessageBox(string message, string title)
        {
            throw new NotImplementedException();
        }
    }
}
