// Directory Compare
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

using DustInTheWind.DirectoryCompare.Ports.ConfigAccess;
using DustInTheWind.DirectoryCompare.Ports.FileSystemAccess;
using MediatR;

namespace DustInTheWind.Clindy.Applications.PresentFilePreview;

public class PresentFilePreviewUseCase : IRequestHandler<PresentFilePreviewRequest, PresentFilePreviewResponse>
{
    private readonly ApplicationState applicationState;
    private readonly IFileSystem fileSystem;
    private readonly IGuiConfig config;
    private PresentFilePreviewResponse response;

    public PresentFilePreviewUseCase(ApplicationState applicationState, IFileSystem fileSystem, IGuiConfig config)
    {
        this.applicationState = applicationState ?? throw new ArgumentNullException(nameof(applicationState));
        this.fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        this.config = config ?? throw new ArgumentNullException(nameof(config));
    }

    public async Task<PresentFilePreviewResponse> Handle(PresentFilePreviewRequest request, CancellationToken cancellationToken)
    {
        response = new PresentFilePreviewResponse
        {
            FileStream = Stream.Null,
            FileType = FileType.None
        };

        if (config.EnableFilePreview)
        {
            string filePath = applicationState.CurrentDuplicateFile;

            if (filePath != null)
            {
                FileType fileType = ComputeFileType(filePath);
                CreatePreviewInfo(fileType, filePath);
            }
        }

        return response;
    }

    private FileType ComputeFileType(string filePath)
    {
        FileExtensions fileExtensions = new();

        fileExtensions.Add(FileType.Image, config.ImageFileExtensions);
        fileExtensions.Add(FileType.Text, config.TextFileExtensions);

        return fileExtensions.FindFileType(filePath);
    }

    private void CreatePreviewInfo(FileType fileType, string filePath)
    {
        switch (fileType)
        {
            case FileType.None:
            case FileType.Unknown:
                break;

            case FileType.Image:
                response.FileStream = SafeRetrieveFileStream(filePath);
                response.FileType = FileType.Image;
                break;

            case FileType.Text:
                response.FileStream = SafeRetrieveFileStream(filePath);
                response.FileType = FileType.Text;
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private Stream SafeRetrieveFileStream(string filePath)
    {
        try
        {
            return fileSystem.GetFileStream(filePath);
        }
        catch
        {
            return Stream.Null;
        }
    }
}