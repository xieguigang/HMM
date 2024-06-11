Namespace ChatGLM

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
End Namespace