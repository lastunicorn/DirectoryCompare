﻿// DirectoryCompare
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

using DirectoryCompare.CliFramework;
using DustInTheWind.DirectoryCompare.Cli.ResultExporters;
using MediatR;
using System;
using System.IO;
using DustInTheWind.DirectoryCompare.Application.UseCases.RemoveDuplicates;
using DustInTheWind.DirectoryCompare.Comparison;

namespace DustInTheWind.DirectoryCompare.Cli.Commands
{
    internal class RemoveDuplicatesCommand : ICommand
    {
        private readonly IMediator mediator;

        public string Description => string.Empty;

        public RemoveDuplicatesCommand(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public void Execute(Arguments arguments)
        {
            RemoveDuplicatesRequest request = CreateRequest(arguments);
            mediator.Send(request).Wait();
        }

        private static RemoveDuplicatesRequest CreateRequest(Arguments arguments)
        {
            if (arguments.Count == 0)
                throw new Exception("Invalid command parameters.");

            string pathRight;
            ComparisonSide fileToRemove;

            if (arguments.Count > 1)
            {
                bool isFileRight = File.Exists(arguments[1]);

                if (isFileRight)
                {
                    pathRight = arguments[1];
                    fileToRemove = arguments.Count > 2
                        ? (ComparisonSide)Enum.Parse(typeof(ComparisonSide), arguments[2])
                        : ComparisonSide.Left;
                }
                else
                {
                    pathRight = null;
                    fileToRemove = (ComparisonSide)Enum.Parse(typeof(ComparisonSide), arguments[1]);
                }
            }
            else
            {
                pathRight = null;
                fileToRemove = ComparisonSide.Right;
            }

            return new RemoveDuplicatesRequest
            {
                PathLeft = arguments[0],
                PathRight = pathRight,
                Exporter = new ConsoleRemoveDuplicatesExporter(),
                FileToRemove = fileToRemove
            };
        }
    }
}