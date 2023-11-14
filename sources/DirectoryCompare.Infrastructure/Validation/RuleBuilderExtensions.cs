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

namespace DustInTheWind.DirectoryCompare.Infrastructure.Validation;

public static class RuleBuilderExtensions
{
    public static IRuleBuilderOptions<T, DiskPath> IsValidPath<T>(this IRuleBuilder<T, DiskPath> ruleBuilder)
    {
        return ruleBuilder.SetValidator(new PathValidator<T>());
    }
}