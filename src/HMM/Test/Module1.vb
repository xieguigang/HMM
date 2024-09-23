Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports TalkGenerator

Module Module1

    Sub Main2()

        Dim corpus As New Corpora()
        Dim corpora As String() = {
            "E:\HMM\test\Methoxamine_datasheet.csv"
        }

        For Each file As String In corpora
            For Each line As String In file.ReadAllLines
                Call corpus.MakeCorpus(line)
            Next
        Next

        Dim talk As New Generator(New IntRange(3, 60), corpus)

        For i As Integer = 0 To 20
            Call Console.WriteLine(talk.Generate(start:="methoxamine"))
        Next

        Pause()
    End Sub
End Module
