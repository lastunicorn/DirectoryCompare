namespace DustInTheWind.DirectoryCompare.Ports.UserAccess;

public interface IDuplicateFilesUi
{
    Task AnnounceStart(DuplicateSearchStartedInfo info);

    Task AnnounceDuplicate(DuplicateFoundInfo filePair);

    Task AnnounceFinished(DuplicateSearchFinishedInfo info);
}