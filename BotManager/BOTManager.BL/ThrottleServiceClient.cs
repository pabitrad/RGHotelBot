﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.276
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.Collections.Generic;
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ServiceModel.ServiceContractAttribute(ConfigurationName = "IThrottleService")]
public interface IThrottleService
{

    [System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/IThrottleService/GetBOTRequests", ReplyAction = "http://tempuri.org/IThrottleService/GetBOTRequestsResponse")]
    System.Collections.Generic.Dictionary<string, KeyValuePair<string,string>> GetBOTRequests(RG.Core.Entities.BOTManager botManager);
    
}

[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
public interface IThrottleServiceChannel : IThrottleService, System.ServiceModel.IClientChannel
{
}

[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
public partial class ThrottleServiceClient : System.ServiceModel.ClientBase<IThrottleService>, IThrottleService
{

    public ThrottleServiceClient()
    {
    }

    public ThrottleServiceClient(string endpointConfigurationName) :
        base(endpointConfigurationName)
    {
    }

    public ThrottleServiceClient(string endpointConfigurationName, string remoteAddress) :
        base(endpointConfigurationName, remoteAddress)
    {
    }

    public ThrottleServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) :
        base(endpointConfigurationName, remoteAddress)
    {
    }

    public ThrottleServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) :
        base(binding, remoteAddress)
    {
    }

    public System.Collections.Generic.Dictionary<string, KeyValuePair<string,string>> GetBOTRequests(RG.Core.Entities.BOTManager botManager)
    {
        return base.Channel.GetBOTRequests(botManager);
    }

   
}
