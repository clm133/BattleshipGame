﻿using System;
using System.Collections.Generic;

namespace BattleshipGame
{
    class Program
    {
        public static void Main(string[] args)
        {
            int gridWidth = 11;
            int gridHeight = 11;  //one more than standard size of a Battleship playing field to account for labels for the rows and columns on the user board. variables are used for the rest of the program, so if the size needs changed, it can be done here

            DisplayUserMenu();
            
            CreateGameBoard(gridHeight, gridWidth);

            DisplayUserBoard(gridHeight, gridWidth);

            static void CreateGameBoard(int gridHeight, int gridWidth)
            {
                string[,] cpuBoard = new string[gridHeight, gridWidth];  //this creates the computer's board, which is where ships will be stored

                for (int row = 0; row < gridHeight; row++)  //these loops define the cpu board, which will keep track of hits/misses and determine the end of the game
                {
                    for (int col = 0; col < gridWidth; col++)
                    {
                        if (row == 0 || col == 0) { cpuBoard[row, col] = ""; }  //these elements are the labels on the user board, so we do not count them here on the cpu board for consistency
                        else { cpuBoard[row, col] = "~"; }
                    }
                }

                List<Ship> shipList = new List<Ship>();  //create the ships in a list using Ship class
                shipList.Add(new Ship("Patrol Boat", 2, "P"));
                shipList.Add(new Ship("Submarine", 3, "S"));
                shipList.Add(new Ship("Destroyer", 3, "D"));
                shipList.Add(new Ship("Battleship", 4, "B"));
                shipList.Add(new Ship("Cruiser", 5, "C"));

                foreach (var ship in shipList)
                {
                    int startRow = 0;
                    int endRow = 0;
                    int startCol = 0;
                    int endCol = 0;

                    bool notPlaced = true;
                    while (notPlaced)
                    {
                        Random rd = new Random();  //create random start points for the ships
                        startRow = rd.Next(1, gridHeight);  //.Next()'s second value is exclusive, no need to subtract 1
                        endRow = startRow;
                        startCol = rd.Next(1, gridWidth);
                        endCol = startCol;
                        int orientation = rd.Next(1, 11) % 2;  //will use 0 for vertical, 1 for horizontal

                        if (orientation == 0)  //this will find the end location of the current ship based on orientation
                        {
                            endRow += ship.Length;
                        }
                        else
                        {
                            endCol += ship.Length;
                        }
                        if (endRow > gridHeight || endCol > gridWidth) { continue; }  //check to make sure the end of the ship is inside the bounds of the board, restart while loop for a new location if it is not

                        if (orientation == 0)  //using temp variables to keep the originals in tact for location assignment
                        {
                            int tempStartRow = startRow;
                            for (int i = 0; i < endRow - startRow; i++) //these loops check if any of the spaces in which we are trying to place the ship are already occupied by a ship based on orientation
                            {
                                if (cpuBoard[tempStartRow, startCol] == "~") { notPlaced = false; tempStartRow++; }
                                else { notPlaced = true; break; }
                            }
                        }
                        else
                        {
                            int tempStartCol = startCol;
                            for (int i = 0; i < endCol - startCol; i++)
                            {
                                if (cpuBoard[startRow, tempStartCol] == "~") { notPlaced = false; tempStartCol++; }
                                else { notPlaced = true; break; }
                            }
                        }
                    }

                    if (startCol == endCol)  // no longer have access to 'orientation' variable, so checking for orientation based on the location variables
                    {
                        for (int i = 0; i < ship.Length; i++)  //these loops set the ship onto the cpu board
                        {
                            cpuBoard[startRow, startCol] = ship.Id;
                            startRow++;
                        }
                    }
                    else
                    {
                        for (int i = 0; i < ship.Length; i++)
                        {
                            cpuBoard[startRow, startCol] = ship.Id;
                            startCol++;
                        }
                    }
                }
            }

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
                            if (row >= 10) { userBoard[row, col] = $" {row}"; }  //removed second space aftye for formatting purposes
                            else { userBoard[row, col] = $" {row} "; }
                        }
                        else if (row == 0) { userBoard[row, col] = $" {Convert.ToChar(col + 64)} "; }  //this labels the columns as A-J instead of 1-10 using ASCII representation for ease of use
                        else { userBoard[row, col] = " ~ "; }

                        Console.Write(userBoard[row, col]);
                    }
                    Console.WriteLine();
                }
            }

            static void DisplayUserMenu()
            {
                Console.WriteLine("        ----- Welcome to -----");
                Console.WriteLine();
                Console.WriteLine("      :::::::::      ::: ::::::::::: ::::::::::: :::        :::::::::: ::::::::  :::    ::: ::::::::::: ::::::::: \n     :+:    :+:   :+: :+:   :+:         :+:     :+:        :+:       :+:    :+: :+:    :+:     :+:     :+:    :+: \n    +:+    +:+  +:+   +:+  +:+         +:+     +:+        +:+       +:+        +:+    +:+     +:+     +:+    +:+  \n   +#++:++#+  +#++:++#++: +#+         +#+     +#+        +#++:++#  +#++:++#++ +#++:++#++     +#+     +#++:++#+    \n  +#+    +#+ +#+     +#+ +#+         +#+     +#+        +#+              +#+ +#+    +#+     +#+     +#+           \n #+#    #+# #+#     #+# #+#         #+#     #+#        #+#       #+#    #+# #+#    #+#     #+#     #+#            \n#########  ###     ### ###         ###     ########## ########## ########  ###    ### ########### ###             ");
                Console.WriteLine("\n                                                          ----- Single-Player vs. the Console -----");
                Console.WriteLine();
            }
        }
    }

    public class Ship
    {
        public string Name { get; set; }
        public int Length { get; set; }
        public string Id { get; set; }
        public Ship(string name, int length, string id) 
        {
            this.Name = name;
            this.Length = length;
            this.Id = id;
        }
    }
}
