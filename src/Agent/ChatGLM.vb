
Imports System.IO
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.[Object]
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization
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

    ''' <summary>
    ''' create json data for run chatglm batch task
    ''' </summary>
    ''' <param name="content">
    ''' a dataframe object that contains the batch task data for run:
    ''' 
    ''' 1. id: the unique reference id of each task
    ''' 2. term: the term for do translation
    ''' </param>
    ''' <returns>
    ''' a collection of the batch task data for run the request
    ''' </returns>
    ''' <remarks>
    ''' the required file extension suffix name by upload 
    ''' batch data from web must be ``jsonl``!
    ''' </remarks>
    <ExportAPI("batch_transaltion")>
    <RApiReturn(GetType(GLMBatchTask))>
    Public Function batch_translation(content As dataframe, Optional env As Environment = Nothing) As Object
        Dim id As String() = CLRVector.asCharacter(content.getBySynonym("id", "unique_id", "task_id"))
        Dim term As String() = CLRVector.asCharacter(content.getBySynonym("term", "data"))

        If id.IsNullOrEmpty Then
            Return Internal.debug.stop("the required of the unique task reference id is missing!", env)
        End If
        If term.IsNullOrEmpty Then
            Return Internal.debug.stop("the required of the term data for do translation is missing!", env)
        End If

        Dim batch As GLMBatchTask() = New GLMBatchTask(id.Length - 1) {}
        Dim text As String

        For i As Integer = 0 To id.Length - 1
            text = term(i)
            text = Strings.Trim(text).Replace(""""c, "'")
            batch(i) = New GLMBatchTask With {
                .custom_id = id(i),
                .method = "POST",
                .url = "/v4/chat/completions",
                .body = New RequestBody With {
                    .temperature = 0.1,
                    .model = "glm-4",
                    .messages = {
                        New History(roles.system, "你是一个从英文到中文的语言翻译器"),
                        New History(roles.user, $"# 任务：对以下用户文本进行英文到中文的翻译，只输出翻译后的结果文本，
# 用户文本： data=""{text}""
# 输出格式： {{""zh-CN"": "" ""}}")
                    }
                }
            }
        Next

        Return batch
    End Function

    ''' <summary>
    ''' parse the result of the chatglm batch request
    ''' </summary>
    ''' <param name="file"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("parse_batch_output")>
    Public Function ParseBatchResult(<RRawVectorArgument> file As Object, Optional env As Environment = Nothing) As Object
        Dim ispath As Boolean = False
        Dim s = SMRUCC.Rsharp.GetFileStream(file, IO.FileAccess.Read, env, is_filepath:=ispath)
        Dim request_id As New List(Of String)
        Dim result As New List(Of String)

        If ispath Then
            Try
                Call s.TryCast(Of Stream).Close()
            Catch ex As Exception

            End Try
        End If

        Return dataframe.Create(
            slot("request_id") = request_id.ToArray,
            slot("response") = result.ToArray
        )
    End Function

End Module
