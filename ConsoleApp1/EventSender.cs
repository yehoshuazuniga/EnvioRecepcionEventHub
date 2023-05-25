using System;
using System.Text;
using System.Threading.Tasks;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;

namespace ConsoleApp1
{
    public  class EventSender
    {
        private const int numOfEvents = 3;
        static EventHubProducerClient producerClient;
        public async Task envioEventos(string connectionString, string eventHubName)
        {
            // Cree un cliente productor que pueda usar para enviar eventos a un centro de eventos
            producerClient = new EventHubProducerClient(connectionString, eventHubName);

            // Crear un lote de eventos
            using EventDataBatch eventBatch = await producerClient.CreateBatchAsync();

            for (int i = 1; i <= numOfEvents; i++)
            {
                if (!eventBatch.TryAdd(new EventData(Encoding.UTF8.GetBytes($"Evento {i}"))))
                {
                    // si es demasiado grande para el lote
                    throw new Exception($"El evento {i} es demasiado grande para el lote y no se puede enviar.");
                }
            }

            try
            {
                //Usar el cliente productor para enviar el lote de eventos al centro de eventos
                await producerClient.SendAsync(eventBatch);
                Console.WriteLine($"Un lote de {numOfEvents} eventos han sido publicados");
            }
            finally
            {
                await producerClient.DisposeAsync();
            }
        }
    }
}
