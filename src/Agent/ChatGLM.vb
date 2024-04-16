
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp.Runtime.Interop
Imports TalkGenerator.ChatGLM

<Package("chatglm")>
<RTypeExport("chat_history", GetType(ChatHistory))>
Public Module ChatGLM

    ''' <summary>
    ''' add a new record of chat input and ai response for create the history data
    ''' </summary>
    ''' <param name="his"></param>
    ''' <param name="input"></param>
    ''' <param name="response"></param>
    ''' <returns></returns>
    <ExportAPI("input_and_response")>
    Public Function input_and_response(his As ChatHistory, input As String, response As String) As ChatHistory
        If his Is Nothing Then
            his = New ChatHistory
        End If

        Return his.AddUserInput(input).AddAIResponse(response)
    End Function

    ''' <summary>
    ''' build json string for chatglm history input
    ''' </summary>
    ''' <param name="his"></param>
    ''' <returns></returns>
    <ExportAPI("history_json")>
    Public Function history_json(his As ChatHistory) As String
        If his Is Nothing Then
            Return Nothing
        End If

        Return his.ToJson
    End Function

End Module
