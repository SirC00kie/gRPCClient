using Grpc.Net.Client;
using GrpcClient;
using Billing;
using Grpc.Core;

namespace GrpcClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using var channel = GrpcChannel.ForAddress("https://localhost:7099");
            var client = new Billing.Billing.BillingClient(channel);

            StreamUsersProfile(client);

            var emissionAmount = new EmissionAmount { Amount = 150 };
            var makeCoinsEmission = client.CoinsEmission(emissionAmount);
            Console.WriteLine($"{makeCoinsEmission.Status}, {makeCoinsEmission.Comment}");

            var moveCoinsTransaction = new MoveCoinsTransaction { Amount = 1, SrcUser = "boris", DstUser = "oleg" };
            var makeCoinsMove = client.MoveCoins(moveCoinsTransaction);
            Console.WriteLine($"{makeCoinsMove.Status}, {makeCoinsMove.Comment}");

            var longestHistory = client.LongestHistoryCoin(new None());
            Console.WriteLine(longestHistory.History);

            Console.ReadLine();
        }

        private static async void StreamUsersProfile(Billing.Billing.BillingClient client)
        {
            using (var call = client.ListUsers(new None()))
            {
                while (await call.ResponseStream.MoveNext())
                {
                    var user = call.ResponseStream.Current;

                    Console.WriteLine($"Name: {user.Name}, Amount:{user.Amount}");
                }
            }
        }
    }
}