Namespace JSON

    Public Class ResponseBody

        ''' <summary>
        ''' 每个batch文件只能包含对单个模型的请求,支持 glm-4、glm-3-turbo.
        ''' </summary>
        ''' <returns></returns>
        Public Property model As String
        Public Property message As History
        Public Property done As Boolean

    End Class
End Namespace