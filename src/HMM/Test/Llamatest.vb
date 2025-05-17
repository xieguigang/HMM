Imports TalkGenerator

Module Llamatest



    Sub Main2()
        ' Call Llama2.Run("E:\HMM\data\Llama2\stories15M.bin", tokenizer:="E:\HMM\data\Llama2\tokenizer.bin")

        '  Dim test_parse = DeepSeekResponse.ParseResponse(DeepSeekResponse.who_are_you)
        '  Dim list As String() = "E:\HMM\test\stream.jsonl".ReadAllLines


        ' test deepseek
        'Dim result = DeepSeekResponse.Chat("who are you?", "127.0.0.1:11434", "deepseek-r1:32b")

        'Call Console.WriteLine(result.think)
        'Call Console.WriteLine(result.output)

        Dim tool_time As New FunctionModel With {
            .description = "get time of now",
            .name = "get_time",
            .parameters = New FunctionParameters With {
                .required = {"loc"},
                .properties = New Dictionary(Of String, ParameterProperties) From {
                    {
                        "loc", New ParameterProperties With {
                            .name = "loc",
                            .description = "the location name for get current local time"
                        }
                    }
                }
            }
        }

        Dim test_call = DeepSeekResponse.Chat("what is the time of beijing city now?", "127.0.0.1:11434", "deepseek-r1:32b", {tool_time})

        Call Console.WriteLine(test_call.think)
        Call Console.WriteLine(test_call.output)

        Pause()
    End Sub


End Module
