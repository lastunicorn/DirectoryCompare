// DirectoryCompare
// Copyright (C) 2017-2024 Dust in the Wind
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System.Reflection;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using DustInTheWind.Clindy.Applications;
using DustInTheWind.Clindy.Applications.LoadDuplicates;
using DustInTheWind.Clindy.Applications.PresentFilePreview;
using DustInTheWind.Clindy.Applications.SetCurrentDuplicateFile;
using DustInTheWind.Clindy.Presentation.DuplicatesNavigatorArea.ViewModels;
using DustInTheWind.Clindy.Presentation.FileGroupDetailsArea;
using DustInTheWind.DirectoryCompare.Infrastructure;
using ReactiveUI;

namespace DustInTheWind.Clindy.Presentation.MainArea;

public class MainWindowViewModel : ViewModelBase
{
    private readonly RequestBus requestBus;
    private Bitmap image;
    private string text;

    public string Title
    {
        get
        {
            Assembly assembly = Assembly.GetEntryAssembly();
            AssemblyName assemblyName = assembly?.GetName();
            Version version = assemblyName?.Version;

            string title = "Clindy";

            if (version != null)
                title += " " + version.ToString(3);

            return title;
        }
    }

    public FileGroupViewModel FileGroupViewModel { get; }

    public DuplicatesNavigatorViewModel DuplicatesNavigatorViewModel { get; }

    public Bitmap Image
    {
        get => image;
        private set => this.RaiseAndSetIfChanged(ref image, value);
    }

    public string Text
    {
        get => text;
        private set => this.RaiseAndSetIfChanged(ref text, value);
    }

    public MainWindowViewModel()
    {
    }

    public MainWindowViewModel(RequestBus requestBus, EventBus eventBus,
        DuplicatesNavigatorViewModel duplicatesNavigatorViewModel, FileGroupViewModel fileGroupViewModel)
    {
        if (eventBus == null) throw new ArgumentNullException(nameof(eventBus));
        this.requestBus = requestBus ?? throw new ArgumentNullException(nameof(requestBus));

        DuplicatesNavigatorViewModel = duplicatesNavigatorViewModel ?? throw new ArgumentNullException(nameof(duplicatesNavigatorViewModel));
        FileGroupViewModel = fileGroupViewModel ?? throw new ArgumentNullException(nameof(fileGroupViewModel));

        eventBus.Subscribe<CurrentDuplicateFileChangedEvent>(HandleCurrentDuplicateFileChangedEvent);

        LoadDuplicates();
    }

    private void HandleCurrentDuplicateFileChangedEvent(CurrentDuplicateFileChangedEvent ev)
    {
        RefreshFilePreview();
    }

    private async void RefreshFilePreview()
    {
        PresentFilePreviewRequest request = new();
        PresentFilePreviewResponse response = await requestBus.PlaceRequest<PresentFilePreviewRequest, PresentFilePreviewResponse>(request);

        Dispatcher.UIThread.Post(() =>
        {
            try
            {
                switch (response.FileType)
                {
                    case FileType.None:
                        Image = null;
                        Text = null;
                        break;

                    case FileType.Unknown:
                        Image = null;
                        Text = null;
                        break;

                    case FileType.Image:
                        Image = new Bitmap(response.FileStream);
                        Text = null;
                        break;

                    case FileType.Text:
                    {
                        Image = null;
                        using StreamReader streamReader = new(response.FileStream);
                        Text = streamReader.ReadToEnd();
                        break;
                    }

                    default:
                        Image = null;
                        Text = null;
                        break;
                }
            }
            catch
            {
                // Ignore
            }
        });
        Dispatcher.UIThread.RunJobs();
    }

    private async void LoadDuplicates()
    {
        LoadDuplicatesRequest request = new();
        await requestBus.PlaceRequest(request);
    }
}