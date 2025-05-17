﻿Imports Ollama.JSON.FunctionCall

Namespace JSON

    Public Class RequestBody

        ''' <summary>
        ''' 每个batch文件只能包含对单个模型的请求,支持 glm-4、glm-3-turbo.
        ''' </summary>
        ''' <returns></returns>
        Public Property model As String
        Public Property temperature As Double?
        Public Property messages As History()
        ''' <summary>
        ''' json list response
        ''' </summary>
        ''' <returns></returns>
        Public Property stream As Boolean = False
        Public Property tools As FunctionTool()

        Sub New()
        End Sub

        Sub New(copy As RequestBody)
            model = copy.model
            temperature = copy.temperature
            messages = copy.messages.ToArray
            stream = copy.stream
            tools = copy.tools.ToArray
        End Sub

    End Class

End Namespace