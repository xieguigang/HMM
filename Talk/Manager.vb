Imports System

Public Class Manager
    Private InstanceFieldsInitialized As Boolean = False

    Private Sub InitializeInstanceFields()
        strings = New String(numSentances - 1) {}
    End Sub

    Friend numSentances As Integer = 10
    Friend generator As Generator = New Generator()
    Friend strings As String()
    Public Sub New(text As String)
        If Not InstanceFieldsInitialized Then
            InitializeInstanceFields()
            InstanceFieldsInitialized = True
        End If

        For i = 0 To numSentances - 1
            strings(i) = generator.generate(text)
        Next
        manage()
        For i = 0 To strings.Length - 1
            Console.WriteLine(strings(i))
        Next
    End Sub
    Public Overridable Sub manage()
        For i = 0 To strings.Length - 1
            Dim [string] = strings(i)
            If Not ([string].EndsWith("the", StringComparison.Ordinal) OrElse [string].EndsWith("and", StringComparison.Ordinal) OrElse [string].EndsWith("take", StringComparison.Ordinal) OrElse [string].EndsWith("your", StringComparison.Ordinal) OrElse [string].EndsWith("*is*", StringComparison.Ordinal) OrElse [string].EndsWith("each", StringComparison.Ordinal) OrElse [string].EndsWith("they", StringComparison.Ordinal) OrElse [string].EndsWith("like", StringComparison.Ordinal) OrElse [string].EndsWith("in", StringComparison.Ordinal) OrElse [string].EndsWith("my", StringComparison.Ordinal) OrElse [string].EndsWith("his", StringComparison.Ordinal) OrElse [string].EndsWith("our", StringComparison.Ordinal) OrElse [string].EndsWith("a", StringComparison.Ordinal) OrElse [string].EndsWith("she's", StringComparison.Ordinal) OrElse [string].EndsWith("he", StringComparison.Ordinal) OrElse [string].EndsWith(" ", StringComparison.Ordinal) OrElse [string].EndsWith("she", StringComparison.Ordinal) OrElse [string].EndsWith("it", StringComparison.Ordinal) OrElse [string].EndsWith("but", StringComparison.Ordinal) OrElse [string].EndsWith("you're", StringComparison.Ordinal) OrElse [string].EndsWith("on", StringComparison.Ordinal) OrElse [string].EndsWith("or", StringComparison.Ordinal) OrElse [string].EndsWith("introduce", StringComparison.Ordinal) OrElse [string].EndsWith("you", StringComparison.Ordinal) OrElse [string].EndsWith("that's", StringComparison.Ordinal) OrElse [string].EndsWith("their", StringComparison.Ordinal) OrElse [string].EndsWith("ain't", StringComparison.Ordinal) OrElse [string].EndsWith("start", StringComparison.Ordinal) OrElse [string].EndsWith("a Go", StringComparison.Ordinal) OrElse [string].EndsWith("a go", StringComparison.Ordinal) OrElse [string].EndsWith("of", StringComparison.Ordinal) OrElse [string].EndsWith("just", StringComparison.Ordinal) OrElse [string].EndsWith("not", StringComparison.Ordinal) OrElse [string].EndsWith("if", StringComparison.Ordinal) OrElse [string].EndsWith("than", StringComparison.Ordinal) OrElse [string].EndsWith("are", StringComparison.Ordinal) OrElse [string].EndsWith("at", StringComparison.Ordinal) OrElse [string].EndsWith("to", StringComparison.Ordinal) OrElse [string].EndsWith("is", StringComparison.Ordinal) OrElse [string].EndsWith(",", StringComparison.Ordinal) OrElse [string].EndsWith(":", StringComparison.Ordinal) OrElse [string].EndsWith(";", StringComparison.Ordinal) OrElse [string].EndsWith("I'll get", StringComparison.Ordinal) OrElse [string].EndsWith("You'll get", StringComparison.Ordinal) OrElse [string].EndsWith("Don't be", StringComparison.Ordinal) OrElse [string].EndsWith("You're totally", StringComparison.Ordinal) OrElse [string].Matches("^\w*[\.\?\!]").Any) OrElse [string].Matches("[A-Z]+[a-z']+[\.\?\!]*$").Any Then
                If Not [string].Matches("[\.\?\!\,]$").Any Then
                    [string] = [string] & "."
                End If
                If [string].StartsWith(" ", StringComparison.Ordinal) Then
                    [string] = [string].Substring(1)
                End If
                If [string].StartsWith(" ", StringComparison.Ordinal) Then
                    [string] = [string].Substring(1)
                End If
                If [string].StartsWith(" ", StringComparison.Ordinal) Then
                    [string] = [string].Substring(1)
                End If
                strings(i) = [string].Substring(0, 1).ToUpper() & [string].Substring(1)
            Else
                strings(i) = """" & [string] & """"
            End If
        Next
    End Sub

End Class
