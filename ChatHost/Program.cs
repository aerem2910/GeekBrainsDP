using DiplomaProject;
using System;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace ChatHost
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Uri baseAddress = new Uri("http://localhost:8733/DiplomaProject/");

            using (var host = new ServiceHost(typeof(ServiceChat), baseAddress))
            {
                try
                {
                    host.AddServiceEndpoint(typeof(IServiceChat), new WSDualHttpBinding(), "ServiceChat");
                    
                    ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
                    smb.HttpGetEnabled = true;
                    host.Description.Behaviors.Add(smb);

                    host.Open();
                    Console.WriteLine("!!!Сервис запущен!!!");
                    Console.ReadLine();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка в работе сервиса: {ex.Message}");
                }
                finally
                {
                    if (host.State == CommunicationState.Opened)
                    {
                        host.Close();
                    }
                }
            }
        }
    }
}