' Transformer and RunState structs, and related memory management
Imports System.Runtime.InteropServices

<StructLayout(LayoutKind.Sequential)>
Public Structure Config
    Public [dim] As Integer ' transformer dimension
    Public hidden_dim As Integer ' for ffn layers
    Public n_layers As Integer ' number of layers
    Public n_heads As Integer ' number of query heads
    Public n_kv_heads As Integer ' number of key/value heads (can be < query heads because of multiquery)
    Public vocab_size As Integer ' vocabulary size, usually 256 (byte-level)
    Public seq_len As Integer ' max sequence length
End Structure