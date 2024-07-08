using Avro.Generic;
using sample_kafka_supplier_di.Options;
using Streamiz.Kafka.Net;
using Streamiz.Kafka.Net.Kafka;
using Streamiz.Kafka.Net.Kafka.Internal;
using Streamiz.Kafka.Net.SchemaRegistry.SerDes.Avro;
using Streamiz.Kafka.Net.SerDes;

namespace sample_kafka_supplier_di
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseContentRoot(AppContext.BaseDirectory)
                .ConfigureServices((hostContext, services) =>
                {
                    hostContext.HostingEnvironment.ApplicationName = nameof(Program);

                    services.Configure<HostOptions>(option =>
                    {
                        option.ShutdownTimeout = TimeSpan.FromSeconds(10);
                    });
                    
                    var streamConfig = new StreamConfig<StringSerDes, SchemaAvroSerDes<GenericRecord>>
                    {
                        ApplicationId = "kafka-topic-splitter",
                        BootstrapServers = string.Join(",", hostContext.Configuration
                            .GetSection(KafkaBusOptions.Section).Get<KafkaBusOptions>()?.BootstrapServers ?? new List<string>()),
                        SchemaRegistryUrl = hostContext.Configuration
                            .GetSection(KafkaBusOptions.Section)
                            .Get<KafkaBusOptions>()?.SchemaRegistry.Url,
                        FollowMetadata = true
                    };

                    services.AddSingleton(streamConfig);
                    
                    services.AddSingleton(
                        (IKafkaSupplier)new DefaultKafkaClientSupplier(new KafkaLoggerAdapter(streamConfig), streamConfig));
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .UseWindowsService();
    }
}