﻿using System;
using System.ComponentModel;
using System.Windows;
using VideoFeatureMatching.Core;
using VideoFeatureMatching.DAL;
using VideoFeatureMatching.L10n;
using VideoFeatureMatching.Utils;

namespace VideoFeatureMatching.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly VideoCloudPointsFileAccessor _baseFileAccessor = new VideoCloudPointsFileAccessor();

        private ProjectFile<VideoCloudPoints> _projectFile;

        #region General Properties

        public string PageTitle
        {
            get
            {
                if (!IsProjectOpened)
                {
                    return Strings.ProgramName;
                }

                return "[" + (_projectFile.Path ?? Strings.Filename_untittled) + (!IsProjectSaved ? "*" : "") + "]";
            }
        }

        public Command OpenAboutCommmand { get { return new Command(() => new About().Show()); } }

        public Command<Window> ExitCommand
        {
            get
            {
                return new Command<Window>(window =>
                {
                    SaveExistingProjectIfNeeded();
                    window.Close();
                });
            }
        }

        #endregion

        #region Project handlers

        public bool IsProjectOpened { get { return _projectFile != null; } }
        public bool IsProjectSaved { get { return _projectFile != null && _projectFile.IsSaved; } }

        private void OpenProject(ProjectFile<VideoCloudPoints> projectFile)
        {
            _projectFile = projectFile;
            _projectFile.ProjectSaved += ProjectFileOnProjectSaved;

            RaisePropertyChanged("IsProjectOpened");
            RaisePropertyChanged("IsProjectSaved");
            RaisePropertyChanged("SaveProjectCommand");
            RaisePropertyChanged("SaveAsProjectCommand");
            RaisePropertyChanged("CloseProjectCommand");
        }

        private void ProjectFileOnProjectSaved(object sender, EventArgs eventArgs)
        {
            RaisePropertyChanged("IsProjectSaved");
            RaisePropertyChanged("SaveProjectCommand");
        }

        private void CloseProject()
        {
            if (_projectFile != null)
            {
                _projectFile.ProjectSaved -= ProjectFileOnProjectSaved;
            }
            _projectFile = null;
            RaisePropertyChanged("IsProjectOpened");
            RaisePropertyChanged("IsProjectSaved");
            RaisePropertyChanged("SaveProjectCommand");
            RaisePropertyChanged("SaveAsProjectCommand");
            RaisePropertyChanged("CloseProjectCommand");
        }

        #endregion

        #region File System Commands

        public Command CreateProjectCommand { get { return new Command(CreateProject); } }
        public Command SaveProjectCommand { get { return new Command(SaveProject, () => IsProjectOpened && !IsProjectSaved); } }
        public Command SaveAsProjectCommand { get { return new Command(SaveAsProject, () => IsProjectOpened); } }
        public Command OpenProjectCommand { get { return new Command(OpenProject); } }
        public Command CloseProjectCommand { get { return new Command(CloseProject, () => IsProjectOpened); } }

        #endregion

        #region File System methods

        private void CreateProject()
        {
            if (SaveExistingProjectIfNeeded() == MessageBoxResult.Cancel)
                return;

            var viewModel = new CreateProjectViewModel();
            var createProjectWindow = new CreateProjectWindow();
            createProjectWindow.DataContext = viewModel;
            createProjectWindow.Closing += (sender, args) =>
            {
                var project = viewModel.GetProjectFile();
                if (project != null)
                {
                    OpenProject(project);
                }
            };
            createProjectWindow.Show();
        }

        private void SaveProject()
        {
            _baseFileAccessor.Save(_projectFile);
        }

        private void SaveAsProject()
        {
            _baseFileAccessor.SaveAs(_projectFile);
        }

        private void OpenProject()
        {
            try
            {
                if (SaveExistingProjectIfNeeded() == MessageBoxResult.Cancel)
                    return;

                var project = _baseFileAccessor.Open();
                OpenProject(project);
            }
            // TODO better handlers
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), Strings.ExceptionMessage_Title, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private MessageBoxResult SaveExistingProjectIfNeeded()
        {
            if (IsProjectOpened && !IsProjectSaved)
            {
                var result = MessageBox.Show(GetCurrentWindowDelegate.Invoke(), Strings.SaveProject, Strings.Attention, MessageBoxButton.YesNoCancel);
                if (result == MessageBoxResult.Yes)
                {
                    SaveProject();
                }
                return result;
            }
            return MessageBoxResult.Yes;
        }

        #endregion

        #region Helpers

        public Func<Window> GetCurrentWindowDelegate { get; set; }

        #endregion
    }
}