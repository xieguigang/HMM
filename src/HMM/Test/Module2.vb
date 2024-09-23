Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.DataMining.HMM
Imports Microsoft.VisualBasic.DataMining.HMM.Model

Module Module2

    Sub HMM2()

        Dim states As New List(Of String) From {
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

            Dim parts = par.Matches(".*?[,.!?]").ToArray
            Dim stat As String = "Begin"
            Dim last As String

            For Each line As String In parts
                Dim tokens As String() = line.Trim(" "c, "."c, ","c, "!"c, "?"c).ToLower.Split(" "c, "("c, ")"c, "["c, "]"c)
                Dim type As String

                tokens = tokens.Where(Function(s) Not s.StringEmpty).ToArray

                If tokens.Length = 0 Then
                    Continue For
                Else
                    observations.AddRange(tokens)
                End If

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
                    transitionMatrix.Plus($"{stat} -> {last}")
                    stat = last
                    emissionMatrix.Plus($"{stat} -> {tokens(0)}")
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

                        transitionMatrix.Plus($"{stat} -> {last}")
                        stat = last
                        emissionMatrix.Plus($"{stat} -> {tokens(i)}")
                    Next
                End If
            Next
        Next

        Dim transistion = transitionMatrix.Values.Sum
        Dim emission = emissionMatrix.Values.Sum

        transitionMatrix = transitionMatrix.ToDictionary(Function(a) a.Key, Function(a) a.Value / transistion)
        emissionMatrix = emissionMatrix.ToDictionary(Function(a) a.Key, Function(a) a.Value / emission)

        Dim stats_vec As New Dictionary(Of String, Double)
        Dim hmm As New HiddenMarkovModel("text generator", states, observations.Distinct.ToList, stats_vec, transitionMatrix, emissionMatrix)

    End Sub

    <Extension>
    Private Sub Plus(ByRef list As Dictionary(Of String, Double), token As String)
        If Not list.ContainsKey(token) Then
            list.Add(token, 1)
        Else
            list(token) += 1
        End If
    End Sub
End Module
