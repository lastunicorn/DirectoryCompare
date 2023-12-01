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

namespace DustInTheWind.DirectoryCompare.DataAccess.PotFiles.BlacklistFileModel;

public class BlackListForReadFile
{
    private readonly string filePath;

    public PathCollection Items { get; private set; }

    public BlackListForReadFile(string filePath)
    {
        this.filePath = filePath;
    }

    public void Open()
    {
        Items = File.Exists(filePath)
            ? ReadBlackList()
            : new PathCollection();
    }

    public void Remove(string item)
    {
        Items.Remove(item);
    }

    private PathCollection ReadBlackList()
    {
        List<string> list = File.Exists(filePath)
            ? File.ReadAllLines(filePath)
                .Where(x => !string.IsNullOrEmpty(x))
                .Where(x => !x.StartsWith("#"))
                .ToList()
            : new List<string>();

        return new PathCollection(list);
    }

    public void Save()
    {
        string backupFilePath = filePath + ".bak";

        if (File.Exists(backupFilePath))
            File.Delete(backupFilePath);

        if (File.Exists(filePath))
            File.Move(filePath, backupFilePath);

        string[] lines = Items
            .ToArray();

        File.WriteAllLines(filePath, lines);

        if (File.Exists(backupFilePath))
            File.Delete(backupFilePath);
    }

    public void Add(string path)
    {
        if (Items.Contains(path))
            return;

        Items.Add(path);
    }
}