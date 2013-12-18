VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "clsSettings"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Attribute VB_Ext_KEY = "SavedWithClassBuilder6" ,"Yes"
Attribute VB_Ext_KEY = "Top_Level" ,"Yes"
'local variable(s) to hold property value(s)

Private mvarClientId As String  'local copy
Private mvarScope As String 'local copy
Private mvarResourceAppRestUrl As String 'local copy
Private mvarConnectionTimeoutSeconds As Integer 'local copy
Private mvarUserAgent As String 'local copy
Private mvarConnectionGroupName As String 'local copy
Private mvarStartAuthorisationAttemptUri As String 'local copy
Private mvarStartAuthorisationAttemptUri_WithCredential As String 'local copy
Private mvarRedirectUri As Variant 'local copy
Private mvarStartAuthorisationResponseType As String 'local copy
Private mvarGetClientCredentialUri As Variant 'local copy
Private mvarGetClientCredentialFormat As Variant 'local copy
Private mvarGetClientCredentialPostDataFormatter As Variant 'local copy
Private mvarUseScreenShot As Boolean 'local copy
Private mvarPfxPasswordLength As Integer 'local copy
Private mvarGetAccessTokenPostDataFormatter As String 'local copy
Private mvarGetAccessTokenPostGrantType As String 'local copy
Private mvarGetAccessTokenUri As String 'local copy
Private mvarRefreshAccessTokenPostDataFormatter As String 'local copy
Private mvarRefreshAccessTokenPostGrantType As String 'local copy
Private iniPath As String

Const ClientIdKey = "ClientId"
Const ScopeKey = "Scope"
Const ResourceAppRestUrlKey = "ResourceAppRestUrl"
Const ConnectionTimeoutSecondsKey = "ConnectionTimeoutSeconds"
Const UserAgentKey = "UserAgent"
Const ConnectionGroupNameKey = "ConnectionGroupName"
Const StartAuthorisationAttemptUriKey = "StartAuthorisationAttemptUri"
Const StartAuthorisationAttemptUri_WithCredentialKey = "StartAuthorisationAttemptUri_WithCredential"
Const RedirectUriKey = "RedirectUri"
Const StartAuthorisationResponseTypeKey = "StartAuthorisationResponseType"
Const GetClientCredentialUriKey = "GetClientCredentialUri"
Const GetClientCredentialFormatKey = "GetClientCredentialFormat"
Const GetClientCredentialPostDataFormatterKey = "GetClientCredentialPostDataFormatter"
Const UseScreenShotKey = "UseScreenShot"
Const PfxPasswordLengthKey = "PfxPasswordLength"
Const GetAccessTokenPostDataFormatterKey = "GetAccessTokenPostDataFormatter"
Const GetAccessTokenPostGrantTypeKey = "GetAccessTokenPostGrantType"
Const GetAccessTokenUriKey = "GetAccessTokenUri"
Const RefreshAccessTokenPostDataFormatterKey = "RefreshAccessTokenPostDataFormatter"
Const RefreshAccessTokenPostGrantTypeKey = "RefreshAccessTokenPostGrantType"
Const DefaultSectionName = "Default"

Public Sub Initialise()
iniPath = App.Path & "\" & App.EXEName & ".ini"

If Dir(iniPath) <> "" Then
    ClientID = IniModule.ReadIni(iniPath, DefaultSectionName, ClientIdKey)
    Scope = IniModule.ReadIni(iniPath, DefaultSectionName, ScopeKey)
    ResourceAppRestUrl = IniModule.ReadIni(iniPath, DefaultSectionName, ResourceAppRestUrlKey)
    ConnectionTimeoutSeconds = CInt(IniModule.ReadIni(iniPath, DefaultSectionName, ConnectionTimeoutSecondsKey))
    UserAgent = IniModule.ReadIni(iniPath, DefaultSectionName, UserAgentKey)
    ConnectionGroupName = IniModule.ReadIni(iniPath, DefaultSectionName, ConnectionGroupNameKey)
    StartAuthorisationAttemptUri = IniModule.ReadIni(iniPath, DefaultSectionName, StartAuthorisationAttemptUriKey)
    StartAuthorisationAttemptUri_WithCredential = IniModule.ReadIni(iniPath, DefaultSectionName, StartAuthorisationAttemptUri_WithCredentialKey)
    RedirectUri = IniModule.ReadIni(iniPath, DefaultSectionName, RedirectUriKey)
    StartAuthorisationResponseType = IniModule.ReadIni(iniPath, DefaultSectionName, StartAuthorisationResponseTypeKey)
    GetClientCredentialUri = IniModule.ReadIni(iniPath, DefaultSectionName, GetClientCredentialUriKey)
    GetClientCredentialFormat = IniModule.ReadIni(iniPath, DefaultSectionName, GetClientCredentialFormatKey)
    GetClientCredentialPostDataFormatter = IniModule.ReadIni(iniPath, DefaultSectionName, GetClientCredentialPostDataFormatterKey)
    UseScreenShot = IniModule.ReadIni(iniPath, DefaultSectionName, UseScreenShotKey)
    PfxPasswordLength = CInt(IniModule.ReadIni(iniPath, DefaultSectionName, PfxPasswordLengthKey))
    GetAccessTokenPostDataFormatter = IniModule.ReadIni(iniPath, DefaultSectionName, GetAccessTokenPostDataFormatterKey)
    GetAccessTokenPostGrantType = IniModule.ReadIni(iniPath, DefaultSectionName, GetAccessTokenPostGrantTypeKey)
    GetAccessTokenUri = IniModule.ReadIni(iniPath, DefaultSectionName, GetAccessTokenUriKey)
    RefreshAccessTokenPostDataFormatter = IniModule.ReadIni(iniPath, DefaultSectionName, RefreshAccessTokenPostDataFormatterKey)
    RefreshAccessTokenPostGrantType = IniModule.ReadIni(iniPath, DefaultSectionName, RefreshAccessTokenPostGrantTypeKey)
Else
    Err.Raise 12345, "Settings Not Found", "Cannot find the settings ini file."
End If

End Sub

Public Property Let RefreshAccessTokenPostGrantType(ByVal vData As String)
'used when assigning a value to the property, on the left side of an assignment.
'Syntax: X.RefreshAccessTokenPostGrantType = 5
    mvarRefreshAccessTokenPostGrantType = vData
End Property


Public Property Get RefreshAccessTokenPostGrantType() As String
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.RefreshAccessTokenPostGrantType
    RefreshAccessTokenPostGrantType = mvarRefreshAccessTokenPostGrantType
End Property



Public Property Let RefreshAccessTokenPostDataFormatter(ByVal vData As String)
'used when assigning a value to the property, on the left side of an assignment.
'Syntax: X.RefreshAccessTokenPostDataFormatter = 5
    mvarRefreshAccessTokenPostDataFormatter = vData
End Property


Public Property Get RefreshAccessTokenPostDataFormatter() As String
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.RefreshAccessTokenPostDataFormatter
    RefreshAccessTokenPostDataFormatter = mvarRefreshAccessTokenPostDataFormatter
End Property



Public Property Let GetAccessTokenUri(ByVal vData As String)
'used when assigning a value to the property, on the left side of an assignment.
'Syntax: X.GetAccessTokenUri = 5
    mvarGetAccessTokenUri = vData
End Property


Public Property Get GetAccessTokenUri() As String
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.GetAccessTokenUri
    GetAccessTokenUri = mvarGetAccessTokenUri
End Property



Public Property Let GetAccessTokenPostGrantType(ByVal vData As String)
'used when assigning a value to the property, on the left side of an assignment.
'Syntax: X.GetAccessTokenPostGrantType = 5
    mvarGetAccessTokenPostGrantType = vData
End Property


Public Property Get GetAccessTokenPostGrantType() As String
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.GetAccessTokenPostGrantType
    GetAccessTokenPostGrantType = mvarGetAccessTokenPostGrantType
End Property



Public Property Let GetAccessTokenPostDataFormatter(ByVal vData As String)
'used when assigning a value to the property, on the left side of an assignment.
'Syntax: X.GetAccessTokenPostDataFormatter = 5
    mvarGetAccessTokenPostDataFormatter = vData
End Property


Public Property Get GetAccessTokenPostDataFormatter() As String
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.GetAccessTokenPostDataFormatter
    GetAccessTokenPostDataFormatter = mvarGetAccessTokenPostDataFormatter
End Property



Public Property Let PfxPasswordLength(ByVal vData As Integer)
'used when assigning a value to the property, on the left side of an assignment.
'Syntax: X.PfxPasswordLength = 5
    mvarPfxPasswordLength = vData
End Property


Public Property Get PfxPasswordLength() As Integer
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.PfxPasswordLength
    PfxPasswordLength = mvarPfxPasswordLength
End Property



Public Property Let UseScreenShot(ByVal vData As Boolean)
'used when assigning a value to the property, on the left side of an assignment.
'Syntax: X.UseScreenShot = 5
    mvarUseScreenShot = vData
End Property


Public Property Get UseScreenShot() As Boolean
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.UseScreenShot
    UseScreenShot = mvarUseScreenShot
End Property



Public Property Let GetClientCredentialPostDataFormatter(ByVal vData As Variant)
'used when assigning a value to the property, on the left side of an assignment.
'Syntax: X.GetClientCredentialPostDataFormatter = 5
    mvarGetClientCredentialPostDataFormatter = vData
End Property


Public Property Set GetClientCredentialPostDataFormatter(ByVal vData As Variant)
'used when assigning an Object to the property, on the left side of a Set statement.
'Syntax: Set x.GetClientCredentialPostDataFormatter = Form1
    Set mvarGetClientCredentialPostDataFormatter = vData
End Property


Public Property Get GetClientCredentialPostDataFormatter() As Variant
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.GetClientCredentialPostDataFormatter
    If IsObject(mvarGetClientCredentialPostDataFormatter) Then
        Set GetClientCredentialPostDataFormatter = mvarGetClientCredentialPostDataFormatter
    Else
        GetClientCredentialPostDataFormatter = mvarGetClientCredentialPostDataFormatter
    End If
End Property



Public Property Let GetClientCredentialFormat(ByVal vData As Variant)
'used when assigning a value to the property, on the left side of an assignment.
'Syntax: X.GetClientCredentialFormat = 5
    mvarGetClientCredentialFormat = vData
End Property


Public Property Set GetClientCredentialFormat(ByVal vData As Variant)
'used when assigning an Object to the property, on the left side of a Set statement.
'Syntax: Set x.GetClientCredentialFormat = Form1
    Set mvarGetClientCredentialFormat = vData
End Property


Public Property Get GetClientCredentialFormat() As Variant
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.GetClientCredentialFormat
    If IsObject(mvarGetClientCredentialFormat) Then
        Set GetClientCredentialFormat = mvarGetClientCredentialFormat
    Else
        GetClientCredentialFormat = mvarGetClientCredentialFormat
    End If
End Property



Public Property Let GetClientCredentialUri(ByVal vData As Variant)
'used when assigning a value to the property, on the left side of an assignment.
'Syntax: X.GetClientCredentialUri = 5
    mvarGetClientCredentialUri = vData
End Property


Public Property Set GetClientCredentialUri(ByVal vData As Variant)
'used when assigning an Object to the property, on the left side of a Set statement.
'Syntax: Set x.GetClientCredentialUri = Form1
    Set mvarGetClientCredentialUri = vData
End Property


Public Property Get GetClientCredentialUri() As Variant
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.GetClientCredentialUri
    If IsObject(mvarGetClientCredentialUri) Then
        Set GetClientCredentialUri = mvarGetClientCredentialUri
    Else
        GetClientCredentialUri = mvarGetClientCredentialUri
    End If
End Property



Public Property Let StartAuthorisationResponseType(ByVal vData As String)
'used when assigning a value to the property, on the left side of an assignment.
'Syntax: X.StartAuthorisationResponseType = 5
    mvarStartAuthorisationResponseType = vData
End Property


Public Property Get StartAuthorisationResponseType() As String
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.StartAuthorisationResponseType
    StartAuthorisationResponseType = mvarStartAuthorisationResponseType
End Property



Public Property Let RedirectUri(ByVal vData As Variant)
'used when assigning a value to the property, on the left side of an assignment.
'Syntax: X.RedirectUri = 5
    mvarRedirectUri = vData
End Property


Public Property Set RedirectUri(ByVal vData As Variant)
'used when assigning an Object to the property, on the left side of a Set statement.
'Syntax: Set x.RedirectUri = Form1
    Set mvarRedirectUri = vData
End Property


Public Property Get RedirectUri() As Variant
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.RedirectUri
    If IsObject(mvarRedirectUri) Then
        Set RedirectUri = mvarRedirectUri
    Else
        RedirectUri = mvarRedirectUri
    End If
End Property



Public Property Let StartAuthorisationAttemptUri_WithCredential(ByVal vData As String)
'used when assigning a value to the property, on the left side of an assignment.
'Syntax: X.StartAuthorisationAttemptUri_WithCredential = 5
    mvarStartAuthorisationAttemptUri_WithCredential = vData
End Property


Public Property Get StartAuthorisationAttemptUri_WithCredential() As String
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.StartAuthorisationAttemptUri_WithCredential
    StartAuthorisationAttemptUri_WithCredential = mvarStartAuthorisationAttemptUri_WithCredential
End Property



Public Property Let StartAuthorisationAttemptUri(ByVal vData As String)
'used when assigning a value to the property, on the left side of an assignment.
'Syntax: X.StartAuthorisationAttemptUri = 5
    mvarStartAuthorisationAttemptUri = vData
End Property


Public Property Get StartAuthorisationAttemptUri() As String
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.StartAuthorisationAttemptUri
    StartAuthorisationAttemptUri = mvarStartAuthorisationAttemptUri
End Property



Public Property Let ConnectionGroupName(ByVal vData As String)
'used when assigning a value to the property, on the left side of an assignment.
'Syntax: X.ConnectionGroupName = 5
    mvarConnectionGroupName = vData
End Property


Public Property Get ConnectionGroupName() As String
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.ConnectionGroupName
    ConnectionGroupName = mvarConnectionGroupName
End Property



Public Property Let UserAgent(ByVal vData As String)
'used when assigning a value to the property, on the left side of an assignment.
'Syntax: X.UserAgent = 5
    mvarUserAgent = vData
End Property


Public Property Get UserAgent() As String
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.UserAgent
    UserAgent = mvarUserAgent
End Property



Public Property Let ConnectionTimeoutSeconds(ByVal vData As Integer)
'used when assigning a value to the property, on the left side of an assignment.
'Syntax: X.ConnectionTimeoutSeconds = 5
    mvarConnectionTimeoutSeconds = vData
End Property


Public Property Get ConnectionTimeoutSeconds() As Integer
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.ConnectionTimeoutSeconds
    ConnectionTimeoutSeconds = mvarConnectionTimeoutSeconds
End Property



Public Property Let ResourceAppRestUrl(ByVal vData As String)
'used when assigning a value to the property, on the left side of an assignment.
'Syntax: X.ResourceAppRestUrl = 5
    mvarResourceAppRestUrl = vData
End Property


Public Property Get ResourceAppRestUrl() As String
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.ResourceAppRestUrl
    ResourceAppRestUrl = mvarResourceAppRestUrl
End Property



Public Property Let Scope(ByVal vData As String)
'used when assigning a value to the property, on the left side of an assignment.
'Syntax: X.Scop = 5
    mvarScope = vData
End Property


Public Property Get Scope() As String
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.Scop
    Scope = mvarScope
End Property



Public Property Let ClientID(ByVal vData As String)
'used when assigning a value to the property, on the left side of an assignment.
'Syntax: X.ClientId = 5
    mvarClientId = vData
End Property


Public Property Get ClientID() As String
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.ClientId
    ClientID = mvarClientId
End Property



