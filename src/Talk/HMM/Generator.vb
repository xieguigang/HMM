Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports rnd = Microsoft.VisualBasic.Math.RandomExtensions

Public Class Generator

    ReadOnly len As IntRange
    ReadOnly corpora As Corpora

    Public Sub New(len As IntRange, corpora As Corpora)
        Me.len = len
        Me.corpora = corpora
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="start">the start word</param>
    ''' <returns></returns>
    Public Function Generate(Optional start As String = Nothing) As String
        Dim generatedText As New List(Of String)
        Dim numWords As Integer = randomizeNumWords()
        Dim curword As String

        ' start word
        If start.StringEmpty Then
            curWord = getRandomWord()
        Else
            curWord = start
        End If

        Call generatedText.Add(curWord)

        For i As Integer = 0 To numWords - 1
            Dim words = corpora.GetCorpusVertex(curWord)
            Dim values = New Double(words.Count - 1) {}

            For j As Integer = 0 To values.Length - 1
                values(j) = words(j).num
            Next

            If values.Length <> 0 Then
                curWord = words(rouletteSelect(values)).str
            Else
                curWord = getRandomWord()
            End If

            generatedText.Add(curWord)
        Next

        Return generatedText.JoinBy(" ")
    End Function

    Private Function getRandomWord() As String
        Dim keys = corpora.Keys.ToArray
re0:
        Dim rndkey = keys(rnd.Next(keys.Length))
        Dim words = corpora.GetCorpusVertex(rndkey)

        If words.Count = 0 Then
            GoTo re0
        End If

        Return words(rnd.Next(words.Count))
    End Function

    Public Function rouletteSelect(weight As Double()) As Integer
        Dim weight_sum As Double = weight.Sum
        Dim value As Double = rnd.NextDouble() * weight_sum

        For i As Integer = 0 To weight.Length - 1
            value -= weight(i)

            If value <= 0 Then
                Return i
            End If
        Next

        Return weight.Length - 1
    End Function

    Public Function randomizeNumWords() As Integer
        Return rnd.Next(len.Max - len.Min + 1) + len.Min
    End Function
End Class
