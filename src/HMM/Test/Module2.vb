Imports Microsoft.VisualBasic.DataMining.HMM.Model

Module Module2

    Sub HMM2()

        Dim states = {
            "Begin",
            "StatementBegining", "StatementFirstHalf", "StatementSecondHalf", "StatementEnd", "StatementScalar", ' .
            "QuestionsBegining", "QuestionsFirstHalf", "QuestionsSecondHalf", "QuestionsEnd", "QuestionsScalar", ' ?
            "ExclamationBegining", "ExclamationFirstHalf", "ExclamationSecondHalf", "ExclamationEnd", "ExclamationScalar", ' !
            "PauseBegining", "PauseFirstHalf", "PauseSecondHalf", "PauseEnd", "PauseScalar" ' ,
        }
        Dim info As New modelInfo With {
           .name = "text generator",
           .[date] = Now.ToString,
           .title = "text generator"
       }
        Dim data As New modelData
        Dim transitionMatrix As New Dictionary(Of String, Double)
        Dim emissionMatrix As New Dictionary(Of String, Double)
        Dim observations As New List(Of String)

        For Each par As String In "G:\HMM\test\abstract.txt".ReadAllLines
            If par.StringEmpty(, True) Then
                Continue For
            End If

            If Not par.Last.ToString.IsPattern("[,.?!]") Then
                par = par & "."
            End If

            Dim parts = par.Matches(".*?[,.!?]")
            Dim stat As String = "Begin"
            Dim last As String

            For Each line As String In parts
                Dim tokens As String() = line.Trim(" "c, "."c, ","c, "!"c, "?"c).Split
                Dim type As String

                Select Case line.Last
                    Case "." : type = "Statement"
                    Case "?" : type = "Questions"
                    Case "!" : type = "Exclamation"
                    Case "," : type = "Pause"
                    Case Else
                        Throw New InvalidOperationException
                End Select

                If tokens.Length = 1 Then
                    last = type & "Scalar"
                    transitionMatrix($"{stat} -> {last}") += 1
                    stat = last
                    emissionMatrix($"{stat} -> {tokens(0)}") += 1
                Else
                    For i As Integer = 0 To tokens.Length - 1
                        If i = 0 Then
                            last = type & "Begining"
                        ElseIf i = tokens.Length - 1 Then
                            last = type & "End"
                        ElseIf i < tokens.Length \ 2 Then
                            last = type & "FirstHalf"
                        Else
                            last = type & "SecondHalf"
                        End If

                        transitionMatrix($"{stat} -> {last}") += 1
                        stat = last
                        emissionMatrix($"{stat} -> {tokens(0)}") += 1
                    Next
                End If
            Next
        Next
    End Sub
End Module
