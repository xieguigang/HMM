Public Class DeepSeekResponse

    Public Property think As String
    Public Property output As String

    Public Function ParseResponse(content_str As String) As DeepSeekResponse
        Dim think_str As String = content_str.Match("[<]think[>].+[<]/think[>]")
        content_str = content_str.Substring(think_str.Length)
        Return New DeepSeekResponse With {
            .think = think_str.GetStackValue(">", "<"),
            .output = content_str
        }
    End Function

End Class
