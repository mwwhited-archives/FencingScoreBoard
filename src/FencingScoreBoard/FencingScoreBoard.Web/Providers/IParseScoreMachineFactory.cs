using BinaryDataDecoders.ElectronicScoringMachines.Fencing.Common;

namespace FencingScoreBoard.Web.Providers
{
    public interface IParseScoreMachineFactory
    {
        IParseScoreMachineState Create(ScoreMachineType type);
    }
}
