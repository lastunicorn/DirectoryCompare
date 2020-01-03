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

using DustInTheWind.DirectoryCompare.Domain.Utils;
using FluentValidation.Resources;
using FluentValidation.Validators;

namespace DustInTheWind.DirectoryCompare.Infrastructure.Validation
{
    public class PathValidator : PropertyValidator
    {
        public PathValidator()
            : base(new LanguageStringSource(nameof(PathValidator)))
        {
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            object propertyValue = context.PropertyValue;

            if (propertyValue == null)
                return true;

            if (propertyValue is string path)
                return new DiskPath(path).IsValid;

            if (propertyValue is DiskPath diskPath)
                return diskPath.IsValid;

            return false;
        }
    }
}