using System;
using System.Collections.Generic;
using System.Linq;
using DustInTheWind.DirectoryCompare.Domain.Entities;
using DustInTheWind.DirectoryCompare.Ports.DataAccess;

namespace DustInTheWind.DirectoryCompare.Domain.PotModel
{
    public class SnapshotFactory
    {
        private readonly ISnapshotRepository snapshotRepository;

        public SnapshotFactory(ISnapshotRepository snapshotRepository)
        {
            this.snapshotRepository = snapshotRepository ?? throw new ArgumentNullException(nameof(snapshotRepository));
        }
        
        public Snapshot RetrieveSnapshot(SnapshotLocation location)
        {
            if (string.IsNullOrEmpty(location.PotName))
                throw new Exception("Pot name was not provided.");

            if (location.SnapshotIndex.HasValue)
                return snapshotRepository.GetByIndex(location.PotName, location.SnapshotIndex.Value);

            if (location.SnapshotDate.HasValue)
            {
                DateTime searchedDate = location.SnapshotDate.Value;

                Snapshot snapshot = snapshotRepository.GetByExactDateTime(location.PotName, searchedDate);

                if (snapshot == null && searchedDate.TimeOfDay == TimeSpan.Zero)
                {
                    List<Snapshot> snapshots = snapshotRepository.GetByDate(location.PotName, searchedDate)
                        .ToList();

                    if (snapshots.Count == 1)
                        snapshot = snapshots[0];
                    else if (snapshots.Count > 1)
                        throw new Exception($"There are multiple snapshots that match the specified date. Pot = {location.PotName}; Date = {searchedDate}");
                }

                return snapshot;
            }

            return snapshotRepository.GetLast(location.PotName);
        }
    }
}