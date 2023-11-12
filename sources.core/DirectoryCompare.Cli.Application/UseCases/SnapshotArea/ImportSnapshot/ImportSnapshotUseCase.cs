// DirectoryCompare
// Copyright (C) 2017-2023 Dust in the Wind
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

using DustInTheWind.DirectoryCompare.Domain.Entities;
using DustInTheWind.DirectoryCompare.Domain.ImportExport;
using DustInTheWind.DirectoryCompare.Domain.PotModel;
using DustInTheWind.DirectoryCompare.Ports.DataAccess;
using MediatR;

namespace DustInTheWind.DirectoryCompare.Cli.Application.UseCases.SnapshotArea.ImportSnapshot;

public class ImportSnapshotUseCase : IRequestHandler<ImportSnapshotRequest>
{
    private readonly IPotRepository potRepository;
    private readonly ISnapshotRepository snapshotRepository;
    private readonly IPotImportExport potImportExport;

    public ImportSnapshotUseCase(IPotRepository potRepository, ISnapshotRepository snapshotRepository, IPotImportExport potImportExport)
    {
        this.potRepository = potRepository ?? throw new ArgumentNullException(nameof(potRepository));
        this.snapshotRepository = snapshotRepository ?? throw new ArgumentNullException(nameof(snapshotRepository));
        this.potImportExport = potImportExport ?? throw new ArgumentNullException(nameof(potImportExport));
    }

    public Task Handle(ImportSnapshotRequest request, CancellationToken cancellationToken)
    {
        //ISnapshotReader reader = null;
        //ISnapshotWriter writer = null;

        //while (reader.MoveNext())
        //{
        //    switch (reader.CurrentItemType)
        //    {
        //        case SnapshotItemType.None:
        //            break;

        //        case SnapshotItemType.SerializerId:
        //            writer.WriteSerializerId(reader.ReadSerializerId());
        //            break;

        //        case SnapshotItemType.OriginalPath:
        //            writer.WriteOriginalPath(reader.ReadOriginalPath());
        //            break;

        //        case SnapshotItemType.CreationTime:
        //            writer.WriteCreationTime(reader.ReadCreationTime());
        //            break;

        //        case SnapshotItemType.FileCollection:
        //            IEnumerable<HFile> files = reader.ReadFiles();

        //            foreach (HFile file in files) 
        //                writer.Add(file);
        //            break;

        //        case SnapshotItemType.DirectoryCollection:
        //            IEnumerable<HDirectoryReader> directories = reader.ReadDirectories();

        //            foreach (HDirectory directory in directories)
        //                writer.Add(directory);
        //            break;
        //    }
        //}

        Snapshot snapshot = potImportExport.ReadSnapshot(request.FilePath);

        Pot pot = potRepository.Get(request.PotName);

        if (pot == null)
        {
            pot = new Pot
            {
                Name = request.PotName,
                Path = snapshot.OriginalPath
            };

            potRepository.Add(pot);
        }
        else
        {
            if (pot.Path != snapshot.OriginalPath)
                throw new Exception("The url of the imported snapshot is different than the one of the pot.");
        }

        snapshotRepository.Add(request.PotName, snapshot);

        return Task.CompletedTask;
    }
}