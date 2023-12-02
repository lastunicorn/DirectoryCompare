using DustInTheWind.DirectoryCompare.DataStructures;

namespace DustInTheWind.DirectoryCompare.Ports.UserAccess;

public interface IDuplicateFilesUi
{
    Task AnnounceStart(SnapshotLocation snapshotLeft, SnapshotLocation snapshotRight);

    Task AnnounceDuplicate(FilePairDto filePair);

    Task AnnounceFinished(int duplicateCount, DataSize totalSize);
}