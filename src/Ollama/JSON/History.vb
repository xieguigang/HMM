Imports Ollama.JSON.FunctionCall

Namespace JSON

    Public Enum roles
        user
        assistant
        system
        tool
    End Enum

    Public Class History

        ''' <summary>
        ''' the role of the message, value of this property can be 
        ''' 
        ''' 1, user
        ''' 2, assistant or system
        ''' 3, tool
        ''' 
        ''' could be get the name tostring of <see cref="roles"/> enums value
        ''' </summary>
        ''' <returns></returns>
        Public Property role As String
        Public Property content As String
        Public Property metadata As String
        Public Property tool_calls As ToolCall()
        Public Property tool_call_id As String

        Sub New()
        End Sub

        Sub New(role As roles, content As String)
            Me.role = role.ToString
            Me.content = content
        End Sub

        Public Overrides Function ToString() As String
            Return $"{role}: {content.TrimNewLine("\n")}"
        End Function

    End Class
End Namespace