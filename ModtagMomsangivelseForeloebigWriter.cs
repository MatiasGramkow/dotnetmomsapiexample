using System;
using System.Globalization;
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

        public ModtagMomsangivelseForeloebigWriter(
            string seNummer,
            string periodeFraDato,
            string periodeTilDato,
            long afgiftTilsvarBeloeb,
            long? salgsMomsBeloeb = null,
            long? koebsMomsBeloeb = null)
        {
            this.seNummer = seNummer;
            this.periodeFraDato = periodeFraDato;
            this.periodeTilDato = periodeTilDato;
            this.afgiftTilsvarBeloeb = afgiftTilsvarBeloeb;
            this.salgsMomsBeloeb = salgsMomsBeloeb;
            this.koebsMomsBeloeb = koebsMomsBeloeb;
        }

        public void Write(XmlTextWriter writer)
        {
            var now = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fff", CultureInfo.InvariantCulture) + DateTimeOffset.Now.ToString("zzz");
            var transaktionId = Guid.NewGuid().ToString().ToUpper();

            // Namespace URIs - PRÆCIS som README eksemplet
            const string nsUrn = "urn:oio:skat:nemvirksomhed:ws:1.0.0";
            const string nsNs = "http://rep.oio.dk/skat.dk/basis/kontekst/xml/schemas/2006/09/01/";
            const string nsNs1 = "http://rep.oio.dk/skat.dk/motor/class/virksomhed/xml/schemas/20080401/";
            const string nsUrn1 = "urn:oio:skat:nemvirksomhed:1.0.0";

            // Root element med urn: prefix (som README)
            writer.WriteStartElement("urn", "ModtagMomsangivelseForeloebig_I", nsUrn);
            writer.WriteAttributeString("xmlns", "urn", null, nsUrn);
            writer.WriteAttributeString("xmlns", "ns", null, nsNs);
            writer.WriteAttributeString("xmlns", "ns1", null, nsNs1);
            writer.WriteAttributeString("xmlns", "urn1", null, nsUrn1);

            // HovedOplysninger
            writer.WriteStartElement("ns", "HovedOplysninger", nsNs);
            writer.WriteElementString("ns", "TransaktionIdentifikator", nsNs, transaktionId);
            writer.WriteElementString("ns", "TransaktionTid", nsNs, now);
            writer.WriteEndElement();

            // Angivelse med urn: prefix
            writer.WriteStartElement("urn", "Angivelse", nsUrn);

            // AngiverVirksomhedSENummer med urn: prefix
            writer.WriteStartElement("urn", "AngiverVirksomhedSENummer", nsUrn);
            writer.WriteElementString("ns1", "VirksomhedSENummerIdentifikator", nsNs1, seNummer);
            writer.WriteEndElement();

            // Angivelsesoplysninger med urn: prefix - datoer UDEN timezone
            writer.WriteStartElement("urn", "Angivelsesoplysninger", nsUrn);
            writer.WriteElementString("urn1", "AngivelsePeriodeFraDato", nsUrn1, periodeFraDato);
            writer.WriteElementString("urn1", "AngivelsePeriodeTilDato", nsUrn1, periodeTilDato);
            writer.WriteEndElement();

            // Angivelsesafgifter med urn: prefix - kun felter med værdier
            writer.WriteStartElement("urn", "Angivelsesafgifter", nsUrn);

            // AfgiftTilsvar (påkrævet)
            writer.WriteElementString("urn1", "MomsAngivelseAfgiftTilsvarBeloeb", nsUrn1, afgiftTilsvarBeloeb.ToString());

            // Valgfrie felter - kun inkluder hvis de har værdi
            if (koebsMomsBeloeb.HasValue)
                writer.WriteElementString("urn1", "MomsAngivelseKoebsMomsBeloeb", nsUrn1, koebsMomsBeloeb.Value.ToString());
            if (salgsMomsBeloeb.HasValue)
                writer.WriteElementString("urn1", "MomsAngivelseSalgsMomsBeloeb", nsUrn1, salgsMomsBeloeb.Value.ToString());

            writer.WriteEndElement(); // Angivelsesafgifter
            writer.WriteEndElement(); // Angivelse
            writer.WriteEndElement(); // ModtagMomsangivelseForeloebig_I
        }
    }
}
