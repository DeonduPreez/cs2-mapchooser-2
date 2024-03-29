using MapChooserPlugin.Interfaces;
using MapChooserPlugin.Models.Config;
using Moq;

namespace MapChooserPlugin.Tests;

public abstract class TestsBase
{
    protected readonly MapChooserConfig Config = new();

    protected readonly Mock<ILogHelper> LogHelperMock = new();
    protected readonly Mock<IRandomizationHelper> RandomizationHelperMock = new();
    protected readonly Mock<IChatHelper> ChatHelperMock = new();
    protected readonly Mock<IConVarHelper> ConVarHelperMock = new();
    protected readonly Mock<IMapChangeHelper> MapChangeHelperMock = new();
    protected readonly Mock<IRtvHelper> RtvHelperMock = new();
    protected readonly Mock<IVoteHelper> VoteHelperMock = new();

    protected TestsBase()
    {
    }
}