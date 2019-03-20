using Microsoft.Xrm.Sdk;
using System;
using System.Linq;
using xrm = SpirePlusPlugin;

namespace Spire_BusinessEntities
{
    public class Spire_Xrm_Base
    {
        protected xrm.XrmServiceContext ctx;
        protected string name;
        protected Guid id;
        protected bool logEnabled = false;

        public Spire_Xrm_Base()
        {
        }

        public void Init(xrm.XrmServiceContext ctx, string PrimaryEntityName, Guid PrimaryEntityId)
        {
            this.ctx = ctx;
            name = PrimaryEntityName;
            id = PrimaryEntityId;
            logEnabled = GetLogEnable();
        }

        public bool GetLogEnable()
        {
            var EnableLog = ctx.tsp_configurationSet.Where(c => c.tsp_name == "DoLogFile").Select(c => c.tsp_Value).FirstOrDefault();

            if (EnableLog == "1") return true;
            else return false;


        }

        public void Init(xrm.XrmServiceContext ctx)
        {
            this.ctx = ctx;
            logEnabled = GetLogEnable();
        }

        public bool _logMessage(string message)
        {
            if (!GetLogEnable()) return true;

            if (this.id != Guid.Empty)
            {
                var xrmNotes = new xrm.Annotation()
                {
                    NoteText = message,
                    Subject = "Message from workflow",
                    ObjectId = new EntityReference(name, this.id),
                    ObjectTypeCode = this.name
                };
                ctx.AddObject(xrmNotes);
                ctx.SaveChanges();
            }
            else
            {
                var xrmNotes = new xrm.Annotation()
                {
                    NoteText = message,
                    Subject = "Message from workflow",

                };
                ctx.AddObject(xrmNotes);
                ctx.SaveChanges();
            }

            return true;
        }
        public bool _logMessage(string message, string pSubject)
        {
            var xrmNotes = new xrm.Annotation()
            {
                NoteText = message,
                Subject = pSubject,

            };
            ctx.AddObject(xrmNotes);
            ctx.SaveChanges();
            return true;
        }

        public Guid? FindContactFromNames(string fn, string ln)
        {
            Guid? cid = (from c in ctx.ContactSet
                         where c.FirstName == fn && c.LastName == ln
                         select c.ContactId).FirstOrDefault();
            if (cid == Guid.Empty) return null;
            return cid;
        }

      
        public xrm.Account GetXrmAccountByExternalID(string externalID)
        {
            return ctx.AccountSet
                .Where(a => a.AccountNumber.Equals(externalID))
                .Select(a => new xrm.Account() { AccountId = a.AccountId })
                .FirstOrDefault();
        }
    }
}
