using BinaryDataDecoders.ElectronicScoringMachines.Fencing.Common;

namespace FencingScoreBoard.Web.Providers
{
    public static class LightEx
    {
        public static string MapColor(this Lights light, string touchColor)
        {
            switch (light)
            {
                default:
                case Lights.None:
                    return "transparent";

                case Lights.Touch:
                    return touchColor;

                case Lights.White:
                case Lights.Yellow:
                    return light.ToString().ToLower();
            }
        }
    }
}
