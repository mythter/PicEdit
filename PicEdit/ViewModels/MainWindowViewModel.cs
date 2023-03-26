using PicEdit.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

	}
}
