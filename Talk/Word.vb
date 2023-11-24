Imports System.Runtime.CompilerServices

Public Class Word

    Public s As String
    Public num As Integer

    Public Sub New(s As String)
        Me.s = s
        num = 1
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Overrides Function ToString() As String
        Return s
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Overloads Shared Narrowing Operator CType(w As Word) As String
        Return w.s
    End Operator
End Class
