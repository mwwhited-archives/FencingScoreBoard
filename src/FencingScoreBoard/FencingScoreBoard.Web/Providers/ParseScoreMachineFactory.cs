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
            return (IParseScoreMachineState)_provider.GetService(ResolveType(type));
        }

        public Type ResolveType(ScoreMachineType type)
        {
            switch (type)
            {
                case ScoreMachineType.SaintGeorge:
                    return typeof(SgStateParser);
                case ScoreMachineType.Favero:
                    return typeof(FaveroStateParser);
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
