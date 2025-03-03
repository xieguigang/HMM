Imports System.Text
Imports Microsoft.VisualBasic.Serialization.JSON
Imports ASCII = Microsoft.VisualBasic.Text.ASCII
Imports TalkGenerator.ChatGLM

Public Class DeepSeekResponse

    Public Property think As String
    Public Property output As String

    Public Const who_are_you = "<think>

</think>

Greetings! I'm DeepSeek-R1, an artificial intelligence assistant created by DeepSeek. I'm at your service and would be delighted to assist you with any inquiries or tasks you may have."

    Public Shared Function ParseResponse(content_str As String) As DeepSeekResponse
        Dim think_str As String = content_str.Match("[<]think[>].+[<]/think[>]", RegularExpressions.RegexOptions.Singleline)
        content_str = content_str.Substring(think_str.Length)
        Return New DeepSeekResponse With {
            .think = think_str.GetStackValue(">", "<").Trim(ASCII.CR, ASCII.LF, ASCII.TAB, " "c),
            .output = Strings.Trim(content_str).Trim(ASCII.CR, ASCII.LF, ASCII.TAB, " "c)
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
        Dim result = resp.LoadJSON(Of DeepSeekResponseBody)
        Dim output As DeepSeekResponse = DeepSeekResponse.ParseResponse(result.message.content)

        Return output
    End Function

End Class


Public Class DeepSeekResponseBody

    ''' <summary>
    ''' 每个batch文件只能包含对单个模型的请求,支持 glm-4、glm-3-turbo.
    ''' </summary>
    ''' <returns></returns>
    Public Property model As String
    Public Property message As History

End Class