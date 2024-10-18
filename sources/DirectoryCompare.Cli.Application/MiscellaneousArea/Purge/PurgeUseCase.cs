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

using DustInTheWind.DirectoryCompare.Cli.Application.SnapshotArea.PresentSnapshot;
using DustInTheWind.DirectoryCompare.Domain.Entities;
using DustInTheWind.DirectoryCompare.Ports.DataAccess;
using MediatR;

namespace DustInTheWind.DirectoryCompare.Cli.Application.MiscellaneousArea.Purge;

internal class PurgeUseCase : IRequestHandler<PurgeRequest, PurgeResponse>
{
    private readonly ISnapshotRepository snapshotRepository;
    private List<string> log = new();

    public PurgeUseCase(ISnapshotRepository snapshotRepository)
    {
        this.snapshotRepository = snapshotRepository ?? throw new ArgumentNullException(nameof(snapshotRepository));
    }

    public async Task<PurgeResponse> Handle(PurgeRequest request, CancellationToken cancellationToken)
    {
        Snapshot snapshot = await snapshotRepository.Get(request.Snapshot);

        if (snapshot == null)
            throw new SnapshotNotFoundException(request.Snapshot);

        ProcessDirectory(snapshot, request.FilePath);
        
        await snapshotRepository.SaveChanges(request.Snapshot.PotName, snapshot);
        
        return new PurgeResponse
        {
            Log = log
        };
    }

    private void ProcessDirectory(HDirectory directory, string fileName)
    {
        List<HFile> filesRemoved = directory.Files.Remove(x => x.Name == fileName);
        log.AddRange(filesRemoved.Select(x => $"File removed: {x.GetPath()}"));
        
        List<HDirectory> directoriesRemoved = directory.Directories.Remove(x => x.Name == fileName);
        log.AddRange(directoriesRemoved.Select(x => $"Directory removed: {x.GetPath()}"));

        foreach (HDirectory hDirectory in directory.Directories)
            ProcessDirectory(hDirectory, fileName);
    }
}