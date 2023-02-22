using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace Match_3_Test
{
    /// <summary>
    /// Core level logic object. Contains main gameplay loops, handles Elements behaviour.
    /// </summary>
    public class GameField
    {
        public Canvas canvas;
        public delegate void GameFieldScoreHandler(int score);
        public event GameFieldScoreHandler ScoreChanged;

        public int Score
        {
            get { return _score; }
            set
            {
                _score = value;
                if (ScoreChanged != null)
                    ScoreChanged.Invoke(_score);
            }
        }
        public Element[,] elements
        {
            get;
            private set;
        }

        private int _columns;
        private int _rows;
        private int _elementTypes;

        // Element refs for handling player actions.
        private Element pickedElement;
        private Element swapedActive;
        private Element swapedPassive;

        private Random _random;
        /// <summary>
        /// Helps restricting player to make only one move per cycle.
        /// </summary>
        private bool _playerMoveMade;
        private int _score;

        public GameField(int width, int height, int elementTypes, Canvas canvas)
        {
            this._columns = width;
            this._rows = height;
            _elementTypes = elementTypes;
            elements = new Element[this._rows, this._columns];
            this.canvas = canvas;
            MainWindow.gameLogicTimer.Tick += this.UpdateGameField;
            _random = new Random();
            _playerMoveMade = false;
            Score = 0;
        }

        /// <summary>
        /// Main game loop.
        /// </summary>
        public void UpdateGameField(object sender, EventArgs e)
        {
            DropElements();
            CheckElementMatches();
            Element bonusElement = CheckElementForBonus(swapedActive);
            HandleMatchedElements();
            HandlePlayerSwappedElements();
            if (bonusElement != null)
                elements[bonusElement.posX, bonusElement.posY] = bonusElement;
            SpawnElements();
        }

        /// <summary>
        /// Returns grid element cell size. For visual elements adjustments.
        /// </summary>
        public Size GetElementSize()
        {
            return new Size(canvas.ActualWidth / this._columns, canvas.ActualHeight / this._rows);
        }

        /// <summary>
        /// Player interaction handler with Elements on GameField.
        /// </summary>
        public void PickElement(Element selectedElement)
        {
            if (_playerMoveMade)
                return;

            UnselectAllElements();
            if (!selectedElement.CanBePicked())
            {
                pickedElement = null;
                return;
            }
            if (pickedElement == null)
            {
                pickedElement = selectedElement;
                pickedElement.SetSelected(true);
                return;
            }
            else if (AreNeightbors(pickedElement, selectedElement))
            {
                _playerMoveMade = true;
                swapedPassive = selectedElement;
                swapedActive = pickedElement;
                SwapElements(pickedElement, selectedElement);
            }
            pickedElement = null;
        }

        /// <summary>
        /// Externally set Element as matched. For example when Element is destroyed by bomb.
        /// </summary>
        public void SetElementIsMatched(int x, int y)
        {
            if (x < 0 || y < 0 || x >= _columns || y >= _rows || elements[x, y] == null)
                return;
            elements[x, y].isMatched = true;
        }

        /// <summary>
        /// Swaps two elements.
        /// </summary>
        private void SwapElements(Element first, Element second)
        {
            elements[first.posX, first.posY] = second;
            elements[second.posX, second.posY] = first;
            int tx = first.posX;
            int ty = first.posY;
            first.SetPosition(second.posX, second.posY);
            second.SetPosition(tx, ty);
        }

        /// <summary>
        /// Is called when game is over. This GameField can be disposed after.
        /// </summary>
        public void ClearField()
        {
            foreach (Element element in elements)
                DestroyElement(element);
            canvas.Children.Clear();
            MainWindow.gameLogicTimer.Tick -= this.UpdateGameField;
        }

        /// <summary>
        /// Drops elements on GameField if nothing is below and sets element as landed otherwise.
        /// </summary>
        private void DropElements()
        {
            // Checking each Element in array starting from the bottom - 1
            for (int y = this._rows - 1; y >= 0; y--)
                for (int x = 0; x < this._columns; x++)
                {
                    // Element is present on this position
                    if (this.elements[x, y] != null)
                    {
                        Element currentElement = this.elements[x, y];
                        // Checking if there are no Elements below
                        if (y < _rows - 1 && this.elements[x, y + 1] == null)
                        {
                            currentElement.isLanded = false;
                            this.elements[x, y] = null;
                            this.elements[x, y + 1] = currentElement;
                            currentElement.SetPosition(x, y + 1);
                        }
                        else
                        {
                            currentElement.isLanded = true;
                        }
                    }
                }
        }

        /// <summary>
        /// Spawning elements on the top row.
        /// </summary>
        private void SpawnElements()
        {
            for (int i = 0; i < _columns; i++)
            {
                if (elements[i, 0] == null)
                    CreateElement(i, 0);
            }
        }

        /// <summary>
        /// Elements that matched in groups more than 3 are marked for destruction.
        /// </summary>
        private void CheckElementMatches()
        {
            foreach (Element element in MatchFinder.FindMatches(elements))
            {
                if (element != null)
                    element.isMatched = true;
            }
        }

        /// <summary>
        /// Checks active Element for bonus patterns and returns element constructed for replacement if active element is destroyed.
        /// </summary>
        private Element CheckElementForBonus(Element element)
        {
            if (element == null)
                return null;
            BonusTypes bonus = MatchFinder.CheckElementForBonus(element, elements);
            if (bonus == BonusTypes.None)
                return null;
            return new Element(element.posX, element.posY, this, element.type, bonus);
        }

        /// <summary>
        /// Elements that are marked as matched are destroyed
        /// </summary>
        private void HandleMatchedElements()
        {
            foreach (Element element in elements)
                if (element != null && element.isMatched)
                {
                    DestroyElement(element);
                    Score++;
                }
        }

        /// <summary>
        /// Swaps back player swapped elements if these were not matched.
        /// </summary>
        private void HandlePlayerSwappedElements()
        {
            if (_playerMoveMade)
            {
                _playerMoveMade = false;
                return;
            }
            if (swapedPassive != null && !swapedPassive.isMatched && swapedActive != null && !swapedActive.isMatched)
                SwapElements(swapedPassive, swapedActive);
            swapedPassive = null;
            swapedActive = null;
        }

        private void CreateElement(int x, int y)
        {
            elements[x, y] = new Element(x, y, this, _random.Next(0, _elementTypes), BonusTypes.None);
        }

        private void CreateElement(int x, int y, int type, BonusTypes bonus)
        {
            elements[x, y] = new Element(x, y, this, type, bonus);
        }

        /// <summary>
        /// Also adds score point
        /// </summary>
        private void DestroyElement(Element element)
        {
            if (element == null)
                return;
            int x = element.posX;
            int y = element.posY;
            if (elements[x, y] == null)
                return;
            elements[x, y].Destroy();
            elements[x, y] = null;
        }

        /// <summary>
        /// Checking if two elements have adjacent sides
        /// </summary>
        private bool AreNeightbors(Element first, Element second)
        {
            if (first.posX == second.posX)
            {
                return (first.posY == second.posY + 1 || first.posY == second.posY - 1);
            }
            else if (first.posY == second.posY)
            {
                return (first.posX == second.posX + 1 || first.posX == second.posX - 1);
            }
            else return false;
        }

        /// <summary>
        /// Unmarking all elements for visual purposes
        /// </summary>
        private void UnselectAllElements()
        {
            foreach (Element element in elements) 
                if (element != null)
                    element.SetSelected(false);
        }
    }
}
