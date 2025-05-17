
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Ollama

<Package("ollama")>
Module OLlamaDemo

    ' start server
    ' docker run --privileged --net=host --env OLLAMA_HOST=0.0.0.0  -itd  -p "11434:11434" ubuntu:deepseek_20250301 ollama serve
    ' start model server
    ' docker run -it --net=host -d  ubuntu:deepseek_20250301 ollama run deepseek-r1:1.5b

    <ExportAPI("deepseek_chat")>
    Public Function deepseek_chat(message As String,
                                  Optional ollama_serve As String = "127.0.0.1:11434",
                                  Optional model As String = "deepseek-r1:671b") As DeepSeekResponse

        Return DeepSeekResponse.Chat(message, ollama_serve, model)
    End Function

End Module
