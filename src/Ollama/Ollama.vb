﻿Imports System.Net.Http
Imports System.Text
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Ollama.JSON
Imports Ollama.JSON.FunctionCall

''' <summary>
''' the ollama client model
''' </summary>
Public Class Ollama

    Public ReadOnly Property server As String
    Public ReadOnly Property model As String

    Public Property temperature As Double = 0.1
    Public Property tools As List(Of FunctionTool)
    Public Property tool_invoke As Func(Of FunctionCall, String)

    Public ReadOnly Property url As String
        Get
            Return $"http://{_server}/api/chat"
        End Get
    End Property

    Dim ai_memory As New List(Of History)
    Dim ai_caller As New FunctionCaller

    Sub New(model As String, Optional server As String = "127.0.0.1:11434")
        Me.model = model
        Me.server = server
    End Sub

    Public Sub AddFunction(func As FunctionModel, Optional f As Func(Of FunctionCall, String) = Nothing)
        If tools Is Nothing Then
            tools = New List(Of FunctionTool)
        End If

        Call tools.Add(New FunctionTool With {.[function] = func})

        If Not f Is Nothing Then
            Call ai_caller.Register(func.name, f)
        End If
    End Sub

    Public Function Chat(message As String) As DeepSeekResponse
        Dim newUserMsg As New History With {.content = message, .role = "user"}

        ai_memory.Add(newUserMsg)

        Dim req As New RequestBody With {
            .messages = ai_memory.ToArray,
            .model = model,
            .stream = True,
            .temperature = 0.1,
            .tools = If(tools.IsNullOrEmpty, Nothing, tools.ToArray)
        }

        Return Chat(req)
    End Function

    Private Function execExternal(arg As FunctionCall) As String
        If tool_invoke Is Nothing Then
            Throw New InvalidProgramException("the invoke engine function intptr should not be nothing!")
        Else
            Return _tool_invoke(arg)
        End If
    End Function

    Private Function Chat(req As RequestBody) As DeepSeekResponse
        Dim json_input As String = req.GetJson
        Dim content = New StringContent(json_input, Encoding.UTF8, "application/json")
        Dim settings As New HttpClientHandler With {
            .Proxy = Nothing,
            .UseProxy = False
        }

        Using client As New HttpClient(settings) With {.Timeout = TimeSpan.FromHours(1)}
            Dim resp As String = RequestMessage(client, url, content).GetAwaiter.GetResult
            Dim jsonl As String() = resp.LineTokens
            Dim msg As New StringBuilder

            For Each stream As String In jsonl
                Dim result = stream.LoadJSON(Of ResponseBody)
                Dim deepseek_think = result.message.content

                Call ai_memory.Add(result.message)

                If deepseek_think = "" AndAlso Not result.message.tool_calls.IsNullOrEmpty Then
                    ' is function calls
                    Dim tool_call As ToolCall = result.message.tool_calls(0)
                    Dim invoke As FunctionCall = tool_call.function
                    Dim fval As String = If(ai_caller.CheckFunction(invoke.name), ai_caller.Call(invoke), execExternal(invoke))
                    Dim [next] As New History With {
                        .content = fval,
                        .role = "tool",
                        .tool_call_id = tool_call.id
                    }
                    Dim messages As New List(Of History)(req.messages)
                    Call messages.Add(result.message)
                    Call messages.Add([next])

                    req = New RequestBody(req)
                    req.messages = messages.ToArray

                    Return Chat(req)
                End If

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
            Dim msg As String = Await response.Content.ReadAsStringAsync()
            msg = $"{response.StatusCode.Description}{vbCrLf}{vbCrLf}{msg}"

            Throw New Exception(msg)
        End If
    End Function
End Class
