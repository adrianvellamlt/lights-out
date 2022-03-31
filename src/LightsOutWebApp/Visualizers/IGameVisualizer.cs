namespace LightsOut.Web
{
    public interface IGameVisualizer
    {
        public string Draw(GameLogic.LightsOut state);
    }
}