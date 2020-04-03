namespace DeleteBoilerplate.Account.Models
{
    public abstract class ConsentAgreementViewModel
    {
     
        public virtual int? ConsentId { get; set; }
        public virtual bool ConsentIsAgreed { get; set; }
        public virtual string ConsentShortText { get; set; }
    }
}