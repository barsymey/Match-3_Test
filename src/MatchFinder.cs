using System;
using System.Collections.Generic;

namespace Match_3_Test
{
    public class MatchFinder
    {
        /// <summary>
        /// Checks an array of elements for matches and returns a list of Elements that are matched
        /// </summary>
        public static List<Element> FindMatches(Element[,] elements)
        {
            List<Element> matchedElements = new List<Element>();
            int width = elements.GetLength(0);
            int height = elements.GetLength(1);
            int matchLength;
            int lastType;

            // Check columns
            for (int i = 0; i < width; i++)
            {
                matchLength = 0;
                lastType = -1;
                for (int j = 0; j < height; j++)
                {
                    if (elements[i, j] == null || !elements[i, j].isLanded)
                    {
                        matchLength = 0;
                        lastType = -1;
                        continue;
                    }
                    matchLength++;
                    if (lastType == elements[i, j].type)
                    {
                        continue;
                    }
                    else
                    {
                        if (matchLength > 2)
                            for (int k = 0; k < matchLength; k++)
                                matchedElements.Add(elements[i, j - k - 1]);
                        matchLength = 0;
                    }
                    lastType = elements[i, j].type;
                }
                if (elements[i, height - 1] != null && lastType == elements[i, height - 1].type)
                    matchLength++;
                if (matchLength > 2)
                    for (int k = 0; k < matchLength; k++)
                        matchedElements.Add(elements[i, height - k - 1]);
                matchLength = 0;
            }

            // Check columns
            for (int i = 0; i < width; i++)
            {
                matchLength = 0;
                lastType = -1;
                for (int j = 0; j < height; j++)
                {
                    if (elements[j, i] == null || !elements[j, i].isLanded)
                    {
                        matchLength = 0;
                        lastType = -1;
                        continue;
                    }
                    matchLength++;
                    if (lastType == elements[j, i].type)
                    {
                        continue;
                    }
                    else
                    {
                        if (matchLength > 2)
                            for (int k = 0; k < matchLength; k++)
                                matchedElements.Add(elements[j - k - 1, i]);
                        matchLength = 0;
                    }
                    lastType = elements[j, i].type;
                }
                if (elements[width -1, i] != null && lastType == elements[width - 1, i].type)
                    matchLength++;
                if (matchLength > 2)
                    for (int k = 0; k < matchLength; k++)
                        matchedElements.Add(elements[width - 1 - k, i]);
                matchLength = 0;
            }

            return matchedElements;
        }

        public static BonusTypes CheckElementForBonus(Element element, Element[,] elements)
        {
            int rows = elements.GetLength(1);
            int cols = elements.GetLength(0);

            int above = 0;
            int below = 0;
            int left = 0;
            int right = 0;

            int vertical;
            int horizontal;

            int i = element.posX - 1;
            while (i >= 0)
            {
                if (IsSameType(element, elements[i, element.posY]))
                    left++;
                else
                    break;
                i--;
            }

            i = element.posX + 1;
            while (i < cols)
            {
                if (IsSameType(element, elements[i, element.posY]))
                    right++;
                else
                    break;
                i++;
            }

            i = element.posY - 1;
            while (i >= 0)
            {
                if (IsSameType(element, elements[element.posX, i]))
                    above++;
                else
                    break;
                i--;
            }

            i = element.posY + 1;
            while (i < rows)
            {
                if (IsSameType(element, elements[element.posX, i]))
                    below++;
                else
                    break;
                i++;
            }

            horizontal = left + right + 1;
            vertical = above + below + 1;

            Console.WriteLine(horizontal + " " + vertical);

            if (vertical > 2 && horizontal > 2)
            {
                return BonusTypes.Bomb;
            }
            else if (vertical >2)
            {
                return BonusTypes.LineVertical;
            }
            else if (horizontal > 2)
            {
                return BonusTypes.LineHorizontal;
            }

            return BonusTypes.None;
        }

        private static bool IsSameType(Element element, Element other)
        {
            return (element != null && other != null && element.type == other.type);
        }
    }
}
