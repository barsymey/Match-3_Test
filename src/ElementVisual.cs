using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Match_3_Test
{
    /// <summary>
    /// Visual representation of an Element
    /// </summary>
    public class ElementVisual
    {
        private Element _parentElement;
        private Grid _grid;
        private Rectangle _bonustRect;

        // Positions where element will move over time
        private double _targetX;
        private double _targetY;

        private BonusTypes _bonusType;
        private float _selectedSizeMarginMultipler = 0;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"> The type of Element to display </param>
        /// <param name="clickHandler"> What should happen on click </param>
        /// <param name="parent"> Parent Element </param>
        /// <param name="bonusType"> Type of bonus to display </param>
        public ElementVisual(int type, MouseButtonEventHandler clickHandler, Element parent, BonusTypes bonusType)
        {
            _parentElement = parent;
            _bonusType = bonusType;

            _grid = new Grid
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Background = ResourceManager.elementGraphics[type]
            };

            // Handling the visual appearance for bonuses
            if(bonusType != BonusTypes.None)
            {
                Brush fill;
                switch (bonusType)
                {
                    default:
                        fill = null;
                        break;
                    case BonusTypes.Bomb:
                        fill = ResourceManager.bonusBomb;
                        break;
                    case BonusTypes.LineHorizontal:
                        fill = ResourceManager.bonusHorizontal;
                        break;
                    case BonusTypes.LineVertical:
                        fill = ResourceManager.bonusVertical;
                        break;
                }

                _bonustRect = new Rectangle
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Fill = fill
                };
                _grid.Children.Add(_bonustRect);
            }

            _grid.MouseLeftButtonDown += clickHandler;
            _parentElement.parentField.canvas.Children.Add(_grid);

            MainWindow.animationTimer.Tick += Update;
            SetTargetPosition(_parentElement.posX, _parentElement.posY);
            SetPositionToTarget();
            Panel.SetZIndex(_grid, -1);
        }

        /// <summary>
        /// Setting position for visual to move toward over time. Used when parent element position changes.
        /// </summary>
        public void SetTargetPosition(int posX, int posY)
        {
            Size elementSize = _parentElement.parentField.GetElementSize();
            _targetY = (elementSize.Height * posY);
            _targetX = (elementSize.Width * posX);
            _grid.Height = elementSize.Height;
            _grid.Width = elementSize.Width;
            Grid.SetZIndex(_grid, 0);
        }

        /// <summary>
        /// Destroying visual elements and unsubscribing from animation timer tick.
        /// </summary>
        public void Destroy()
        {
            _parentElement.parentField.canvas.Children.Remove(_grid);
            if (_bonustRect != null)
                _grid.Children.Clear();
            MainWindow.animationTimer.Tick -= Update;
        }

        /// <summary>
        /// Set graphical representation to selected state. Shrinks element a bit for now.
        /// </summary>
        public void SetSelected(bool state)
        {
            if (state)
                _selectedSizeMarginMultipler = 5;
            else
                _selectedSizeMarginMultipler = 0;
        }

        /// <summary>
        /// Positioning visual to target. For initial positioning.
        /// </summary>
        private void SetPositionToTarget()
        {
            Canvas.SetLeft(_grid, _targetX);
            Canvas.SetTop(_grid, _targetY);
        }


        private void Update(object sender, EventArgs e)
        {
            SetElementSize(_parentElement.parentField.GetElementSize());
            Canvas.SetLeft(_grid, (_targetX + Canvas.GetLeft(_grid)) / 2);
            Canvas.SetTop(_grid, (_targetY + Canvas.GetTop(_grid)) / 2 );
        }

        /// <summary>
        /// Setting Element size in case if parent canvas size is changed. Also manages changing size for marking selected Element
        /// </summary>
        private void SetElementSize(Size size)
        {
            _grid.Margin = new Thickness(_selectedSizeMarginMultipler);
            _grid.Width = size.Width - _selectedSizeMarginMultipler * 2;
            _grid.Height = size.Height - _selectedSizeMarginMultipler * 2;
            if (_bonustRect != null)
            {
                _bonustRect.Height = _grid.ActualHeight;
                _bonustRect.Width = _grid.ActualWidth;
            }
        }
    }
}
