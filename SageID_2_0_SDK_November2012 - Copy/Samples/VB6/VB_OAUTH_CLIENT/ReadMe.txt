Sage ID - Software Development Kit - VB6 Sample
===============================================

To use this VB sample you need to register the Sage.Authorization Sage.Authorization.Client and Sage.Authorization.Server assembly for use from COM.

To do this run 'regasm'.

http://msdn.microsoft.com/en-us/library/tzat5yw6(v=vs.71).aspx

Open a command prompt and CD to the Libs directory at the SDK\Samples.DOTNET\Libs

Then run the following commands.

regasm Sage.Authorisation.dll /tlb:Sage.Authorization.tlb
regasm Sage.Authorisation.Client.dll /tlb:Sage.Authorization.Client.tlb
regasm Sage.Authorisation.Server.dll /tlb:Sage.Authorization.Server.tlb

The the VB project doesn't automatically, re-add the references to Sage ID OAuth Core, Sage ID OAuth Client and Sage ID OAuth Server in Microsoft Visual Basic.