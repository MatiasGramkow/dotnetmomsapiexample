using System;
using System.Xml;

namespace UFSTWSSecuritySample
{
    public class ModtagMomsangivelseForeloebigWriter : IPayloadWriter
    {
        private readonly string seNummer;
        private readonly string periodeFraDato;
        private readonly string periodeTilDato;
        private readonly long afgiftTilsvarBeloeb;
        private readonly long? salgsMomsBeloeb;
        private readonly long? koebsMomsBeloeb;
        private readonly long? euKoebBeloeb;
        private readonly long? euSalgVarerBeloeb;
        private readonly long? eksportOmsaetningBeloeb;

        public ModtagMomsangivelseForeloebigWriter(
            string seNummer,
            string periodeFraDato,
            string periodeTilDato,
            long afgiftTilsvarBeloeb,
            long? salgsMomsBeloeb = null,
            long? koebsMomsBeloeb = null,
            long? euKoebBeloeb = null,
            long? euSalgVarerBeloeb = null,
            long? eksportOmsaetningBeloeb = null)
        {
            this.seNummer = seNummer;
            this.periodeFraDato = periodeFraDato;
            this.periodeTilDato = periodeTilDato;
            this.afgiftTilsvarBeloeb = afgiftTilsvarBeloeb;
            this.salgsMomsBeloeb = salgsMomsBeloeb;
            this.koebsMomsBeloeb = koebsMomsBeloeb;
            this.euKoebBeloeb = euKoebBeloeb;
            this.euSalgVarerBeloeb = euSalgVarerBeloeb;
            this.eksportOmsaetningBeloeb = eksportOmsaetningBeloeb;
        }

        public void Write(XmlTextWriter writer)
        {
            var now = DateTime.UtcNow.ToString("o").Substring(0, 23) + "Z";
            var transactionId = Guid.NewGuid().ToString();

            writer.WriteStartElement("urn", "ModtagMomsangivelseForeloebig_I", "urn:oio:skat:nemvirksomhed:ws:1.0.0");

            // HovedOplysninger
            writer.WriteStartElement("ns", "HovedOplysninger", "http://rep.oio.dk/skat.dk/basis/kontekst/xml/schemas/2006/09/01/");
            writer.WriteStartElement("ns", "TransaktionIdentifikator", null);
            writer.WriteString(transactionId);
            writer.WriteEndElement();
            writer.WriteStartElement("ns", "TransaktionTid", null);
            writer.WriteString(now);
            writer.WriteEndElement();
            writer.WriteEndElement();

            // Angivelse
            writer.WriteStartElement("urn", "Angivelse", null);

            // AngiverVirksomhedSENummer (wrapper element)
            writer.WriteStartElement("urn", "AngiverVirksomhedSENummer", null);
            writer.WriteStartElement("ns1", "VirksomhedSENummerIdentifikator", "http://rep.oio.dk/skat.dk/motor/class/virksomhed/xml/schemas/20080401/");
            writer.WriteString(seNummer);
            writer.WriteEndElement();
            writer.WriteEndElement();

            // Angivelsesoplysninger (wrapper for datoer)
            writer.WriteStartElement("urn", "Angivelsesoplysninger", null);
            writer.WriteStartElement("urn1", "AngivelsePeriodeFraDato", "urn:oio:skat:nemvirksomhed:1.0.0");
            writer.WriteString(periodeFraDato);
            writer.WriteEndElement();
            writer.WriteStartElement("urn1", "AngivelsePeriodeTilDato", "urn:oio:skat:nemvirksomhed:1.0.0");
            writer.WriteString(periodeTilDato);
            writer.WriteEndElement();
            writer.WriteEndElement();

            // Angivelsesafgifter
            writer.WriteStartElement("urn", "Angivelsesafgifter", null);

            // MomsAngivelseAfgiftTilsvarBeloeb (påkrævet)
            writer.WriteStartElement("urn1", "MomsAngivelseAfgiftTilsvarBeloeb", "urn:oio:skat:nemvirksomhed:1.0.0");
            writer.WriteString(afgiftTilsvarBeloeb.ToString());
            writer.WriteEndElement();

            // Valgfrie felter - i korrekt rækkefølge ifølge schema
            if (koebsMomsBeloeb.HasValue)
            {
                writer.WriteStartElement("urn1", "MomsAngivelseKoebsMomsBeloeb", "urn:oio:skat:nemvirksomhed:1.0.0");
                writer.WriteString(koebsMomsBeloeb.Value.ToString());
                writer.WriteEndElement();
            }

            if (salgsMomsBeloeb.HasValue)
            {
                writer.WriteStartElement("urn1", "MomsAngivelseSalgsMomsBeloeb", "urn:oio:skat:nemvirksomhed:1.0.0");
                writer.WriteString(salgsMomsBeloeb.Value.ToString());
                writer.WriteEndElement();
            }

            if (euKoebBeloeb.HasValue)
            {
                writer.WriteStartElement("urn1", "MomsAngivelseEUKoebBeloeb", "urn:oio:skat:nemvirksomhed:1.0.0");
                writer.WriteString(euKoebBeloeb.Value.ToString());
                writer.WriteEndElement();
            }

            if (euSalgVarerBeloeb.HasValue)
            {
                writer.WriteStartElement("urn1", "MomsAngivelseEUSalgBeloebVarerBeloeb", "urn:oio:skat:nemvirksomhed:1.0.0");
                writer.WriteString(euSalgVarerBeloeb.Value.ToString());
                writer.WriteEndElement();
            }

            if (eksportOmsaetningBeloeb.HasValue)
            {
                writer.WriteStartElement("urn1", "MomsAngivelseEksportOmsaetningBeloeb", "urn:oio:skat:nemvirksomhed:1.0.0");
                writer.WriteString(eksportOmsaetningBeloeb.Value.ToString());
                writer.WriteEndElement();
            }

            writer.WriteEndElement(); // Angivelsesafgifter
            writer.WriteEndElement(); // Angivelse
            writer.WriteEndElement(); // ModtagMomsangivelseForeloebig_I
        }
    }
}
