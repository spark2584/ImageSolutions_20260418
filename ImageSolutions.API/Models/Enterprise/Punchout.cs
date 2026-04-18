using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ImageSolutions.API.Models.Enterprise
{
    public class Punchout
    {
    }

    public class PunchoutLogin
    {
        public Header header { get; set; }
        public string type { get; set; }
        public string operation { get; set; }
        public string mode { get; set; }
        public Body body { get; set; }
        public object[] custom { get; set; }
    }

    public class Header
    {
        public To to { get; set; }
        public From from { get; set; }
        public Sender sender { get; set; }
    }

    public class To
    {
        public object[] data { get; set; }
        public _0 _0 { get; set; }
    }

    public class _0
    {
        public object[] data { get; set; }
        public string value { get; set; }
        public string domain { get; set; }
    }

    public class From
    {
        public object[] data { get; set; }
        public _01 _0 { get; set; }
    }

    public class _01
    {
        public object[] data { get; set; }
        public string value { get; set; }
        public string domain { get; set; }
    }

    public class Sender
    {
        public Data data { get; set; }
        public _02 _0 { get; set; }
    }

    public class Data
    {
        public string UserAgent { get; set; }
    }

    public class _02
    {
        public Data1 data { get; set; }
        public string value { get; set; }
        public string domain { get; set; }
    }

    public class Data1
    {
        public string SharedSecret { get; set; }
    }

    public class Body
    {
        public Data2 data { get; set; }
        public Contact contact { get; set; }
        public string buyercookie { get; set; }
        public string postform { get; set; }
        public Shipping shipping { get; set; }
        public object[] items { get; set; }
    }

    public class Data2
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UniqueName { get; set; }
        public string UserEmail { get; set; }
        public string User { get; set; }
        public string employee_id { get; set; }
        public string group { get; set; }
        public string group_branch { get; set; }
        public string userbusinessunit { get; set; }
        public string BusinessUnit { get; set; }
    }

    public class Contact
    {
        public object[] data { get; set; }
        public string email { get; set; }
        public string name { get; set; }
        public string unique { get; set; }
    }

    public class Shipping
    {
        public Data3 data { get; set; }
    }

    public class Data3
    {
        public string address_name { get; set; }
        public string shipping_id { get; set; }
        public string shipping_business { get; set; }
        public string shipping_to { get; set; }
        public string shipping_street { get; set; }
        public string shipping_city { get; set; }
        public string shipping_state { get; set; }
        public string shipping_zip { get; set; }
        public string shipping_country { get; set; }
        public string country_id { get; set; }
    }

}