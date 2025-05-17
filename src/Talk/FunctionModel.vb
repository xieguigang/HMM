Imports System.Runtime.CompilerServices

Public Class FunctionModel

    Public Property name As String
    Public Property description As String
    Public Property parameters As FunctionParameters

End Class

Public Class FunctionParameters

    Public Property type As String
    Public Property properties As Dictionary(Of String, ParameterProperties)
    Public Property required As String()

End Class

Public Class ParameterProperties

    Public Property name As String
    Public Property description As String
    Public Property type As String

End Class

Public Class FunctionTool

    Public Property type As String = "function"
    Public Property [function] As FunctionModel

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function CreateToolSet(ParamArray f As FunctionModel()) As FunctionTool()
        Return (From fi As FunctionModel
                In f
                Select New FunctionTool With {
                    .[function] = fi
                }).ToArray
    End Function

End Class