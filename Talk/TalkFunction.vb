Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model

Public Module TalkFunction

    ''' <summary>
    ''' Make a simple talker
    ''' </summary>
    ''' <param name="len"></param>
    ''' <param name="corpora"></param>
    ''' <returns></returns>
    Public Function SimpleTalker(len As IntRange, ParamArray corpora As String()) As Generator
        Dim corpus As New Corpora()

        For Each line As String In corpora
            Call corpus.MakeCorpus(line)
        Next

        Return New Generator(len, corpus)
    End Function
End Module
