using PicEdit.Infrastructure.Commands;
using PicEdit.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace PicEdit.ViewModels
{
    internal class MainWindowViewModel : ViewModel
    {
		#region Window title
		private string _title = "PicEdit";

		/// <summary>
		/// Window title.
		/// </summary>
		public string Title
		{
			get => _title;
			//set
			//{
			//	if (Equals(_title, value)) return;
			//	_title = value;
			//	OnPropertyChanged();

			//	Set(ref _title, value);
			//}
			set => Set(ref _title, value);
		}
        #endregion

        #region Commands

        #region CloseApplicationCommand
        public ICommand CloseApplicationCommand { get; }

		private bool OnCloseApplicationCommandExecute(object p) => true;

		private void OnCloseApplicationCommandExecuted(object p)
		{
			Application.Current.Shutdown();
		}
        #endregion

        #endregion

        public MainWindowViewModel()
        {
			#region Commands

			CloseApplicationCommand = new LambdaCommand(OnCloseApplicationCommandExecuted, OnCloseApplicationCommandExecute);

            #endregion
        }
    }
}
