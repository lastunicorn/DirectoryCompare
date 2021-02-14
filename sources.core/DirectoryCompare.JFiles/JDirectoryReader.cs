using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace DustInTheWind.DirectoryCompare.JsonHashesFile.JsonExport
{
    public class JDirectoryReader : JReader
    {
        public JDirectoryFieldType CurrentPropertyType { get; private set; } = JDirectoryFieldType.None;

        public JDirectoryReader(JsonTextReader jsonTextReader)
            : base(jsonTextReader)
        {
        }

        public JDirectoryFieldType MoveToNext()
        {
            switch (state)
            {
                case JReaderState.New:
                    state = JReaderState.InProgress;
                    break;

                case JReaderState.InProgress:
                    break;

                case JReaderState.Finished:
                    throw new Exception("The reader already finished reading the json object.");

                default:
                    throw new Exception("Invalid reader state.");
            }

            try
            {
                bool success = MoveToNextProperty();

                CurrentPropertyType = success
                    ? jsonTextReader.Value switch
                    {
                        "n" => JDirectoryFieldType.DirectoryName,
                        "f" => JDirectoryFieldType.FileCollection,
                        "d" => JDirectoryFieldType.SubDirectoryCollection,
                        _ => throw new Exception("Invalid field in directory object.")
                    }
                    : JDirectoryFieldType.None;

                return CurrentPropertyType;
            }
            catch
            {
                CurrentPropertyType = JDirectoryFieldType.None;
                throw;
            }
        }

        public string ReadName()
        {
            if (CurrentPropertyType != JDirectoryFieldType.DirectoryName)
                throw new Exception("Current property is not the directory name.");

            return jsonTextReader.ReadAsString();
        }

        public IEnumerable<JFileReader> ReadFiles()
        {
            if (CurrentPropertyType != JDirectoryFieldType.FileCollection)
                throw new Exception("Current property is not the file collection.");

            return ReadFilesCollection();
        }

        public IEnumerable<JDirectoryReader> ReadSubDirectories()
        {
            if (CurrentPropertyType != JDirectoryFieldType.SubDirectoryCollection)
                throw new Exception("Current property is not the sub-directory collection.");

            return ReadDirectoriesCollection();
        }
    }
}