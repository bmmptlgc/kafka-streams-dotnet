using Avro.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using sample_kafka_supplier_di;
using Streamiz.Kafka.Net;
using Streamiz.Kafka.Net.Kafka;
using Streamiz.Kafka.Net.SchemaRegistry.SerDes.Avro;
using Streamiz.Kafka.Net.SerDes;

namespace Sample.Kafka.Supplier.DI.UnitTests;

internal class CustomWebApplicationFactory<TProgram>
    : WebApplicationFactory<TProgram> where TProgram : class
{
    private readonly IKafkaSupplier _kafkaSupplier;
    private readonly StreamConfig<StringSerDes, SchemaAvroSerDes<GenericRecord>> _streamConfig;
    
    public CustomWebApplicationFactory(IKafkaSupplier kafkaSupplier, StreamConfig<StringSerDes, SchemaAvroSerDes<GenericRecord>> streamConfig)
    {
        _kafkaSupplier = kafkaSupplier;
        _streamConfig = streamConfig;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var streamConfigDescriptor = services
                .SingleOrDefault(d => d.ServiceType == typeof(StreamConfig<StringSerDes, SchemaAvroSerDes<GenericRecord>>));
            
            services.Remove(streamConfigDescriptor);
            
            services.AddSingleton(_ => _streamConfig);
            
            var kafkaSupplierDescriptor = services
                .SingleOrDefault(d => d.ServiceType == typeof(IKafkaSupplier));

            services.Remove(kafkaSupplierDescriptor);
            
            services.AddSingleton<IKafkaSupplier>(_ => _kafkaSupplier);
        });

        builder.UseEnvironment("Development");
    }
}