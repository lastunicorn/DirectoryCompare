// DirectoryCompare
// Copyright (C) 2017-2019 Dust in the Wind
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

using System;
using System.IO;

namespace DustInTheWind.DirectoryCompare.Domain.Utils
{
    public struct DiskPath
    {
        private readonly string value;
        private bool? isValid;

        public bool IsValid
        {
            get
            {
                if (isValid == null)
                    isValid = CalculateIsValid();

                return isValid.Value;
            }
        }

        public bool IsRooted => Path.IsPathRooted(value);

        public DiskPath(string value)
        {
            this.value = value ?? throw new ArgumentNullException(nameof(value));
            isValid = null;
        }

        private bool CalculateIsValid()
        {
            if (value == null)
                return false;

            try
            {
                Path.GetFullPath(value);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static implicit operator string(DiskPath diskPath)
        {
            return diskPath.value;
        }

        public static implicit operator DiskPath(string path)
        {
            return new DiskPath(path);
        }

        public static DiskPath operator +(DiskPath diskPath1, DiskPath diskPath2)
        {
            string newPath = Path.Combine(diskPath1.value, diskPath2.value);
            return new DiskPath(newPath);
        }

        public static DiskPath operator +(DiskPath diskPath, string path)
        {
            string newPath = Path.Combine(diskPath.value, path);
            return new DiskPath(newPath);
        }

        public static DiskPath operator +(string path, DiskPath diskPath)
        {
            string newPath = Path.Combine(path, diskPath.value);
            return new DiskPath(newPath);
        }
    }
}