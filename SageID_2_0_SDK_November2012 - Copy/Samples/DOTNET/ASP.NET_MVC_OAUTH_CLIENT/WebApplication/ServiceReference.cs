﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.225
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace OAuthClientWebApp.ResourceAppSoap
{


    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName = "ResourceAppSoap.AuthenticatingServiceSoap")]
    public interface AuthenticatingServiceSoap
    {

        // CODEGEN: Generating message contract since element name HelloWorldResult from namespace http://tempuri.org/ is not marked nillable
        [System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/HelloWorld", ReplyAction = "*")]
        OAuthClientWebApp.ResourceAppSoap.HelloWorldResponse HelloWorld(OAuthClientWebApp.ResourceAppSoap.HelloWorldRequest request);
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
    public partial class HelloWorldRequest
    {

        [System.ServiceModel.MessageBodyMemberAttribute(Name = "HelloWorld", Namespace = "http://tempuri.org/", Order = 0)]
        public OAuthClientWebApp.ResourceAppSoap.HelloWorldRequestBody Body;

        public HelloWorldRequest()
        {
        }

        public HelloWorldRequest(OAuthClientWebApp.ResourceAppSoap.HelloWorldRequestBody Body)
        {
            this.Body = Body;
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute()]
    public partial class HelloWorldRequestBody
    {

        public HelloWorldRequestBody()
        {
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
    public partial class HelloWorldResponse
    {

        [System.ServiceModel.MessageBodyMemberAttribute(Name = "HelloWorldResponse", Namespace = "http://tempuri.org/", Order = 0)]
        public OAuthClientWebApp.ResourceAppSoap.HelloWorldResponseBody Body;

        public HelloWorldResponse()
        {
        }

        public HelloWorldResponse(OAuthClientWebApp.ResourceAppSoap.HelloWorldResponseBody Body)
        {
            this.Body = Body;
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace = "http://tempuri.org/")]
    public partial class HelloWorldResponseBody
    {

        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
        public string HelloWorldResult;

        public HelloWorldResponseBody()
        {
        }

        public HelloWorldResponseBody(string HelloWorldResult)
        {
            this.HelloWorldResult = HelloWorldResult;
        }
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface AuthenticatingServiceSoapChannel : OAuthClientWebApp.ResourceAppSoap.AuthenticatingServiceSoap, System.ServiceModel.IClientChannel
    {
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class AuthenticatingServiceSoapClient : System.ServiceModel.ClientBase<OAuthClientWebApp.ResourceAppSoap.AuthenticatingServiceSoap>, OAuthClientWebApp.ResourceAppSoap.AuthenticatingServiceSoap
    {

        public AuthenticatingServiceSoapClient()
        {
        }

        public AuthenticatingServiceSoapClient(string endpointConfigurationName) :
            base(endpointConfigurationName)
        {
        }

        public AuthenticatingServiceSoapClient(string endpointConfigurationName, string remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        public AuthenticatingServiceSoapClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        public AuthenticatingServiceSoapClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) :
            base(binding, remoteAddress)
        {
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        OAuthClientWebApp.ResourceAppSoap.HelloWorldResponse OAuthClientWebApp.ResourceAppSoap.AuthenticatingServiceSoap.HelloWorld(OAuthClientWebApp.ResourceAppSoap.HelloWorldRequest request)
        {
            return base.Channel.HelloWorld(request);
        }

        public string HelloWorld()
        {
            OAuthClientWebApp.ResourceAppSoap.HelloWorldRequest inValue = new OAuthClientWebApp.ResourceAppSoap.HelloWorldRequest();
            inValue.Body = new OAuthClientWebApp.ResourceAppSoap.HelloWorldRequestBody();
            OAuthClientWebApp.ResourceAppSoap.HelloWorldResponse retVal = ((OAuthClientWebApp.ResourceAppSoap.AuthenticatingServiceSoap)(this)).HelloWorld(inValue);
            return retVal.Body.HelloWorldResult;
        }
    }
}
