using BinaryDataDecoders.ElectronicScoringMachines.Fencing.Common;

namespace FencingScoreBoard.Web.Providers
{
    public static class CardsEx
    {
        public static string MapColor(this Cards card)
        {
            switch (card)
            {
                case Cards.None:
                    return "transparent";

                default:
                    return MapColor(card & Cards.Red); //If not matched check for red;

                case Cards.Yellow:
                case Cards.Red:
                case Cards.Black:
                    return card.ToString().ToLower();
            }
        }
    }
}
