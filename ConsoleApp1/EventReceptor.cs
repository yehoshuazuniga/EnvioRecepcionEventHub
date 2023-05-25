using System;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Consumer;
using Azure.Messaging.EventHubs.Processor;

namespace ConsoleApp1
{
    public  class EventReceptor
    {
        static BlobContainerClient storageClient;
        // Los tipos de clientes de Event Hubs son seguros para almacenar en caché y usar como singleton de por vida
        // de la aplicación, que es la mejor práctica cuando los eventos se publican o se leen regularmente.
        static EventProcessorClient processor;
        public async Task recepcionEvento(string ehubNamespaceConnectionString, string eventHubName, string blobStorageConnectionString, string blobContainerName)
        {
            // Leer del grupo de consumidores predeterminado: $default
            string consumerGroup = EventHubConsumerClient.DefaultConsumerGroupName;

            // Crear un cliente de contenedor de blobs que usará el procesador de eventos
            storageClient = new BlobContainerClient(blobStorageConnectionString, blobContainerName);

            // Crear un cliente de procesador de eventos para procesar eventos en el centro de eventos
            processor = new EventProcessorClient(storageClient, consumerGroup, ehubNamespaceConnectionString, eventHubName);

            // Registrar manejadores para procesar eventos y manejar errores
            processor.ProcessEventAsync += ProcessEventHandler;
            processor.ProcessErrorAsync += ProcessErrorHandler;

            //Iniciar el procesamiento
            await processor.StartProcessingAsync();

            // Espere 30 segundos para que se procesen los eventos
            await Task.Delay(TimeSpan.FromSeconds(30));

            // Parar el proceso
            await processor.StopProcessingAsync();
        }

        static async Task ProcessEventHandler(ProcessEventArgs eventArgs)
        {
            // Escriba el cuerpo del evento en la ventana de la consola
            Console.WriteLine("\tEvento recibido: {0}", Encoding.UTF8.GetString(eventArgs.Data.Body.ToArray()));

            //Actualice el punto de control en el almacenamiento de blobs para que la aplicación reciba solo eventos nuevos la próxima vez que se ejecute
            await eventArgs.UpdateCheckpointAsync(eventArgs.CancellationToken);
        }

        static Task ProcessErrorHandler(ProcessErrorEventArgs eventArgs)
        {
            // Escriba detalles sobre el error en la ventana de la consola
            Console.WriteLine($"\tPartición '{eventArgs.PartitionId}': se encontró una excepción no controlada. Esto no se esperaba que sucediera.");
            Console.WriteLine(eventArgs.Exception.Message);
            return Task.CompletedTask;
        }

    }
}
