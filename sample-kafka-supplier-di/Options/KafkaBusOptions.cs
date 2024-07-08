using System.ComponentModel.DataAnnotations;
using Confluent.SchemaRegistry;

namespace sample_kafka_supplier_di.Options;

public class KafkaBusOptions
{
    public const string Section = "Kafka";

    public List<string> BootstrapServers { get; set; } = new();

    [Required]
    public SchemaRegistryConfig SchemaRegistry { get; set; }
}