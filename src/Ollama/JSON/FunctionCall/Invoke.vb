
Namespace JSON.FunctionCall

    Public Class ToolCall

        Public Property id As String
        Public Property [function] As FunctionCall

    End Class

    ''' <summary>
    ''' the function invoke parameters
    ''' </summary>
    Public Class FunctionCall

        Public Property name As String
        Public Property arguments As Dictionary(Of String, String)

    End Class
End Namespace