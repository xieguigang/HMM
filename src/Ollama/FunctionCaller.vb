Imports System.Runtime.CompilerServices
Imports Ollama.JSON.FunctionCall

Public Class FunctionCaller

    ReadOnly registry As New Dictionary(Of String, Func(Of FunctionCall, String))

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function GetCaller() As Func(Of FunctionCall, String)
        Return AddressOf Caller
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function CheckFunction(name As String) As Boolean
        Return registry.ContainsKey(If(name, "no_name"))
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function [Call](arg As FunctionCall) As String
        Return Caller(arg)
    End Function

    Private Function Caller(arg As FunctionCall) As String
        If arg.name.StringEmpty(, True) Then
            Return "the function name to call should not be empty!"
        End If

        If Not registry.ContainsKey(arg.name) Then
            Return $"the given function '{arg.name}' to invoke is not existed!"
        End If

        Return registry(arg.name)(arg)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Sub Register(name As String, func As Func(Of FunctionCall, String))
        registry(name) = func
    End Sub

End Class
