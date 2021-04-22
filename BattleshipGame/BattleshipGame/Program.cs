using System;

namespace BattleshipGame
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("-----Welcome to Battleship!-----");
            Console.WriteLine();

            int gridWidth = 11;
            int gridHeight = 11;  //one more than standard width and height of a Battleship playing field to account for labels for rows and columns on the user board

            CreateGameBoard(gridHeight, gridWidth);

            DisplayUserBoard(gridHeight, gridWidth);

            static void DisplayUserBoard(int gridHeight, int gridWidth)
            {
                string[,] userBoard = new string[gridHeight, gridWidth];  //this creates the user's board, where the user will announce targets and monitor hits/misses

                for (int row = 0; row < gridHeight; row++)  //these loops populate the grid to be displayed to the user
                {
                    for (int col = 0; col < gridWidth; col++)
                    {
                        if (row == 0 && col == 0) { userBoard[row, col] = "   "; }

                        else if (col == 0) 
                        {
                            if (row == 10) { userBoard[row, col] = $" 10"; } //removed second space for formatting purposes
                            else { userBoard[row, col] = $" {row} "; }
                        }
                        else if (row == 0) { userBoard[row, col] = $" {Convert.ToChar(col + 64)} "; } //this labels the columns as A-J instead of 1-10 using ASCII representation
                        else { userBoard[row, col] = " ~ "; }

                        Console.Write(userBoard[row, col]);
                    }
                    Console.WriteLine();
                }
            }
            static void CreateGameBoard(int gridHeight, int gridWidth)
            {
                string[,] cpuBoard = new string[gridHeight, gridWidth];  //this creates the computer's board, which is where ships will be stored

                for (int row = 0; row < gridHeight - 1; row++)  //these loops define the cpu board, which will keep track of hits/misses and determine the end of the game
                {
                    for (int col = 0; col < gridWidth - 1; col++)
                    {
                        if (row == 0 || col == 0) { cpuBoard[row, col] = ""; }  //these elements are the labels on the user board, so we do not count them here on the cpu board for consistency
                        else { cpuBoard[row, col] = "~"; }
                    }
                }
                string[] Destroyer = new Ship;
            }
        }
    }
    public class Ship
    {
        
    }
}
