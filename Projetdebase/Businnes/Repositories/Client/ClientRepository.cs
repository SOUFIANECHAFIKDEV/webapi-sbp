using InovaFileManager;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProjetBase.Businnes.Contexts;
using ProjetBase.Businnes.Entities;
using ProjetBase.Businnes.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static ProjetBase.Businnes.Models.FicheTechniqueModal;

namespace ProjetBase.Businnes.Repositories.Client
{
    public class ClientRepository : EntityFrameworkRepository<Entities.Client, int>, IClientRepository
    {
        private readonly FileManager fileManager;
        public ClientRepository(ProjetBaseContext dbContext) : base(dbContext)
        {
            this.fileManager = new FileManager();
        }

        public bool CheckUniqueCodeClient(string codeclient)
        {
            var NbrReference = DbContext.Clients.Where(x => x.Codeclient == codeclient).Count();
            return NbrReference > 0;
        }

        /*
         * Save Memos de client
         */
        public async Task<bool> saveMemos(int id, string memos)
        {
            try
            {
                var client = GetById(id);
                client.Memos = memos;
                Update(client);
                await DbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return false;
            }
        }

        /*
         *  Changer les ids à des noms
         */
        public List<ModifyEntryModel> ChangeIdToNameHistorique(List<ModifyEntryModel> historiqueChamps)
        {
            try
            {
                historiqueChamps.ForEach(h =>
                {
                    if (h.Attribute != "Historique")
                    {
                        if (h.Attribute == "IdAgent")
                        {
                            h.Attribute = "Agent";
                            if (h.Before != "" && h.Before != null)
                            {
                                var user = DbContext.User.Where(x => x.Id == Int32.Parse(h.Before)).FirstOrDefault();
                                h.Before = string.Join(" ", user.Nom, user.Prenom);
                            }

                            if (h.After != "" && h.After != null)
                            {
                                var user = DbContext.User.Where(x => x.Id == Int32.Parse(h.After)).FirstOrDefault();
                                h.After = string.Join(" ", user.Nom, user.Prenom);
                            }
                        }
                        if (h.Attribute == "IdGroupe")
                        {
                            h.Attribute = "Groupe";
                            if (h.Before != "" && h.Before != null)
                            {
                                var user = DbContext.Groupes.Where(x => x.Id == Int32.Parse(h.Before)).FirstOrDefault();
                                h.Before = string.Join(" ", user.Nom);
                            }

                            if (h.After != "" && h.After != null)
                            {
                                var user = DbContext.Groupes.Where(x => x.Id == Int32.Parse(h.After)).FirstOrDefault();
                                h.After = string.Join(" ", user.Nom);
                            }
                        }
                    }


                });

                return historiqueChamps;
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                throw ex;
            }
        }

        public bool supprimerClient(int id)
        {
            if (DbContext.Chantier.Where(c => c.IdClient == id).Count() != 0)
            {
                return false;
            }
            var Client = DbContext.Clients.SingleOrDefault(C => C.Id == id);
            DbContext.Remove(Client);
            DbContext.SaveChanges();
            return true;
        }

        public Task<List<Entities.Client>> GetAllClient()
        {

            try
            {
                var clients = DbContext.Clients.ToListAsync();
                return clients;
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                throw ex;
            }
        }

        async public Task<bool> UpdateMemos(EditMemosViewModel body)
        {
            try
            {
                var clientInDb = DbContext.Clients.SingleOrDefault(x => x.Id == body.id);

                #region delete files
                //recuperer l'article a partir du base de donnée
                List<ficheTechniqueModal> ficheTechniquesInDB = JsonConvert.DeserializeObject<List<ficheTechniqueModal>>(clientInDb.Memos);
                var namesofOldFilesInDb = ficheTechniquesInDB.SelectMany(FT => FT.pieceJointes).Select(PJ => PJ.name).ToList();
                List<ficheTechniqueModal> ficheTechniques = JsonConvert.DeserializeObject<List<ficheTechniqueModal>>(body.memos);
                var namesOfNewFiles = ficheTechniques.SelectMany(FT => FT.pieceJointes).Select(PJ => PJ.name).ToList();
                var namesOfDeletedFiles = namesofOldFilesInDb.Except(namesOfNewFiles).ToList();
                //delete files
                fileManager.DeleteFiles(namesOfDeletedFiles);
                #endregion delete files

                #region add new files
                var pieceJointes = ficheTechniques.SelectMany(FT => FT.pieceJointes).Where(x => x.file != "").ToList();
                foreach (FTPieceJointes pieceJointe in pieceJointes)
                {
                    fileManager.Save(pieceJointe.file, pieceJointe.name);
                }
                #endregion

                clientInDb.Memos = JsonConvert.SerializeObject(ficheTechniques);
                DbContext.Update(clientInDb);
                return await DbContext.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

}
