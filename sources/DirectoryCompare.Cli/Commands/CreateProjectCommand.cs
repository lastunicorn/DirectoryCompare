using System;
using DirectoryCompare.CliFramework;
using DustInTheWind.DirectoryCompare.Application.CreateProject;
using MediatR;

namespace DustInTheWind.DirectoryCompare.Cli.Commands
{
    internal class CreateProjectCommand : ICommand
    {
        private readonly IMediator mediator;

        public string Description => "Creates a new project in the specified location.";

        public CreateProjectCommand(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public void Execute(Arguments arguments)
        {
            CreateProjectRequest request = CreateRequest(arguments);
            mediator.Send(request).Wait();
        }

        private static CreateProjectRequest CreateRequest(Arguments arguments)
        {
            return new CreateProjectRequest
            {
                DirectoryPath = arguments[0],
                Name = arguments.Count >= 2
                    ? arguments[1]
                    : null
            };
        }
    }
}