Imports TalkGenerator

Module Llamatest

    Sub Main2()
        Call Llama2.Run("\HMM\Talk\Llama2Models\stories15M.bin", tokenizer:="\HMM\Talk\Llama2Models\tokenizer.bin")

        Pause()
    End Sub
End Module
