namespace DustInTheWind.DirectoryCompare
{
    public interface IDiskExport
    {
        void Open(string originalPath);
        void OpenNewDirectory(XDirectory xDirectory);
        void CloseDirectory();
        void Add(XFile xFile);
        void Add(XDirectory xDirectory);
        void Close();
    }
}