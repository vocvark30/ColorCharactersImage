using System.Drawing;

namespace ImageToCharacters
{
    class ImageBuilder
    {
        private Font font;
        private string[] lines;

        private int stringLength
        {
            get
            {
                if (lines == null || lines.Length == 0)
                    throw new ArgumentException("lines is null or empty");
                else
                    return lines[0].Length;
            }
        }

        private int outputImageWidth
        {
            get
            {
                if (lines != null)
                    return (int)Graphics.FromImage(new Bitmap(1, 1)).MeasureString(lines[0], font).Width;
                else
                    throw new NullReferenceException("lines is null");
            }
        }

        private int outputImageHeight
        {
            get
            {
                if (lines != null)
                    return (int)(lines.Length * font.GetHeight());
                else
                    throw new NullReferenceException("lines is null");
            }
        }

        public Bitmap Build(Color[,] colors)
        {
            if (colors == null || colors.GetLength(0) == 0 || colors.GetLength(1) == 0)
                throw new ArgumentException("colors is null or empty");

            if (font == null)
                throw new NullReferenceException("font is null");

            Bitmap textImage = new Bitmap(outputImageWidth, outputImageHeight);

            Graphics graphics = Graphics.FromImage(textImage);
            graphics.Clear(Color.FromArgb(0, 0, 0));

            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
            graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bicubic;

            // Width and height of one character
            float w = Graphics.FromImage(new Bitmap(1, 1)).MeasureString("AB", font).Width -
                        Graphics.FromImage(new Bitmap(1, 1)).MeasureString("A", font).Width;
            float h = font.GetHeight();

            Dictionary<Color, Brush> brushes = new Dictionary<Color, Brush>();
            PointF point = new PointF();

            for (int y = 0; y < lines.Length; y++)
            {
                point.Y = y * h;
                for (int x = 0; x < stringLength; x++)
                {
                    if (!brushes.ContainsKey(colors[y, x]))
                        brushes.Add(colors[y, x], new SolidBrush(colors[y, x]));

                    point.X = x * w;

                    graphics.DrawString(lines[y][x].ToString(), font,
                        brushes[colors[y, x]], point);
                }
            }

            graphics.Flush();

            return textImage;
        }

        public ImageBuilder SetFont(Font font)
        {
            this.font = font;
            return this;
        }

        public ImageBuilder SetText(string[] lines)
        {
            this.lines = lines;
            return this;
        }
    }
}