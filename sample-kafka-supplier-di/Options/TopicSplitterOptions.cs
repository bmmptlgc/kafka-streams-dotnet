using System.ComponentModel.DataAnnotations;

namespace sample_kafka_supplier_di.Options;

public class TopicSplitterOptions
{
    [Required]
    public List<TopicConfig> Topics { get; set; }
}

public class TopicConfig
{
    [Required]
    public string SourceTopic { get; set; }
    [Required]public List<string> MessageTypes { get; set; }
}