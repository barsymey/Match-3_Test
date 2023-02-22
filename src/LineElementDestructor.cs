using System;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Threading;

namespace Match_3_Test
{
    public class LineElementDestructor
    {
        public int posX
        {
            get;
            private set;
        }
        public int posY
        {
            get;
            private set;
        }
        private GameField _parentField;
        private LineElementDestructorVisual _visual;
        private DispatcherTimer _logicTimer;
        private DispatcherTimer _animationTimer;
        private int _verticalIncrement = 0;
        private int _horizontalIncrement = 0;

        public LineElementDestructor(int posX, int posY, LineElementDestructorDirection direction, GameField gameField, DispatcherTimer logicTimer, DispatcherTimer animationTimer)
        {
            this.posX = posX;
            this.posY = posY;

            _parentField = gameField;
            _logicTimer = logicTimer;
            _animationTimer = animationTimer;

            switch (direction)
            {
                case LineElementDestructorDirection.Left:
                    _horizontalIncrement = -1;
                    break;
                case LineElementDestructorDirection.Right:
                    _horizontalIncrement = 1;
                    break;
                case LineElementDestructorDirection.Up:
                    _verticalIncrement = -1;
                    break;
                case LineElementDestructorDirection.Down:
                    _verticalIncrement = 1;
                    break;
            }

            _logicTimer.Tick += Update;
            _visual = new LineElementDestructorVisual(_parentField, _animationTimer, this);
        }

        private void Update(object sender, EventArgs e)
        {
            if (posX > _parentField.elements.GetLength(0) || posY > _parentField.elements.GetLength(1) || posX <0 ||posY < 0)
            {
                Destroy();
                return;
            }
            _parentField.SetElementIsMatched(posX, posY);
            posX += _horizontalIncrement;
            posY += _verticalIncrement;
            _parentField.SetElementIsMatched(posX, posY);

            _visual.SetTargetPosition(posX, posY);
        }

        private void Destroy()
        {
            _logicTimer.Tick -= Update;
            _visual.Destroy();
        }
    }
    public enum LineElementDestructorDirection
    {
        Left,
        Right,
        Up,
        Down
    }

    public class LineElementDestructorVisual
    {
        Grid _grid;
        GameField _parentField;
        LineElementDestructor _parent;
        DispatcherTimer _animationTimer;
        double _targetX;
        double _targetY;

        public LineElementDestructorVisual(GameField gameField, DispatcherTimer timer, LineElementDestructor parent)
        {
            _animationTimer = timer;
            _parentField = gameField;
            _parent = parent;
            _grid = new Grid
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Background = ResourceManager.destructor
            };
            _parentField.canvas.Children.Add(_grid);
            _animationTimer.Tick += Update;
            SetTargetPosition(_parent.posX, _parent.posY);
            SetPosition(_targetX, _targetY);
            Canvas.SetZIndex(_grid, 1);
        }

        public void SetTargetPosition(double posX, double posY)
        {
            Size elementSize = _parentField.GetElementSize();
            _targetX = posX * elementSize.Width;
            _targetY = posY * elementSize.Height;
        }

        public void Destroy()
        {
            _parentField.canvas.Children.Remove(_grid);
            _animationTimer.Tick -= Update;
        }

        private void Update(object sender, EventArgs e)
        {
            double newX = (_targetX + Canvas.GetLeft(_grid)) / 2;
            double newY = (_targetY + Canvas.GetTop(_grid)) / 2;
            SetPosition(newX, newY);
        }

        private void SetPosition(double posX, double posY)
        {
            Size elementSize = _parentField.GetElementSize();
            Canvas.SetLeft(_grid, posX);
            Canvas.SetTop(_grid, posY);
            _grid.Height = elementSize.Height;
            _grid.Width = elementSize.Width;
        }
    }
}
