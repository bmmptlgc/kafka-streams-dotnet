using Avro.Generic;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using sample_kafka_supplier_di.Options;
using Streamiz.Kafka.Net;
using Streamiz.Kafka.Net.Kafka;
using Streamiz.Kafka.Net.Kafka.Internal;
using Streamiz.Kafka.Net.SchemaRegistry.SerDes.Avro;
using Streamiz.Kafka.Net.SerDes;

namespace sample_kafka_supplier_di
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // services.AddSingleton(c =>
            //     new CachedSchemaRegistryClient(
            //         Configuration.GetSection(KafkaBusOptions.Section).Get<KafkaBusOptions>()?.SchemaRegistry));
            
            services.AddOptions<TopicSplitterOptions>()
                .Bind(Configuration)
                .ValidateDataAnnotations()
                .ValidateOnStart();

            services.AddHostedService<TopicSplitterService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure()
        {
        }
    }
}
