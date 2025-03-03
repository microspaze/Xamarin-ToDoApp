﻿using Prism.Navigation;
using Prism.Regions.Navigation;
using Prism.Services.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Input;
using ToDoApp.Auth;
using ToDoApp.Helpers;
using ToDoApp.Helpers.Validations;
using ToDoApp.Helpers.Validations.Rules;
using ToDoApp.Models;
using ToDoApp.Repositories;
using ToDoApp.Views.Dialogs;
using Xamarin.Forms;

namespace ToDoApp.ViewModels.Templates.AddEditItem
{
    public class AddEditTaskViewModel : BaseRegionViewModel
    {
        #region Private & Protected

        private IDialogService _dialogService;
        private IRepository<TaskModel> _taskRepository;

        private bool _archived;
        private string _id;

        #endregion

        #region Properties

        public ObservableCollection<ListModel> ProjectList { get; set; }
        public ValidatableObject<string> Name { get; set; }
        public ValidatableObject<string> List { get; set; }
        public ValidatableObject<DateTime> Date { get; set; }
        public DateTime MinDate { get; set; }
        public string Mode { get; set; }

        #endregion

        #region Commands

        public ICommand CreateCommand { get; set; }
        public ICommand OpenListDialogCommand { get; set; }

        public ICommand ValidateCommand { get; set; }

        #endregion

        #region Constructors

        public AddEditTaskViewModel(
            INavigationService navigationService,
            IDialogService dialogService,
            IRepository<TaskModel> taskRepository) : base(navigationService)
        {
            _dialogService = dialogService;
            _taskRepository = taskRepository;

            CreateCommand = new Command(CreateCommandHandler);
            OpenListDialogCommand = new Command(OpenListDialogCommandHandler);

            ValidateCommand = new Command<string>(ValidateCommandHandler);

            AddValidations();

            InitForm();
        }

        #endregion

        #region Validation Handlers

        private void ValidateCommandHandler(string field)
        {
            switch (field)
            {
                case "name": Name.Validate(); break;
                case "list": List.Validate(); break;
                case "date": Date.Validate(); break;
            }
        }

        #endregion

        #region Command Handlers

        private async void CreateCommandHandler()
        {
            ValidateForm();
            if (!IsFormValid())
            {
                return;
            }
            try
            {
                var auth = DependencyService.Get<IFirebaseAuthentication>();
                var userId = auth.GetUserId();

                if (Mode == "Edit")
                {
                    var model = new TaskModel()
                    {
                        Archived = false,
                        List = List.Value,
                        Task = Name.Value,
                        UserId = userId,
                        Date = Date.Value.ToString("dd/MM/yyyy"),
                        Id = _id
                    };
                    var wasUpdated = await _taskRepository.Update(model);
                    if(wasUpdated)
                    {
                        await _navigationService.GoBackAsync();
                    }
                    else
                    {
                        var param = new DialogParameters()
                        {
                            { "message", Constants.Errors.GeneralError }
                        };
                        _dialogService.ShowDialog(nameof(ErrorDialog), param);
                    }
                }
                else
                {
                    var model = new TaskModel()
                    {
                        Archived = false,
                        List = List.Value,
                        Task = Name.Value,
                        UserId = userId,
                        Date = Date.Value.ToString("dd/MM/yyyy")
                    };
                    var wasAdded = await _taskRepository.Add(model);
                    if (wasAdded)
                    {
                        await _navigationService.GoBackAsync();
                    }
                    else
                    {
                        var param = new DialogParameters()
                        {
                            { "message", Constants.Errors.GeneralError }
                        };
                        _dialogService.ShowDialog(nameof(ErrorDialog), param);
                    }
                }
            }
            catch (Exception ex)
            {
                var param = new DialogParameters()
                {
                    { "message", Constants.Errors.GeneralError }
                };
                _dialogService.ShowDialog(nameof(ErrorDialog), param);
                Debug.Write(ex.Message);
            }
        }

        private void OpenListDialogCommandHandler()
        {
            var param = new DialogParameters()
            {
                { "fromPage", "AddEdit" },
                { "selectedItem", List.Value }
            };
            _dialogService.ShowDialog(nameof(ListDialog), param, (IDialogResult r) =>
            {
                var res = r.Parameters.GetValue<string>("selectedList");
                List.Value = res;
            });
        }

        #endregion

        #region Region Navigation Handlers

        public override void OnNavigatedTo(INavigationContext navigationContext)
        {
            var isEdit = navigationContext.Parameters.GetValue<bool>("isEdit");
            var task = navigationContext.Parameters.GetValue<TaskModel>("task");

            Mode = isEdit ? "Edit" : "Add";

            if (Mode == "Edit")
            {
                Name.Value = task.Task;
                _archived = task.Archived;
                Date.Value = DateTime.ParseExact(task.Date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                List.Value = task.List;
                _id = task.Id;
            }
            else
            {
                Name.Value = Constants.DefaultTask.Task;
                _archived = Constants.DefaultTask.Archived;
                Date.Value = DateTime.ParseExact(Constants.DefaultTask.Date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                List.Value = Constants.DefaultTask.List;
            }

            List.Validate();
            Date.Validate();
        }

        #endregion

        #region Private Methods

        private void InitForm()
        {
            MinDate = DateTime.Now;
        }

        private bool IsFormValid()
        {
            return Name.IsButtonActive && List.IsButtonActive && Date.IsButtonActive;
        }

        private void ValidateForm()
        {
            Name.Validate();
            List.Validate();
            Date.Validate();
        }

        private void AddValidations()
        {
            Name = new ValidatableObject<string>();
            List = new ValidatableObject<string>();
            Date = new ValidatableObject<DateTime>();

            Name.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "A name is required." });
            List.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "A list is required." });
            Date.Validations.Add(new IsNotNullOrEmptyRule<DateTime> { ValidationMessage = "A date is required." });
        }

        #endregion
    }
}
