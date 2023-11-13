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

using DustInTheWind.DirectoryCompare.DataStructures;
using FluentValidation;
using FluentValidation.Validators;

namespace DustInTheWind.DirectoryCompare.Infrastructure.Validation;

public class PathValidator<T> : PropertyValidator<T, DiskPath>
{
    public override string Name { get; } = "Disk Path Validator";

    public override bool IsValid(ValidationContext<T> context, DiskPath value)
    {
        return value.IsEmpty || value.IsValid;
    }

    protected override string GetDefaultMessageTemplate(string errorCode)
    {
        return "The provided path is not valid.";
    }
}