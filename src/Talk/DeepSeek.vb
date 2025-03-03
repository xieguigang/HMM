Imports System.Text
Imports Microsoft.VisualBasic.Serialization.JSON
Imports TalkGenerator.ChatGLM

Public Class DeepSeekResponse

    Public Property think As String
    Public Property output As String

    Public Shared Function ParseResponse(content_str As String) As DeepSeekResponse
        Dim think_str As String = content_str.Match("[<]think[>].+[<]/think[>]")
        content_str = content_str.Substring(think_str.Length)
        Return New DeepSeekResponse With {
            .think = think_str.GetStackValue(">", "<"),
            .output = content_str
        }
    End Function

    Public Shared Function Chat(message As String, ollama_server As String, Optional model As String = "deepseek-r1:671b") As DeepSeekResponse
        Dim url As String = $"http://{ollama_server}/api/chat"
        Dim req As New RequestBody With {
            .messages = {
                New History With {.content = message, .role = "user"}
            },
            .model = model,
            .stream = False,
            .temperature = 0.1
        }
        Dim resp As String = url.POSTFile(Encoding.UTF8.GetBytes(req.GetJson))
        Dim result = resp.LoadJSON(Of RequestBody)
        Dim output As DeepSeekResponse = DeepSeekResponse.ParseResponse(result.messages(0).content)

        Return output
    End Function

End Class
