A note about certificates
--------------------------------------------------------------------------------
The Azure SDK sample application requires two certificates be uploaded to Azure
for the application to function and identities be exchanged  correctly.  
These two certificates are
	- your applications X509 identity certificate (CERT_A)
	- the pre-production identity root cerificate that has been stripped of 
	  private key information (CERT_B)
	
CERT_A is the identity that the sample application will use to identify itself 
to the SSO services.

CERT_B is the identity that the SSO services will use to send notifications to 
the sample app.	This certificate can be found in the sample application project 
root folder with the name SSOIdentityRoot_nokey.pfx, it has the password 
"SSOIdentityRoot" (without the quotes).

There is some configuration that needs to be performed to enable the sample 
application to be able to read the certificate when it has been uploaded to 
Azure.  There are three places where these changes need to be made:
	- WebApplication.Azure\ServiceDefinition.csdef
	- WebApplication.Azure\ServiceConfiguration.Cloud.cscfg
	- WebApplication.Azure\ServiceConfiguration.Local.cscfg
	
The ServiceDefinition.csdef file should be modified so that it's Certificates 
section looks like this

    <Certificates>
	  
	  <!-- This is CERT_A -->
      <Certificate 
		name="YOUR X509 IDENTITY NAME HERE" 
		storeLocation="LocalMachine" 
		storeName="My"/>
		
	  <!-- THIS IS CERT_B -->
      <Certificate 
		name="Sage SSO Identity Root (Pre-Production)" 
		storeLocation="LocalMachine" 
		storeName="My"/>
		
    </Certificates>

Both of the ServiceConfiguration.*.cscfg files should be modified so that the 
Certificates section looks like this

	<Certificates>
		
	  <!-- This is CERT_A -->
      <Certificate 
		name="YOUR X509 IDENTITY NAME HERE" 
		thumbprint="YOUR X509 IDENTITY THUMBPRINT HERE" 
		thumbprintAlgorithm="sha1" />
		
      <!-- This is CERT_B -->
	  <Certificate 
		name="Sage SSO Identity Root (Pre-Production)" 
		thumbprint="C3294FE0E12991374CA96DA73D7F578A42D78C24" 
		thumbprintAlgorithm="sha1" />
		
    </Certificates>
	
Certificates uploaded to Azure can only be places in the "LocalMachine" store 
location and the "My" store.  Additionally, Azure only supports the "sha1" 
thumprint algorithm for certificates.

There is one more point to note about deploying the sample application.When 
querying for the "Sage SSO Identity Root (Pre-production)" certificate you
must perform the search in such a way that invalid certificates are also 
searched.  This is due to the fact the the identity root certificate is 
essentially self signed and Azure will see it as invalid.  This behaviour can be
seen by comparing the property 

WebApplication.Controllers.SSOController.SSORootCertificate

between both the MVC3 (Azure and non-Azure) sample apps.  The only difference 
between these implementations of this property is that the Azure sample does 
this

_ssoRootCertificate = certStore.Certificates.Find(findType, findValue, false)[0];

whereas the normal MVC3 app does this

_ssoRootCertificate = certStore.Certificates.Find(findType, findValue, true)[0];



	
	