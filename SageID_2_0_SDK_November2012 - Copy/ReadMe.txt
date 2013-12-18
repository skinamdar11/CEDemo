Sage ID - Software Development Kit - November 2012
==================================================

This software development kit supports development against the hosted Sage ID production and pre-production services.

========================================================================================
= For support with any aspect of Sage ID, please contact ssdpdevelopersupport@sage.com =
========================================================================================

New features in this release
============================

- Support for the OAuth 2.0 authorization code grant type and bearer tokens.

- A client library for desktop applications integrating with Sage ID's OAuth 2.0 support.

- SDK samples and preliminary documentation which cover the new features.

Information about this release
==============================

This release of Sage ID includes support for the following:

- Securing REST and SOAP web service APIs offered by web applications and services which integrate with Sage ID as an identity provider.

- Support for third-party web applications and Sage desktop applications making calls against those web services.

Native mobile device applications are not supported by this release.

Sage ID supports the Authorization Code Grant type (see: https://tools.ietf.org/html/rfc6749#section-4.1) with Bearer Tokens (see:http://tools.ietf.org/html/rfc6750). There is no support for the Implicit Grant, Resource Owner Password Credentials Grant or Client Credentials Grant types.

Using this release
==================

The latest SDK API documentation is available online at http://docs.sso.sagessdp.co.uk - sign in as user ssodocs with the password Q9VdcpfkWFbT. You may also download the latest version of the SDK from here.

The ASP.NET and ASP.NET_MVC samples (in the SDK\Samples folder) illustrate how a web application should authenticate and authorise calls being received from clients presenting bearer tokens. The ASP.NET sample demonstrates a SOAP endpoint and the ASP.NET_MVC sample demonstrates a web/REST endpoint. These samples are hosted online at http://webappa.sso.staging.services.sage.com and http://webappb.sso.staging.services.sage.com, respectively.

The ASP.NET_MVC_OAUTH_CLIENT sample is a simple web application which acts as a client for the two web applications above. It illustrates the steps a client web application needs to take in order to authenticate a Sage ID user and make a call to another application using a bearer token. This sample is hosted online at http://oauthclientwebapp.sso.sagessdp.com.

The WINFORMS_OAUTH_CLIENT sample is a simple .NET based desktop application which illustrates how to use the Sage ID desktop client library to authenticate a Sage ID user and make a call to a hosted application using a bearer token.

In order to use the OAuth 2.0 support in your own applications, you will need:

- A web application which is configured in Sage ID and the corresponding client certificate.
- A key which will allow your application to decrypt bearer tokens presented by clients.
- One or more permissions which your users will delegate to client applications.

If you're writing a desktop application which integrates with your hosted web application or service you'll need:

- A client ID which identifies your desktop application to Sage ID.

If you're writing a web application which integrates with your hosted web application or service you'll need:

- A client ID and secret key which your web application can use to authenticate with Sage ID.

Please contact ssdpdevelopersupport@sage.com to make your request.

Quick Start
===========

Alternatively, we've configured a "shared" resource application which you can use if you want to quickly try out these features on a development machine.

The client certificate can be found in SDK\Certificates\PreProduction\SampleWebApplication. You can use this certificate in one of the sample web applications or in your own code if you already have a web application. The private key password is SampleWebApplication.

The bearer token key information is as follows:

EncryptionKey: yJ/egyrSA2piEheNVfis3WEFjOen6IhO2ZUzRtRCxDY=
IV           : Xz6vuKV0oaWJv6PqK4p4OA==

This key is used to decrypt the tokens which are presented by client applications in web service calls.

There are ten test user account which are registered for this application: samplewebappuser0@mailinator.com - samplewebappuser9@mailinator.com. The password for these accounts is testpassword.

We've configured a shared web client for this resource application with the following credentials:

ClientId : bd515gezepkwcufy0flo5ypwevh2wqe
SecretKey: 0fvhzdvno1xsavaswdu4

You can use these with the sample web client in SDK\Samples\DOTNET\ASP.NET_MVC_OAUTH_CLIENT or in your own client web applications.

The OAuth 2.0 protocol requires a redirect back to the client web application with an authorization code (which is then used by the application to collect the tokens it needs to make calls on behalf of the user). In the shared client web application this redirect URI is fixed to:

http://oauthclientsample.sso.sagessdp.co.uk/home/authorise

The DNS name oauthclientsample.sso.sagessdp.co.uk resolves to 127.0.0.1. You may need to use host headers if you have more than one web application running on your development machine. If you want to redirect to another server, edit your HOSTS file to point to the other server and use proxy bypass settings as appropriate.

There is also a shared desktop client with the following ID:

ClientId: xvuhubv35lvqthf4jjob1ifk4ws3cxy

This can be used with the sample code in SDK\Samples\DOTNET\WINFORMS_OAUTH_CLIENT or in your own desktop client applications.

The scope handle for web and desktop clients is gwxav2b4(); . This scope handle results in a refresh token and an access token with a privilege "Authenticate" on the resource "/SSO/AlphaClient".
    
Contents of the SDK
===================

The SDK includes the following folders:

- "Certificates" contains the Sage ID root certificates which are required to trust SSO notifications, Sage ID authentication tokens and Sage ID X509 identity certificates.

  Certificates for production and pre-production are supplied in both .cer and .pem format.
  
  The root certificates which are used to generate the certificates which desktop applications use as a client credential are supplied for pre-production and production. This is required if your hosted application expects clients to present their client certificate (along with the bearer token) when making a call to your services.

- "Customisation Kit" contains:

  - The Sage ID web template viewer, which can be used to develop custom web pages in Sage ID for your application.

    The Sage ID web template viewer is a Visual Studio project which can be opened in the full version of Visual Studio 2010 or the free Visual Web Developer 2010 Express which can be downloaded here:

    http://www.microsoft.com/visualstudio/en-us/products/2010-editions/visual-web-developer-express
  
  - A set of basic email templates which can be used as the basis for creating customised versions for your application.

- "Doc" includes the Sage ID developer guide and supporting documentation.

- "OAuth Library" contains the desktop OAuth 2.0 client library build for .NET 2.0/3.0/3.5 and .NET 4.0. The reference documentation for the library can be accessed via a link from the online documentation above.

- "Samples" contains:

  - Web application samples which use ASP.NET, ASP.NET MVC 3 and PHP to illustrate how to integrate with Sage ID.

    These applications are equivalent as far as possible and running at least one of them is highly recommended to gain familiarity with Sage ID.

    In order to run the applications you will need a Sage ID web application certificate. To receive one, please contact ssdpdevelopersupport@sage.com or use the "shared" one described in the Quick Start section.
    
    The ASP.NET MVC sample requires ASP.NET MVC 3 which can be downloaded here:

    http://www.asp.net/mvc/mvc3

  - An ASP.NET MVC 3 sample which targets the Microsoft Azure platform. This sample includes a file "CERT-INFO-README.txt" which contains important information on using Sage ID certificates with Microsoft Azure. Please read this information before running the sample.

  - A simple ASP.NET MVC3 web application which acts as an OAuth 2 client. 
  
  - A simple .NET Windows Forms application which illustrates the use of the OAuth 2.0 client library.
  
  - A simple VB6 Application which illustrates the use of the OAuth 2.0 client library from COM.
  
  - A simple .NET application which targets the REST interface of Sage ID.
  
- "Schema" contains annotated XSD schema files for the major Sage ID entity types used in the SOAP and REST interfaces.

- "WSDL" contains SOAP WSDL which targets the production and pre-production Sage ID services.

  The only difference between the WSDL files is in the soap:address elements which specify the web address of the each web service. The pre-production WSDL targets pre-production Sage ID, for example:

  <soap:address location="https://services.sso.staging.services.sage.com/sso/IdentityManagementService"/>

  The production WSDL targets production Sage ID:

  <soap:address location="https://services.sso.services.sage.com/sso/IdentityManagementService"/>

  Most SOAP frameworks allow these addresses to be overridden. If you are using SOAP and your framework allows this then you may use either set of WSDL in both your pre-production and production applications. PHP, however, does not support this and you should use the appropriate WSDL for the service which you are targeting.
  
Note: The .NET sample projects can be opened in the full version of Visual Studio 2010 or the free Visual C# 2010 Express which can be downloaded here:
    http://www.microsoft.com/visualstudio/en-us/products/2010-editions/visual-csharp-express