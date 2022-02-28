using Prism.Navigation;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using ToDoApp.Auth;
using ToDoApp.Helpers;
using ToDoApp.Models;
using ToDoApp.Repositories;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace ToDoApp.ViewModels.Dialogs
{
    public class ListDialogViewModel : BaseViewModel, IDialogAware
    {
        #region Private & Protected

        private string _list;
        private string _fromPage;

        private IRepository<ListModel> _listRepository;

        #endregion

        #region Properties

        public bool HasError { get; set; }
        public ObservableCollection<ListModel> ProjectList { get; set; }
        public ListModel SelectedList { get; set; }

        #endregion

        #region Commands 

        public ICommand ChangeSelectListCommand { get; private set; }

        public ICommand CloseDialogCommand { get; private set; }

        #endregion

        #region Constructors

        public ListDialogViewModel(
            INavigationService navigationService,
            IRepository<ListModel> listRepository) : base(navigationService)
        {
            _listRepository = listRepository;

            ChangeSelectListCommand = new Command(ChangeSelectListCommandHandler);
            CloseDialogCommand = new Command(CloseListDialog);

            MainState = LayoutState.Loading;
        }

        #endregion

        #region Command Handlers

        private void ChangeSelectListCommandHandler()
        {
            var list = ProjectList.First(a => a.Name == _list);
            if(SelectedList != list)
            {
                if(_fromPage == "More")
                {
                    Preferences.Set("taskFilterByList", SelectedList.Name);
                }
                var param = new DialogParameters()
                {
                    { "selectedList", SelectedList.Name }
                };
                RequestClose(param);
            }
        }

        private void CloseListDialog()
        {
            var param = new DialogParameters()
            {
                { "selectedList", SelectedList.Name }
            };
            RequestClose(param);
        }

        #endregion

        #region Dialog

        public event Action<IDialogParameters> RequestClose;

        public virtual bool CanCloseDialog()
        {
            return true;
        }

        public virtual void OnDialogClosed()
        { }

        public async void OnDialogOpened(IDialogParameters parameters)
        {
            _fromPage = parameters.GetValue<string>("fromPage");
            var selectedItem = parameters.GetValue<string>("selectedItem");
            var projectList = await GetProjectList(_fromPage);
            ProjectList = new ObservableCollection<ListModel>(projectList);

            if(selectedItem == null)
            {
                _list = Preferences.Get("taskFilterByList", "All lists");
                SelectedList = ProjectList.First(a => a.Name == _list);
            }
            else
            {
                _list = selectedItem;
                SelectedList = ProjectList.First(a => a.Name == selectedItem);
            }

            MainState = LayoutState.None;
        }

        #endregion

        #region Private Methods

        private async Task<List<ListModel>> GetProjectList(string fromPage)
        {
            var auth = DependencyService.Get<IFirebaseAuthentication>();
            var userId = auth.GetUserId();

            var listToAdd = new List<ListModel>();
            var list = await _listRepository.GetAllAsync(userId);
            if (list.Count() > 0)
            {
                listToAdd = list.ToList();
                listToAdd.Insert(0, Constants.InboxList);
                if(fromPage == "More")
                {
                    listToAdd.Insert(0, Constants.AllLists);
                }
            }
            else
            {
                listToAdd.Add(Constants.InboxList);
            }
            return listToAdd;
        }

        #endregion
    }
}
