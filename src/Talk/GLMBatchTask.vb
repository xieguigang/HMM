Namespace ChatGLM

    ''' <summary>
    ''' a task request data for invoke chatglm
    ''' </summary>
    Public Class GLMBatchTask

        ''' <summary>
        ''' 每个请求必须包含custom_id且是唯一的,长度必须为 6 -64 位.用来将结果和输入进行匹配.
        ''' </summary>
        ''' <returns></returns>
        Public Property custom_id As String
        Public Property method As String = "POST"
        Public Property url As String
        Public Property body As RequestBody

    End Class

    Public Class RequestBody

        ''' <summary>
        ''' 每个batch文件只能包含对单个模型的请求,支持 glm-4、glm-3-turbo.
        ''' </summary>
        ''' <returns></returns>
        Public Property model As String
        Public Property temperature As Double
        Public Property messages As History()

    End Class

    Public Class Result

        Public Property response As response
        Public Property custom_id As String
        Public Property id As String

        Public Iterator Function GetResponseText() As IEnumerable(Of String)
            If response Is Nothing OrElse response.body Is Nothing Then
                Return
            End If

            If response.body.choices.IsNullOrEmpty Then
                Return
            End If

            For Each talk As Choice In response.body.choices
                If talk.message Is Nothing Then
                    Continue For
                End If

                Yield talk.message.content
            Next
        End Function

    End Class

    Public Class response

        Public Property status_code As Integer
        Public Property body As ResponseBody

    End Class

    Public Class ResponseBody

        Public Property created As UInteger
        Public Property model As String
        Public Property id As String
        Public Property request_id As String
        Public Property usage As TokenUsage
        Public Property choices As choice()

    End Class

    Public Class Choice

        Public Property finish_reason As String
        Public Property index As Integer
        Public Property message As History

    End Class

    Public Class TokenUsage

        Public Property completion_tokens As Integer
        Public Property prompt_tokens As Integer
        Public Property total_tokens As Integer

    End Class
End Namespace