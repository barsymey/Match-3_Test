using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace Match_3_Test
{
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

        private int _columns;
        private int _rows;
        private int _elementTypes;
        private Element[,] elements;
        private Element pickedElement;
        private Element swapedPassive;
        private Element swapedActive;
        private Random _random;
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

        public void UpdateGameField(object sender, EventArgs e)
        {
            DropElements();
            SpawnElements();
            CheckElementMatches();
            CheckElementForBonus(swapedActive);
            HandleDestroyedElements();
            HandlePlayerSwappedElements();
        }

        public Size GetElementSize()
        {
            return new Size(canvas.ActualWidth / this._columns, canvas.ActualHeight / this._rows);
        }

        public void PickElement(Element selectedElement)
        {
            if (_playerMoveMade)
                return;
            if (!selectedElement.CanBePicked())
            {
                pickedElement = null;
                return;
            }
            if (pickedElement == null)
            {
                pickedElement = selectedElement;
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

        private void SwapElements(Element first, Element second)
        {
            elements[first.posX, first.posY] = second;
            elements[second.posX, second.posY] = first;
            int tx = first.posX;
            int ty = first.posY;
            first.SetPosition(second.posX, second.posY);
            second.SetPosition(tx, ty);
        }

        public void ClearField()
        {
            foreach (Element element in elements)
                DestroyElement(element);
        }

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

        private void SpawnElements()
        {
            for (int i = 0; i < _columns; i++)
            {
                if (elements[i, 0] == null)
                    CreateElement(i, 0);
            }
        }

        private void CheckElementMatches()
        {
            foreach (Element element in MatchFinder.FindMatches(elements))
            {
                if (element != null)
                    element.isMatched = true;
            }
        }

        private void CheckElementForBonus(Element element)
        {
            if (element != null)
            {
                MatchFinder.CheckElementForBonus(element, elements);
            }
        }

        private void HandleDestroyedElements()
        {
            foreach (Element element in elements)
                if (element != null && element.isMatched)
                {
                    DestroyElement(element);
                    Score++;
                }
        }

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

        private void DestroyElement(Element element)
        {
            int x = element.posX;
            int y = element.posY;
            if (elements[x, y] == null)
                return;
            elements[x, y].Destroy();
            elements[x, y] = null;
        }

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
    }
}
