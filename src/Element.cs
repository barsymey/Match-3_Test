using System;
using System.Windows;

namespace Match_3_Test
{
    /// <summary>
    /// GameField element behaviour
    /// </summary>
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
        /// <summary>
        /// Marked to be destroyed and counted next cycle.
        /// </summary>
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
            _elementVisual = new ElementVisual(type, ElementAction, this, bonus);
            this.bonus = bonus;
        }

        public void SetPosition(int posX, int posY)
        {
            this.posX = posX;
            this.posY = posY;
            if (_elementVisual != null)
                _elementVisual.SetTargetPosition(posX, posY);
        }

        public void SetSelected(bool state)
        {
            if(_elementVisual != null)
                _elementVisual.SetSelected(state);
            isPicked = state;
        }

        /// <summary>
        /// Also activates bonus if such is present.
        /// </summary>
        public void Destroy()
        {
            if(_elementVisual != null)
                _elementVisual.Destroy();
            _elementVisual = null;
            switch(bonus)
            {
                default:
                    break;
                case BonusTypes.Bomb:
                    new BombBonus().Activate(this, parentField);
                    break;
                case BonusTypes.LineVertical:
                    new LineVerticalBonus().Activate(this, parentField);
                    break;
                case BonusTypes.LineHorizontal:
                    new LineHorizontalBonus().Activate(this, parentField);
                    break;
            }
        }

        public bool CanBePicked()
        {
            return isLanded && !isMatched && !isPicked;
        }

        /// <summary>
        /// Handle for player interaction with element.
        /// </summary>
        private void ElementAction(object sender, RoutedEventArgs e)
        {
            parentField.PickElement(this);
        }
    }
}
