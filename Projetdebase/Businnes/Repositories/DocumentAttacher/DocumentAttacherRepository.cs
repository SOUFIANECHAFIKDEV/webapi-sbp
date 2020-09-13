using Newtonsoft.Json;
using ProjetBase.Businnes.Contexts;
using ProjetBase.Businnes.Entities;
using ProjetBase.Businnes.Enum;
using ProjetBase.Businnes.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetBase.Businnes.Repositories.DocumentAttacher
{
    public class DocumentAttacherRepository : EntityFrameworkRepository<Entities.DocumentAttacher, int>, IDocumentAttacherRepository
    {
        private readonly ProjetBaseContext dbContext;

        public DocumentAttacherRepository(ProjetBaseContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<bool> Create(Entities.DocumentAttacher documentAttacher)
        {
            try
            {


                List<string> labels = JsonConvert.DeserializeObject<List<string>>(documentAttacher.LabelDocument);
                //!!!!!!!!!!!!!!!!!!!!   il faut savgarder tout l'objet du label
                if (labels.Where(label => label == "Ordre service").Count() != 0)
                {
                    #region Le chantier change de statut de "En étude" à "Accepté" si Un Orde de service est uploadé 
                    Entities.Chantier chantier = this.dbContext.Chantier.SingleOrDefault(X => X.Id == documentAttacher.IdChantier);
                    if (chantier.Statut == StatutChantier.Enetude)
                    {
                        chantier.Statut = StatutChantier.Accepte;
                        this.dbContext.Update(chantier);
                        await this.dbContext.SaveChangesAsync();
                    }
                    #endregion
                }

                //var td = dbContext.Parametrages.Where(x => (int)x.Type == (int)TypeParametrage.typedocument).FirstOrDefault();
                //List<TypeDocuments> typedocument = JsonConvert.DeserializeObject<List<TypeDocuments>>(td.Contenu);
                //List<TypeDocuments> newlabels = new List<TypeDocuments>();
                //foreach (string l in labels)
                //{
                //    var result = typedocument.Where(w => w.nom == l).FirstOrDefault();
                //    if (result == null)
                //    {
                //        var lastId = typedocument.Max(x => x.id);
                //        newlabels.Add(new TypeDocuments
                //        {
                //            id = lastId + 1,
                //            isFixed = false,
                //            nom = l
                //        });
                //    }
                //}
                //typedocument.AddRange(newlabels);
                //td.Contenu = JsonConvert.SerializeObject(typedocument);
                //dbContext.Add(td);

                documentAttacher.DateAjout = DateTime.Now;
                documentAttacher.UpdateAt = DateTime.Now;
                this.dbContext.Add(documentAttacher);

                await this.dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> Delete(Entities.DocumentAttacher documentAttacher)
        {
            try
            {
                ////!!!!!!!!!!!!!!!!!!!!   il faut savgarder tout l'objet du label
                //if (labels.Where(label => label == "Ordre service").Count() != 0)
                //{
                int documentAttacherNb = this.dbContext.DocumentAttacher
                    .Where(
                    X => X.IdChantier == documentAttacher.IdChantier
                    && X.LabelDocument.Contains("Ordre service")
                    ).Count();


                int nbDevisAcceptee = this.dbContext.Devis.Where(w => w.IdChantier == documentAttacher.IdChantier && w.Status == StatutDevis.Acceptee).Count();
                if (documentAttacherNb == 1 && nbDevisAcceptee == 0)
                {
                    Entities.Chantier chantier = this.dbContext.Chantier.SingleOrDefault(X => X.Id == documentAttacher.IdChantier);
                    chantier.Statut = StatutChantier.Enetude;
                    this.dbContext.Update(chantier);
                }
                this.dbContext.Remove(documentAttacher);
                await this.dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<bool> Update(Entities.DocumentAttacher documentAttacher)
        {
            try
            {


                List<string> labels = JsonConvert.DeserializeObject<List<string>>(documentAttacher.LabelDocument);

                //!!!!!!!!!!!!!!!!!!!!   il faut savgarder tout l'objet du label
                if (labels.Where(label => label == "Ordre service").Count() != 0)
                {
                    #region Le chantier change de statut de "En étude" à "Accepté" si Un Orde de service est uploadé 
                    Entities.Chantier chantier = this.dbContext.Chantier.SingleOrDefault(X => X.Id == documentAttacher.IdChantier);
                    if (chantier.Statut == StatutChantier.Enetude)
                    {
                        chantier.Statut = StatutChantier.Accepte;
                        this.dbContext.Update(chantier);
                        await this.dbContext.SaveChangesAsync();
                    }
                    #endregion
                }

                var documentAttacherInDB = dbContext.DocumentAttacher.SingleOrDefault(x => x.Id == documentAttacher.Id);
                documentAttacherInDB.IdChantier = documentAttacher.IdChantier;
                documentAttacherInDB.idRubrique = documentAttacher.idRubrique;
                documentAttacherInDB.LabelDocument = documentAttacher.LabelDocument;
                documentAttacherInDB.PieceJointes = documentAttacher.PieceJointes;
                documentAttacherInDB.UpdateAt = DateTime.Now;

                dbContext.Update(documentAttacherInDB);
                await dbContext.SaveChangesAsync();
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}