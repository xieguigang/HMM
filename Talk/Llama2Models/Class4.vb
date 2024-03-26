Imports System.Runtime.InteropServices

<StructLayout(LayoutKind.Sequential)>
Public Structure RunState
    ' current wave of activations
    Public x As Single() ' activation at current time stamp (dim,)
    Public xb As Single() ' same, but inside a residual branch (dim,)
    Public xb2 As Single() ' an additional buffer just for convenience (dim,)
    Public hb As Single() ' buffer for hidden dimension in the ffn (hidden_dim,)
    Public hb2 As Single() ' buffer for hidden dimension in the ffn (hidden_dim,)
    Public q As Single() ' query (dim,)
    Public k As Single() ' key (dim,)
    Public v As Single() ' value (dim,)
    Public att As Single() ' buffer for scores/attention values (n_heads, seq_len)
    Public logits As Single() ' output logits

    Public probindex As ProbIndex() ' buffer used in top-p sampling

    ' kv cache
    Public key_cache As Single() ' (layer, seq_len, dim)
    Public value_cache As Single() ' (layer, seq_len, dim)
End Structure