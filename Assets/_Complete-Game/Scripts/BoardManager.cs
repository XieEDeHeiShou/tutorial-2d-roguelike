﻿using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Scripts {
    public class BoardManager : MonoBehaviour {
        /// <summary>
        ///     Number of columns in our game board.
        /// </summary>
        private const int Columns = 8;

        /// <summary>
        ///     Number of rows in our game board.
        /// </summary>
        private const int Rows = Columns;

        /// <summary>
        ///     Lower and upper limit for our random number of food items per level.
        /// </summary>
        private readonly Count _foodCount = new Count(1, 5);

        /// <summary>
        ///     A list of possible locations to place tiles.
        /// </summary>
        private readonly List<Vector3> _gridPositions = new List<Vector3>();

        /// <summary>
        ///     Lower and upper limit for our random number of walls per level.
        /// </summary>
        private readonly Count _wallCount = new Count(5, 9);

        private Transform _boardHolder; //A variable to store a reference to the transform of our Board object.
        public GameObject[] enemyTiles; //Array of enemy prefabs.
        public GameObject exit; //Prefab to spawn for exit.
        public GameObject[] floorTiles; //Array of floor prefabs.

        public GameObject[] foodTiles; //Array of food prefabs.

        public GameObject[] outerWallTiles; //Array of outer tile prefabs.

        public GameObject[] wallTiles; //Array of wall prefabs.

        //SetupScene initializes our level and calls the previous functions to lay out the game board
        public void SetupScene(int level) {
            //Creates the outer walls and floor.
            BoardSetup();

            //Reset our list of grid positions.
            InitialiseList();

            //Instantiate a random number of wall tiles based on minimum and maximum, at randomized positions.
            LayoutObjectAtRandom(wallTiles, _wallCount.Minimum, _wallCount.Maximum);

            //Instantiate a random number of food tiles based on minimum and maximum, at randomized positions.
            LayoutObjectAtRandom(foodTiles, _foodCount.Minimum, _foodCount.Maximum);

            //Determine number of enemies based on current level number, based on a logarithmic progression
            var enemyCount = (int) Math.Log(level, 2f);

            //Instantiate a random number of enemies based on minimum and maximum, at randomized positions.
            LayoutObjectAtRandom(enemyTiles, enemyCount, enemyCount);

            //Instantiate the exit tile in the upper right hand corner of our game board
            //Instantiate(exit, new Vector3(columns - 1, rows - 1, 0f), Quaternion.identity);
            //Instantiate the exit tile in random place of our game board
            LayoutObjectAtRandom(new[] {exit}, 1, 1);
        }

        /// <summary>
        ///     Sets up the outer walls and floor (background) of the game board.
        /// </summary>
        private void BoardSetup() {
            //Instantiate Board and set boardHolder to its transform.
            _boardHolder = new GameObject("Board").transform;

            //Loop along x axis, starting from -1 (to fill corner) with floor or outer wall edge tiles.
            for (var x = -1;
                x < Columns + 1;
                x++) //Loop along y axis, starting from -1 to place floor or outer wall tiles.                
            for (var y = -1; y < Rows + 1; y++) {
                //Choose a random tile from our array of floor tile prefabs and prepare to instantiate it.
                var toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)];

                //Check if we current position is at board edge, if so choose a random outer wall prefab from our array of outer wall tiles.
                if (x == -1 || x == Columns || y == -1 || y == Rows)
                    toInstantiate = outerWallTiles[Random.Range(0, outerWallTiles.Length)];

                //Instantiate the GameObject instance using the prefab chosen for toInstantiate at the Vector3
                //corresponding to current grid position in loop, cast it to GameObject. Set the parent of our newly
                //instantiated object instance to boardHolder, this is just organizational to avoid cluttering hierarchy.
                Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity, _boardHolder);
            }
        }

        /// <summary>
        ///     Clears our list gridPositions and prepares it to generate a new board.
        /// </summary>
        private void InitialiseList() {
            //Clear our list gridPositions.
            _gridPositions.Clear();

            //Loop through x axis (columns).
            for (var x = 1; x < Columns - 1; x++) //Within each column, loop through y axis (rows).
            for (var y = 1;
                y < Rows - 1;
                y++) //At each index add a new Vector3 to our list with the x and y coordinates of that position.
                _gridPositions.Add(new Vector3(x, y, 0f));
        }

        /// <summary>
        ///     LayoutObjectAtRandom accepts an array of game objects to choose from along with a minimum and maximum range for the
        ///     number of objects to create.
        /// </summary>
        /// <param name="tileArray">Objects to be setup location</param>
        /// <param name="minimum">Minimum of objects</param>
        /// <param name="maximum">Maximum of objects</param>
        private void LayoutObjectAtRandom([NotNull] IReadOnlyList<GameObject> tileArray, int minimum, int maximum) {
            //Choose a random number of objects to instantiate within the minimum and maximum limits
            var objectCount = Random.Range(minimum, maximum + 1);

            //Instantiate objects until the randomly chosen limit objectCount is reached
            for (var i = 0; i < objectCount; i++) {
                //Choose a position for randomPosition by getting a random position from our list of available Vector3s stored in gridPosition
                var randomPosition = RandomPosition();

                //Choose a random tile from tileArray and assign it to tileChoice
                var tileChoice = tileArray[Random.Range(0, tileArray.Count)];

                //Instantiate tileChoice at the position returned by RandomPosition with no change in rotation
                Instantiate(tileChoice, randomPosition, Quaternion.identity);
            }
        }

        /// <summary>
        ///     RandomPosition returns a random position from our list gridPositions.
        /// </summary>
        /// <returns>The random position in 3d</returns>
        private Vector3 RandomPosition() {
            //Declare an integer randomIndex, set it's value to a random number between 0 and the count of items in our List gridPositions.
            var randomIndex = Random.Range(0, _gridPositions.Count);

            //Declare a variable of type Vector3 called randomPosition, set it's value to the entry at randomIndex from our List gridPositions.
            var randomPosition = _gridPositions[randomIndex];

            //Remove the entry at randomIndex from the list so that it can't be re-used.
            _gridPositions.RemoveAt(randomIndex);

            //Return the randomly selected Vector3 position.
            return randomPosition;
        }


        // Using Serializable allows us to embed a class with sub properties in the inspector.
        [Serializable]
        private class Count {
            public readonly int Maximum; //Maximum value for our Count class.
            public readonly int Minimum; //Minimum value for our Count class.

            //Assignment constructor.
            public Count(int min, int max) {
                Minimum = min;
                Maximum = max;
            }
        }
    }
}