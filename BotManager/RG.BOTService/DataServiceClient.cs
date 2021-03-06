﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.276
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------



[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ServiceModel.ServiceContractAttribute(ConfigurationName="IDataService")]
public interface IDataService
{
    
    [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IDataService/Echo", ReplyAction="http://tempuri.org/IDataService/EchoResponse")]
    string Echo(string input);
    
    [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IDataService/SubmitBOTResponse", ReplyAction="http://tempuri.org/IDataService/SubmitBOTResponseResponse")]
    string SubmitBOTResponse([System.ServiceModel.MessageParameterAttribute(Name="submitBOTResponse")] string submitBOTResponse1);
    
    [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IDataService/SubmitReportRequest", ReplyAction="http://tempuri.org/IDataService/SubmitReportRequestResponse")]
    RG.Core.Entities.ReportRequest SubmitReportRequest(RG.Core.Entities.ReportRequest reportRequest);
    
    [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IDataService/GetBOTRequests", ReplyAction="http://tempuri.org/IDataService/GetBOTRequestsResponse")]
    RG.Core.Entities.RateAvailabilityRequest[] GetBOTRequests();
    
    [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IDataService/GetURIAddress", ReplyAction="http://tempuri.org/IDataService/GetURIAddressResponse")]
    string GetURIAddress();
    
    [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IDataService/SubmitRequestStatus", ReplyAction="http://tempuri.org/IDataService/SubmitRequestStatusResponse")]
    string SubmitRequestStatus(string requestId, string status);
    
    [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IDataService/ProcessRatesAndProperties", ReplyAction="http://tempuri.org/IDataService/ProcessRatesAndPropertiesResponse")]
    string ProcessRatesAndProperties();
    
    [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IDataService/GenerateReports", ReplyAction="http://tempuri.org/IDataService/GenerateReportsResponse")]
    string GenerateReports();
    
    [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IDataService/GenerateReport", ReplyAction="http://tempuri.org/IDataService/GenerateReportResponse")]
    string GenerateReport(long reportId);
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
    
    public string SubmitBOTResponse(string submitBOTResponse1)
    {
        return base.Channel.SubmitBOTResponse(submitBOTResponse1);
    }
    
    public RG.Core.Entities.ReportRequest SubmitReportRequest(RG.Core.Entities.ReportRequest reportRequest)
    {
        return base.Channel.SubmitReportRequest(reportRequest);
    }
    
    public RG.Core.Entities.RateAvailabilityRequest[] GetBOTRequests()
    {
        return base.Channel.GetBOTRequests();
    }
    
    public string GetURIAddress()
    {
        return base.Channel.GetURIAddress();
    }
    
    public string SubmitRequestStatus(string requestId, string status)
    {
        return base.Channel.SubmitRequestStatus(requestId, status);
    }
    
    public string ProcessRatesAndProperties()
    {
        return base.Channel.ProcessRatesAndProperties();
    }
    
    public string GenerateReports()
    {
        return base.Channel.GenerateReports();
    }
    
    public string GenerateReport(long reportId)
    {
        return base.Channel.GenerateReport(reportId);
    }
}
