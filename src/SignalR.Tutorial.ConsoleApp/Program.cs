using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.AspNetCore.SignalR.Client;

namespace SignalR.Tutorial.ConsoleApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var builder = new HubConnectionBuilder();

            var connection = builder.WithUrl(args[0]).WithCredentials(CredentialCache.DefaultCredentials).Build();

            connection.On<string, string>("SendMessage",
                (sender, message) => Console.WriteLine($"{sender}: {message}"));

            connection.On<string, string, IList<string>>("SendAction",
                (sender, action, users) =>
                    Console.WriteLine($"{sender} {action} -- Active Users: {string.Join(", ", users)}"));

            connection.StartAsync().ContinueWith(task =>
            {
                Console.WriteLine(task.IsFaulted
                    ? $"Failed to connect: {task.Exception.GetBaseException()}"
                    : "Connected...");

                while (true)
                {
                    var line = Console.ReadLine();
                    if (!string.IsNullOrEmpty(line))
                    {
                        connection.SendAsync("Send", line).ContinueWith(sendTask =>
                        {
                            if (task.IsFaulted)
                            {
                                Console.WriteLine("Send failed {0}", sendTask.Exception.GetBaseException());
                            }
                        });
                    }
                }
            }).Wait();
        }
    }
}