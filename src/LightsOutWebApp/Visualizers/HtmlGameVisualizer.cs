using System.Text;

namespace LightsOut.GameLogic
{
    public class HtmlGameVisualizer : IGameVisualizer
    {
        public string Draw(LightsOut state)
        {
            var sb = new StringBuilder();

            sb.Append("<div class=\"wrapper\" rows=\"")
                .Append(state.NoOfRows)
                .Append("\" columns=\"")
                .Append(state.NoOfColumns)
                .Append("\">");

            for (var row = 0; row < state.NoOfRows; row++)
            {
                for (var column = 0; column < state.NoOfColumns; column++) 
                {
                    sb.Append("<div id=\"r")
                        .Append(row)
                        .Append("c")
                        .Append(column)
                        .Append("\" class=\"cell\">");

                    var cellState = state.Matrix[row, column] ? "🌞" : "🌚";

                    sb.Append(cellState);

                    sb.Append("</span></div>");
                }
            }

            sb.Append("</div>");

            return sb.ToString();
        }
    }
}