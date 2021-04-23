using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace BattleshipGame
{
    class Program
    {
        public static void Main(string[] args)
        {
            int gridWidth = 11;
            int gridHeight = 11;  //one more than standard size of a Battleship playing field to account for labels for the rows and columns on the user board. variables are used for the rest of the program, so if the size needs changed, it can be done here

            DisplayUserMenu();

            string[,] cpuBoard = CreateGameBoard(gridHeight, gridWidth);

            string[,] userBoard = CreateUserBoard(gridHeight, gridWidth);

            bool game = true;
            while (game)
            {
                Console.Clear();
                DisplayUserBoard(userBoard, gridHeight, gridWidth);

                List<Ship> shipHealth = new List<Ship>();

                List<UserCoordinates> userTarget = GetUserTarget();

                if (cpuBoard[userTarget[0].Row, userTarget[0].Col] != "░░░")  //triggers when a user target is not water, meaning a hit
                {
                    DisplayHit();
                    Console.WriteLine("\n\n\n\n\n\n\n\n(Press any key to continue.)");
                    Console.ReadKey(true);

                    if (cpuBoard[userTarget[0].Row, userTarget[0].Col] == " P ") { shipHealth[0].Length--; }  //subtract life from the ships to know when they are sunk
                    else if (cpuBoard[userTarget[0].Row, userTarget[0].Col] == " S ") { shipHealth[1].Length--; }
                    else if (cpuBoard[userTarget[0].Row, userTarget[0].Col] == " D ") { shipHealth[2].Length--; }
                    else if (cpuBoard[userTarget[0].Row, userTarget[0].Col] == " B ") { shipHealth[3].Length--; }
                    else if (cpuBoard[userTarget[0].Row, userTarget[0].Col] == " C ") { shipHealth[4].Length--; }

                    userBoard[userTarget[0].Row, userTarget[0].Col] = "HIT";
                }
                else if (cpuBoard[userTarget[0].Row, userTarget[0].Col] == "░░░")
                {
                    Console.WriteLine("Miss!".PadLeft(60));
                    userBoard[userTarget[0].Row, userTarget[0].Col] = " M ";
                    Console.WriteLine("\n\n\n(Press any key to continue.)");
                    Console.ReadKey(true);
                }
                else if (cpuBoard[userTarget[0].Row, userTarget[0].Col] == " M ")  //if user selects a miss again
                {
                    Console.WriteLine("You already missed here once, silly! Try again!".PadLeft(60));
                    Console.WriteLine("\n\n\n(Press any key to continue.)");
                    Console.ReadKey(true);
                }
                else if (userBoard[userTarget[0].Row, userTarget[0].Col] == "HIT")  //if user selects a target they already hit
                {
                    Console.WriteLine("You already hit this ship here. Try again!".PadLeft(60));
                    Console.WriteLine("\n\n\n(Press any key to continue.)");
                    Console.ReadKey(true);
                }

                int shipCounter = 0;  //this will count how many ships are sunk - when it equals 5, the game is over           
                foreach (var ship in shipHealth)
                {
                    if (ship.Length == 0) { DisplaySink(ship.Id); }
                    shipCounter++;
                }
                if (shipCounter == 5) { game = false; DisplayWin(); }
            }

            static string[,] CreateGameBoard(int gridHeight, int gridWidth)
            {
                string[,] cpuBoard = new string[gridHeight, gridWidth];  //this creates the computer's board, which is where ships will be stored

                for (int row = 0; row < gridHeight; row++)  //these loops define the cpu board, which will keep track of hits/misses and determine the end of the game
                {
                    for (int col = 0; col < gridWidth; col++)
                    {
                        if (row == 0 || col == 0) { cpuBoard[row, col] = ""; }  //these elements are the labels on the user board, so we do not count them here on the cpu board for consistency
                        else { cpuBoard[row, col] = "░░░"; }
                    }
                }

                List<Ship> shipList = new List<Ship>();  //create the ships in a list using Ship class
                shipList.Add(new Ship("Patrol Boat", 2, " P "));
                shipList.Add(new Ship("Submarine", 3, " S "));
                shipList.Add(new Ship("Destroyer", 3, " D "));
                shipList.Add(new Ship("Battleship", 4, " B "));
                shipList.Add(new Ship("Cruiser", 5, " C "));

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

                        if (orientation == 0) { endRow += ship.Length; } //this will find the end location of the current ship based on orientation
                        else { endCol += ship.Length; }

                        if (endRow > gridHeight || endCol > gridWidth) { continue; }  //check to make sure the end of the ship is inside the bounds of the board, restart while loop for a new location if it is not

                        if (orientation == 0)  //using temp variables to keep the originals in tact for location assignment
                        {
                            int tempStartRow = startRow;
                            for (int i = 0; i < endRow - startRow; i++) //these loops check if any of the spaces in which we are trying to place the ship are already occupied by a ship based on orientation
                            {
                                if (cpuBoard[tempStartRow, startCol] == "░░░") { notPlaced = false; tempStartRow++; }
                                else { notPlaced = true; break; }
                            }
                        }
                        else
                        {
                            int tempStartCol = startCol;
                            for (int i = 0; i < endCol - startCol; i++)
                            {
                                if (cpuBoard[startRow, tempStartCol] == "░░░") { notPlaced = false; tempStartCol++; }
                                else { notPlaced = true; break; }
                            }
                        }
                    }

                    if (startCol == endCol)  // no longer have access to 'orientation' var, so checking for orientation based on the location variables
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
                return cpuBoard;
            }
            static string[,] CreateUserBoard(int gridHeight, int gridWidth)
            {
                string[,] userBoard = new string[gridHeight, gridWidth];  //this creates the user's board, where the user will announce targets and monitor hits/misses

                for (int row = 0; row < gridHeight; row++)  //these loops populate the grid to be displayed to the user
                {
                    for (int col = 0; col < gridWidth; col++)
                    {
                        if (row == 0 && col == 0) { userBoard[row, col] = "   "; }

                        else if (col == 0)
                        {
                            if (row >= 10) { userBoard[row, col] = $" {row}"; }  //removed second space for formatting purposes
                            else { userBoard[row, col] = $" {row} "; }
                        }
                        else if (row == 0) { userBoard[row, col] = $" {Convert.ToChar(col + 64)} "; }  //this labels the columns as A-J instead of 1-10 using ASCII representation for ease of use
                        else { userBoard[row, col] = "░░░"; }
                    }
                }
                return userBoard;
            }

            static string[,] DisplayUserBoard(string[,] userBoard, int gridHeight, int gridWidth)
            {
                Console.WriteLine("\n\n\n\n\n\n\n");  //leading whitespace for formatting

                for (int row = 0; row < gridHeight; row++)
                {
                    for (int col = 0; col < gridWidth; col++)
                    {
                        if (col == 0) { Console.Write(userBoard[row, col].PadLeft(40)); } //indents first column by 40 spaces, centering the board on screen
                        else { Console.Write(userBoard[row, col]); }
                    }
                    Console.WriteLine();
                }
                return userBoard;
            }

            static List<UserCoordinates> GetUserTarget()
            {
                Console.WriteLine("\n\n\n\n");  //leading whitespace for formatting

                string userInput = "";
                bool loop = true;
                while (loop)
                {
                    Console.WriteLine("Enter the coordinates (column, row) of your target (ie. B3) then press 'Enter'.");
                    userInput = Console.ReadLine().ToUpper();

                    if (string.IsNullOrEmpty(userInput)) { Console.WriteLine("\nYou must fire!\n"); continue; } //check if input is empty

                    else if (userInput.Length > 3) { Console.WriteLine("\nYour coordinates were too long!\n"); continue; }; //check if input is too long - string length 3 should be longest input
                    Match match = Regex.Match(userInput, (@"(^[A-J][1-9]$)|(^[A-J][1][0]$)"));  //regex match check for correct user input
                    if (match.Success) { loop = false; }
                    else { Console.WriteLine("\nThose weren't valid coordinates. Try again!\n"); }
                }
                char tempUserCol = userInput.Substring(0, 1).ToCharArray()[0];  //collect column and row and turn both to ints
                int userCol = Convert.ToInt32(tempUserCol) - 64;
                int userRow = Convert.ToInt32(userInput.Substring(1));

                List<UserCoordinates> userCoords = new List<UserCoordinates>();  //put into list to return to main function
                userCoords.Add(new UserCoordinates(userCol, userRow));

                return userCoords;
            }

            static void DisplayHit()
            {

                Console.WriteLine("\n\n\n\n\n\n");
                Console.WriteLine("              _.-^^---..,,^^---_".PadLeft(60));
                Console.WriteLine("          _--                   \"\\__".PadLeft(64));
                Console.WriteLine("        <      THAT'S A HIT!      >)".PadLeft(65));
                Console.WriteLine("          \\._                  _./".PadLeft(63));
                Console.WriteLine("              ```--. . , ; .--''''".PadLeft(60));
                Console.WriteLine("                    | |   |".PadLeft(53));
                Console.WriteLine("                 .-=||  | |=-.".PadLeft(56));
                Console.WriteLine("                 `-=#$%&%$#=-'".PadLeft(56));
                Console.WriteLine("                    | ;  :|".PadLeft(53));
                Console.WriteLine("           _____.,-#%&$@%#&#~,._____".PadLeft(62));
            }

            static void DisplayUserMenu()
            {
                Console.WriteLine("\n\n\n\n\n\n\n\n        ----- Welcome to -----");
                Console.WriteLine();
                Console.WriteLine("      :::::::::      ::: ::::::::::: ::::::::::: :::        :::::::::: ::::::::  :::    ::: ::::::::::: ::::::::: \n     :+:    :+:   :+: :+:   :+:         :+:     :+:        :+:       :+:    :+: :+:    :+:     :+:     :+:    :+: \n    +:+    +:+  +:+   +:+  +:+         +:+     +:+        +:+       +:+        +:+    +:+     +:+     +:+    +:+  \n   +#++:++#+  +#++:++#++: +#+         +#+     +#+        +#++:++#  +#++:++#++ +#++:++#++     +#+     +#++:++#+    \n  +#+    +#+ +#+     +#+ +#+         +#+     +#+        +#+              +#+ +#+    +#+     +#+     +#+           \n #+#    #+# #+#     #+# #+#         #+#     #+#        #+#       #+#    #+# #+#    #+#     #+#     #+#            \n#########  ###     ### ###         ###     ########## ########## ########  ###    ### ########### ###             ");
                Console.WriteLine("\n                                                          ----- Single-Player vs. the Console -----");
                Console.WriteLine("\n\n\n\n\n\n\n\n\n\n\n\n\n(Press any key to continue)");
                Console.ReadKey(true);
                Console.Clear();
                Console.WriteLine("\n\n#### ##    ##  ######  ######## ########  ##     ##  ######  ######## ####  #######  ##    ##  ######  \n ##  ###   ## ##    ##    ##    ##     ## ##     ## ##    ##    ##     ##  ##     ## ###   ## ##    ## \n ##  ####  ## ##          ##    ##     ## ##     ## ##          ##     ##  ##     ## ####  ## ##       \n ##  ## ## ##  ######     ##    ########  ##     ## ##          ##     ##  ##     ## ## ## ##  ######  \n ##  ##  ####       ##    ##    ##   ##   ##     ## ##          ##     ##  ##     ## ##  ####       ## \n ##  ##   ### ##    ##    ##    ##    ##  ##     ## ##    ##    ##     ##  ##     ## ##   ### ##    ## \n#### ##    ##  ######     ##    ##     ##  #######   ######     ##    ####  #######  ##    ##  ######  ");
                Console.WriteLine("\n");
                Console.WriteLine("The object of Battleship is to sink all of the computer's ships.\n\n\nThe computer has 5 ships:\n\nPatrol Boat (two spaces), Submarine (three spaces), Destroyer (three spaces),\nBattleship (four spaces), and Cruiser (five spaces).\n\n\nThe ships are placed at random. Aim at your target using the column and row labels.\nYou will be notified if it is a MISS or a HIT.\n\n\nHit all the spaces of a ship to sink it. Once you sink all the ships, you win! Good luck!\n\n\n(Press any key to begin)");
                Console.ReadKey(true);
                Console.Clear();
            }
            static void DisplayWin()
            {
                Console.WriteLine("");

                Console.WriteLine("   :::   :::  ::::::::  :::    :::        :::       :::  ::::::::  ::::    :::          ::: \n  :+:   :+: :+:    :+: :+:    :+:        :+:       :+: :+:    :+: :+:+:   :+:          :+:  \n  +:+ +:+ +:+ +:+ +:+ +:+ +:+ +:+ +:+ +:+ :+:+:+ +:+ +:+\n+#++:   +#+    +:+ +#+    +:+        +#+  +:+  +#+ +#+    +:+ +#+ +:+ +#+          +#+    \n  +#+    +#+    +#+ +#+    +#+        +#+ +#+#+ +#+ +#+    +#+ +#+  +#+#+#          +#+     \n #+#    #+#    #+# #+#    #+#         #+#+# #+#+#  #+#    #+# #+#   #+#+#                   \n###     ########   ########           ###   ###    ########  ###    ####          ###       ");
            }
            static void DisplaySink(string id)
            {
                if (id == " P ")
                {
                    Console.WriteLine(" +-+-+-+ +-+-+-+-+ +-+-+-+ +-+-+-+-+-+-+ +-+-+-+-+-+".PadLeft(25));
                    Console.WriteLine(" | Y | O | U | | S | U | N | K | | T | H | E | | P | A | T | R | O | L | | B | O | A | T | !|".PadLeft(25));
                    Console.WriteLine(" +-+-+-+ +-+-+-+-+ +-+-+-+ +-+-+-+-+-+-+ +-+-+-+-+-+".PadLeft(25));
                }
                else if (id == " S ")
                {
                    Console.WriteLine(" +-+-+-+ +-+-+-+-+ +-+-+-+ +-+-+-+-+-+-+-+-+-+-+".PadLeft(25));
                    Console.WriteLine(" | Y | O | U | | S | U | N | K | | T | H | E | | S | U | B | M | A | R | I | N | E | !|".PadLeft(25));
                    Console.WriteLine(" +-+-+-+ +-+-+-+-+ +-+-+-+ +-+-+-+-+-+-+-+-+-+-+".PadLeft(25));
                }
                else if (id == " D ")
                {
                    Console.WriteLine(" +-+-+-+ +-+-+-+-+ +-+-+-+ +-+-+-+-+-+-+-+-+-+-+".PadLeft(25));
                    Console.WriteLine(" | Y | O | U | | S | U | N | K | | T | H | E | | D | E | S | T | R | O | Y | E | R | !|".PadLeft(25));
                    Console.WriteLine(" +-+-+-+ +-+-+-+-+ +-+-+-+ +-+-+-+-+-+-+-+-+-+-+".PadLeft(25));
                }
                else if (id == " B ")
                {
                    Console.WriteLine(" +-+-+-+ +-+-+-+-+ +-+-+-+ +-+-+-+-+-+-+-+-+-+-+-+".PadLeft(25));
                    Console.WriteLine(" | Y | O | U | | S | U | N | K | | T | H | E | | B | A | T | T | L | E | S | H | I | P | !|".PadLeft(25));
                    Console.WriteLine(" +-+-+-+ +-+-+-+-+ +-+-+-+ +-+-+-+-+-+-+-+-+-+-+-+".PadLeft(25));
                }
                else if (id == " C ")
                {
                    Console.WriteLine(" +-+-+-+ +-+-+-+-+ +-+-+-+ +-+-+-+-+-+-+-+-+".PadLeft(25));
                    Console.WriteLine(" | Y | O | U | | S | U | N | K | | T | H | E | | C | R | U | I | S | E | R | !|".PadLeft(25));
                    Console.WriteLine(" +-+-+-+ +-+-+-+-+ +-+-+-+ +-+-+-+-+-+-+-+-+".PadLeft(25));
                }
            }
        }
        public class UserCoordinates
        {
            public int Col { get; set; }
            public int Row { get; set; }

            public UserCoordinates(int userCol, int userRow)
            {
                this.Col = userCol;
                this.Row = userRow;
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
}
