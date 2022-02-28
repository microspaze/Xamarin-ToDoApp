using Prism.Navigation;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using ToDoApp.Auth;
using ToDoApp.Helpers;
using ToDoApp.Models;
using ToDoApp.Repositories;
using ToDoApp.Repositories.FirestoreRepository;
using ToDoApp.Repositories.Localstore;
using ToDoApp.Views;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace ToDoApp.ViewModels
{
    public class ProfilePageViewModel : 
        BaseViewModel,
        IInitialize
    {
        #region Private & Protected

        private IRepository<TaskModel> _taskRepository;
        private IRepository<ListModel> _listRepository;

        #endregion

        #region Commands

        public ICommand BackCommand { get; set; }
        public ICommand LogOutCommand { get; set; }
        public ICommand DarkModeToggleCommand { get; set; }
        public ICommand HideDoneToggleCommand { get; set; }

        #endregion

        #region Properties

        public ProfileDetailsModel ProfileDetails { get; set; }
        public string Username { get; set; }
        public bool IsLocalMode { get; set; } = true;
        public bool IsDarkMode { get; set; }
        public bool IsHideEnabled { get; set; }

        #endregion

        #region Constructors

        public ProfilePageViewModel(
            INavigationService navigationService,
            IRepository<TaskModel> taskRepository,
            IRepository<ListModel> listRepository) : base(navigationService)
        {
            _taskRepository = taskRepository;
            _listRepository = listRepository;

            BackCommand = new Command(BackCommandHandler);
            LogOutCommand = new Command(LogOutCommandHandler);
        }

        public async void Initialize(INavigationParameters parameters)
        {
            MainState = LayoutState.Loading;

            await GetProfileDetails();

            IsDarkMode = Application.Current.UserAppTheme.Equals(OSAppTheme.Dark);
            IsHideEnabled = Preferences.Get("hideDoneTasks", false);

            var auth = DependencyService.Get<IFirebaseAuthentication>();
            Username = auth.GetUsername();

            MainState = LayoutState.None;
        }

        #endregion

        #region Command Handlers

        private void LogOutCommandHandler()
        {
            var auth = DependencyService.Get<IFirebaseAuthentication>();
            var response = auth.LogOut();
            if(response)
            {
                _navigationService.NavigateAsync($"/{nameof(WelcomePage)}");
            }
            else
            {
                Debug.WriteLine("Failed to log out");
            }
        }

        #endregion

        #region Private Methods

        private void OnIsDarkModeChanged()
        {
            if (IsDarkMode)
            {
                Application.Current.UserAppTheme = OSAppTheme.Dark;
                Preferences.Set("theme", "dark");
            }
            else
            {
                Application.Current.UserAppTheme = OSAppTheme.Light;
                Preferences.Set("theme", "light");
            }
        }

        private void OnIsHideEnabledChanged()
        {
            Preferences.Set("hideDoneTasks", IsHideEnabled);
        }

        private void OnIsLocalModeChanged()
        {
            Constants.IsLocalMode = IsLocalMode;
            Preferences.Set("IsLocalMode", IsLocalMode);

            if (IsLocalMode)
            {
                DependencyService.Register<IRepository<TaskModel>, TasksLocalRepository>();
                DependencyService.Register<IRepository<ListModel>, ListsLocalRepository>();
            }
            else
            {
                DependencyService.Register<IRepository<TaskModel>, TasksRepository>();
                DependencyService.Register<IRepository<ListModel>, ListsRepository>();
            }

            this.LogOutCommandHandler();
        }

        private async Task GetProfileDetails()
        {
            var auth = DependencyService.Get<IFirebaseAuthentication>();
            var userId = auth.GetUserId();
            var lists = await _listRepository.GetAllAsync(userId);
            var tasks = await _taskRepository.GetAllAsync(userId);

            ProfileDetails = new ProfileDetailsModel()
            {
                TotalLists = lists.Count(),
                TotalTasks = tasks.Count(),
                DoneTasks = tasks.Count(t => t.Archived == true)
            };
        }

        #endregion
    }
}
