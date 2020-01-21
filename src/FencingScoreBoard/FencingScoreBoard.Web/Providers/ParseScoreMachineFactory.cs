using BinaryDataDecoders.ElectronicScoringMachines.Fencing.Common;
using BinaryDataDecoders.ElectronicScoringMachines.Fencing.Favero;
using BinaryDataDecoders.ElectronicScoringMachines.Fencing.SaintGeorge;
using System;

namespace FencingScoreBoard.Web.Providers
{
    public class ParseScoreMachineFactory : IParseScoreMachineFactory
    {
        private readonly IServiceProvider _provider;
        public ParseScoreMachineFactory(
            IServiceProvider provider
            )
        {
            _provider = provider;
        }

        public IParseScoreMachineState Create(ScoreMachineType type)
        {
            switch (type)
            {
                case ScoreMachineType.SaintGeorge:
                    return new SgStateParser();
                case ScoreMachineType.Favero:
                    return new FaveroStateParser();
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
