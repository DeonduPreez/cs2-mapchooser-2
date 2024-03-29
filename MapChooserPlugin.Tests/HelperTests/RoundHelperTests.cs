using MapChooserPlugin.Helpers;
using MapChooserPlugin.Interfaces;

namespace MapChooserPlugin.Tests.HelperTests;

public class RoundHelperTests : TestsBase
{
    [Fact(DisplayName = "ShouldStartVoting should only return true if RoundsLeft is less than or equal to VoteOnRoundsBeforeMapEnd Config entry")]
    public void RoundsLeftTest()
    {
        ConVarHelperMock.Setup(convarHelper => convarHelper.GetMaxRounds())
            .Returns(26);

        // Should always return null because the next map has not been selected
        MapChangeHelperMock.Setup(mapChangeHelper => mapChangeHelper.NextMap)
            .Returns((string?)null);

        // Should always return false because we are not already voting in this test
        VoteHelperMock.Setup(voteHelper => voteHelper.Voting)
            .Returns(false);

        var roundHelper = new RoundHelper(LogHelperMock.Object, Config, MapChangeHelperMock.Object, VoteHelperMock.Object, ConVarHelperMock.Object);
        roundHelper.OnMapStart();

        SimulateAndTestRoundsBeforeVoting(roundHelper);

        // If the map restarts or the next map starts, we should not start voting until the above rounds have elapsed again
        roundHelper.OnMapStart();
        Assert.False(roundHelper.ShouldStartVoting());

        SimulateAndTestRoundsBeforeVoting(roundHelper);
    }

    [Fact(DisplayName = "ShouldStartVoting should always return false if we are already voting")]
    public void AlreadyVotingTest()
    {
        ConVarHelperMock.Setup(convarHelper => convarHelper.GetMaxRounds())
            .Returns(26);

        // Should always return null because the next map has not been selected
        MapChangeHelperMock.Setup(mapChangeHelper => mapChangeHelper.NextMap)
            .Returns((string?)null);

        // Should always return true because we are already voting in this test
        VoteHelperMock.Setup(voteHelper => voteHelper.Voting)
            .Returns(true);

        var roundHelper = new RoundHelper(LogHelperMock.Object, Config, MapChangeHelperMock.Object, VoteHelperMock.Object, ConVarHelperMock.Object);
        roundHelper.OnMapStart();

        // Always needs to return false no matter how many rounds have passed
        for (var i = 0; i < ConVarHelperMock.Object.GetMaxRounds() * 2; i++)
        {
            roundHelper.OnRoundStart();
            Assert.False(roundHelper.ShouldStartVoting());
        }
    }

    [Fact(DisplayName = "ShouldStartVoting should always return false if the map has already been selected")]
    public void MapAlreadySelectedTest()
    {
        ConVarHelperMock.Setup(convarHelper => convarHelper.GetMaxRounds())
            .Returns(26);

        // Should always return a map because the next map has already been selected
        MapChangeHelperMock.Setup(mapChangeHelper => mapChangeHelper.NextMap)
            .Returns(Config.Maps[0]);

        // Should always return false because we are not already voting in this test
        VoteHelperMock.Setup(voteHelper => voteHelper.Voting)
            .Returns(false);

        var roundHelper = new RoundHelper(LogHelperMock.Object, Config, MapChangeHelperMock.Object, VoteHelperMock.Object, ConVarHelperMock.Object);
        roundHelper.OnMapStart();

        // Always needs to return false no matter how many rounds have passed
        for (var i = 0; i < ConVarHelperMock.Object.GetMaxRounds() * 2; i++)
        {
            roundHelper.OnRoundStart();
            Assert.False(roundHelper.ShouldStartVoting());
        }
    }

    /// <summary>
    /// Simulates and tests ShouldStartVoting for the amount of rounds that should elapse until voting should occur.
    /// Only call this to test if RoundsLeft and VoteOnRoundsBeforeMapEnd config entry is working correctly 
    /// </summary>
    private void SimulateAndTestRoundsBeforeVoting(IRoundHelper roundHelper)
    {
        for (var i = 0; i < ConVarHelperMock.Object.GetMaxRounds() - Config.VoteOnRoundsBeforeMapEnd - 1; i++)
        {
            roundHelper.OnRoundStart();
            Assert.False(roundHelper.ShouldStartVoting());
        }

        // Should start voting on next round
        roundHelper.OnRoundStart();
        Assert.True(roundHelper.ShouldStartVoting());
    }
}