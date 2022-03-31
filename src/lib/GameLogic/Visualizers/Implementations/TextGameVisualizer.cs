using System;
using System.Text;

namespace LightsOut.GameLogic
{
    public class TextGameVisualizer : IGameVisualizer
    {
        public string Draw(GameLogic.LightsOut state)
        {
            var sb = new StringBuilder();

            for (var row = 0; row < state.NoOfRows; row++)
            {
                sb.Append("│");

                for (var column = 0; column < state.NoOfColumns; column++) 
                {
                    var cellState = state.Matrix[row, column] ? "🌞" : "🌚";

                    sb.Append(cellState + "│");
                }

                sb.Append(Environment.NewLine);
            }

            return sb.ToString();
        }
    }
}