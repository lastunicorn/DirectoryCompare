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

using DustInTheWind.DirectoryCompare.Ports.FileSystemAccess;
using MediatR;

namespace DustInTheWind.Clindy.Applications.PresentFilePreview;

public class PresentFilePreviewUseCase : IRequestHandler<PresentFilePreviewRequest, PresentFilePreviewResponse>
{
    private readonly ApplicationState applicationState;
    private readonly IFileSystem fileSystem;

    public PresentFilePreviewUseCase(ApplicationState applicationState, IFileSystem fileSystem)
    {
        this.applicationState = applicationState ?? throw new ArgumentNullException(nameof(applicationState));
        this.fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
    }

    public Task<PresentFilePreviewResponse> Handle(PresentFilePreviewRequest request, CancellationToken cancellationToken)
    {
        PresentFilePreviewResponse response = new();

        string filePath = applicationState.CurrentDuplicateFile;

        if (filePath != null)
        {
            string fileExtension = Path.GetExtension(filePath);

            switch (fileExtension.ToLower())
            {
                case ".jpg":
                case ".jpeg":
                case ".gif":
                case ".png":
                case ".webp":
                case ".bmp":
                case ".exe":
                case ".dll":
                {
                    response.FileStream = SaveRetrieveFileStream(filePath);
                    response.FileType = FileType.Image;
                    break;
                }

                case ".txt":
                {
                    response.FileStream = SaveRetrieveFileStream(filePath);
                    response.FileType = FileType.Text;
                    break;
                }

                default:
                {
                    response.FileStream = Stream.Null;
                    response.FileType = FileType.Unknown;
                    break;
                }
            }
        }
        else
        {
            response.FileStream = Stream.Null;
            response.FileType = FileType.None;
        }

        return Task.FromResult(response);
    }

    private Stream SaveRetrieveFileStream(string filePath)
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