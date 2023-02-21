using System;
using System.Windows;

namespace Match_3_Test
{
    public class Element
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
        public GameField parentField;
        /// <summary>
        /// Is solidly landed and ready to be checked for matches
        /// </summary>
        public bool isLanded = false;
        public bool isMatched = false;
        public bool isPicked = false;
        public BonusTypes bonus;
        public int type;

        private ElementVisual _elementVisual;

        public Element(int posX, int posY, GameField parentField, int type, BonusTypes bonus)
        {
            this.posX = posX;
            this.posY = posY;
            this.parentField = parentField;
            this.type = type;
            _elementVisual = new ElementVisual(type, ElementAction, this);
            this.bonus = bonus;
        }

        public void SetPosition(int posX, int posY)
        {
            this.posX = posX;
            this.posY = posY;
            _elementVisual.SetTargetPosition(posX, posY);
        }

        public void Destroy()
        {
            _elementVisual.Destroy();
        }

        public bool CanBePicked()
        {
            return isLanded && !isMatched && !isPicked;
        }

        private void ElementAction(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("landed: " + isLanded + " destroyed: " + isMatched + " pos: " + posX + "," + posY);
            parentField.PickElement(this);
        }
    }
}
