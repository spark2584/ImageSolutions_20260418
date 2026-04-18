using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnterpriseIntegration.Models
{

    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class cXML
    {

        private cXMLHeader headerField;

        private cXMLRequest requestField;

        private string versionField;

        private string payloadIDField;

        private System.DateTime timestampField;

        /// <remarks/>
        public cXMLHeader Header
        {
            get
            {
                return this.headerField;
            }
            set
            {
                this.headerField = value;
            }
        }

        /// <remarks/>
        public cXMLRequest Request
        {
            get
            {
                return this.requestField;
            }
            set
            {
                this.requestField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string version
        {
            get
            {
                return this.versionField;
            }
            set
            {
                this.versionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string payloadID
        {
            get
            {
                return this.payloadIDField;
            }
            set
            {
                this.payloadIDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public System.DateTime timestamp
        {
            get
            {
                return this.timestampField;
            }
            set
            {
                this.timestampField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class cXMLHeader
    {

        private cXMLHeaderFrom fromField;

        private cXMLHeaderTO toField;

        private cXMLHeaderSender senderField;

        /// <remarks/>
        public cXMLHeaderFrom From
        {
            get
            {
                return this.fromField;
            }
            set
            {
                this.fromField = value;
            }
        }

        /// <remarks/>
        public cXMLHeaderTO To
        {
            get
            {
                return this.toField;
            }
            set
            {
                this.toField = value;
            }
        }

        /// <remarks/>
        public cXMLHeaderSender Sender
        {
            get
            {
                return this.senderField;
            }
            set
            {
                this.senderField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class cXMLHeaderFrom
    {

        private cXMLHeaderFromCredential credentialField;

        /// <remarks/>
        public cXMLHeaderFromCredential Credential
        {
            get
            {
                return this.credentialField;
            }
            set
            {
                this.credentialField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class cXMLHeaderFromCredential
    {

        private uint identityField;

        private string domainField;

        /// <remarks/>
        public uint Identity
        {
            get
            {
                return this.identityField;
            }
            set
            {
                this.identityField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string domain
        {
            get
            {
                return this.domainField;
            }
            set
            {
                this.domainField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class cXMLHeaderTO
    {

        private cXMLHeaderTOCredential credentialField;

        /// <remarks/>
        public cXMLHeaderTOCredential Credential
        {
            get
            {
                return this.credentialField;
            }
            set
            {
                this.credentialField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class cXMLHeaderTOCredential
    {

        private uint identityField;

        private string domainField;

        /// <remarks/>
        public uint Identity
        {
            get
            {
                return this.identityField;
            }
            set
            {
                this.identityField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string domain
        {
            get
            {
                return this.domainField;
            }
            set
            {
                this.domainField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class cXMLHeaderSender
    {

        private cXMLHeaderSenderCredential credentialField;

        private string userAgentField;

        /// <remarks/>
        public cXMLHeaderSenderCredential Credential
        {
            get
            {
                return this.credentialField;
            }
            set
            {
                this.credentialField = value;
            }
        }

        /// <remarks/>
        public string UserAgent
        {
            get
            {
                return this.userAgentField;
            }
            set
            {
                this.userAgentField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class cXMLHeaderSenderCredential
    {

        private string identityField;

        private string sharedSecretField;

        private string domainField;

        /// <remarks/>
        public string Identity
        {
            get
            {
                return this.identityField;
            }
            set
            {
                this.identityField = value;
            }
        }

        /// <remarks/>
        public string SharedSecret
        {
            get
            {
                return this.sharedSecretField;
            }
            set
            {
                this.sharedSecretField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string domain
        {
            get
            {
                return this.domainField;
            }
            set
            {
                this.domainField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class cXMLRequest
    {

        private cXMLRequestOrderRequest orderRequestField;

        /// <remarks/>
        public cXMLRequestOrderRequest OrderRequest
        {
            get
            {
                return this.orderRequestField;
            }
            set
            {
                this.orderRequestField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class cXMLRequestOrderRequest
    {

        private cXMLRequestOrderRequestOrderRequestHeader orderRequestHeaderField;

        private cXMLRequestOrderRequestItemOut[] itemOutField;

        /// <remarks/>
        public cXMLRequestOrderRequestOrderRequestHeader OrderRequestHeader
        {
            get
            {
                return this.orderRequestHeaderField;
            }
            set
            {
                this.orderRequestHeaderField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("ItemOut")]
        public cXMLRequestOrderRequestItemOut[] ItemOut
        {
            get
            {
                return this.itemOutField;
            }
            set
            {
                this.itemOutField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class cXMLRequestOrderRequestOrderRequestHeader
    {

        private cXMLRequestOrderRequestOrderRequestHeaderTotal totalField;

        private cXMLRequestOrderRequestOrderRequestHeaderShipTo shipToField;

        private cXMLRequestOrderRequestOrderRequestHeaderBillTo billToField;

        private cXMLRequestOrderRequestOrderRequestHeaderShipping shippingField;

        private cXMLRequestOrderRequestOrderRequestHeaderContact contactField;

        private cXMLRequestOrderRequestOrderRequestHeaderComments commentsField;

        private cXMLRequestOrderRequestOrderRequestHeaderExtrinsic[] extrinsicField;

        private string orderIDField;

        private System.DateTime orderDateField;

        private string orderTypeField;

        private string typeField;

        /// <remarks/>
        public cXMLRequestOrderRequestOrderRequestHeaderTotal Total
        {
            get
            {
                return this.totalField;
            }
            set
            {
                this.totalField = value;
            }
        }

        /// <remarks/>
        public cXMLRequestOrderRequestOrderRequestHeaderShipTo ShipTo
        {
            get
            {
                return this.shipToField;
            }
            set
            {
                this.shipToField = value;
            }
        }

        /// <remarks/>
        public cXMLRequestOrderRequestOrderRequestHeaderBillTo BillTo
        {
            get
            {
                return this.billToField;
            }
            set
            {
                this.billToField = value;
            }
        }

        /// <remarks/>
        public cXMLRequestOrderRequestOrderRequestHeaderShipping Shipping
        {
            get
            {
                return this.shippingField;
            }
            set
            {
                this.shippingField = value;
            }
        }

        /// <remarks/>
        public cXMLRequestOrderRequestOrderRequestHeaderContact Contact
        {
            get
            {
                return this.contactField;
            }
            set
            {
                this.contactField = value;
            }
        }

        /// <remarks/>
        public cXMLRequestOrderRequestOrderRequestHeaderComments Comments
        {
            get
            {
                return this.commentsField;
            }
            set
            {
                this.commentsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Extrinsic")]
        public cXMLRequestOrderRequestOrderRequestHeaderExtrinsic[] Extrinsic
        {
            get
            {
                return this.extrinsicField;
            }
            set
            {
                this.extrinsicField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string orderID
        {
            get
            {
                return this.orderIDField;
            }
            set
            {
                this.orderIDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public System.DateTime orderDate
        {
            get
            {
                return this.orderDateField;
            }
            set
            {
                this.orderDateField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string orderType
        {
            get
            {
                return this.orderTypeField;
            }
            set
            {
                this.orderTypeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class cXMLRequestOrderRequestOrderRequestHeaderTotal
    {

        private cXMLRequestOrderRequestOrderRequestHeaderTotalMoney moneyField;

        /// <remarks/>
        public cXMLRequestOrderRequestOrderRequestHeaderTotalMoney Money
        {
            get
            {
                return this.moneyField;
            }
            set
            {
                this.moneyField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class cXMLRequestOrderRequestOrderRequestHeaderTotalMoney
    {

        private string currencyField;

        private decimal valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string currency
        {
            get
            {
                return this.currencyField;
            }
            set
            {
                this.currencyField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public decimal Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class cXMLRequestOrderRequestOrderRequestHeaderShipTo
    {

        private cXMLRequestOrderRequestOrderRequestHeaderShipToAddress addressField;

        /// <remarks/>
        public cXMLRequestOrderRequestOrderRequestHeaderShipToAddress Address
        {
            get
            {
                return this.addressField;
            }
            set
            {
                this.addressField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class cXMLRequestOrderRequestOrderRequestHeaderShipToAddress
    {

        private cXMLRequestOrderRequestOrderRequestHeaderShipToAddressName nameField;

        private cXMLRequestOrderRequestOrderRequestHeaderShipToAddressPostalAddress postalAddressField;

        private string emailField;

        private cXMLRequestOrderRequestOrderRequestHeaderShipToAddressPhone phoneField;

        private string isoCountryCodeField;

        private uint addressIDField;

        /// <remarks/>
        public cXMLRequestOrderRequestOrderRequestHeaderShipToAddressName Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        public cXMLRequestOrderRequestOrderRequestHeaderShipToAddressPostalAddress PostalAddress
        {
            get
            {
                return this.postalAddressField;
            }
            set
            {
                this.postalAddressField = value;
            }
        }

        /// <remarks/>
        public string Email
        {
            get
            {
                return this.emailField;
            }
            set
            {
                this.emailField = value;
            }
        }

        /// <remarks/>
        public cXMLRequestOrderRequestOrderRequestHeaderShipToAddressPhone Phone
        {
            get
            {
                return this.phoneField;
            }
            set
            {
                this.phoneField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string isoCountryCode
        {
            get
            {
                return this.isoCountryCodeField;
            }
            set
            {
                this.isoCountryCodeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public uint addressID
        {
            get
            {
                return this.addressIDField;
            }
            set
            {
                this.addressIDField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class cXMLRequestOrderRequestOrderRequestHeaderShipToAddressName
    {

        private string langField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = "http://www.w3.org/XML/1998/namespace")]
        public string lang
        {
            get
            {
                return this.langField;
            }
            set
            {
                this.langField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class cXMLRequestOrderRequestOrderRequestHeaderShipToAddressPostalAddress
    {

        private string deliverToField;

        private string[] streetField;

        private string cityField;

        private string stateField;

        private uint postalCodeField;

        private cXMLRequestOrderRequestOrderRequestHeaderShipToAddressPostalAddressCountry countryField;

        /// <remarks/>
        public string DeliverTo
        {
            get
            {
                return this.deliverToField;
            }
            set
            {
                this.deliverToField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Street")]
        public string[] Street
        {
            get
            {
                return this.streetField;
            }
            set
            {
                this.streetField = value;
            }
        }

        /// <remarks/>
        public string City
        {
            get
            {
                return this.cityField;
            }
            set
            {
                this.cityField = value;
            }
        }

        /// <remarks/>
        public string State
        {
            get
            {
                return this.stateField;
            }
            set
            {
                this.stateField = value;
            }
        }

        /// <remarks/>
        public uint PostalCode
        {
            get
            {
                return this.postalCodeField;
            }
            set
            {
                this.postalCodeField = value;
            }
        }

        /// <remarks/>
        public cXMLRequestOrderRequestOrderRequestHeaderShipToAddressPostalAddressCountry Country
        {
            get
            {
                return this.countryField;
            }
            set
            {
                this.countryField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class cXMLRequestOrderRequestOrderRequestHeaderShipToAddressPostalAddressCountry
    {

        private string isoCountryCodeField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string isoCountryCode
        {
            get
            {
                return this.isoCountryCodeField;
            }
            set
            {
                this.isoCountryCodeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class cXMLRequestOrderRequestOrderRequestHeaderShipToAddressPhone
    {

        private cXMLRequestOrderRequestOrderRequestHeaderShipToAddressPhoneTelephoneNumber telephoneNumberField;

        /// <remarks/>
        public cXMLRequestOrderRequestOrderRequestHeaderShipToAddressPhoneTelephoneNumber TelephoneNumber
        {
            get
            {
                return this.telephoneNumberField;
            }
            set
            {
                this.telephoneNumberField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class cXMLRequestOrderRequestOrderRequestHeaderShipToAddressPhoneTelephoneNumber
    {

        private cXMLRequestOrderRequestOrderRequestHeaderShipToAddressPhoneTelephoneNumberCountryCode countryCodeField;

        private object areaOrCityCodeField;

        private object numberField;

        /// <remarks/>
        public cXMLRequestOrderRequestOrderRequestHeaderShipToAddressPhoneTelephoneNumberCountryCode CountryCode
        {
            get
            {
                return this.countryCodeField;
            }
            set
            {
                this.countryCodeField = value;
            }
        }

        /// <remarks/>
        public object AreaOrCityCode
        {
            get
            {
                return this.areaOrCityCodeField;
            }
            set
            {
                this.areaOrCityCodeField = value;
            }
        }

        /// <remarks/>
        public object Number
        {
            get
            {
                return this.numberField;
            }
            set
            {
                this.numberField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class cXMLRequestOrderRequestOrderRequestHeaderShipToAddressPhoneTelephoneNumberCountryCode
    {

        private string isoCountryCodeField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string isoCountryCode
        {
            get
            {
                return this.isoCountryCodeField;
            }
            set
            {
                this.isoCountryCodeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class cXMLRequestOrderRequestOrderRequestHeaderBillTo
    {

        private cXMLRequestOrderRequestOrderRequestHeaderBillToAddress addressField;

        /// <remarks/>
        public cXMLRequestOrderRequestOrderRequestHeaderBillToAddress Address
        {
            get
            {
                return this.addressField;
            }
            set
            {
                this.addressField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class cXMLRequestOrderRequestOrderRequestHeaderBillToAddress
    {

        private cXMLRequestOrderRequestOrderRequestHeaderBillToAddressName nameField;

        private cXMLRequestOrderRequestOrderRequestHeaderBillToAddressPostalAddress postalAddressField;

        private cXMLRequestOrderRequestOrderRequestHeaderBillToAddressPhone phoneField;

        private string isoCountryCodeField;

        private string addressIDField;

        /// <remarks/>
        public cXMLRequestOrderRequestOrderRequestHeaderBillToAddressName Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        public cXMLRequestOrderRequestOrderRequestHeaderBillToAddressPostalAddress PostalAddress
        {
            get
            {
                return this.postalAddressField;
            }
            set
            {
                this.postalAddressField = value;
            }
        }

        /// <remarks/>
        public cXMLRequestOrderRequestOrderRequestHeaderBillToAddressPhone Phone
        {
            get
            {
                return this.phoneField;
            }
            set
            {
                this.phoneField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string isoCountryCode
        {
            get
            {
                return this.isoCountryCodeField;
            }
            set
            {
                this.isoCountryCodeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string addressID
        {
            get
            {
                return this.addressIDField;
            }
            set
            {
                this.addressIDField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class cXMLRequestOrderRequestOrderRequestHeaderBillToAddressName
    {

        private string langField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = "http://www.w3.org/XML/1998/namespace")]
        public string lang
        {
            get
            {
                return this.langField;
            }
            set
            {
                this.langField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class cXMLRequestOrderRequestOrderRequestHeaderBillToAddressPostalAddress
    {

        private string[] streetField;

        private string cityField;

        private string stateField;

        private uint postalCodeField;

        private cXMLRequestOrderRequestOrderRequestHeaderBillToAddressPostalAddressCountry countryField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Street")]
        public string[] Street
        {
            get
            {
                return this.streetField;
            }
            set
            {
                this.streetField = value;
            }
        }

        /// <remarks/>
        public string City
        {
            get
            {
                return this.cityField;
            }
            set
            {
                this.cityField = value;
            }
        }

        /// <remarks/>
        public string State
        {
            get
            {
                return this.stateField;
            }
            set
            {
                this.stateField = value;
            }
        }

        /// <remarks/>
        public uint PostalCode
        {
            get
            {
                return this.postalCodeField;
            }
            set
            {
                this.postalCodeField = value;
            }
        }

        /// <remarks/>
        public cXMLRequestOrderRequestOrderRequestHeaderBillToAddressPostalAddressCountry Country
        {
            get
            {
                return this.countryField;
            }
            set
            {
                this.countryField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class cXMLRequestOrderRequestOrderRequestHeaderBillToAddressPostalAddressCountry
    {

        private string isoCountryCodeField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string isoCountryCode
        {
            get
            {
                return this.isoCountryCodeField;
            }
            set
            {
                this.isoCountryCodeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class cXMLRequestOrderRequestOrderRequestHeaderBillToAddressPhone
    {

        private cXMLRequestOrderRequestOrderRequestHeaderBillToAddressPhoneTelephoneNumber telephoneNumberField;

        /// <remarks/>
        public cXMLRequestOrderRequestOrderRequestHeaderBillToAddressPhoneTelephoneNumber TelephoneNumber
        {
            get
            {
                return this.telephoneNumberField;
            }
            set
            {
                this.telephoneNumberField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class cXMLRequestOrderRequestOrderRequestHeaderBillToAddressPhoneTelephoneNumber
    {

        private cXMLRequestOrderRequestOrderRequestHeaderBillToAddressPhoneTelephoneNumberCountryCode countryCodeField;

        private object areaOrCityCodeField;

        private object numberField;

        /// <remarks/>
        public cXMLRequestOrderRequestOrderRequestHeaderBillToAddressPhoneTelephoneNumberCountryCode CountryCode
        {
            get
            {
                return this.countryCodeField;
            }
            set
            {
                this.countryCodeField = value;
            }
        }

        /// <remarks/>
        public object AreaOrCityCode
        {
            get
            {
                return this.areaOrCityCodeField;
            }
            set
            {
                this.areaOrCityCodeField = value;
            }
        }

        /// <remarks/>
        public object Number
        {
            get
            {
                return this.numberField;
            }
            set
            {
                this.numberField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class cXMLRequestOrderRequestOrderRequestHeaderBillToAddressPhoneTelephoneNumberCountryCode
    {

        private string isoCountryCodeField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string isoCountryCode
        {
            get
            {
                return this.isoCountryCodeField;
            }
            set
            {
                this.isoCountryCodeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class cXMLRequestOrderRequestOrderRequestHeaderShipping
    {

        private cXMLRequestOrderRequestOrderRequestHeaderShippingMoney moneyField;

        private cXMLRequestOrderRequestOrderRequestHeaderShippingDescription descriptionField;

        private string trackingDomainField;

        /// <remarks/>
        public cXMLRequestOrderRequestOrderRequestHeaderShippingMoney Money
        {
            get
            {
                return this.moneyField;
            }
            set
            {
                this.moneyField = value;
            }
        }

        /// <remarks/>
        public cXMLRequestOrderRequestOrderRequestHeaderShippingDescription Description
        {
            get
            {
                return this.descriptionField;
            }
            set
            {
                this.descriptionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string trackingDomain
        {
            get
            {
                return this.trackingDomainField;
            }
            set
            {
                this.trackingDomainField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class cXMLRequestOrderRequestOrderRequestHeaderShippingMoney
    {

        private string currencyField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string currency
        {
            get
            {
                return this.currencyField;
            }
            set
            {
                this.currencyField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class cXMLRequestOrderRequestOrderRequestHeaderShippingDescription
    {

        private string langField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = "http://www.w3.org/XML/1998/namespace")]
        public string lang
        {
            get
            {
                return this.langField;
            }
            set
            {
                this.langField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class cXMLRequestOrderRequestOrderRequestHeaderContact
    {

        private cXMLRequestOrderRequestOrderRequestHeaderContactName nameField;

        private cXMLRequestOrderRequestOrderRequestHeaderContactPostalAddress postalAddressField;

        private string emailField;

        private cXMLRequestOrderRequestOrderRequestHeaderContactPhone phoneField;

        private string roleField;

        /// <remarks/>
        public cXMLRequestOrderRequestOrderRequestHeaderContactName Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        public cXMLRequestOrderRequestOrderRequestHeaderContactPostalAddress PostalAddress
        {
            get
            {
                return this.postalAddressField;
            }
            set
            {
                this.postalAddressField = value;
            }
        }

        /// <remarks/>
        public string Email
        {
            get
            {
                return this.emailField;
            }
            set
            {
                this.emailField = value;
            }
        }

        /// <remarks/>
        public cXMLRequestOrderRequestOrderRequestHeaderContactPhone Phone
        {
            get
            {
                return this.phoneField;
            }
            set
            {
                this.phoneField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string role
        {
            get
            {
                return this.roleField;
            }
            set
            {
                this.roleField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class cXMLRequestOrderRequestOrderRequestHeaderContactName
    {

        private string langField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = "http://www.w3.org/XML/1998/namespace")]
        public string lang
        {
            get
            {
                return this.langField;
            }
            set
            {
                this.langField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class cXMLRequestOrderRequestOrderRequestHeaderContactPostalAddress
    {

        private string[] streetField;

        private string cityField;

        private object stateField;

        private uint postalCodeField;

        private cXMLRequestOrderRequestOrderRequestHeaderContactPostalAddressCountry countryField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Street")]
        public string[] Street
        {
            get
            {
                return this.streetField;
            }
            set
            {
                this.streetField = value;
            }
        }

        /// <remarks/>
        public string City
        {
            get
            {
                return this.cityField;
            }
            set
            {
                this.cityField = value;
            }
        }

        /// <remarks/>
        public object State
        {
            get
            {
                return this.stateField;
            }
            set
            {
                this.stateField = value;
            }
        }

        /// <remarks/>
        public uint PostalCode
        {
            get
            {
                return this.postalCodeField;
            }
            set
            {
                this.postalCodeField = value;
            }
        }

        /// <remarks/>
        public cXMLRequestOrderRequestOrderRequestHeaderContactPostalAddressCountry Country
        {
            get
            {
                return this.countryField;
            }
            set
            {
                this.countryField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class cXMLRequestOrderRequestOrderRequestHeaderContactPostalAddressCountry
    {

        private string isoCountryCodeField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string isoCountryCode
        {
            get
            {
                return this.isoCountryCodeField;
            }
            set
            {
                this.isoCountryCodeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class cXMLRequestOrderRequestOrderRequestHeaderContactPhone
    {

        private cXMLRequestOrderRequestOrderRequestHeaderContactPhoneTelephoneNumber telephoneNumberField;

        /// <remarks/>
        public cXMLRequestOrderRequestOrderRequestHeaderContactPhoneTelephoneNumber TelephoneNumber
        {
            get
            {
                return this.telephoneNumberField;
            }
            set
            {
                this.telephoneNumberField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class cXMLRequestOrderRequestOrderRequestHeaderContactPhoneTelephoneNumber
    {

        private cXMLRequestOrderRequestOrderRequestHeaderContactPhoneTelephoneNumberCountryCode countryCodeField;

        private object areaOrCityCodeField;

        private object numberField;

        /// <remarks/>
        public cXMLRequestOrderRequestOrderRequestHeaderContactPhoneTelephoneNumberCountryCode CountryCode
        {
            get
            {
                return this.countryCodeField;
            }
            set
            {
                this.countryCodeField = value;
            }
        }

        /// <remarks/>
        public object AreaOrCityCode
        {
            get
            {
                return this.areaOrCityCodeField;
            }
            set
            {
                this.areaOrCityCodeField = value;
            }
        }

        /// <remarks/>
        public object Number
        {
            get
            {
                return this.numberField;
            }
            set
            {
                this.numberField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class cXMLRequestOrderRequestOrderRequestHeaderContactPhoneTelephoneNumberCountryCode
    {

        private string isoCountryCodeField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string isoCountryCode
        {
            get
            {
                return this.isoCountryCodeField;
            }
            set
            {
                this.isoCountryCodeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class cXMLRequestOrderRequestOrderRequestHeaderComments
    {

        private cXMLRequestOrderRequestOrderRequestHeaderCommentsAttachment attachmentField;

        /// <remarks/>
        public cXMLRequestOrderRequestOrderRequestHeaderCommentsAttachment Attachment
        {
            get
            {
                return this.attachmentField;
            }
            set
            {
                this.attachmentField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class cXMLRequestOrderRequestOrderRequestHeaderCommentsAttachment
    {

        private cXMLRequestOrderRequestOrderRequestHeaderCommentsAttachmentURL uRLField;

        /// <remarks/>
        public cXMLRequestOrderRequestOrderRequestHeaderCommentsAttachmentURL URL
        {
            get
            {
                return this.uRLField;
            }
            set
            {
                this.uRLField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class cXMLRequestOrderRequestOrderRequestHeaderCommentsAttachmentURL
    {

        private string nameField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class cXMLRequestOrderRequestOrderRequestHeaderExtrinsic
    {

        private string nameField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class cXMLRequestOrderRequestItemOut
    {

        private cXMLRequestOrderRequestItemOutItemID itemIDField;

        private cXMLRequestOrderRequestItemOutItemDetail itemDetailField;

        private cXMLRequestOrderRequestItemOutSupplierList supplierListField;

        private cXMLRequestOrderRequestItemOutShipTo shipToField;

        private cXMLRequestOrderRequestItemOutTax taxField;

        private byte quantityField;

        private byte lineNumberField;

        private System.DateTime requestedDeliveryDateField;

        /// <remarks/>
        public cXMLRequestOrderRequestItemOutItemID ItemID
        {
            get
            {
                return this.itemIDField;
            }
            set
            {
                this.itemIDField = value;
            }
        }

        /// <remarks/>
        public cXMLRequestOrderRequestItemOutItemDetail ItemDetail
        {
            get
            {
                return this.itemDetailField;
            }
            set
            {
                this.itemDetailField = value;
            }
        }

        /// <remarks/>
        public cXMLRequestOrderRequestItemOutSupplierList SupplierList
        {
            get
            {
                return this.supplierListField;
            }
            set
            {
                this.supplierListField = value;
            }
        }

        /// <remarks/>
        public cXMLRequestOrderRequestItemOutShipTo ShipTo
        {
            get
            {
                return this.shipToField;
            }
            set
            {
                this.shipToField = value;
            }
        }

        /// <remarks/>
        public cXMLRequestOrderRequestItemOutTax Tax
        {
            get
            {
                return this.taxField;
            }
            set
            {
                this.taxField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte quantity
        {
            get
            {
                return this.quantityField;
            }
            set
            {
                this.quantityField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte lineNumber
        {
            get
            {
                return this.lineNumberField;
            }
            set
            {
                this.lineNumberField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public System.DateTime requestedDeliveryDate
        {
            get
            {
                return this.requestedDeliveryDateField;
            }
            set
            {
                this.requestedDeliveryDateField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class cXMLRequestOrderRequestItemOutItemID
    {

        private string supplierPartIDField;

        private string supplierPartAuxiliaryIDField;

        /// <remarks/>
        public string SupplierPartID
        {
            get
            {
                return this.supplierPartIDField;
            }
            set
            {
                this.supplierPartIDField = value;
            }
        }

        /// <remarks/>
        public string SupplierPartAuxiliaryID
        {
            get
            {
                return this.supplierPartAuxiliaryIDField;
            }
            set
            {
                this.supplierPartAuxiliaryIDField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class cXMLRequestOrderRequestItemOutItemDetail
    {

        private cXMLRequestOrderRequestItemOutItemDetailUnitPrice unitPriceField;

        private cXMLRequestOrderRequestItemOutItemDetailDescription descriptionField;

        private string unitOfMeasureField;

        private cXMLRequestOrderRequestItemOutItemDetailClassification classificationField;

        private cXMLRequestOrderRequestItemOutItemDetailExtrinsic[] extrinsicField;

        /// <remarks/>
        public cXMLRequestOrderRequestItemOutItemDetailUnitPrice UnitPrice
        {
            get
            {
                return this.unitPriceField;
            }
            set
            {
                this.unitPriceField = value;
            }
        }

        /// <remarks/>
        public cXMLRequestOrderRequestItemOutItemDetailDescription Description
        {
            get
            {
                return this.descriptionField;
            }
            set
            {
                this.descriptionField = value;
            }
        }

        /// <remarks/>
        public string UnitOfMeasure
        {
            get
            {
                return this.unitOfMeasureField;
            }
            set
            {
                this.unitOfMeasureField = value;
            }
        }

        /// <remarks/>
        public cXMLRequestOrderRequestItemOutItemDetailClassification Classification
        {
            get
            {
                return this.classificationField;
            }
            set
            {
                this.classificationField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Extrinsic")]
        public cXMLRequestOrderRequestItemOutItemDetailExtrinsic[] Extrinsic
        {
            get
            {
                return this.extrinsicField;
            }
            set
            {
                this.extrinsicField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class cXMLRequestOrderRequestItemOutItemDetailUnitPrice
    {

        private cXMLRequestOrderRequestItemOutItemDetailUnitPriceMoney moneyField;

        /// <remarks/>
        public cXMLRequestOrderRequestItemOutItemDetailUnitPriceMoney Money
        {
            get
            {
                return this.moneyField;
            }
            set
            {
                this.moneyField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class cXMLRequestOrderRequestItemOutItemDetailUnitPriceMoney
    {

        private string currencyField;

        private decimal valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string currency
        {
            get
            {
                return this.currencyField;
            }
            set
            {
                this.currencyField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public decimal Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class cXMLRequestOrderRequestItemOutItemDetailDescription
    {

        private string langField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = "http://www.w3.org/XML/1998/namespace")]
        public string lang
        {
            get
            {
                return this.langField;
            }
            set
            {
                this.langField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class cXMLRequestOrderRequestItemOutItemDetailClassification
    {

        private string domainField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string domain
        {
            get
            {
                return this.domainField;
            }
            set
            {
                this.domainField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class cXMLRequestOrderRequestItemOutItemDetailExtrinsic
    {

        private string nameField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class cXMLRequestOrderRequestItemOutSupplierList
    {

        private cXMLRequestOrderRequestItemOutSupplierListSupplier supplierField;

        /// <remarks/>
        public cXMLRequestOrderRequestItemOutSupplierListSupplier Supplier
        {
            get
            {
                return this.supplierField;
            }
            set
            {
                this.supplierField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class cXMLRequestOrderRequestItemOutSupplierListSupplier
    {

        private cXMLRequestOrderRequestItemOutSupplierListSupplierName nameField;

        private cXMLRequestOrderRequestItemOutSupplierListSupplierSupplierID supplierIDField;

        private cXMLRequestOrderRequestItemOutSupplierListSupplierSupplierLocation supplierLocationField;

        /// <remarks/>
        public cXMLRequestOrderRequestItemOutSupplierListSupplierName Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        public cXMLRequestOrderRequestItemOutSupplierListSupplierSupplierID SupplierID
        {
            get
            {
                return this.supplierIDField;
            }
            set
            {
                this.supplierIDField = value;
            }
        }

        /// <remarks/>
        public cXMLRequestOrderRequestItemOutSupplierListSupplierSupplierLocation SupplierLocation
        {
            get
            {
                return this.supplierLocationField;
            }
            set
            {
                this.supplierLocationField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class cXMLRequestOrderRequestItemOutSupplierListSupplierName
    {

        private string langField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = "http://www.w3.org/XML/1998/namespace")]
        public string lang
        {
            get
            {
                return this.langField;
            }
            set
            {
                this.langField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class cXMLRequestOrderRequestItemOutSupplierListSupplierSupplierID
    {

        private string domainField;

        private ulong valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string domain
        {
            get
            {
                return this.domainField;
            }
            set
            {
                this.domainField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public ulong Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class cXMLRequestOrderRequestItemOutSupplierListSupplierSupplierLocation
    {

        private cXMLRequestOrderRequestItemOutSupplierListSupplierSupplierLocationAddress addressField;

        private cXMLRequestOrderRequestItemOutSupplierListSupplierSupplierLocationOrderMethods orderMethodsField;

        /// <remarks/>
        public cXMLRequestOrderRequestItemOutSupplierListSupplierSupplierLocationAddress Address
        {
            get
            {
                return this.addressField;
            }
            set
            {
                this.addressField = value;
            }
        }

        /// <remarks/>
        public cXMLRequestOrderRequestItemOutSupplierListSupplierSupplierLocationOrderMethods OrderMethods
        {
            get
            {
                return this.orderMethodsField;
            }
            set
            {
                this.orderMethodsField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class cXMLRequestOrderRequestItemOutSupplierListSupplierSupplierLocationAddress
    {

        private cXMLRequestOrderRequestItemOutSupplierListSupplierSupplierLocationAddressName nameField;

        private cXMLRequestOrderRequestItemOutSupplierListSupplierSupplierLocationAddressPostalAddress postalAddressField;

        private string emailField;

        private cXMLRequestOrderRequestItemOutSupplierListSupplierSupplierLocationAddressPhone phoneField;

        private cXMLRequestOrderRequestItemOutSupplierListSupplierSupplierLocationAddressFax faxField;

        private string isoCountryCodeField;

        /// <remarks/>
        public cXMLRequestOrderRequestItemOutSupplierListSupplierSupplierLocationAddressName Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        public cXMLRequestOrderRequestItemOutSupplierListSupplierSupplierLocationAddressPostalAddress PostalAddress
        {
            get
            {
                return this.postalAddressField;
            }
            set
            {
                this.postalAddressField = value;
            }
        }

        /// <remarks/>
        public string Email
        {
            get
            {
                return this.emailField;
            }
            set
            {
                this.emailField = value;
            }
        }

        /// <remarks/>
        public cXMLRequestOrderRequestItemOutSupplierListSupplierSupplierLocationAddressPhone Phone
        {
            get
            {
                return this.phoneField;
            }
            set
            {
                this.phoneField = value;
            }
        }

        /// <remarks/>
        public cXMLRequestOrderRequestItemOutSupplierListSupplierSupplierLocationAddressFax Fax
        {
            get
            {
                return this.faxField;
            }
            set
            {
                this.faxField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string isoCountryCode
        {
            get
            {
                return this.isoCountryCodeField;
            }
            set
            {
                this.isoCountryCodeField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class cXMLRequestOrderRequestItemOutSupplierListSupplierSupplierLocationAddressName
    {

        private string langField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = "http://www.w3.org/XML/1998/namespace")]
        public string lang
        {
            get
            {
                return this.langField;
            }
            set
            {
                this.langField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class cXMLRequestOrderRequestItemOutSupplierListSupplierSupplierLocationAddressPostalAddress
    {

        private string[] streetField;

        private string cityField;

        private string stateField;

        private uint postalCodeField;

        private cXMLRequestOrderRequestItemOutSupplierListSupplierSupplierLocationAddressPostalAddressCountry countryField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Street")]
        public string[] Street
        {
            get
            {
                return this.streetField;
            }
            set
            {
                this.streetField = value;
            }
        }

        /// <remarks/>
        public string City
        {
            get
            {
                return this.cityField;
            }
            set
            {
                this.cityField = value;
            }
        }

        /// <remarks/>
        public string State
        {
            get
            {
                return this.stateField;
            }
            set
            {
                this.stateField = value;
            }
        }

        /// <remarks/>
        public uint PostalCode
        {
            get
            {
                return this.postalCodeField;
            }
            set
            {
                this.postalCodeField = value;
            }
        }

        /// <remarks/>
        public cXMLRequestOrderRequestItemOutSupplierListSupplierSupplierLocationAddressPostalAddressCountry Country
        {
            get
            {
                return this.countryField;
            }
            set
            {
                this.countryField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class cXMLRequestOrderRequestItemOutSupplierListSupplierSupplierLocationAddressPostalAddressCountry
    {

        private string isoCountryCodeField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string isoCountryCode
        {
            get
            {
                return this.isoCountryCodeField;
            }
            set
            {
                this.isoCountryCodeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class cXMLRequestOrderRequestItemOutSupplierListSupplierSupplierLocationAddressPhone
    {

        private cXMLRequestOrderRequestItemOutSupplierListSupplierSupplierLocationAddressPhoneTelephoneNumber telephoneNumberField;

        /// <remarks/>
        public cXMLRequestOrderRequestItemOutSupplierListSupplierSupplierLocationAddressPhoneTelephoneNumber TelephoneNumber
        {
            get
            {
                return this.telephoneNumberField;
            }
            set
            {
                this.telephoneNumberField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class cXMLRequestOrderRequestItemOutSupplierListSupplierSupplierLocationAddressPhoneTelephoneNumber
    {

        private cXMLRequestOrderRequestItemOutSupplierListSupplierSupplierLocationAddressPhoneTelephoneNumberCountryCode countryCodeField;

        private object areaOrCityCodeField;

        private object numberField;

        /// <remarks/>
        public cXMLRequestOrderRequestItemOutSupplierListSupplierSupplierLocationAddressPhoneTelephoneNumberCountryCode CountryCode
        {
            get
            {
                return this.countryCodeField;
            }
            set
            {
                this.countryCodeField = value;
            }
        }

        /// <remarks/>
        public object AreaOrCityCode
        {
            get
            {
                return this.areaOrCityCodeField;
            }
            set
            {
                this.areaOrCityCodeField = value;
            }
        }

        /// <remarks/>
        public object Number
        {
            get
            {
                return this.numberField;
            }
            set
            {
                this.numberField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class cXMLRequestOrderRequestItemOutSupplierListSupplierSupplierLocationAddressPhoneTelephoneNumberCountryCode
    {

        private string isoCountryCodeField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string isoCountryCode
        {
            get
            {
                return this.isoCountryCodeField;
            }
            set
            {
                this.isoCountryCodeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class cXMLRequestOrderRequestItemOutSupplierListSupplierSupplierLocationAddressFax
    {

        private cXMLRequestOrderRequestItemOutSupplierListSupplierSupplierLocationAddressFaxTelephoneNumber telephoneNumberField;

        /// <remarks/>
        public cXMLRequestOrderRequestItemOutSupplierListSupplierSupplierLocationAddressFaxTelephoneNumber TelephoneNumber
        {
            get
            {
                return this.telephoneNumberField;
            }
            set
            {
                this.telephoneNumberField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class cXMLRequestOrderRequestItemOutSupplierListSupplierSupplierLocationAddressFaxTelephoneNumber
    {

        private cXMLRequestOrderRequestItemOutSupplierListSupplierSupplierLocationAddressFaxTelephoneNumberCountryCode countryCodeField;

        private object areaOrCityCodeField;

        private object numberField;

        /// <remarks/>
        public cXMLRequestOrderRequestItemOutSupplierListSupplierSupplierLocationAddressFaxTelephoneNumberCountryCode CountryCode
        {
            get
            {
                return this.countryCodeField;
            }
            set
            {
                this.countryCodeField = value;
            }
        }

        /// <remarks/>
        public object AreaOrCityCode
        {
            get
            {
                return this.areaOrCityCodeField;
            }
            set
            {
                this.areaOrCityCodeField = value;
            }
        }

        /// <remarks/>
        public object Number
        {
            get
            {
                return this.numberField;
            }
            set
            {
                this.numberField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class cXMLRequestOrderRequestItemOutSupplierListSupplierSupplierLocationAddressFaxTelephoneNumberCountryCode
    {

        private string isoCountryCodeField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string isoCountryCode
        {
            get
            {
                return this.isoCountryCodeField;
            }
            set
            {
                this.isoCountryCodeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class cXMLRequestOrderRequestItemOutSupplierListSupplierSupplierLocationOrderMethods
    {

        private cXMLRequestOrderRequestItemOutSupplierListSupplierSupplierLocationOrderMethodsOrderMethod orderMethodField;

        private cXMLRequestOrderRequestItemOutSupplierListSupplierSupplierLocationOrderMethodsContact contactField;

        /// <remarks/>
        public cXMLRequestOrderRequestItemOutSupplierListSupplierSupplierLocationOrderMethodsOrderMethod OrderMethod
        {
            get
            {
                return this.orderMethodField;
            }
            set
            {
                this.orderMethodField = value;
            }
        }

        /// <remarks/>
        public cXMLRequestOrderRequestItemOutSupplierListSupplierSupplierLocationOrderMethodsContact Contact
        {
            get
            {
                return this.contactField;
            }
            set
            {
                this.contactField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class cXMLRequestOrderRequestItemOutSupplierListSupplierSupplierLocationOrderMethodsOrderMethod
    {

        private cXMLRequestOrderRequestItemOutSupplierListSupplierSupplierLocationOrderMethodsOrderMethodOrderTarget orderTargetField;

        /// <remarks/>
        public cXMLRequestOrderRequestItemOutSupplierListSupplierSupplierLocationOrderMethodsOrderMethodOrderTarget OrderTarget
        {
            get
            {
                return this.orderTargetField;
            }
            set
            {
                this.orderTargetField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class cXMLRequestOrderRequestItemOutSupplierListSupplierSupplierLocationOrderMethodsOrderMethodOrderTarget
    {

        private string otherOrderTargetField;

        /// <remarks/>
        public string OtherOrderTarget
        {
            get
            {
                return this.otherOrderTargetField;
            }
            set
            {
                this.otherOrderTargetField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class cXMLRequestOrderRequestItemOutSupplierListSupplierSupplierLocationOrderMethodsContact
    {

        private cXMLRequestOrderRequestItemOutSupplierListSupplierSupplierLocationOrderMethodsContactName nameField;

        private string emailField;

        private cXMLRequestOrderRequestItemOutSupplierListSupplierSupplierLocationOrderMethodsContactPhone phoneField;

        /// <remarks/>
        public cXMLRequestOrderRequestItemOutSupplierListSupplierSupplierLocationOrderMethodsContactName Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        public string Email
        {
            get
            {
                return this.emailField;
            }
            set
            {
                this.emailField = value;
            }
        }

        /// <remarks/>
        public cXMLRequestOrderRequestItemOutSupplierListSupplierSupplierLocationOrderMethodsContactPhone Phone
        {
            get
            {
                return this.phoneField;
            }
            set
            {
                this.phoneField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class cXMLRequestOrderRequestItemOutSupplierListSupplierSupplierLocationOrderMethodsContactName
    {

        private string langField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = "http://www.w3.org/XML/1998/namespace")]
        public string lang
        {
            get
            {
                return this.langField;
            }
            set
            {
                this.langField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class cXMLRequestOrderRequestItemOutSupplierListSupplierSupplierLocationOrderMethodsContactPhone
    {

        private cXMLRequestOrderRequestItemOutSupplierListSupplierSupplierLocationOrderMethodsContactPhoneTelephoneNumber telephoneNumberField;

        /// <remarks/>
        public cXMLRequestOrderRequestItemOutSupplierListSupplierSupplierLocationOrderMethodsContactPhoneTelephoneNumber TelephoneNumber
        {
            get
            {
                return this.telephoneNumberField;
            }
            set
            {
                this.telephoneNumberField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class cXMLRequestOrderRequestItemOutSupplierListSupplierSupplierLocationOrderMethodsContactPhoneTelephoneNumber
    {

        private cXMLRequestOrderRequestItemOutSupplierListSupplierSupplierLocationOrderMethodsContactPhoneTelephoneNumberCountryCode countryCodeField;

        private object areaOrCityCodeField;

        private object numberField;

        /// <remarks/>
        public cXMLRequestOrderRequestItemOutSupplierListSupplierSupplierLocationOrderMethodsContactPhoneTelephoneNumberCountryCode CountryCode
        {
            get
            {
                return this.countryCodeField;
            }
            set
            {
                this.countryCodeField = value;
            }
        }

        /// <remarks/>
        public object AreaOrCityCode
        {
            get
            {
                return this.areaOrCityCodeField;
            }
            set
            {
                this.areaOrCityCodeField = value;
            }
        }

        /// <remarks/>
        public object Number
        {
            get
            {
                return this.numberField;
            }
            set
            {
                this.numberField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class cXMLRequestOrderRequestItemOutSupplierListSupplierSupplierLocationOrderMethodsContactPhoneTelephoneNumberCountryCode
    {

        private string isoCountryCodeField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string isoCountryCode
        {
            get
            {
                return this.isoCountryCodeField;
            }
            set
            {
                this.isoCountryCodeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class cXMLRequestOrderRequestItemOutShipTo
    {

        private cXMLRequestOrderRequestItemOutShipToAddress addressField;

        /// <remarks/>
        public cXMLRequestOrderRequestItemOutShipToAddress Address
        {
            get
            {
                return this.addressField;
            }
            set
            {
                this.addressField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class cXMLRequestOrderRequestItemOutShipToAddress
    {

        private cXMLRequestOrderRequestItemOutShipToAddressName nameField;

        private cXMLRequestOrderRequestItemOutShipToAddressPostalAddress postalAddressField;

        private string emailField;

        private cXMLRequestOrderRequestItemOutShipToAddressPhone phoneField;

        private string isoCountryCodeField;

        private uint addressIDField;

        /// <remarks/>
        public cXMLRequestOrderRequestItemOutShipToAddressName Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        public cXMLRequestOrderRequestItemOutShipToAddressPostalAddress PostalAddress
        {
            get
            {
                return this.postalAddressField;
            }
            set
            {
                this.postalAddressField = value;
            }
        }

        /// <remarks/>
        public string Email
        {
            get
            {
                return this.emailField;
            }
            set
            {
                this.emailField = value;
            }
        }

        /// <remarks/>
        public cXMLRequestOrderRequestItemOutShipToAddressPhone Phone
        {
            get
            {
                return this.phoneField;
            }
            set
            {
                this.phoneField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string isoCountryCode
        {
            get
            {
                return this.isoCountryCodeField;
            }
            set
            {
                this.isoCountryCodeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public uint addressID
        {
            get
            {
                return this.addressIDField;
            }
            set
            {
                this.addressIDField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class cXMLRequestOrderRequestItemOutShipToAddressName
    {

        private string langField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = "http://www.w3.org/XML/1998/namespace")]
        public string lang
        {
            get
            {
                return this.langField;
            }
            set
            {
                this.langField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class cXMLRequestOrderRequestItemOutShipToAddressPostalAddress
    {

        private string deliverToField;

        private string[] streetField;

        private string cityField;

        private string stateField;

        private uint postalCodeField;

        private cXMLRequestOrderRequestItemOutShipToAddressPostalAddressCountry countryField;

        /// <remarks/>
        public string DeliverTo
        {
            get
            {
                return this.deliverToField;
            }
            set
            {
                this.deliverToField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Street")]
        public string[] Street
        {
            get
            {
                return this.streetField;
            }
            set
            {
                this.streetField = value;
            }
        }

        /// <remarks/>
        public string City
        {
            get
            {
                return this.cityField;
            }
            set
            {
                this.cityField = value;
            }
        }

        /// <remarks/>
        public string State
        {
            get
            {
                return this.stateField;
            }
            set
            {
                this.stateField = value;
            }
        }

        /// <remarks/>
        public uint PostalCode
        {
            get
            {
                return this.postalCodeField;
            }
            set
            {
                this.postalCodeField = value;
            }
        }

        /// <remarks/>
        public cXMLRequestOrderRequestItemOutShipToAddressPostalAddressCountry Country
        {
            get
            {
                return this.countryField;
            }
            set
            {
                this.countryField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class cXMLRequestOrderRequestItemOutShipToAddressPostalAddressCountry
    {

        private string isoCountryCodeField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string isoCountryCode
        {
            get
            {
                return this.isoCountryCodeField;
            }
            set
            {
                this.isoCountryCodeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class cXMLRequestOrderRequestItemOutShipToAddressPhone
    {

        private cXMLRequestOrderRequestItemOutShipToAddressPhoneTelephoneNumber telephoneNumberField;

        /// <remarks/>
        public cXMLRequestOrderRequestItemOutShipToAddressPhoneTelephoneNumber TelephoneNumber
        {
            get
            {
                return this.telephoneNumberField;
            }
            set
            {
                this.telephoneNumberField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class cXMLRequestOrderRequestItemOutShipToAddressPhoneTelephoneNumber
    {

        private cXMLRequestOrderRequestItemOutShipToAddressPhoneTelephoneNumberCountryCode countryCodeField;

        private object areaOrCityCodeField;

        private object numberField;

        /// <remarks/>
        public cXMLRequestOrderRequestItemOutShipToAddressPhoneTelephoneNumberCountryCode CountryCode
        {
            get
            {
                return this.countryCodeField;
            }
            set
            {
                this.countryCodeField = value;
            }
        }

        /// <remarks/>
        public object AreaOrCityCode
        {
            get
            {
                return this.areaOrCityCodeField;
            }
            set
            {
                this.areaOrCityCodeField = value;
            }
        }

        /// <remarks/>
        public object Number
        {
            get
            {
                return this.numberField;
            }
            set
            {
                this.numberField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class cXMLRequestOrderRequestItemOutShipToAddressPhoneTelephoneNumberCountryCode
    {

        private string isoCountryCodeField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string isoCountryCode
        {
            get
            {
                return this.isoCountryCodeField;
            }
            set
            {
                this.isoCountryCodeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class cXMLRequestOrderRequestItemOutTax
    {

        private cXMLRequestOrderRequestItemOutTaxMoney moneyField;

        private cXMLRequestOrderRequestItemOutTaxDescription descriptionField;

        private cXMLRequestOrderRequestItemOutTaxTaxDetail taxDetailField;

        /// <remarks/>
        public cXMLRequestOrderRequestItemOutTaxMoney Money
        {
            get
            {
                return this.moneyField;
            }
            set
            {
                this.moneyField = value;
            }
        }

        /// <remarks/>
        public cXMLRequestOrderRequestItemOutTaxDescription Description
        {
            get
            {
                return this.descriptionField;
            }
            set
            {
                this.descriptionField = value;
            }
        }

        /// <remarks/>
        public cXMLRequestOrderRequestItemOutTaxTaxDetail TaxDetail
        {
            get
            {
                return this.taxDetailField;
            }
            set
            {
                this.taxDetailField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class cXMLRequestOrderRequestItemOutTaxMoney
    {

        private string currencyField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string currency
        {
            get
            {
                return this.currencyField;
            }
            set
            {
                this.currencyField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class cXMLRequestOrderRequestItemOutTaxDescription
    {

        private string langField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = "http://www.w3.org/XML/1998/namespace")]
        public string lang
        {
            get
            {
                return this.langField;
            }
            set
            {
                this.langField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class cXMLRequestOrderRequestItemOutTaxTaxDetail
    {

        private cXMLRequestOrderRequestItemOutTaxTaxDetailTaxAmount taxAmountField;

        private string categoryField;

        /// <remarks/>
        public cXMLRequestOrderRequestItemOutTaxTaxDetailTaxAmount TaxAmount
        {
            get
            {
                return this.taxAmountField;
            }
            set
            {
                this.taxAmountField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string category
        {
            get
            {
                return this.categoryField;
            }
            set
            {
                this.categoryField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class cXMLRequestOrderRequestItemOutTaxTaxDetailTaxAmount
    {

        private cXMLRequestOrderRequestItemOutTaxTaxDetailTaxAmountMoney moneyField;

        /// <remarks/>
        public cXMLRequestOrderRequestItemOutTaxTaxDetailTaxAmountMoney Money
        {
            get
            {
                return this.moneyField;
            }
            set
            {
                this.moneyField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class cXMLRequestOrderRequestItemOutTaxTaxDetailTaxAmountMoney
    {

        private string currencyField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string currency
        {
            get
            {
                return this.currencyField;
            }
            set
            {
                this.currencyField = value;
            }
        }
    }
}
