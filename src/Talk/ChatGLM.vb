
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace ChatGLM

    ''' <summary>
    ''' helper for create chatglm history
    ''' </summary>
    Public Class ChatHistory

        ReadOnly history As New List(Of History)

        Public Function AddAIResponse(content As String) As ChatHistory
            history.Add(New History With {.role = roles.assistant.Description, .content = content, .metadata = ""})
            Return Me
        End Function

        Public Function AddUserInput(content As String) As ChatHistory
            history.Add(New History With {.role = roles.user.Description, .metadata = Nothing, .content = content})
            Return Me
        End Function

        Public Function ToJson() As String
            Return history.ToArray.GetJson
        End Function

    End Class

    Public Enum roles
        user
        assistant
    End Enum

    Public Class History

        Public Property role As String
        Public Property content As String
        Public Property metadata As String

    End Class

End Namespace
