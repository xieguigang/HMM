Imports TalkGenerator

Module Llamatest

    Sub Main2()
        ' Call Llama2.Run("E:\HMM\data\Llama2\stories15M.bin", tokenizer:="E:\HMM\data\Llama2\tokenizer.bin")

        ' test deepseek
        Dim result = DeepSeekResponse.Chat("who are you?", "192.168.0.207:11434", "deepseek-r1:1.5b")



        Pause()
    End Sub


End Module
