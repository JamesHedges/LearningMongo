using System.Threading.Tasks;

namespace MongoWithCSharp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var app = new MongoWithCSharpApp();
            await app.RunAsync();
        }
    }
}
