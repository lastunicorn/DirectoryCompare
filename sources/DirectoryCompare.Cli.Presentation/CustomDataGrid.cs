using DustInTheWind.ConsoleTools.Controls.Tables;

namespace DustInTheWind.DirectoryCompare.Cli.Presentation;

internal class CustomDataGrid : DataGrid
{
    public CustomDataGrid()
    {
        TitleRow.BackgroundColor = ConsoleColor.DarkGray;
        TitleRow.ForegroundColor = ConsoleColor.Black;

        HeaderRow.ForegroundColor = ConsoleColor.White;
        
        Border.ForegroundColor = ConsoleColor.DarkGray;
    }
}