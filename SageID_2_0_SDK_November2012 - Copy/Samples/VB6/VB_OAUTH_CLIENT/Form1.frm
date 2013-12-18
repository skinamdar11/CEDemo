VERSION 5.00
Begin VB.Form frmMain 
   Caption         =   "Sage ID OAuth COM SDK Sample"
   ClientHeight    =   6135
   ClientLeft      =   120
   ClientTop       =   450
   ClientWidth     =   13245
   BeginProperty Font 
      Name            =   "Tahoma"
      Size            =   8.25
      Charset         =   0
      Weight          =   400
      Underline       =   0   'False
      Italic          =   0   'False
      Strikethrough   =   0   'False
   EndProperty
   LinkTopic       =   "Form1"
   ScaleHeight     =   6135
   ScaleWidth      =   13245
   StartUpPosition =   3  'Windows Default
   Begin VB.CommandButton btnSvrStart 
      Caption         =   "Server Start Authorization Attempt"
      Height          =   615
      Left            =   120
      TabIndex        =   8
      Top             =   1680
      Width           =   4335
   End
   Begin VB.CommandButton btnSvrGetTokens 
      Caption         =   "Server Get Tokens"
      Enabled         =   0   'False
      Height          =   615
      Left            =   120
      TabIndex        =   7
      Top             =   3840
      Width           =   4335
   End
   Begin VB.CommandButton btnClientSignInAsync 
      Caption         =   "Client Authenticate Async"
      Enabled         =   0   'False
      Height          =   615
      Left            =   120
      TabIndex        =   6
      Top             =   3120
      Width           =   4335
   End
   Begin VB.CommandButton btnClientSignIn 
      Caption         =   "Client - Authenticate"
      Enabled         =   0   'False
      Height          =   615
      Left            =   120
      TabIndex        =   5
      Top             =   2400
      Width           =   4335
   End
   Begin VB.TextBox txtLog 
      Height          =   5895
      Left            =   4560
      MultiLine       =   -1  'True
      ScrollBars      =   2  'Vertical
      TabIndex        =   4
      Top             =   120
      Width           =   8535
   End
   Begin VB.CommandButton btnStartAttemptAsync 
      Caption         =   "Start Authorisation Attempt Async"
      Height          =   615
      Left            =   120
      TabIndex        =   3
      Top             =   840
      Width           =   4335
   End
   Begin VB.CommandButton btnCleanup 
      Caption         =   "Cleanup"
      Height          =   615
      Left            =   120
      TabIndex        =   2
      Top             =   5400
      Width           =   4335
   End
   Begin VB.CommandButton btnUseToken 
      Caption         =   "Use Token"
      Enabled         =   0   'False
      Height          =   615
      Left            =   120
      TabIndex        =   1
      Top             =   4680
      Width           =   4335
   End
   Begin VB.CommandButton btnStartAuthAttempt 
      Caption         =   "Start Authorisation Attempt"
      Default         =   -1  'True
      Height          =   615
      Left            =   120
      TabIndex        =   0
      Top             =   120
      Width           =   4335
   End
   Begin VB.Line Line2 
      X1              =   120
      X2              =   4440
      Y1              =   4560
      Y2              =   4560
   End
   Begin VB.Line Line1 
      X1              =   120
      X2              =   4440
      Y1              =   1560
      Y2              =   1560
   End
End
Attribute VB_Name = "frmMain"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
Option Explicit

Private WithEvents mClient As OAuthClient
Attribute mClient.VB_VarHelpID = -1
Private WithEvents mThinClient As OAuthClientThin
Attribute mThinClient.VB_VarHelpID = -1

Private mServer As OAuthClientService

Private mResult As AuthorisationResult
Private mThinResult As AuthorisationResultThin

Private mAccessToken As String
Private mAuthorizeUri As String

Private mAuthorizationCode As String
Private mRefreshCredential As Boolean


Private mServerResult As ServerAuthorisationResult
Private mSettings As clsSettings

Private mServerInProgress As Boolean

'
' Starts a client-server authorisation
Private Sub btnSvrStart_Click()
On Error GoTo Error
    
    ' in real life this would be a per user setting, we don't want a user to have more than one attempt in progress
    If mServerInProgress = True Then
        MsgBox "You already have an authorization in progress, finish it or cleanup", vbCritical, "Authorization In Progress"
        GoTo Finally
    End If
    
    Dim authInfo As New AuthorisationInfo
    Dim proxy As New WebProxySettings
    Dim arrLogEvents() As LogEvent
    Dim i As Long
    
    Set mServer = GetOAuthService(App.Path, "UserId")
    authInfo.Scope = mSettings.Scope
    
    proxy.Address = "127.0.0.1:888"
    proxy.UseProxy = True
    proxy.BypassProxyOnLocal = False
        
    ' Call authorise synchronously to get the authorisation result and token
    Set mServerResult = mServer.StartAuthorisation(authInfo)

    mServerInProgress = True
    
    ' it is out of the scope of this demo to specify how the AuthorisationUri is sent to the client
    If mServerResult.UserAuthorisationRequired Then
        MsgBox "User should authorise", vbOKOnly, "Result"
        mAuthorizeUri = mServerResult.AuthorisationUri
        btnSvrStart.Enabled = False
        btnClientSignIn.Enabled = True
        btnClientSignInAsync.Enabled = True
        
    Else
        MsgBox "Access token retrievied.", vbOKOnly, "Result"
        mAccessToken = mServerResult.AccessToken
    End If
    
Finally:
    Set authInfo = Nothing
    
    Exit Sub
Error:
    MsgBox Err.Description, vbCritical, "Error Occurred"
    Resume Finally
End Sub

'
' Performs client portion of client-server authorisation
Private Sub btnClientSignIn_Click()
    On Error GoTo Error
    
    ' thin client doesn't require contructor parameters
    Set mThinClient = New OAuthClientThin
 
    ' authorise method takes the uri retrieved by the initial service portion of the protocol
    Set mThinResult = mThinClient.Authorise(mAuthorizeUri)
    
    If mThinResult.Success = False Then
        MsgBox "Error", vbCritical, mThinResult.Error
    Else
        mAuthorizationCode = mThinResult.AuthorisationCode
        mRefreshCredential = mThinResult.RefreshCredential
        btnSvrGetTokens.Enabled = False
        btnClientSignIn.Enabled = False
        btnClientSignInAsync.Enabled = False
        btnSvrGetTokens.Enabled = True
        
        MsgBox "Success, now get the access token", vbOKOnly, "Success"
    End If
Finally:
    Set mThinClient = Nothing
    Exit Sub
Error:
    MsgBox Err.Description, vbCritical, "Error Occurred"
    Resume Finally
End Sub

'
' Performs client portion of client-server authorisation asynchronusly
Private Sub btnClientSignInAsync_Click()
On Error GoTo Error
    
    Dim proxy As New WebProxySettings
    Dim strAsyncId As String
    
    Set mThinClient = New OAuthClientThin
        
    proxy.Address = "127.0.0.1:888"
    proxy.UseProxy = True
    proxy.BypassProxyOnLocal = False
        
    strAsyncId = mThinClient.BeginAuthorise(mAuthorizeUri)
        
Finally:
    Exit Sub
Error:
    MsgBox Err.Description, vbCritical, "Error Occurred"
    Resume Finally

End Sub

'
' Exchanges the access code retrieived by the client portion of the protocol for an access token
Private Sub btnSvrGetTokens_Click()
    On Error GoTo Error
    
    If mServerInProgress = False Then
        MsgBox "You don't have an authorization in progress", vbCritical, "No Authorization In Progress"
        GoTo Finally
    End If
    
    Set mServerResult = mServer.FinishAuthorisation(mAuthorizationCode, mRefreshCredential)
    
    If mServerResult.Success = True Then
        btnUseToken.Enabled = True
        mServerInProgress = False
        btnSvrGetTokens.Enabled = True
        btnClientSignIn.Enabled = False
        btnClientSignInAsync.Enabled = False
        btnSvrGetTokens.Enabled = False
    End If
           
Finally:
    Set mServer = Nothing
    
    Exit Sub
Error:
    MsgBox Err.Description, vbCritical, "Error Occurred"
    Resume Finally
End Sub

'
' Performs a "normal" authorisation
Private Sub btnStartAuthAttempt_Click()
    
On Error GoTo Error
    
    Dim authInfo As New AuthorisationInfo
    Dim proxy As New WebProxySettings
    Dim arrLogEvents() As LogEvent
    Dim i As Long
    
    Set mClient = GetOAuthClient(App.Path)
        
    authInfo.Scope = mSettings.Scope
    
    proxy.Address = "127.0.0.1:888"
    proxy.UseProxy = True
    proxy.BypassProxyOnLocal = False
        
    ' Call authorise synchronously to get the authorisation result and token
    Set mResult = mClient.Authorise(authInfo)
    mAccessToken = mResult.AccessToken
    
    btnUseToken.Enabled = True
    
Finally:

    ' Get the log events that were raised during the call
    arrLogEvents = mClient.GetLogEvents
    
    ' Print out the log events
    For i = 0 To UBound(arrLogEvents) - 1
        txtLog.Text = txtLog.Text & Time & ": " & arrLogEvents(i).Message & vbCrLf
    Next i
    
    txtLog.SelStart = Len(txtLog.Text)

    Set authInfo = Nothing
    Set mClient = Nothing
    
    Exit Sub
Error:
    MsgBox Err.Description, vbCritical, "Error Occurred"
    Resume Finally
End Sub

'
' Performs a "normal" authorization asynchronously
Private Sub btnStartAttemptAsync_Click()
On Error GoTo Error
    
    Dim authInfo As New AuthorisationInfo
    Dim proxy As New WebProxySettings
    Dim strAsyncId As String
    
    Set mClient = GetOAuthClient(App.Path)
        
    authInfo.Scope = mSettings.Scope
    
    proxy.Address = "127.0.0.1:888"
    proxy.UseProxy = True
    proxy.BypassProxyOnLocal = False
        
    strAsyncId = mClient.BeginAuthorise(authInfo)
        
Finally:
    Set authInfo = Nothing
    
    Exit Sub
Error:
    MsgBox Err.Description, vbCritical, "Error Occurred"
    Resume Finally

End Sub

'
' Cleanup certificates and tokens
Private Sub btnCleanup_Click()
    On Error Resume Next
    Set mClient = GetOAuthClient(App.Path)
    mClient.CleanUp
        
    If Not mServer Is Nothing Then
        mServerInProgress = False
        mServer.CleanUp
        Set mServer = Nothing
    End If
    
    btnUseToken.Enabled = False
    
    btnSvrStart.Enabled = True
    btnClientSignIn.Enabled = False
    btnClientSignInAsync.Enabled = False
    btnSvrGetTokens.Enabled = False
    
    Set mResult = Nothing
    Set mClient = Nothing
End Sub

Private Sub btnUseToken_Click()
    
    On Error GoTo Error
    
    If mAccessToken = "" Then
        MsgBox "Authorisation needs to be obtained first", vbInformation, "Perform Authorisation"
        Exit Sub
    End If

    Dim xhr As MSXML2.XMLHTTP30
    Set xhr = New MSXML2.XMLHTTP30
        
    xhr.open "POST", mSettings.ResourceAppRestUrl, False
    xhr.setRequestHeader "Authorization", "Bearer " & mAccessToken
    xhr.send

    If xhr.Status = 401 Then
        btnStartAuthAttempt_Click
        btnUseToken_Click
    Else
        MsgBox xhr.responseText
    End If
    
Finally:
    Set xhr = Nothing
    Exit Sub
Error:
    MsgBox Err.Description, vbCritical, "Error Occurred"
    Resume Finally
End Sub

Private Function GetOAuthClient(strAppData As String) As OAuthClient
    Set GetOAuthClient = New OAuthClient
    GetOAuthClient.ClientID = mSettings.ClientID
    GetOAuthClient.ConfigurationPath = strAppData
End Function

Private Function GetOAuthService(strAppData As String, strUserId As String) As OAuthClientService
    Set GetOAuthService = New OAuthClientService
    GetOAuthService.ClientID = mSettings.ClientID
    GetOAuthService.ConfigurationPath = strAppData
    GetOAuthService.UserId = strUserId
End Function

Private Function GetConfigurationSettings() As clsSettings
    Set GetConfigurationSettings = New clsSettings
    GetConfigurationSettings.Initialise
End Function

Private Sub mClient_AuthoriseCompleted(ByVal client As IOAuth, ByVal authoriseResult As AuthorisationResult)

    Set mResult = authoriseResult
    mAccessToken = authoriseResult.AccessToken
        
    If authoriseResult.Success = False Then
        If Not authoriseResult = "" Then
            MsgBox authoriseResult.Error, vbCritical, "Error Occurred"
        Else
            MsgBox "An error occured", vbCritical, "Error Occurred"
        End If
        Exit Sub
    End If
        
    btnUseToken.Enabled = True

End Sub

Private Sub mClient_LogEvent(ByVal e As Sage_Authorisation.ILogEvent)
    txtLog.Text = txtLog.Text & Time & ": " & e.Message & vbCrLf
    txtLog.SelStart = Len(txtLog.Text)
End Sub


Private Sub Form_Load()
    If App.PrevInstance Then
        Unload Me
    End If

    Set mSettings = GetConfigurationSettings()
    
End Sub

Private Sub mThinClient_LogEvent(ByVal e As Sage_Authorisation.ILogEvent)
    txtLog.Text = txtLog.Text & Time & ": " & e.Message & vbCrLf
    txtLog.SelStart = Len(txtLog.Text)
End Sub

Private Sub mThinClient_ThinAuthoriseCompleted(ByVal thinClient As Sage_Authorisation_Client.IOAuthThin, ByVal authoriseResult As Sage_Authorisation_Client.IAuthorisationResultThin)
    Set mThinResult = authoriseResult
     
    mAuthorizationCode = authoriseResult.AuthorisationCode
    mRefreshCredential = authoriseResult.RefreshCredential
        
    If authoriseResult.Success = False Then
        MsgBox authoriseResult.Error, vbCritical, "Error Occurred"
        Exit Sub
    End If
        
    btnUseToken.Enabled = True
End Sub
