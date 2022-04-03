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

            // creates matrix based on specified params
            Matrix = new bool[noOfRows, noOfColumns];

            var rnd = new Random();

            // randomly switch on lights until the specified noOfSwitchedOnLights is reached
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
            // invert the specified cell
            Matrix[row, column] = !Matrix[row, column];

            // if it is not the first row
            if (row > 0)
            {
                // flip the cell in the row above it
                Matrix[row - 1, column] = !Matrix[row - 1, column];
            }

            // if it is not the last row
            if (row + 1 < NoOfRows)
            {
                // flip the cell in the row below it
                Matrix[row + 1, column] = !Matrix[row + 1, column];
            }

            // if it is not the first column
            if (column > 0)
            {
                // flip the cell to the left of the specified cell
                Matrix[row, column - 1] = !Matrix[row, column - 1];
            }

            // if it is not the last column
            if (column + 1 < NoOfColumns)
            {
                // flip the cell to right of the specified cell
                Matrix[row, column + 1] = !Matrix[row, column + 1];
            }

            // if there is at least one cell which is on then exit
            foreach (var cell in Matrix)
            {
                if (cell) return;
            }

            // else the puzzle is solved
            IsSolved = true;
        }
    }
}


