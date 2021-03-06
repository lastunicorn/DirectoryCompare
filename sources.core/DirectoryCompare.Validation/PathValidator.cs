﻿// DirectoryCompare
// Copyright (C) 2017-2020 Dust in the Wind
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

using DustInTheWind.DirectoryCompare.Domain.Utils;
using FluentValidation.Validators;

namespace DustInTheWind.DirectoryCompare.Validation
{
    public class PathValidator : PropertyValidator
    {
        public PathValidator()
            : base("The provided path is not valid.")
        {
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            DiskPath diskPath = GetDiskPath(context.PropertyValue);
            return diskPath.IsEmpty || diskPath.IsValid;
        }

        private static DiskPath GetDiskPath(object propertyValue)
        {
            switch (propertyValue)
            {
                case string path:
                    return new DiskPath(path);

                case DiskPath path:
                    return path;

                default:
                    return DiskPath.Empty;
            }
        }
    }
}