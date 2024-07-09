using Avro.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Streamiz.Kafka.Net;
using Streamiz.Kafka.Net.Kafka;
using Streamiz.Kafka.Net.SchemaRegistry.SerDes.Avro;
using Streamiz.Kafka.Net.SerDes;

namespace Sample.Kafka.Supplier.DI.UnitTests;

internal class CustomWebApplicationFactory<TProgram>
    : WebApplicationFactory<TProgram> where TProgram : class
{
    private readonly IKafkaSupplier _kafkaSupplier;
    public CustomWebApplicationFactory(IKafkaSupplier kafkaSupplier)
    {
        _kafkaSupplier = kafkaSupplier;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var streamConfigDescriptor = services
                .SingleOrDefault(d => d.ServiceType == typeof(StreamConfig<StringSerDes, SchemaAvroSerDes<GenericRecord>>));
            
            services.Remove(streamConfigDescriptor);
            
            services.AddSingleton(_ => new StreamConfig<StringSerDes, SchemaAvroSerDes<GenericRecord>>
            {
                ApplicationId = "kafka-topic-splitter",
                BootstrapServers = "mock://test",
                SchemaRegistryUrl = "mock://test",
                FollowMetadata = true
            });
            //
            // var topicSplitterOptionsDescriptor = services
            //     .SingleOrDefault(d => d.ServiceType == typeof(IOptions<TopicSplitterOptions>));
            //
            // services.Remove(topicSplitterOptionsDescriptor);
            //
            // var appSettingsStub = new Dictionary<string, string> {
            //     {"Topics:SomeService:RetryCount", "3"},
            //     {"Services:SomeService:RetrySleepDuration", "2"}
            // };
            //
            // var configuration = new ConfigurationBuilder()
            //     .AddInMemoryCollection(appSettingsStub)
            //     .Build();
            //
            // services.AddOptions<TopicSplitterOptions>()
            //     .Bind(Configuration)
            //     .ValidateDataAnnotations()
            //     .ValidateOnStart()
                
            var kafkaSupplierDescriptor = services
                .SingleOrDefault(d => d.ServiceType == typeof(IKafkaSupplier));

            services.Remove(kafkaSupplierDescriptor);
            
            services.AddSingleton<IKafkaSupplier>(_ => _kafkaSupplier);
        });

        builder.UseEnvironment("Development");
    }
}