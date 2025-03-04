Imports System.Text
Imports Microsoft.VisualBasic.Serialization.JSON
Imports ASCII = Microsoft.VisualBasic.Text.ASCII
Imports TalkGenerator.ChatGLM
Imports System.Net.Http
Imports System.Reflection.Metadata

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
            .stream = True,
            .temperature = 0.1
        }
        Dim json_input As String = req.GetJson
        Dim content = New StringContent(json_input, Encoding.UTF8, "application/json")

        Using client As New HttpClient With {.Timeout = TimeSpan.FromHours(1)}
            Dim resp As String = RequestMessage(client, url, content).GetAwaiter.GetResult
            Dim jsonl As String() = resp.LineTokens
            Dim msg As New StringBuilder

            For Each stream As String In jsonl
                Dim result = stream.LoadJSON(Of DeepSeekResponseBody)
                Dim deepseek_think = result.message.content

                Call msg.Append(deepseek_think)
            Next

            Dim output As DeepSeekResponse = DeepSeekResponse.ParseResponse(msg.ToString)
            Return output
        End Using
    End Function

    Private Shared Async Function RequestMessage(client As HttpClient, url As String, content As StringContent) As Task(Of String)
        Dim response As HttpResponseMessage = Await client.PostAsync(url, content)

        If response.IsSuccessStatusCode Then
            Return Await response.Content.ReadAsStringAsync()
        Else
            Throw New Exception(response.StatusCode.Description)
        End If
    End Function

End Class


Public Class DeepSeekResponseBody

    ''' <summary>
    ''' 每个batch文件只能包含对单个模型的请求,支持 glm-4、glm-3-turbo.
    ''' </summary>
    ''' <returns></returns>
    Public Property model As String
    Public Property message As History
    Public Property done As Boolean

End Class