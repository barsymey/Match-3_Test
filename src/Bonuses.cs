using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Match_3_Test
{
    public enum BonusTypes
    {
        None,
        LineVertical,
        LineHorizontal,
        Bomb
    }

    /// <summary>
    /// Bonus interface for universal interactions. Not necessary now.
    /// </summary>
    public interface IElementBonus
    {
        void Activate(Element parentElement, GameField gameField);
    }

    /// <summary>
    /// Destroys a square of elements 3x3 on a given GameField around Element after delay. 
    /// </summary>
    public class BombBonus : IElementBonus
    {
        public int _bombDelay = 250;
        public async void Activate(Element parentElement, GameField gameField)
        {
            int posX = parentElement.posX;
            int posY = parentElement.posY;
            int cols = gameField.elements.GetLength(0);
            int rows = gameField.elements.GetLength(1);
            await Task.Delay(_bombDelay);
            for (int i = -1; i < 2; i++)
                for (int j = -1; j < 2; j++)
                {
                    gameField.SetElementIsMatched(posX + i, posY + j);
                }
        }
    }

    /// <summary>
    /// Creates two destructors moving up and down
    /// </summary>
    public class LineVerticalBonus : IElementBonus
    {
        public void Activate(Element parentElement, GameField gameField)
        {
            new LineElementDestructor(parentElement.posX, parentElement.posY, LineElementDestructorDirection.Up, gameField, MainWindow.gameLogicTimer, MainWindow.animationTimer);
            new LineElementDestructor(parentElement.posX, parentElement.posY, LineElementDestructorDirection.Down, gameField, MainWindow.gameLogicTimer, MainWindow.animationTimer);
        }
    }

    /// <summary>
    /// Creates two destructors moving left and right
    /// </summary>
    public class LineHorizontalBonus : IElementBonus
    {
        public void Activate(Element parentElement, GameField gameField)
        {
            new LineElementDestructor(parentElement.posX, parentElement.posY, LineElementDestructorDirection.Left, gameField, MainWindow.gameLogicTimer, MainWindow.animationTimer);
            new LineElementDestructor(parentElement.posX, parentElement.posY, LineElementDestructorDirection.Right, gameField, MainWindow.gameLogicTimer, MainWindow.animationTimer);
        }
    }
}
