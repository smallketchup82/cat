using System.Net;


var client = new HttpClient();

void downloadCat()
{
    Console.WriteLine("Obtaining cat image...");

    var image = client.GetStreamAsync("https://cataas.com/cat/cute");

    var idk = new Random()

    var file = new FileStream($"{"".Join(idk.Next()}");
    Console.WriteLine(image);
}

downloadCat();