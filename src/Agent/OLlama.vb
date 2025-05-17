
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Ollama
Imports SMRUCC.Rsharp.Runtime.Components.[Interface]
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop

<Package("ollama")>
Module OLlamaDemo

    <ExportAPI("new")>
    Public Function create(model As String, Optional ollama_server As String = "127.0.0.1:11434") As Ollama.Ollama
        Return New Ollama.Ollama(model, ollama_server)
    End Function

    <ExportAPI("chat")>
    Public Function chat(model As Ollama.Ollama, msg As String) As Object
        Return model.Chat(msg)
    End Function

    <ExportAPI("add_tool")>
    Public Function add_tool(model As Ollama.Ollama, name$, desc$,
                             <RByRefValueAssign> fcall As RFunction,
                             <RListObjectArgument>
                             Optional args As list = Nothing,
                             Optional env As Environment = Nothing) As Object

    End Function

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
