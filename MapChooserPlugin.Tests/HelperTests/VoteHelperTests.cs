using MapChooserPlugin.Helpers;
using MapChooserPlugin.Interfaces;
using MapChooserPlugin.Models.Config;
using MapChooserPlugin.Tests.Helpers;
using Moq;

namespace MapChooserPlugin.Tests.HelperTests;

public class VoteHelperTests : TestsBase
{
    [Fact(DisplayName = "")]
    public void Test1()
    {
        var mapChangeHelper = new Mock<IMapChangeHelper>();
        var mapChangedTo = string.Empty;
        mapChangeHelper.Setup(e => e.SetNextMap(It.IsAny<string>()))
            .Callback((string map) => mapChangedTo = map);

        var rtvHelper = new Mock<IRtvHelper>();

        var config = new MapChooserConfig()
        {
            Maps = TestHelper.GetAllMaps(),
            VoteOnRoundsBeforeMapEnd = 2,
            ExcludeMapCount = 2,
            VoteDurationSeconds = 2
        };

        var voteHelper = new VoteHelper(LogHelperMock.Object, config, ChatHelperMock.Object, MapChangeHelperMock.Object, RandomizationHelperMock.Object);
    }
}