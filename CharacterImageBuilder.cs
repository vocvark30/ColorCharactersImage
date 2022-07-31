using System.Drawing;
using System.Text;

namespace ImageToCharacters
{
    interface IColorPickStrategy
    {
        Color GetColor(Bitmap image, Size size, int x, int y);
    }

    interface ICharacterPickStrategy
    {
        char GetCharacter(char[] dictionary, Color color);
    }

    class DefaultColorPickStrategy : IColorPickStrategy
    {
        public Color GetColor(Bitmap image, Size size, int x, int y)
        {
            float position_x = (float)x / size.Width;
            float position_y = (float)y / size.Height;

            return image.GetPixel((int)(position_x * image.Width), (int)(position_y * image.Height));
        }
    }

    class DefaultCharacterPickStrategy : ICharacterPickStrategy
    {
        public char GetCharacter(char[] dictionary, Color color)
        {
            int average = (color.R + color.G + color.B) / 3;
            int choose = (int)((average / 255f) * (dictionary.Length - 1));
            return dictionary[choose];
        }
    }


    // Builds array of strings and 2D array of colors
    class CharacterImageBuilder
    {
        private Bitmap image;
        private Size size;

        private char[] dictionary;

        private IColorPickStrategy colorPickStrategy;
        private ICharacterPickStrategy characterPickStrategy;


        public (Color[,] colorMap, string[] lines) Build()
        {
            if (image == null)
                throw new NullReferenceException("image is null");

            if (dictionary == null || dictionary.Length == 0)
                throw new ArgumentException("Dictionary is null or empty");

            if (colorPickStrategy == null)
                throw new NullReferenceException("Color pick strategy is null");

            if (characterPickStrategy == null)
                throw new NullReferenceException("Character pick strategy is null");

            if (size.Width <= 0 || size.Height <= 0)
                throw new ArgumentException("width or height <= 0");

            StringBuilder builder = new StringBuilder();

            string[] lines = new string[size.Height];
            Color[,] colorMap = new Color[size.Height, size.Width];

            for (int y = 0; y < size.Height; y++)
            {
                for (int x = 0; x < size.Width; x++)
                {
                    Color color = colorPickStrategy.GetColor(image, size, x, y);
                    char character = characterPickStrategy.GetCharacter(dictionary, color);

                    builder.Append(character);
                    colorMap[y, x] = color;
                }
                lines[y] = builder.ToString();
                builder.Clear();
            }

            return (colorMap, lines);
        }

        public CharacterImageBuilder SetImage(Bitmap image)
        {
            this.image = image;
            return this;
        }

        public CharacterImageBuilder SetSize(Size size)
        {
            this.size = size;
            return this;
        }

        public CharacterImageBuilder SetDictionary(char[] dictionary)
        {
            this.dictionary = dictionary;
            return this;
        }

        public CharacterImageBuilder SetColorPickStrategy(IColorPickStrategy colorPickStrategy)
        {
            this.colorPickStrategy = colorPickStrategy;
            return this;
        }

        public CharacterImageBuilder SetCharacterPickStrategy(ICharacterPickStrategy characterPickStrategy)
        {
            this.characterPickStrategy = characterPickStrategy;
            return this;
        }
    }
}