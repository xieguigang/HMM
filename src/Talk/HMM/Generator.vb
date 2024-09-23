Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports rnd = Microsoft.VisualBasic.Math.RandomExtensions

Public Class Generator

    ReadOnly len As IntRange
    ReadOnly corpora As Corpora
    ReadOnly temperature As Double = 0

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="len">word count ranges for output sentense.</param>
    ''' <param name="corpora"></param>
    Public Sub New(len As IntRange, corpora As Corpora)
        Me.len = len
        Me.corpora = corpora
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="start">the start word</param>
    ''' <returns></returns>
    Public Function Generate(Optional start As String = Nothing, Optional temperature As Double = 0) As String
        Dim generatedText As New List(Of String)
        Dim numWords As Integer = randomizeNumWords()
        Dim curword As String

        ' start word
        If start.StringEmpty Then
            curword = getRandomWord()
        Else
            curword = start
        End If

        Call generatedText.Add(curword)

        For i As Integer = 0 To numWords - 1
            Dim words = corpora.GetCorpusVertex(curword)
            Dim values = New Double(words.Count - 1) {}

            For j As Integer = 0 To values.Length - 1
                values(j) = words(j).num
            Next

            If values.Length <> 0 Then
                curword = words(rouletteSelect(values, temperature)).str
            Else
                curword = getRandomWord()
            End If

            generatedText.Add(curword)
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

    ''' <summary>
    ''' get common words
    ''' </summary>
    ''' <param name="weight"></param>
    ''' <returns></returns>
    Public Function rouletteSelect(weight As Double(), temperature As Double) As Integer
        Dim weight_sum As Double = weight.Sum
        Dim value As Double = rnd.NextDouble() * weight_sum + Double.Epsilon

        For i As Integer = 0 To weight.Length - 1
            value -= weight(i)

            If value <= temperature Then
                Return i
            End If
        Next

        Return weight.Length - 1
    End Function

    Public Function randomizeNumWords() As Integer
        Return rnd.Next(len.Max - len.Min + 1) + len.Min
    End Function
End Class
