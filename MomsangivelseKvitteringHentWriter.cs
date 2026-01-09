using System;
using System.Xml;

namespace UFSTWSSecuritySample
{
    public class MomsangivelseKvitteringHentWriter : IPayloadWriter
    {
        private readonly string seNummer;
        private readonly string transaktionIdentifier;

        public MomsangivelseKvitteringHentWriter(string seNummer, string transaktionIdentifier)
        {
            this.seNummer = seNummer;
            this.transaktionIdentifier = transaktionIdentifier;
        }

        public void Write(XmlTextWriter writer)
        {
            var now = DateTime.UtcNow.ToString("o").Substring(0, 23) + "Z";
            var transactionId = Guid.NewGuid().ToString();

            writer.WriteStartElement("urn", "MomsangivelseKvitteringHent_I", "urn:oio:skat:nemvirksomhed:ws:1.0.0");

            // HovedOplysninger
            writer.WriteStartElement("ns", "HovedOplysninger", "http://rep.oio.dk/skat.dk/basis/kontekst/xml/schemas/2006/09/01/");
            writer.WriteStartElement("ns", "TransaktionIdentifikator", null);
            writer.WriteString(transactionId);
            writer.WriteEndElement();
            writer.WriteStartElement("ns", "TransaktionTid", null);
            writer.WriteString(now);
            writer.WriteEndElement();
            writer.WriteEndElement();

            // TransaktionIdentifier - ID for den momsangivelse vi vil hente kvittering for
            writer.WriteStartElement("urn1", "TransaktionIdentifier", "urn:oio:skat:nemvirksomhed:1.0.0");
            writer.WriteString(transaktionIdentifier);
            writer.WriteEndElement();

            // Angiver
            writer.WriteStartElement("urn", "Angiver", null);
            writer.WriteStartElement("ns1", "VirksomhedSENummerIdentifikator", "http://rep.oio.dk/skat.dk/motor/class/virksomhed/xml/schemas/20080401/");
            writer.WriteString(seNummer);
            writer.WriteEndElement();
            writer.WriteEndElement();

            writer.WriteEndElement(); // MomsangivelseKvitteringHent_I
        }
    }
}
