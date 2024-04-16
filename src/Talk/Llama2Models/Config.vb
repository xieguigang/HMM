' Transformer and RunState structs, and related memory management
Imports System.Runtime.InteropServices

<StructLayout(LayoutKind.Sequential)>
Public Structure Config
    ''' <summary>
    ''' transformer dimension
    ''' </summary>
    Public [dim] As Integer
    ''' <summary>
    ''' for ffn layers
    ''' </summary>
    Public hidden_dim As Integer
    ''' <summary>
    ''' number of layers
    ''' </summary>
    Public n_layers As Integer
    ''' <summary>
    ''' number of query heads
    ''' </summary>
    Public n_heads As Integer
    ''' <summary>
    ''' number of key/value heads (can be &lt; query heads because of multiquery)
    ''' </summary>
    Public n_kv_heads As Integer
    ''' <summary>
    ''' vocabulary size, usually 256 (byte-level)
    ''' </summary>
    Public vocab_size As Integer
    ''' <summary>
    ''' max sequence length
    ''' </summary>
    Public seq_len As Integer
End Structure