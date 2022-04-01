using System;

namespace LightsOut.GameLogic
{
    public record LightsOut
    {
        public byte NoOfRows { get; init; }
        public byte NoOfColumns { get; init; }
        public bool[,] Matrix { get; init; }
        public bool IsSolved { get; private set; }

        public LightsOut(byte noOfRows, byte noOfColumns, byte noOfSwitchedOnLights)
        {
            NoOfRows = noOfRows;
            NoOfColumns = noOfColumns;

            Matrix = new bool[noOfRows, noOfColumns];

            var rnd = new Random();

            do
            {
                var randomRow = rnd.Next(0, noOfRows);

                var randomColumn = rnd.Next(0, noOfColumns);

                if (Matrix[randomRow, randomColumn])
                {
                    continue; // this light is already on, was picked up randomly before
                }

                Matrix[randomRow, randomColumn] = true;

                noOfSwitchedOnLights--;

            }
            while (noOfSwitchedOnLights > 0);
        }

        public void ToggleCell(byte row, byte column)
        {
            Matrix[row, column] = !Matrix[row, column];

            if (row > 0)
            {
                Matrix[row - 1, column] = !Matrix[row - 1, column];
            }

            if (row + 1 < NoOfRows)
            {
                Matrix[row + 1, column] = !Matrix[row + 1, column];
            }

            if (column > 0)
            {
                Matrix[row, column - 1] = !Matrix[row, column - 1];
            }

            if (column + 1 < NoOfColumns)
            {
                Matrix[row, column + 1] = !Matrix[row, column + 1];
            }

            foreach (var cell in Matrix)
            {
                if (cell) return;
            }

            IsSolved = true;
        }
    }
}


