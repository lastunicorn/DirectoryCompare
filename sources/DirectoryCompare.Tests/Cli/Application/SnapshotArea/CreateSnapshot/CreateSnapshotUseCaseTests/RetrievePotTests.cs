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

using DustInTheWind.DirectoryCompare.Cli.Application.SnapshotArea.CreateSnapshot;
using DustInTheWind.DirectoryCompare.Cli.Application.SnapshotArea.CreateSnapshot.DiskAnalysis;
using DustInTheWind.DirectoryCompare.Domain.PotModel;
using DustInTheWind.DirectoryCompare.Ports.DataAccess;
using DustInTheWind.DirectoryCompare.Ports.FileSystemAccess;
using DustInTheWind.DirectoryCompare.Ports.LogAccess;
using FluentAssertions;
using Moq;
using Xunit;

namespace DustInTheWind.DirectoryCompare.Tests.Cli.Application.SnapshotArea.CreateSnapshot.CreateSnapshotUseCaseTests;

public class RetrievePotTests
{
    private readonly CreateSnapshotUseCase useCase;
    private readonly Mock<IPotRepository> potRepository;
    private readonly Mock<IFileSystem> fileSystem;

    public RetrievePotTests()
    {
        potRepository = new Mock<IPotRepository>();

        fileSystem = new Mock<IFileSystem>();

        useCase = new CreateSnapshotUseCase(Mock.Of<ILog>(), potRepository.Object, Mock.Of<IBlackListRepository>(),
            Mock.Of<ISnapshotRepository>(), fileSystem.Object);
    }

    [Fact]
    public async Task PotNameInRequest_WhenCreateSnapshot_ThenPotWithThatNameIsRetrievedFromRepository()
    {
        CreateSnapshotRequest request = new()
        {
            PotName = "pot1"
        };

        try
        {
            await useCase.Handle(request, CancellationToken.None);
        }
        catch
        {
            // ignored
        }

        potRepository.Verify(x => x.GetByNameOrId("pot1", false), Times.Once);
    }

    [Fact]
    public async Task HavingNoPotWithRequestedNameInRepository_WhenCreateSnapshot_ThenThrows()
    {
        CreateSnapshotRequest request = new()
        {
            PotName = "pot1"
        };

        Func<Task> action = () => useCase.Handle(request, CancellationToken.None);

        await action.Should().ThrowAsync<PotNotFoundException>();
    }

    [Fact]
    public async Task HavingRequestedPotInRepository_WhenCreateSnapshot_ThenPotPathIsCheckedThatExistOnDisk()
    {
        potRepository
            .Setup(x => x.GetByNameOrId("pot1", false))
            .ReturnsAsync(new Pot
            {
                Path = "path1"
            });

        CreateSnapshotRequest request = new()
        {
            PotName = "pot1"
        };

        try
        {
            await useCase.Handle(request, CancellationToken.None);
        }
        catch
        {
            // ignored
        }

        fileSystem.Verify(x => x.ExistsDirectory("path1"), Times.Once);
    }

    [Fact]
    public async Task HavingPotWithPathThatDoesNotExistOnDisk_WhenCreateSnapshot_ThenThrows()
    {
        potRepository
            .Setup(x => x.GetByNameOrId("pot1", false))
            .ReturnsAsync(new Pot
            {
                Path = "non-existent"
            });

        fileSystem
            .Setup(x => x.ExistsDirectory("non-existent"))
            .Returns(false);

        CreateSnapshotRequest request = new()
        {
            PotName = "pot1"
        };

        Func<Task> action = () => useCase.Handle(request, CancellationToken.None);

        await action.Should().ThrowAsync<PotPathDoesNotExistException>();
    }

    [Fact]
    public async Task HavingValidPotInRepository_WhenCreateSnapshot_ThenAnalysisStartedSuccessfully()
    {
        potRepository
            .Setup(x => x.GetByNameOrId("pot1", false))
            .ReturnsAsync(new Pot
            {
                Path = "path2"
            });

        fileSystem
            .Setup(x => x.ExistsDirectory("path2"))
            .Returns(true);

        CreateSnapshotRequest request = new()
        {
            PotName = "pot1"
        };

        IDiskAnalysisReport report = await useCase.Handle(request, CancellationToken.None);

        report.Should().NotBeNull();
    }
}