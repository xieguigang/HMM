Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.NLP.Model
Imports Microsoft.VisualBasic.Data.Trinity.NLP
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON

Public Class Corpora

    ''' <summary>
    ''' word graph, map a word to a set of possible next word
    ''' </summary>
    ReadOnly graph As New Dictionary(Of String, WordIndex)()

    Public ReadOnly Property Keys As IEnumerable(Of String)
        Get
            Return graph.Keys
        End Get
    End Property

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function GetCorpusVertex(word As String) As List(Of Word)
        Return graph(word).set
    End Function

    Public Sub MakeCorpus(text As String)
        For Each sent In Paragraph.Segmentation(text) _
            .Select(Function(par) par.sentences) _
            .IteratesALL

            Dim words As String() = sent.words.Select(Function(wi) wi.str).ToArray
            Dim key As String
            Dim previous As String = ""

            For i As Integer = 0 To words.Length - 1
                key = words(i).ToLower

                If key.StringEmpty(, True) Then
                    Continue For
                End If

                If Not graph.ContainsKey(key) Then
                    graph(key) = New WordIndex
                End If

                Call joinLink(previous, current:=key)
            Next
        Next
    End Sub

    Private Sub joinLink(ByRef previous As String, current As String)
        Dim w As WordIndex = Nothing

        If graph.ContainsKey(previous) Then
            w = graph(previous)
            w.Add(current)
        End If

        graph(previous) = If(w, New WordIndex)
        previous = current
    End Sub

    Private Class WordIndex

        Public index As New Dictionary(Of String, Word)
        Public [set] As New List(Of Word)

        Sub Add(word As String)
            If index.ContainsKey(word) Then
                index(word).num += 1
            Else
                index.Add(word, New Word(word))
                [set].Add(index(word))
            End If
        End Sub

        Public Overrides Function ToString() As String
            Return index.Keys.GetJson
        End Function

    End Class
End Class
