using System.Drawing;

namespace ImageToCharacters
{
    class Progam
    {
        private const string defaultDictionary = "345789ABCD";

        // ImageToCharacters.exe input.png output.png processing width dictionary
        // input.png must be in the same directory as ImageToCharacters.exe
        // width => positive integer, number of characters per line
        // dictionary => string of characters in double quotes, that will be used in processing (Optional)
        // 
        // Example:
        // ImageToCharacters.exe input.png output.png 240 "ABCD345789"

        static void Main(string[] args)
        {
            (string inputImage, string outputImage, int width, char[] dictionary) arguments;
            try
            {
                arguments = ParseArguments(args);
                string imagePath = Directory.GetCurrentDirectory() + "\\" + arguments.inputImage;
                Bitmap image = (Bitmap)Image.FromFile(imagePath);

                int height = (int)((float)image.Height * arguments.width / (image.Width * 2));

                CharacterImageBuilder builder = new CharacterImageBuilder();
                (Color[,] colors, string[] lines) = builder
                    .SetColorPickStrategy(new DefaultColorPickStrategy())
                    .SetCharacterPickStrategy(new DefaultCharacterPickStrategy())
                    .SetDictionary(arguments.dictionary)
                    .SetSize(new Size(arguments.width, height))
                    .SetImage(image)
                    .Build();

                ImageBuilder imageBuilder = new ImageBuilder();
                imageBuilder
                    .SetFont(new Font("Courier New", 16, FontStyle.Bold))
                    .SetText(lines);


                Bitmap result = imageBuilder.Build(colors);

                string path = Directory.GetCurrentDirectory() + "\\" + arguments.outputImage;
                result.Save(path, System.Drawing.Imaging.ImageFormat.Png);
            }
            catch (ArgumentException exception)
            {
                Console.WriteLine($"Exception in argument processing: {exception.Message}");
            }
            catch(FileNotFoundException exception)
            {
                Console.WriteLine($"File not found: {exception.Message}");
            }
        }


        private static (string inputImage, string outputImage, int width, char[] dictionary)
            ParseArguments(string[] args)
        {
            if (args.Length == 3 || args.Length == 4)
            {
                string inputImage = args[0].EndsWith(".png") || args[0].EndsWith(".jpg") ? args[0] : "";
                string outputImage = args[1].EndsWith(".png") ? args[1] : "";

                int width;

                char[] dictionary = (args.Length == 4 ? args[3] : defaultDictionary).ToCharArray();

                if (!int.TryParse(args[2], out width) || width <= 0 ||
                    inputImage == "" || outputImage == "" || dictionary.Length == 0)
                    throw new ArgumentException("Wrong format of arguments");


                return (inputImage, outputImage, width, dictionary);
            }
            else
            {
                throw new ArgumentException("Wrong count of arguments");
            }
        }
    }
}