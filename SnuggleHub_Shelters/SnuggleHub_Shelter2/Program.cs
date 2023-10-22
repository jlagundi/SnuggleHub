using HtmlAgilityPack;

namespace WebScraping
{
    public class PetGet
    {
        private static async Task<List<(string, string)>> GetS1Data(string url, string imageSelector, string nameSelector)
        {
            var httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync(url);

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var imageNodes = doc.DocumentNode.SelectNodes(imageSelector);
            var nameNodes = doc.DocumentNode.SelectNodes(nameSelector);

            var petDataList = new List<(string, string)>();

            if (imageNodes != null && nameNodes != null)
            {
                for (int i = 0; i < Math.Min(imageNodes.Count, nameNodes.Count); i++)
                {
                    var imageUrl = imageNodes[i].GetAttributeValue("src", "");
                    var petName = nameNodes[i].InnerText.Trim();
                    petDataList.Add((petName, imageUrl));
                }
            }

            return petDataList;
        }

        private static async Task<List<(string, string)>> GetS2Data(string url, string imageSelector)
        {
            var httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync(url);

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var imageNodes = doc.DocumentNode.SelectNodes(imageSelector);

            var petDataList = new List<(string, string)>();

            if (imageNodes != null)
            {
                foreach (var imageNode in imageNodes)
                {
                    var imageUrl = imageNode.GetAttributeValue("src", "");
                    var imageName = imageUrl.Split('/').Last().Split('-').First();
                    petDataList.Add((imageName, imageUrl));
                }
            }

            return petDataList;
        }


        private static async Task<List<(string, string)>> GetS3Data(string url, string imageSelector, string nameSelector)
        {
            var httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync(url);

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var imageNodes = doc.DocumentNode.SelectNodes(imageSelector);
            var nameNodes = doc.DocumentNode.SelectNodes(nameSelector);

            var petDataList = new List<(string, string)>();

            if (imageNodes != null && nameNodes != null)
            {
                for (int i = 0; i < Math.Min(imageNodes.Count, nameNodes.Count); i++)
                {
                    var imageUrl = imageNodes[i].GetAttributeValue("src", "");

                    // Extracting the second part of the image URL after 'https://' if it starts with 'https://sp-ao.shortpixel.ai/' too many extra
                    if (imageUrl.StartsWith("https://sp-ao.shortpixel.ai/"))
                    {
                        var startIndex = imageUrl.IndexOf("https://", "https://sp-ao.shortpixel.ai/".Length);
                        if (startIndex != -1)
                        {
                            imageUrl = imageUrl.Substring(startIndex);
                        }
                    }

                    var petName = nameNodes[i].InnerText.Trim();
                    petDataList.Add((petName, imageUrl));
                }
            }

            return petDataList;
        }




        private static string FormatPetName(string petName)
        {
            return petName.Replace("WS", "").Replace("_sm", "").Trim();
        }

        public static async Task Main(string[] args)
        {
            var pawsCatsUrl = "https://paws.org.ph/cats/";
            var pawsDogsUrl = "https://paws.org.ph/dogs/";
            var caraPhilUrl = "https://www.caraphil.org/pets-for-adoption/";
            var aklanRescueUrl = "https://aklananimalrescue.com/our-animals/";



            var pawsCatsData = await GetS1Data(pawsCatsUrl, "//div[contains(@class, 'elementor-post__thumbnail')]/img", "//h3[contains(@class, 'elementor-post__title')]/a");
            var pawsDogsData = await GetS1Data(pawsDogsUrl, "//div[contains(@class, 'elementor-post__thumbnail')]/img", "//h3[contains(@class, 'elementor-post__title')]/a");
            var caraPhilData = await GetS2Data(caraPhilUrl, "//img[@class='swiper-slide-image']");
            var aklanRescueData = await GetS3Data(aklanRescueUrl, "//div[contains(@class, 'elementor-post__thumbnail')]/img", "//h3[contains(@class, 'elementor-post__title')]/a");



            var allPetData = pawsCatsData.Concat(pawsDogsData).Concat(caraPhilData).Concat(aklanRescueData)
                                          .GroupBy(x => x.Item1)
                                          .Select(g => g.First())
                                          .OrderBy(x => x.Item1)
                                          .ToList();

            if (allPetData.Count > 0)
            {
                var filePath = "C:\\SnuggleHub_Repo\\pets_data.csv";

                using (var writer = new StreamWriter(filePath))
                {
                    writer.WriteLine("Name,ImageURL");

                    foreach (var (petName, imageUrl) in allPetData)
                    {
                        var formattedName = FormatPetName(petName);
                        writer.WriteLine($"{formattedName},{imageUrl}");

                    }
                }

                Console.WriteLine("Data saved to " + filePath);


            }
            else
            {
                Console.WriteLine("No pet information found.");
            }
        }
    }
}
