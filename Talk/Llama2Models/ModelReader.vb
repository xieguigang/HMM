
Imports System.IO
Imports System.IO.MemoryMappedFiles
Imports System.Runtime.InteropServices

''' <summary>
''' read in the model.bin file
''' </summary>
Public Class ModelReader : Implements IDisposable

    Private disposedValue As Boolean

    Dim fileStream As FileStream
    Dim checkpoint As String

    ' read in the model.bin file
    Friend config As Config
    Friend weights As TransformerWeights

    Sub New(checkpoint As String)
        Me.fileStream = checkpoint.OpenReadonly
        Me.checkpoint = checkpoint
    End Sub

    Public Function Read() As Boolean
        Try
            Call ReadModel()
            Return True
        Catch e As Exception
            Console.Error.WriteLine($"Couldn't read {checkpoint}: {e.Message}")
            Return False
        End Try
    End Function

    Private Sub ReadModel()
        ' Read in the config header
        Dim configBytes = New Byte(Marshal.SizeOf(GetType(Config)) - 1) {}
        If fileStream.Read(configBytes, 0, configBytes.Length) <> configBytes.Length Then Environment.Exit(1)

        Dim handle = GCHandle.Alloc(configBytes, GCHandleType.Pinned)
        Try
            Dim pointer As IntPtr = handle.AddrOfPinnedObject()
            config = CType(Marshal.PtrToStructure(pointer, GetType(Config)), Config)
        Finally
            handle.Free()
        End Try

        ' Negative vocab size is a hacky way of signaling unshared weights. Bit yikes.
        Dim sharedWeights = config.vocab_size > 0
        config.vocab_size = Math.Abs(config.vocab_size)

        ' Figure out the file size
        Dim fileSize = fileStream.Length ' size of the checkpoint file in bytes

        Dim memoryMappedFile = MemoryMappedFiles.MemoryMappedFile.CreateFromFile(fileStream, Nothing, fileSize, MemoryMappedFileAccess.Read, HandleInheritability.None, False)
        Dim configSizeInBytes As Long = Marshal.SizeOf(GetType(Config))
        Dim accessor = memoryMappedFile.CreateViewAccessor(configSizeInBytes, fileSize - configSizeInBytes, MemoryMappedFileAccess.Read)
        weights = New TransformerWeights()

        CheckpointInitWeights(weights, config, accessor, sharedWeights)
    End Sub

    Private Shared Function ReadFloatArray(accessor As MemoryMappedViewAccessor, ByRef offset As Long, size As Integer) As Single()
        Dim array = New Single(size - 1) {}
        accessor.ReadArray(offset, array, 0, size)
        offset += Marshal.SizeOf(Of Single)() * CLng(size)
        Return array
    End Function

    Private Shared Sub CheckpointInitWeights(ByRef w As TransformerWeights, ByRef p As Config, accessor As MemoryMappedViewAccessor, sharedWeights As Boolean)
        Dim offset As Long = 0

        w.token_embedding_table = ReadFloatArray(accessor, offset, p.vocab_size * p.dim)
        w.rms_att_weight = ReadFloatArray(accessor, offset, p.n_layers * p.dim)
        w.wq = ReadFloatArray(accessor, offset, p.n_layers * p.dim * p.dim)
        w.wk = ReadFloatArray(accessor, offset, p.n_layers * p.dim * p.dim)
        w.wv = ReadFloatArray(accessor, offset, p.n_layers * p.dim * p.dim)
        w.wo = ReadFloatArray(accessor, offset, p.n_layers * p.dim * p.dim)
        w.rms_ffn_weight = ReadFloatArray(accessor, offset, p.n_layers * p.dim)
        w.w1 = ReadFloatArray(accessor, offset, p.n_layers * p.dim * p.hidden_dim)
        w.w2 = ReadFloatArray(accessor, offset, p.n_layers * p.hidden_dim * p.dim)
        w.w3 = ReadFloatArray(accessor, offset, p.n_layers * p.dim * p.hidden_dim)
        w.rms_final_weight = ReadFloatArray(accessor, offset, p.dim)
        Dim headSize As Integer = p.dim / p.n_heads
        w.freq_cis_real = ReadFloatArray(accessor, offset, p.seq_len * headSize / 2)
        w.freq_cis_imag = ReadFloatArray(accessor, offset, p.seq_len * headSize / 2)

        If sharedWeights Then
            w.wcls = w.token_embedding_table
        End If
    End Sub

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: 释放托管状态(托管对象)
            End If

            ' TODO: 释放未托管的资源(未托管的对象)并重写终结器
            ' TODO: 将大型字段设置为 null
            disposedValue = True
        End If
    End Sub

    ' ' TODO: 仅当“Dispose(disposing As Boolean)”拥有用于释放未托管资源的代码时才替代终结器
    ' Protected Overrides Sub Finalize()
    '     ' 不要更改此代码。请将清理代码放入“Dispose(disposing As Boolean)”方法中
    '     Dispose(disposing:=False)
    '     MyBase.Finalize()
    ' End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        ' 不要更改此代码。请将清理代码放入“Dispose(disposing As Boolean)”方法中
        Dispose(disposing:=True)
        GC.SuppressFinalize(Me)
    End Sub
End Class
