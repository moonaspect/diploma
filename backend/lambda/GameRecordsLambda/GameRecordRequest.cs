public record GameRecordRequest
{
    public required string PlayerId { get; set; }
    public int Score { get; set; }
}
