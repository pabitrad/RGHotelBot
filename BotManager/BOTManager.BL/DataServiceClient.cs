
using System.Collections.Specialized;
using System.Net;
using System.Collections.Generic;
using RG.Core.Entities;
using RG.Core.Entities.BOT;
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ServiceModel.ServiceContractAttribute(ConfigurationName = "IDataService")]
public interface IDataService
{
    [System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/IDataService/Echo", ReplyAction = "http://tempuri.org/IDataService/EchoResponse")]
    string Echo(string input);
    
    [System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/IDataService/SubmitBOTResponse", ReplyAction = "http://tempuri.org/IDataService/SubmitBOTResponseResponse")]
    string SubmitBOTResponse(RG.Core.Entities.BOTManager botManager, System.Collections.Generic.KeyValuePair<long, string> responseKeyValue);


    [System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/IDataService/SetQueuedDRStatus", ReplyAction = "http://tempuri.org/IDataService/SetQueuedDRStatus")]
    string SetQueuedDRStatus(string queuedDRId, string status);

    [System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/IDataService/RemoveQueueFromCache", ReplyAction = "http://tempuri.org/IDataService/RemoveQueueFromCache")]
    void RemoveQueueFromCache(long queueId);

    [System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/IDataService/GetBot", ReplyAction = "http://tempuri.org/IDataService/GetBot")]
    RGBot GetBot(string channelName);

    [System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/IDataService/GetChannelProxyList", ReplyAction = "http://tempuri.org/IDataService/GetChannelProxyList")]
    List<RGProxy> GetChannelProxyList(string channelName);

    [System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/IDataService/GetBotManagerConfiguration", ReplyAction = "http://tempuri.org/IDataService/GetBotManagerConfiguration")]
    BotServerConfig GetBotManagerConfiguration(RG.Core.Entities.BOTManager botManager);
}

[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
public interface IDataServiceChannel : IDataService, System.ServiceModel.IClientChannel
{
}

[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
public partial class DataServiceClient : System.ServiceModel.ClientBase<IDataService>, IDataService
{

    public DataServiceClient()
    {
    }

    public DataServiceClient(string endpointConfigurationName) :
        base(endpointConfigurationName)
    {
    }

    public DataServiceClient(string endpointConfigurationName, string remoteAddress) :
        base(endpointConfigurationName, remoteAddress)
    {
    }

    public DataServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) :
        base(endpointConfigurationName, remoteAddress)
    {
    }

    public DataServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) :
        base(binding, remoteAddress)
    {
    }

    public string Echo(string input)
    {
        return base.Channel.Echo(input);
    }



    public string SubmitBOTResponse(RG.Core.Entities.BOTManager botManager, System.Collections.Generic.KeyValuePair<long, string> responseKeyValue)
    {
        return base.Channel.SubmitBOTResponse(botManager, responseKeyValue);
    }

    public string SetQueuedDRStatus(string queuedDRId, string status)
    {
        return base.Channel.SetQueuedDRStatus(queuedDRId, status);
    }


    public void RemoveQueueFromCache(long queueId)
    {
        base.Channel.RemoveQueueFromCache(queueId);
    }

    public RGBot GetBot(string channelName)
    {
        return base.Channel.GetBot(channelName);
    }

    public List<RGProxy> GetChannelProxyList(string source)
    {
        return base.Channel.GetChannelProxyList(source);
    }

    public BotServerConfig GetBotManagerConfiguration(RG.Core.Entities.BOTManager botManager)
    {
        return base.Channel.GetBotManagerConfiguration(botManager);
    }
}
