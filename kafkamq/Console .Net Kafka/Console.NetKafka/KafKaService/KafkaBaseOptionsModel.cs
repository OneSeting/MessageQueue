namespace KafKaService
{
    public abstract class KafkaBaseOptionsModel
    {
        /// <summary>
        /// 服务器地址
        /// </summary>
        public string[] BootstrapServers { get; set; }
    }
}