namespace SamSAFE.Interfaces
{
    public interface IProposalModel
    {
        string __action { get; set; }
        string __actionId { get; set; }
        string __token { get; set; }
        string __session { get; set; }
    }
}
