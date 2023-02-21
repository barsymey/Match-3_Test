using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;

namespace Match_3_Test
{
    public class ElementVisual
    {
        private Element _parentElement;
        private Rectangle _button;
        private double _targetX;
        private double _targetY;

        public ElementVisual(int type, MouseButtonEventHandler clickFunction, Element parent)
        {
            _parentElement = parent;
            _button = new Rectangle
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Fill = ResourceManager.elementGraphics[type]
            };
            _button.MouseLeftButtonDown += clickFunction;
            _parentElement.parentField.canvas.Children.Add(_button);
            MainWindow.animationTimer.Tick += ProceedToTarget;
            SetTargetPosition(_parentElement.posX, _parentElement.posY);
            SetPositionToTarget();
        }

        public void SetTargetPosition(int posX, int posY)
        {
            Size elementSize = _parentElement.parentField.GetElementSize();
            _targetY = (elementSize.Height * posY);
            _targetX = (elementSize.Width * posX);
            _button.Height = elementSize.Height;
            _button.Width = elementSize.Width;
        }

        public void Destroy()
        {
            _parentElement.parentField.canvas.Children.Remove(_button);
            MainWindow.animationTimer.Tick -= ProceedToTarget;
        }

        private void SetPositionToTarget()
        {
            Canvas.SetLeft(_button, _targetX);
            Canvas.SetTop(_button, _targetY);
        }

        private void ProceedToTarget(object sender, EventArgs e)
        {
            Canvas.SetLeft(_button, (_targetX + Canvas.GetLeft(_button)) / 2);
            Canvas.SetTop(_button, (_targetY + Canvas.GetTop(_button)) / 2 );
        }

    }
}
