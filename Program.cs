using Tesseract;

namespace Ocr
{
    class Program
    {
        const string TessDataPath = @"OcrData";
        const string TxtImagesFolder = @"TXTImages";
        const string TxtResultFolder = @"TXTResults";
        
        static void Main(string[] args)
        {
            string lang = "rus";
            if (args is ["--help" or "/?"])
            {
                Console.WriteLine(
                    """
                    OCR for VideoSubFinder
                    
                    --help          Help
                    --lang rus+eng      Choose lang: [rus, eng, rus+eng]
                                    By default: rus
                                    More languages: https://github.com/tesseract-ocr/tessdata
                                    Stored in OcrData folder
                    """);
                return;
            }

            switch (args)
            {
                case ["--lang", { } selectedLang]:
                    lang = selectedLang;
                    break;
            }

            if (!Directory.Exists(TxtImagesFolder))
            {
                return;
            }

            if (!Directory.Exists(TxtResultFolder))
            {
                Directory.CreateDirectory(TxtResultFolder);
            }
            var files = Directory.GetFiles(TxtImagesFolder);
            
            using var engine = new TesseractEngine(TessDataPath, lang, EngineMode.Default);
            Array.ForEach(files, filepath => HandleImage(engine, filepath));

            Console.WriteLine("Done");
        }

        static void HandleImage(TesseractEngine engine, string imagePath)
        {
            using var img = Pix.LoadFromFile(imagePath);
            using var page = engine.Process(img);
            var text = page.GetText().Trim();
            var filename = Path.GetFileNameWithoutExtension(imagePath) + ".txt";
            
            File.WriteAllText(Path.Combine(TxtResultFolder, filename), text);
        }
    }
}

