namespace DokterspraktijkLib
{
    /// <summary>
    /// Geeft aan via welk kanaal een patiënt notificaties wenst te ontvangen.
    /// De integerwaarden komen overeen met de kolom 'notificaties' in de database.
    /// </summary>
    public enum Notificaties
    {
        /// <summary>De patiënt ontvangt geen notificaties.</summary>
        Geen = 0,

        /// <summary>De patiënt ontvangt notificaties via e-mail.</summary>
        Mail = 1,

        /// <summary>De patiënt ontvangt notificaties via SMS.</summary>
        Sms = 2,

        /// <summary>De patiënt ontvangt notificaties via zowel e-mail als SMS.</summary>
        Beide = 3
    }
}
